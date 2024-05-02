using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AlePostProcessUPR
{
    /// <summary>
    /// Custom Post Processing injection points.
    /// Since this is a flag, you can write a renderer that can be injected at multiple locations.
    /// </summary>
    [Flags]
    public enum CustomPostProcessInsertPoint 
    {
        /// <summary>
        /// 不透明 后
        /// </summary>
        AfterOpaqueAndSky = 1,
        
        /// <summary>
        /// 后处理 前
        /// </summary>
        BeforePostProcess = 2,
        
        /// <summary>
        /// 后处理 后
        /// </summary>
        AfterPostProcess = 4,
    }

    /// <summary>
    /// 后处理渲染器 基类
    /// </summary>
    public abstract class CustomPostProcessRenderer : IDisposable
    {
        #region 配置
        /// <summary>
        /// 通道输入 默认为帧缓冲（颜色图）
        /// </summary>
        public virtual ScriptableRenderPassInput Input => ScriptableRenderPassInput.Color;

        /// <summary>
        /// 是否可见 场景视口
        /// </summary>
        public virtual bool IsVisibleSceneView => true;
        #endregion

        #region 初始化
        /// <summary>
        /// 是否 初始化完成
        /// </summary>
        public bool Initialized => IsInit;
        private bool IsInit = false;

        public CustomPostProcessRenderer()
        {
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init()
        {
            Initialize();
            IsInit = true;
        }

        /// <summary>
        /// Initialize function, called once before the effect is first rendered.
        /// If the effect is never rendered, then this function will never be called.
        /// </summary>
        public virtual void Initialize()
        {
            
        }

        /// <summary>
        /// 后处理容器
        /// </summary>
        protected CustomVolumeComponent m_VolumeComponent = null;

        /// <summary>
        /// 后处理容器 类型
        /// </summary>
        protected Type m_VolumeComponentType = null;

        /// <summary>
        /// 初始化 后处理容器
        /// </summary>
        /// <typeparam name="T">后处理容器组件 实例</typeparam>
        /// <returns></returns>
        protected T InitVolumeComponent<T>() where T : CustomVolumeComponent
        {
            m_VolumeComponent = VolumeManager.instance.stack.GetComponent<T>();
            //获取 后处理容器 组件
            m_VolumeComponentType = typeof(T);

            return m_VolumeComponent as T;
        }

        /// <summary>
        /// 获取 后处理容器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetVolumeComponent<T>() where T : CustomVolumeComponent
        {
            if (m_VolumeComponent == null)
                InitVolumeComponent<T>();

            return m_VolumeComponent as T;
        }

        /// <summary>
        /// 后处理材质球
        /// </summary>
        protected Material m_PostProcessMaterial;

        /// <summary>
        /// 初始化 材质球
        /// </summary>
        /// <param name="shaderName">着色器名称</param>
        /// <returns></returns>
        protected Material InitPostProcessMaterial(string shaderName)
        {
            m_PostProcessMaterial = CoreUtils.CreateEngineMaterial(shaderName);

            return m_PostProcessMaterial;
        }
        #endregion

        #region 装载
        /// <summary>
        /// 装载 成功装载的才会进行后处理渲染
        /// </summary>
        /// <param name="renderingData"></param>
        /// <param name="insertPoint"></param>
        /// <returns>是否 装载</returns>
        public virtual bool Setup(ref RenderingData renderingData, CustomPostProcessInsertPoint insertPoint)
        {
            //VolumeComponent.active 有Bug始终为true
            //使用Enable的overrideState作为Volume的激活状态
            return m_PostProcessMaterial != null && m_VolumeComponent != null && m_VolumeComponent.Enable.overrideState;
        }
        #endregion

        #region 渲染
        private CommandBuffer m_CommandBuffer; //当前的 命令缓冲区
        private RenderTargetIdentifier m_RTSource;
        private RenderTargetIdentifier m_RTDestination;

        /// <summary>
        /// 渲染 执行一帧相机的后处理渲染
        /// 由子类调用  定义自定义后处理逻辑
        /// </summary>
        /// <param name="cmd">命令缓冲区</param>
        /// <param name="src">RT 包含相机当前帧渲染的颜色缓冲</param>
        /// <param name="dest">RT 后处理的渲染目标</param>
        /// <param name="renderingData">渲染数据</param>
        /// <param name="insertPoint">插入点 后处理的渲染时机</param>
        public virtual void Render(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dest, ref RenderingData renderingData, CustomPostProcessInsertPoint insertPoint)
        {
            m_CommandBuffer = cmd;
            m_RTSource = src;
            m_RTDestination = dest;
        }

        /// <summary>
        /// 全屏渲染
        /// 通常在Render的最后调用
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        protected bool DrawFullScreen(int passID = 0, RenderTargetIdentifier src = default, RenderTargetIdentifier dest = default)
        {
            if (m_CommandBuffer == null) return false;
            if (src == default) src = m_RTSource;
            if (dest == default) dest = m_RTDestination;

            m_CommandBuffer.SetGlobalTexture(GetShaderPropertyID("_MainTex"), src);
            CoreUtils.DrawFullScreen(m_CommandBuffer, m_PostProcessMaterial, dest, null, passID);

            return true;
        }
        #region 材质球属性
        #region 着色器属性ID 缓存
        //着色器属性ID 字典
        internal static Dictionary<string, int> m_DicShaderPropertyID = new Dictionary<string, int>();

        /// <summary>
        /// 获取 着色器属性ID
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private int GetShaderPropertyID(string propertyName)
        {
            int id = 0;

            if (m_DicShaderPropertyID.TryGetValue(propertyName, out id) == false)
            {
                id = Shader.PropertyToID(propertyName);
                m_DicShaderPropertyID.Add(propertyName, id);
            }

            return id;
        }
        #endregion
        /// <summary>
        /// 设置 布尔值
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="param">参数器</param>
        protected void SetBool(string propertyName, BoolParameter param)
        {
            m_PostProcessMaterial.SetFloat(GetShaderPropertyID(propertyName), param.value ? 1f : 0f);
        }

        /// <summary>
        /// 设置 整数
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="param">参数器</param>
        protected void SetInt(string propertyName, IntParameter param)
        {
            m_PostProcessMaterial.SetInt(GetShaderPropertyID(propertyName), param.value);
        }

        /// <summary>
        /// 设置 浮点数
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="param">参数器</param>
        protected void SetFloat(string propertyName, FloatParameter param)
        {
            m_PostProcessMaterial.SetFloat(GetShaderPropertyID(propertyName), param.value);
        }

        /// <summary>
        /// 设置 浮点数
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="param">参数器</param>
        protected void SetFloat(string propertyName, float param)
        {
            m_PostProcessMaterial.SetFloat(GetShaderPropertyID(propertyName), param);
        }

        /// <summary>
        /// 设置 向量
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="param">参数器</param>
        protected void SetVector(string propertyName, Vector2Parameter param)
        {
            m_PostProcessMaterial.SetVector(GetShaderPropertyID(propertyName), param.value);
        }

        /// <summary>
        /// 设置 向量
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="param">参数器</param>
        protected void SetVector(string propertyName, Vector2 param)
        {
            m_PostProcessMaterial.SetVector(GetShaderPropertyID(propertyName), param);
        }

        /// <summary>
        /// 设置 向量
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="param">参数器</param>
        protected void SetVector(string propertyName, Vector3Parameter param)
        {
            m_PostProcessMaterial.SetVector(GetShaderPropertyID(propertyName), param.value);
        }

        /// <summary>
        /// 设置 向量
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="param">参数器</param>
        protected void SetVector(string propertyName, Vector3 param)
        {
            m_PostProcessMaterial.SetVector(GetShaderPropertyID(propertyName), param);
        }

        /// <summary>
        /// 设置 向量
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="param">参数器</param>
        protected void SetVector(string propertyName, Vector4Parameter param)
        {
            m_PostProcessMaterial.SetVector(GetShaderPropertyID(propertyName), param.value);
        }

        /// <summary>
        /// 设置 向量
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="param">参数器</param>
        protected void SetVector(string propertyName, Vector4 param)
        {
            m_PostProcessMaterial.SetVector(GetShaderPropertyID(propertyName), param);
        }

        /// <summary>
        /// 设置 纹理
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="param">参数器</param>
        protected void SetTexture(string propertyName, TextureParameter param)
        {
            m_PostProcessMaterial.SetTexture(GetShaderPropertyID(propertyName), param.value);
        }

        /// <summary>
        /// 设置 颜色
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="param">参数器</param>
        protected void SetColor(string propertyName, ColorParameter param)
        {
            m_PostProcessMaterial.SetColor(GetShaderPropertyID(propertyName), param.value);
        }
        #endregion
        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose function, called when the renderer is disposed.
        /// </summary>
        /// <param name="disposing"> If true, dispose of managed objects </param>
        public virtual void Dispose(bool disposing)
        {

        }

        /// <summary>
        /// Create a descriptor for intermediate render targets based on the rendering data.
        /// Mainly used to create intermediate render targets.
        /// </summary>
        /// <returns>a descriptor similar to the camera target but with no depth buffer or multisampling</returns>
        public static RenderTextureDescriptor GetTempRTDescriptor(in RenderingData renderingData)
        {
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;
            descriptor.msaaSamples = 1;
            return descriptor;
        }
    }

    /// <summary>
    /// Use this attribute to mark classes that can be used as a custom post-processing renderer
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class CustomPostProcessAttribute : System.Attribute 
    {

        // Name of the effect in the custom post-processing render feature editor
        readonly string name;

        // In which render pass this effect should be injected
        readonly CustomPostProcessInsertPoint m_InsertPoint;

        // In case the renderer is added to multiple injection points,
        // If shareInstance = true, one instance of the renderer will be constructed and shared between the injection points.
        // Otherwise, a different instance will be  constructed for every injection point.
        readonly bool shareInstance;

        /// <value> Name of the effect in the custom post-processing render feature editor </value>
        public string Name => name;

        /// <value> In which render pass this effect should be injected </value>
        public CustomPostProcessInsertPoint InsertPoint => m_InsertPoint;

        /// <value>
        /// In case the renderer is added to multiple injection points,
        /// If shareInstance = true, one instance of the renderer will be constructed and shared between the injection points.
        /// Otherwise, a different instance will be  constructed for every injection point.
        /// </value>
        public bool ShareInstance => shareInstance;

        /// <summary>
        /// Marks this class as a custom post processing renderer
        /// </summary>
        /// <param name="name"> Name of the effect in the custom post-processing render feature editor </param>
        /// <param name="injectPoint"> In which render pass this effect should be injected </param>
        public CustomPostProcessAttribute(string name, CustomPostProcessInsertPoint insertPoint, bool shareInstance = false)
        {
            this.name = name;
            this.m_InsertPoint = insertPoint;
            this.shareInstance = shareInstance;
        }

        /// <summary>
        /// Get the CustomPostProcessAttribute attached to the type.
        /// </summary>
        /// <param name="type">the type on which the attribute is attached</param>
        /// <returns>the attached CustomPostProcessAttribute or null if none were attached</returns>
        public static CustomPostProcessAttribute GetAttribute(Type type)
        {
            if(type == null) return null;
            var atttributes = type.GetCustomAttributes(typeof(CustomPostProcessAttribute), false);
            return (atttributes.Length != 0) ? (atttributes[0] as CustomPostProcessAttribute) : null;
        }
    }
}