using UnityEngine;
using UnityEditor;

namespace UnityEditor.AleToonURP.ShaderGUI
{
    /// <summary>
    /// 材质球编辑器界面-AleToonURP/WaterPlane
    /// </summary>
    public class ShaderGUIWaterPlane : ShaderGUIBase
    {
        protected override void OnGUIAleToon()
        {
            base.OnGUIAleToon();

            //主面板列表
            ShaderGUIExtension.FoldoutPanel("【基础 Basic】颜色", PanelMainBasic);
            ShaderGUIExtension.FoldoutPanel("【水波 Wave】法线贴图、缩放、速度", PanelMainWave);
            ShaderGUIExtension.FoldoutPanel("【反射 Reflect】反射贴图、强度、模糊、菲涅尔", PanelMainReflect);
            ShaderGUIExtension.FoldoutPanel("【折射 Refract】强度", PanelMainRefract);
            ShaderGUIExtension.FoldoutPanel("【边缘泡沫 EdgeFoam】阈值贴图、裁剪、距离、模糊、透明度", PanelMainEdgeFoam);
        }

        #region 主面板-基础
        /// <summary>
        /// 主面板 基础设置
        /// </summary>
        private void PanelMainBasic()
        {
            //条目 浅水颜色
            m_MaterialEditor.ColorProperty(GetMaterialProperty("_ColorShallow"), "浅水颜色");
            //条目 深水颜色
            m_MaterialEditor.ColorProperty(GetMaterialProperty("_ColorDeep"), "深水颜色");
        }
        #endregion

        #region 主面板-水波
        private static GUIContent m_ContentWaveBumpMap = new GUIContent("法线贴图", "法线偏移 : 贴图采样矢量(sRGB)进行法线偏移");

        /// <summary>
        /// 主面板 基础贴图
        /// </summary>
        /// <param name="material"></param>
        private void PanelMainWave()
        {
            //条目 法线贴图
            m_MaterialEditor.TexturePropertySingleLine(m_ContentWaveBumpMap, GetMaterialProperty("_BumpMap"), GetMaterialProperty("_BumpScale"));
            //条目 纹理缩放 首要
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_BumpScaleFir"), "纹理缩放 首要");
            //条目 纹理缩放 次要
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_BumpScaleSec"), "纹理缩放 次要");
            //条目 速度X轴
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatWaveSpeedX"), "速度X轴");
            //条目 速度Y轴
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatWaveSpeedY"), "速度Y轴");
        }
        #endregion

        #region 主面板-反射
        private static GUIContent m_ContentReflectCubeMap = new GUIContent("反射贴图", "CubeMap 法线偏移 : 贴图采样矢量(sRGB)进行法线偏移");

        /// <summary>
        /// 主面板 外描边
        /// </summary>
        /// <param name="material"></param>
        private void PanelMainReflect()
        {
            //条目 反射贴图
            m_MaterialEditor.TexturePropertySingleLine(m_ContentReflectCubeMap, GetMaterialProperty("_ReflectCubeMap"));
            //条目 反射强度
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_ReflectIntensity"), "反射强度");
            //条目 模糊强度
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_ReflectBulrIntensity"), "模糊强度");
            //条目 菲涅尔系数
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FresnelFactor"), "菲涅尔系数");
            //条目 菲涅尔强度
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FresnelIntensity"), "菲涅尔强度");
        }
        #endregion

        #region 主面板-折射
        /// <summary>
        /// 主面板-边缘光
        /// </summary>
        /// <param name="material"></param>
        private void PanelMainRefract()
        {
            //条目 折射强度
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_RefractIntensity"), "折射强度");
        }
        #endregion

        #region 主面板-边缘泡沫
        private static GUIContent m_ContentEdgeThresholdMap = new GUIContent("阈值贴图", "阈值 : 贴图采样矢量(sRGB)");

        /// <summary>
        /// 主界面 法线贴图
        /// </summary>
        /// <param name="material"></param>
        private void PanelMainEdgeFoam()
        {
            //条目 阈值贴图
            var matPropEdgeThresholdMap = GetMaterialProperty("_EdgeThresholdMap");
            m_MaterialEditor.TexturePropertySingleLine(m_ContentEdgeThresholdMap, matPropEdgeThresholdMap);
            m_MaterialEditor.TextureScaleOffsetProperty(matPropEdgeThresholdMap);
            //条目 颜色
            m_MaterialEditor.ColorProperty(GetMaterialProperty("_EdgeFoamColor"), "颜色");
            //条目 阈值裁剪
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_EdgeFoamThresholdCutoff"), "阈值裁剪");
            //条目 距离
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_EdgeFoamDis"), "距离");
            //条目 模糊
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_EdgeFoamBlur"), "模糊");
        }
        #endregion
    }
}