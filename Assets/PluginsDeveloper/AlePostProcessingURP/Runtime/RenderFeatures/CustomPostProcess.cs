using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AlePostProcessUPR
{
    /// <summary>
    /// 自定义后处理 RF
    /// <list type="bullet">
    /// <item>
    /// <description> 桥接URP的PostProcessing，使用相同的方式在Volume中进行添加与设置 </description>
    /// </item>
    /// </list>
    /// </summary>
    [Serializable]
    public class CustomPostProcess : ScriptableRendererFeature
    {
        /// <summary>
        /// 自定义RF配置
        /// </summary>
        [Serializable]
        public class CustomPostProcessSettings 
        {
            public CustomPostProcessSettings()
            {
                m_RenderersAfterOpaqueAndSky = new List<string>();
                m_RenderersBeforePostProcess = new List<string>();
                m_RenderersAfterPostProcess = new List<string>();
            }

            /// <summary>
            /// 渲染列表 不透明之后
            /// </summary>
            [SerializeField]
            public List<string> m_RenderersAfterOpaqueAndSky;

            /// <summary>
            /// 渲染列表 后处理之前
            /// </summary>
            [SerializeField]
            public List<string> m_RenderersBeforePostProcess;

            /// <summary>
            /// 渲染列表 后处理之后
            /// </summary>
            [SerializeField]
            public List<string> m_RenderersAfterPostProcess;
        }

        /// <summary>
        /// 自定义RF 配置
        /// </summary>
        [SerializeField] public CustomPostProcessSettings m_Settings = new CustomPostProcessSettings();

        /// <summary>
        /// 自定义渲染通道 不透明 后
        /// </summary>
        private CustomPostProcessRenderPass m_AfterOpaqueAndSky;

        /// <summary>
        /// 自定义渲染通道 后处理 前
        /// </summary>
        private CustomPostProcessRenderPass m_BeforePostProcess;

        /// <summary>
        /// 自定义渲染通道 后处理 后
        /// </summary>
        private CustomPostProcessRenderPass m_AfterPostProcess;

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Create()
        {
            //创建 自定义后处理 渲染通道
            Dictionary<string, CustomPostProcessRenderer> shared = new Dictionary<string, CustomPostProcessRenderer>();
            m_AfterOpaqueAndSky = new CustomPostProcessRenderPass(CustomPostProcessInsertPoint.AfterOpaqueAndSky, InstantiateRenderers(m_Settings.m_RenderersAfterOpaqueAndSky, shared));
            m_BeforePostProcess = new CustomPostProcessRenderPass(CustomPostProcessInsertPoint.BeforePostProcess, InstantiateRenderers(m_Settings.m_RenderersBeforePostProcess, shared));
            m_AfterPostProcess = new CustomPostProcessRenderPass(CustomPostProcessInsertPoint.AfterPostProcess, InstantiateRenderers(m_Settings.m_RenderersAfterPostProcess, shared));
        }

        /// <summary>
        /// 将类名(AssemblyQualifiedName)转换为实例。过滤掉不存在或不匹配需求的类型。
        /// </summary>
        /// <param name="names">名称列表</param>
        /// <param name="shared">共享实例字典 以名字作为Key</param>
        /// <returns>List of renderers</returns>
        private List<CustomPostProcessRenderer> InstantiateRenderers(List<String> names, Dictionary<string, CustomPostProcessRenderer> shared)
        {
            var renderers = new List<CustomPostProcessRenderer>(names.Count);
            foreach (var name in names)
            {
                if(shared.TryGetValue(name, out var renderer))
                    renderers.Add(renderer);
                else
                {
                    var type = Type.GetType(name);
                    if (type == null || !type.IsSubclassOf(typeof(CustomPostProcessRenderer))) continue;
                    var attribute = CustomPostProcessAttribute.GetAttribute(type);
                    if(attribute == null) continue;

                    renderer = Activator.CreateInstance(type) as CustomPostProcessRenderer;
                    renderers.Add(renderer);
                    
                    if(attribute.ShareInstance)
                        shared.Add(name, renderer);
                }
            }

            return renderers;
        }

        /// <summary>
        /// 插入 自定义后处理渲染通道
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="renderingData"></param>
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            //摄像机未开启后处理
            if (renderingData.cameraData.postProcessEnabled == false) return;

            //检查后处理是否开启 并插入渲染通道
            if (m_AfterOpaqueAndSky.HasPostProcessRenderers && m_AfterOpaqueAndSky.PrepareRenderers(ref renderingData))
                renderer.EnqueuePass(m_AfterOpaqueAndSky);

            if (m_BeforePostProcess.HasPostProcessRenderers && m_BeforePostProcess.PrepareRenderers(ref renderingData))
                renderer.EnqueuePass(m_BeforePostProcess);

            if (m_AfterPostProcess.HasPostProcessRenderers && m_AfterPostProcess.PrepareRenderers(ref renderingData))
                renderer.EnqueuePass(m_AfterPostProcess);
        }
    }

    /// <summary>
    /// 自定义 后处理 渲染通道
    /// </summary>
    public class CustomPostProcessRenderPass : ScriptableRenderPass
    {
        /// <summary>
        /// 插入点
        /// </summary>
        private CustomPostProcessInsertPoint m_InsertPoint;

        /// <summary>
        /// Pass名称 用于在Profiler分析器上分组显示
        /// </summary>
        private string m_PassName;

        /// <summary>
        /// 后处理渲染器列表
        /// </summary>
        private List<CustomPostProcessRenderer> m_PostProcessRenderers;

        /// <summary>
        /// 后处理渲染器列表 激活
        /// </summary>
        private List<int> m_ActivePostProcessRenderers;

        /// <summary>
        /// 分析器采样器 列表
        /// </summary>
        private List<ProfilingSampler> m_ProfilingSampler;

        /// <summary>
        /// 是否 有后处理渲染器
        /// </summary>
        public bool HasPostProcessRenderers => m_PostProcessRenderers.Count != 0;

        /// <summary>
        /// Construct the custom post-processing render pass
        /// </summary>
        /// <param name="insertPoint">插入点</param>
        /// <param name="renderers">后处理渲染器 列表</param>
        public CustomPostProcessRenderPass(CustomPostProcessInsertPoint insertPoint, List<CustomPostProcessRenderer> renderers)
        {
            this.m_InsertPoint = insertPoint;
            this.m_ProfilingSampler = new List<ProfilingSampler>(renderers.Count);
            this.m_PostProcessRenderers = renderers;

            //获取所有 后处理渲染器名称
            foreach (var renderer in renderers)
            {
                var attribute = CustomPostProcessAttribute.GetAttribute(renderer.GetType());
                m_ProfilingSampler.Add(new ProfilingSampler(attribute?.Name));
            }

            //激活的 渲染器列表
            this.m_ActivePostProcessRenderers = new List<int>(renderers.Count);

            //设置 渲染事件 插入的节点
            switch(insertPoint)
            {
                case CustomPostProcessInsertPoint.AfterOpaqueAndSky: 
                    renderPassEvent = RenderPassEvent.AfterRenderingSkybox; 
                    m_PassName = "Custom PostProcess - After Opaque & Sky";
                    break;
                case CustomPostProcessInsertPoint.BeforePostProcess: 
                    renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
                    m_PassName = "Custom PostProcess - Before PostProcess";
                    break;
                case CustomPostProcessInsertPoint.AfterPostProcess:
                    renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
                    m_PassName = "Custom PostProcess - After PostProcess";
                    break;
            }

            //初始化 临时绘制纹理
            InitTempRenderTexture();
        }

        /// <summary>
        /// Prepares the renderer for executing on this frame and checks if any of them actually requires rendering
        /// </summary>
        /// <returns>是否 有后处理渲染器执行</returns>
        public bool PrepareRenderers(ref RenderingData renderingData)
        {
            //是否为 Scene窗口摄像机
            bool isSceneView = renderingData.cameraData.cameraType == CameraType.SceneView;

            //通道输入类型
            ScriptableRenderPassInput passInput = ScriptableRenderPassInput.None;

            //记录 激活的后处理渲染器
            m_ActivePostProcessRenderers.Clear();
            for (int i = 0; i < m_PostProcessRenderers.Count; i++)
            {
                var renderer = m_PostProcessRenderers[i];
                // Skips current renderer if "visibleInSceneView" = false and the current camera is a scene view camera. 
                if(isSceneView && !renderer.IsVisibleSceneView) continue;

                // Setup the camera for the renderer and if it will render anything, add to active renderers and get its required inputs
                if (renderer.Setup(ref renderingData, m_InsertPoint))
                {
                    m_ActivePostProcessRenderers.Add(i);
                    passInput |= renderer.Input;
                }
            }

            //设置 通道输入配置
            ConfigureInput(passInput);

            //是否有 激活的后处理渲染器
            return m_ActivePostProcessRenderers.Count != 0;
        }

        #region 中间RenderTexture
        /// <summary>
        /// 临时RT 绘制目标句柄
        /// </summary>
        private RenderTargetHandle[] m_TempRenderTextureHandle;

        /// <summary>
        /// 临时RT 分配状态
        /// </summary>
        private bool[] m_TempRenderTextureIsAllocated;

        /// <summary>
        /// RT描述参数 当前Renderer
        /// </summary>
        private RenderTextureDescriptor m_TempRenderTextureDesc;

        //RT数量
        private int m_TempRenderTextureCount = 2;

        /// <summary>
        /// 初始化 临时绘制纹理
        /// </summary>
        private void InitTempRenderTexture()
        {
            m_TempRenderTextureHandle = new RenderTargetHandle[m_TempRenderTextureCount];
            m_TempRenderTextureIsAllocated = new bool[m_TempRenderTextureCount];

            for (int i = 0; i < m_TempRenderTextureHandle.Length; i++)
            {
                m_TempRenderTextureHandle[i].Init($"_TempRenderTexture{i}");
                m_TempRenderTextureIsAllocated[i] = false;
            }
        }

        /// <summary>
        /// 获取 临时RT
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private RenderTargetIdentifier GetIntermediate(CommandBuffer cmd, int index)
        {
            //未分配时 进行RT分配
            if (!m_TempRenderTextureIsAllocated[index])
            {
                cmd.GetTemporaryRT(m_TempRenderTextureHandle[index].id, m_TempRenderTextureDesc);
                m_TempRenderTextureIsAllocated[index] = true;
            }

            return m_TempRenderTextureHandle[index].Identifier();
        }

        /// <summary>
        /// 释放已经分配的RT
        /// </summary>
        /// <param name="cmd">The command buffer to use for deallocation</param>
        private void ReleaseAllTempRenderTexture(CommandBuffer cmd)
        {
            for (int i = 0; i < m_TempRenderTextureCount; i++)
            {
                if (m_TempRenderTextureIsAllocated[i] == false) continue;

                cmd.ReleaseTemporaryRT(m_TempRenderTextureHandle[i].id);
                m_TempRenderTextureIsAllocated[i] = false;
            }
        }
        #endregion

        /// <summary>
        /// 执行 后处理特效绘制
        /// </summary>
        /// <param name="context">SRP的上下文数据</param>
        /// <param name="renderingData">当前绘制数据</param>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            RenderTargetIdentifier target = renderingData.cameraData.renderer.cameraColorTarget;

            //相机描述参数 用于设置RT
            m_TempRenderTextureDesc = renderingData.cameraData.cameraTargetDescriptor;
            m_TempRenderTextureDesc.msaaSamples = 1; //多重采样 等级1
            m_TempRenderTextureDesc.depthBufferBits = 0; //深度缓冲 关闭

            CommandBuffer cmd = CommandBufferPool.Get(m_PassName);
            //context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            int width = m_TempRenderTextureDesc.width;
            int height = m_TempRenderTextureDesc.height;
            cmd.SetGlobalVector("_ScreenSize", new Vector4(width, height, 1.0f / width, 1.0f / height));

            int intermediateIndex = 0; //当前使用的 临时RT 下标
            //将帧缓冲先绘制进临时RT
            //直接用于后处理绘制 会导致Shader的_MainTex无法正常传入
            cmd.Blit(target, GetIntermediate(cmd, intermediateIndex));
            //绘制所有后处理Renderer
            for (int index = 0; index < m_ActivePostProcessRenderers.Count; ++index)
            {
                var rendererIndex = m_ActivePostProcessRenderers[index];
                var renderer = m_PostProcessRenderers[rendererIndex];
                
                RenderTargetIdentifier source, destination;

                //获取 源贴图
                source = GetIntermediate(cmd, intermediateIndex);
                //获取 绘制的目标贴图
                if (index >= m_ActivePostProcessRenderers.Count - 1)
                    destination = target; //最后一次绘制 直接绘制进帧缓冲贴图
                else
                {
                    //更换RT 反复绘制后处理效果
                    intermediateIndex = 1 - intermediateIndex;
                    destination = GetIntermediate(cmd, intermediateIndex);
                }

                using (new ProfilingScope(cmd, m_ProfilingSampler[rendererIndex]))
                {
                    //初始化后处理Renderer
                    if (!renderer.Initialized)
                        renderer.Init();
                    //执行后处理绘制
                    renderer.Render(cmd, source, destination, ref renderingData, m_InsertPoint);
                }
            } 

            //执行CommandBuffer 并释放
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            //释放所有临时RT
            ReleaseAllTempRenderTexture(cmd);
        }
    }
}