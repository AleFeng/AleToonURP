using UnityEngine;
using UnityEditor;

namespace UnityEditor.AleToonURP.ShaderGUI
{
    /// <summary>
    /// 材质球编辑器界面-AleToonURP/Lit
    /// </summary>
    public class ShaderGUILit : ShaderGUIBase
    {
        protected override void OnGUIAleToon()
        {
            base.OnGUIAleToon();

            //主面板列表
            ShaderGUIExtension.FoldoutPanel("【基础设置 Basic】透明类型、渲染面、模板测试、剔除", PanelMainBasicSetting);
            ShaderGUIExtension.FoldoutPanel("【基础贴图 BaseMap】基础贴图&颜色、暗部贴图&颜色", PanelMainBasicMap);
            ShaderGUIExtension.FoldoutPanel("【法线贴图 NormalMap】强度、效果开关", PanelMainNormalMap);
            ShaderGUIExtension.FoldoutPanel("【外描边 Outline】粗细、颜色", PanelMainOutline);
            ShaderGUIExtension.FoldoutPanel("【边缘光 RimLight】颜色、大小、遮罩", PanelMainRimLight);
            ShaderGUIExtension.FoldoutPanel("【高光 HighLight】颜色、大小、遮罩", PanelMainHighLight);
            ShaderGUIExtension.FoldoutPanel("【自发光 Emissive】遮罩、颜色(HDR)、动画", PanelMainEmissive);
            ShaderGUIExtension.FoldoutPanel("【材质捕获 Matcap】贴图、遮罩、法线", PanelMainMatCap);
            ShaderGUIExtension.FoldoutPanel("【光照设置 LightSetting】光照开关、光照强度", PanelMainGlobalLight);
        }

        #region 主面板-基础设置
        private static GUIContent m_ContentSurfaceType = new GUIContent("表面类型", "物体的表面渲染类型，不透明或透明");
        private static GUIContent m_ContentRenderFaceType = new GUIContent("渲染面类型", "渲染物体的双面、反面或正面。");
        //裁剪
        private static GUIContent m_ContentClipType = new GUIContent("裁剪类型", "Off-关闭，Clip-像素剔除，Alpha-透明度变化");
        private static GUIContent m_ContentClipMap = new GUIContent("裁剪贴图", "直接从贴图中采样裁剪强度，阈值0.0-1.0 : 纹理采样(linear)");
        private static GUIContent m_ContentClipBaseMapAlphaToggle = new GUIContent("基础贴图A通道生效", "当基础贴图为透明贴图，或需要使用到基础贴图的A通道。");
        //模板测试
        private static GUIContent m_ContentStencilType = new GUIContent("模板类型", "决定了渲染的方式，丢弃或保留。");
        private static GUIContent m_ContentStencilGroupNum = new GUIContent("模板组序号", "将想要互相影响的材质球，设置为相同模板组序号。");

        /// <summary>
        /// 关键词 表面类型 不透明
        /// </summary>
        private const string m_MatKeywordSurfaceTypeOpaque = "_SURFACETYPE_OPAQUE";
        /// <summary>
        /// 关键词 表面类型 透明
        /// </summary>
        private const string m_MatKeywordSurfaceTypeTransparent = "_SURFACETYPE_TRANSPARENT";

        /// <summary>
        /// 关键词 裁剪模式 关闭
        /// </summary>
        private const string m_MatKeywordClipOff = "_CLIP_OFF";
        /// <summary>
        /// 关键词 裁剪模式 挖孔
        /// </summary>
        private const string m_MatKeywordClipDither = "_CLIP_DITHER";
        /// <summary>
        /// 关键词 裁剪模式 透明
        /// </summary>
        private const string m_MatKeywordClipAlpha = "_CLIP_ALPHA";

        /// <summary>
        /// 表面类型（不透明或透明）
        /// </summary>
        public enum ESurfaceType
        {
            /// <summary>
            /// 不透明
            /// </summary>
            Opaque,
            /// <summary>
            /// 透明
            /// </summary>
            Transparent,
        }

        /// <summary>
        /// 渲染面 类型
        /// </summary>
        public enum ERenderFaceType
        {
            /// <summary>
            /// 渲染双面
            /// </summary>
            Both,
            /// <summary>
            /// 渲染反面
            /// </summary>
            Back,
            /// <summary>
            /// 渲染正面
            /// </summary>
            Front
        }

        /// <summary>
        /// 溶解类型
        /// </summary>
        private enum EClipType
        {
            /// <summary>
            /// 关闭
            /// </summary>
            Off,
            /// <summary>
            /// 挖孔
            /// </summary>
            Dither,
            /// <summary>
            /// 透明
            /// </summary>
            Alpha
        }

        #region 枚举 模板测试
        /// <summary>
        /// 模板测试类型
        /// </summary>
        private enum EStencilType
        {
            /// <summary>
            /// 关闭
            /// </summary>
            Off,
            /// <summary>
            /// 丢弃
            /// </summary>
            Discard,
            /// <summary>
            /// 保留
            /// </summary>
            Reserve
        }

        /// <summary>
        /// 模板测试 通过的比较规则
        /// </summary>
        private enum EStencilCompType
        {
            /// <summary>
            /// 关闭模板测试
            /// </summary>
            Disabled,
            /// <summary>
            /// 从不通过
            /// </summary>
            Never,
            /// <summary>
            /// 参考值小于缓冲区值
            /// </summary>
            Less,
            /// <summary>
            /// 参考值等于缓冲区值
            /// </summary>
            Equal,
            /// <summary>
            /// 参考值小于等于缓冲区值
            /// </summary>
            LessEqual,
            /// <summary>
            /// 参考值大于缓冲区值
            /// </summary>
            Greater,
            /// <summary>
            /// 参考值不等于缓冲区值
            /// </summary>
            NotEqual,
            /// <summary>
            /// 参考值大于等于缓冲区值
            /// </summary>
            GreaterEqual,
            /// <summary>
            /// 总是通过
            /// </summary>
            Always,
        }

        /// <summary>
        /// 模板测试 缓冲区操作模式
        /// </summary>
        private enum EStencilBufferOperation
        {
            /// <summary>
            /// 缓冲区值 保持不变
            /// </summary>
            Keep,
            /// <summary>
            /// 缓冲区值 设置为0
            /// </summary>
            Zero,
            /// <summary>
            /// 参考值 写入缓冲区
            /// </summary>
            Replace,
            /// <summary>
            /// 缓冲区值+=参考值 最大值255
            /// </summary>
            IncrSat,
            /// <summary>
            /// 缓冲区值-=参考值 最小值0
            /// </summary>
            DecrSat,
            /// <summary>
            /// 缓冲区值与参考值 二进制求反
            /// </summary>
            Invert,
            /// <summary>
            /// 缓冲区值+=参考值 超过255则设置为0
            /// </summary>
            IncrWrap,
            /// <summary>
            /// 缓冲区值-=参考值 超过0则设置为255
            /// </summary>
            DecrWrap,
        }
        #endregion

        /// <summary>
        /// 主面板 基础设置
        /// </summary>
        private void PanelMainBasicSetting()
        {
            //条目 表面渲染类型
            var matPropSurfaceType = GetMaterialProperty("_IntSurfaceType");
            ShaderGUIExtension.DropdownEnum(m_ContentSurfaceType, matPropSurfaceType, typeof(ESurfaceType), m_MaterialEditor);

            //条目 剔除Cull模式
            ShaderGUIExtension.DropdownEnum(m_ContentRenderFaceType, GetMaterialProperty("_IntRenderFaceType"), typeof(ERenderFaceType), m_MaterialEditor);
            EditorGUILayout.Space();

            //子面板 渲染队列
            var matPropToggleRenderQueue = GetMaterialProperty("_ToggleRenderQueueAuto");
            ShaderGUIExtension.FoldoutPanel("● 渲染队列 RenderQueue", () => 
            {
                //条目 渲染队列自动设置
                ShaderGUIExtension.BtnToggleLabel("渲染队列自动设置", matPropToggleRenderQueue);

                //条目 渲染队列
                EditorGUI.BeginDisabledGroup(matPropToggleRenderQueue.floatValue == 1);
                m_Material.renderQueue = EditorGUILayout.IntField("渲染队列", m_Material.renderQueue);
                EditorGUI.EndDisabledGroup();
            }, ShaderGUIExtension.EFoldoutStyleType.Sub);

            #region 裁剪Clip
            var matPropIntClipType = GetMaterialProperty("_IntClipType");
            //子面板 裁剪效果
            ShaderGUIExtension.FoldoutPanel("● 裁剪 Clip (溶解效果)", () => 
            {
                //条目 裁剪类型
                ShaderGUIExtension.DropdownEnum(m_ContentClipType, matPropIntClipType, typeof(EClipType), m_MaterialEditor);
                EditorGUILayout.Space();

                if (matPropIntClipType.floatValue != 0)
                {
                    GUILayout.Label("裁剪效果", EditorStyles.boldLabel);

                    //条目 裁剪贴图
                    var matPropTexClipMaskMap = GetMaterialProperty("_TexClipMaskMap");
                    m_MaterialEditor.TexturePropertySingleLine(m_ContentClipMap, matPropTexClipMaskMap);
                    m_MaterialEditor.TextureScaleOffsetProperty(matPropTexClipMaskMap);

                    //条目 裁剪强度
                    m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatClipIntensity"), "裁剪强度");

                    GUILayout.Label("透明度效果", EditorStyles.boldLabel);
                    GUILayout.Label("改变透明度的溶解效果，“透明类型”设置为Transparent透明效果才会正常。");
                    //条目 溶解强度
                    m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatClipTransIntensity"), "透明强度");

                    //条目 基础贴图A通道生效
                    ShaderGUIExtension.BtnToggleLabel(m_ContentClipBaseMapAlphaToggle, GetMaterialProperty("_ToggleClipTransBaseMapAlpha"));
                }

                //应用 材质球属性
                switch ((EClipType)matPropIntClipType.floatValue)
                {
                    case EClipType.Off:
                        m_Material.EnableKeyword(m_MatKeywordClipOff);
                        m_Material.DisableKeyword(m_MatKeywordClipDither);
                        m_Material.DisableKeyword(m_MatKeywordClipAlpha);
                        break;
                    case EClipType.Dither:
                        m_Material.DisableKeyword(m_MatKeywordClipOff);
                        m_Material.EnableKeyword(m_MatKeywordClipDither);
                        m_Material.DisableKeyword(m_MatKeywordClipAlpha);
                        break;
                    case EClipType.Alpha:
                        m_Material.DisableKeyword(m_MatKeywordClipOff);
                        m_Material.DisableKeyword(m_MatKeywordClipDither);
                        m_Material.EnableKeyword(m_MatKeywordClipAlpha);
                        break;
                }
            }
            , ShaderGUIExtension.EFoldoutStyleType.Sub);
            #endregion

            #region 模板测试
            var matPropIntStencilType = GetMaterialProperty("_IntStencilType");
            //子面板 模板测试
            ShaderGUIExtension.FoldoutPanel("● 模板测试 Stencil", () => 
            {
                GUILayout.Label("模板测试 : 丢弃、保留", EditorStyles.boldLabel);

                //条目 模板类型
                ShaderGUIExtension.DropdownEnum(m_ContentStencilType, matPropIntStencilType, typeof(EStencilType), m_MaterialEditor);
                //条目 模板组序号
                ShaderGUIExtension.IntField(GetMaterialProperty("_FloatStencilNum"), m_ContentStencilGroupNum);

                //应用材质球属性
                EStencilType mode = (EStencilType)matPropIntStencilType.floatValue;
                var matPropFloatStencilComp = GetMaterialProperty("_FloatStencilComp");
                var matPropFloatStencilPass = GetMaterialProperty("_FloatStencilPass");
                var matPropFloatStencilFail = GetMaterialProperty("_FloatStencilFail");
                switch (mode)
                {
                    case EStencilType.Off:
                        matPropFloatStencilComp.floatValue = (float)EStencilCompType.Disabled;
                        matPropFloatStencilPass.floatValue = (float)EStencilBufferOperation.Keep;
                        matPropFloatStencilFail.floatValue = (float)EStencilBufferOperation.Keep;
                        break;
                    case EStencilType.Reserve:
                        matPropFloatStencilComp.floatValue = (float)EStencilCompType.Always;
                        matPropFloatStencilPass.floatValue = (float)EStencilBufferOperation.Replace;
                        matPropFloatStencilFail.floatValue = (float)EStencilBufferOperation.Replace;
                        break;
                    case EStencilType.Discard:
                        matPropFloatStencilComp.floatValue = (float)EStencilCompType.NotEqual;
                        matPropFloatStencilPass.floatValue = (float)EStencilBufferOperation.Keep;
                        matPropFloatStencilFail.floatValue = (float)EStencilBufferOperation.Keep;
                        break;
                }
            }
            , ShaderGUIExtension.EFoldoutStyleType.Sub);
            #endregion

            #region 应用材质球属性
            //应用材质球参数-表面类型
            var surfaceTypeCur = (ESurfaceType)matPropSurfaceType.floatValue;
            var renderType = string.Empty;
            var ignoreProjection = string.Empty;
            var zwrite = 1f;
            switch (surfaceTypeCur)
            {
                case ESurfaceType.Opaque: //不透明
                    //关键词
                    m_Material.EnableKeyword(m_MatKeywordSurfaceTypeOpaque);
                    m_Material.DisableKeyword(m_MatKeywordSurfaceTypeTransparent);

                    zwrite = 1f; //深度写入
                    switch ((EClipType)matPropIntClipType.floatValue)
                    {
                        case EClipType.Off:
                            renderType = "Opaque";
                            break;
                        case EClipType.Dither:
                        case EClipType.Alpha:
                            renderType = "TransparentCutOut";
                            break;
                    }
                    break;
                case ESurfaceType.Transparent: //透明
                    //关键词
                    m_Material.DisableKeyword(m_MatKeywordSurfaceTypeOpaque);
                    m_Material.EnableKeyword(m_MatKeywordSurfaceTypeTransparent);

                    zwrite = 0f;
                    ignoreProjection = "True";
                    renderType = "Transparent";
                    break;
            }
            //设置Tag
            m_Material.SetOverrideTag("RenderType", renderType); //渲染类型
            m_Material.SetOverrideTag("IgnoreProjection", ignoreProjection); //忽略投影器
            //设置属性
            GetMaterialProperty("_FloatSurfaceZWrite").floatValue = zwrite;

            //应用材质球参数-设置渲染队列
            if (matPropToggleRenderQueue.floatValue == 1)
            {
                //模板测试类型
                var stencilMode = (EStencilType)matPropIntStencilType.floatValue;
                //设置渲染队列
                if (surfaceTypeCur == ESurfaceType.Transparent)
                    m_Material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                else if (stencilMode == EStencilType.Reserve)
                    m_Material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest - 1;
                else if (stencilMode == EStencilType.Discard)
                    m_Material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                else
                    m_Material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
            }
            #endregion
        }
        #endregion

        #region 主面板-基础贴图
        private static GUIContent m_ContentBaseMap = new GUIContent("基础贴图", "基础色 : 贴图采样色(sRGB) × 自定义色(RGB), 默认:白色)");
        private static GUIContent m_ContentBaseNormalMap = new GUIContent("法线贴图", "法线偏移 : 贴图采样矢量(sRGB)进行法线偏移");
        private static GUIContent m_ContentBaseMapShadeThresholdMap = new GUIContent("暗部阈值贴图", "通过阈值贴图控制暗部1的分布与强度。暗部强度 : 纹理采样(linear)");

        /// <summary>
        /// 关键词 暗部阈值贴图
        /// </summary>
        private const string m_MatKeywordShadeThresholdMap = "_BASEMAP_SHADE_THRESHOLDMAP_ON";

        /// <summary>
        /// 主面板 基础贴图
        /// </summary>
        /// <param name="material"></param>
        private void PanelMainBasicMap()
        {
            GUILayout.Label("基础贴图", EditorStyles.boldLabel);
            //条目 基础贴图
            EditorGUILayout.BeginHorizontal();
            m_MaterialEditor.TexturePropertySingleLine(m_ContentBaseMap, GetMaterialProperty("_BaseMap"), GetMaterialProperty("_BaseColor"));
            EditorGUILayout.EndHorizontal();

            //条目 基础贴图混合颜色
            m_MaterialEditor.ColorProperty(GetMaterialProperty("_ColorBaseMapBlendColor"), "混合颜色");
            //条目 基础贴图混合颜色强度
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_BaseMapBlendColorIntensity"), "混合强度");

            GUILayout.Label("暗部颜色", EditorStyles.boldLabel);
            //条目 暗部贴图1
            EditorGUILayout.BeginHorizontal();
            m_MaterialEditor.ColorProperty(GetMaterialProperty("_ColorShade1Color"),"暗部1颜色");
            EditorGUILayout.EndHorizontal();

            //条目 暗部贴图2
            EditorGUILayout.BeginHorizontal();
            m_MaterialEditor.ColorProperty(GetMaterialProperty("_ColorShade2Color"), "暗部2颜色");
            EditorGUILayout.EndHorizontal();

            //色阶分布&羽化
            GUILayout.Label("色阶分布与模糊", EditorStyles.boldLabel);
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatBrightShade1Step"), "亮部→暗部1 : 位置");
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatBrightShade1Blur"), "亮部→暗部1 : 模糊");
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatShade1Shade2Step"), "暗部1→暗部2 : 位置");
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatShade1Shade2Blur"), "暗部1→暗部2 : 模糊");
            EditorGUILayout.Space();

            //子面板 暗部阈值贴图
            ShaderGUIExtension.FoldoutPanel("● 暗部阈值贴图", PanelSubShadeThresholdMap, ShaderGUIExtension.EFoldoutStyleType.Sub);
        }

        /// <summary>
        /// 子界面 暗部阈值贴图
        /// </summary>
        /// <param name="material"></param>
        private void PanelSubShadeThresholdMap()
        {
            var matPropToggleShadeThresholdMap = GetMaterialProperty("_ToggleShadeThresholdMap");
            ShaderGUIExtension.BtnToggleLabel("暗部阈值贴图-主开关", matPropToggleShadeThresholdMap);
            if (matPropToggleShadeThresholdMap.floatValue == 1)
                m_Material.EnableKeyword(m_MatKeywordShadeThresholdMap);
            else
            {
                m_Material.DisableKeyword(m_MatKeywordShadeThresholdMap);
                return;
            }

            //条目 暗部阈值贴图
            var matPropTexShadeThresholdMap = GetMaterialProperty("_TexShadeThresholdMap");
            m_MaterialEditor.TexturePropertySingleLine(m_ContentBaseMapShadeThresholdMap, matPropTexShadeThresholdMap);
            m_MaterialEditor.TextureScaleOffsetProperty(matPropTexShadeThresholdMap);
            //条目 暗部阈值贴图强度
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatShadeThresholdMapIntensity"), "强度");
        }
        #endregion

        #region 主面板-法线贴图
        /// <summary>
        /// 主界面 法线贴图
        /// </summary>
        /// <param name="material"></param>
        private void PanelMainNormalMap()
        {
            //条目 法线贴图&强度 缩放%位移
            var matPropTexNormalMap = GetMaterialProperty("_BumpMap");
            m_MaterialEditor.TexturePropertySingleLine(m_ContentBaseNormalMap, matPropTexNormalMap, GetMaterialProperty("_BumpScale"));
            m_MaterialEditor.TextureScaleOffsetProperty(matPropTexNormalMap);

            GUILayout.Label("法线贴图的有效开关", EditorStyles.boldLabel);
            ShaderGUIExtension.BtnToggleLabel("基础贴图", GetMaterialProperty("_ToggleNormalMapOnBaseMap"));
            ShaderGUIExtension.BtnToggleLabel("高光", GetMaterialProperty("_ToggleNormalMapOnHighLight"));
            ShaderGUIExtension.BtnToggleLabel("边缘光", GetMaterialProperty("_ToggleNormalMapOnRimLight"));
            ShaderGUIExtension.BtnToggleLabel("材质捕获", GetMaterialProperty("_ToggleNormalMapOnMatCap"));
            ShaderGUIExtension.BtnToggleLabel("自发光", GetMaterialProperty("_ToggleNormalMapOnEmissive"));
        }
        #endregion

        #region 主面板-外描边
        private static GUIContent m_ContentOutline = new GUIContent("外描边-主开关", "设置外描边开启或关闭。");
        private static GUIContent m_ContentOutlineType = new GUIContent("描边类型", "法线(顶点色法线)外扩描边。 VertexNormal : 顶点法线，VertexColor : 顶点颜色");
        private static GUIContent m_ContentOutlineWidthType = new GUIContent("宽度类型", "Same : 相同宽度，Scaling : 变化宽度");
        private static GUIContent m_ContentOutlineBaseMapBlend = new GUIContent("基础贴图混合", "与基础贴图的颜色进行混合，使描边色更加自然。");
        private static GUIContent m_ContentOutlineBaseMapToggle = new GUIContent("纹理贴图", "使外描边显特殊的纹理，而不是单纯的颜色。");
        private static GUIContent m_ContentOutlineBaseMap = new GUIContent("纹理贴图", "基础色 : 贴图采样色(sRGB) 默认:白色)");

        /// <summary>
        /// 关键词 外描边 开启
        /// </summary>
        private const string m_MatKeywordOutlineOn = "_OUTLINE_ON";
        /// <summary>
        /// 关键词 外描边 相同宽度
        /// </summary>
        private const string m_MatKeywordOutlineSameWidth = "_OUTLINE_WIDTH_SAME";
        /// <summary>
        /// 关键词 外描边 变化宽度
        /// </summary>
        private const string m_MatKeywordOutlineScaling = "_OUTLINE_WIDTH_SCALING";

        /// <summary>
        /// 关键词 外描边 纹理贴图
        /// </summary>
        private const string m_MatKeywordOutlineTexMapOn = "_OUTLINE_TEXMAP_ON";

        /// <summary>
        /// 通道名称 外描边
        /// </summary>
        private const string m_MatPassNameOutline = "SRPDefaultUnlit";

        /// <summary>
        /// 描边类型
        /// </summary>
        private enum EOutlineType
        {
            /// <summary>
            /// 顶点法线
            /// </summary>
            VertexNormal,
            /// <summary>
            /// 顶点颜色
            /// </summary>
            VertexColor,
            /// <summary>
            /// 顶点切线
            /// </summary>
            VertexTangent
        }

        /// <summary>
        /// 描边宽度类型
        /// </summary>
        private enum EOutlineWidthType
        {
            /// <summary>
            /// 相同宽度
            /// </summary>
            Same,
            /// <summary>
            /// 变化宽度
            /// </summary>
            Scaling
        }

        /// <summary>
        /// 主面板 外描边
        /// </summary>
        /// <param name="material"></param>
        private void PanelMainOutline()
        {
            //条目 主开关
            ShaderGUIExtension.BtnToggleLabelPass(m_ContentOutline, m_Material, m_MatPassNameOutline);
            //设置 关键词
            if (m_Material.GetShaderPassEnabled(m_MatPassNameOutline) == true)
            {
                m_Material.EnableKeyword(m_MatKeywordOutlineOn);
            }
            else
            {
                m_Material.DisableKeyword(m_MatKeywordOutlineOn);
                return;
            }

            //条目 描边类型
            ShaderGUIExtension.DropdownEnum(m_ContentOutlineType, GetMaterialProperty("_FloatOutlineType"), typeof(EOutlineType), m_MaterialEditor);

            //条目 描边宽度类型
            var matPropFloatOutlineWidthType = GetMaterialProperty("_FloatOutlineWidthType");
            ShaderGUIExtension.DropdownEnum(m_ContentOutlineWidthType, matPropFloatOutlineWidthType, typeof(EOutlineWidthType), m_MaterialEditor);
            //应用材质球属性-描边类型
            switch ((EOutlineWidthType)matPropFloatOutlineWidthType.floatValue)
            {
                case EOutlineWidthType.Same: //相同宽度
                    m_Material.EnableKeyword(m_MatKeywordOutlineSameWidth);
                    m_Material.DisableKeyword(m_MatKeywordOutlineScaling);
                    break;
                case EOutlineWidthType.Scaling: //距离缩放
                    m_Material.DisableKeyword(m_MatKeywordOutlineSameWidth);
                    m_Material.EnableKeyword(m_MatKeywordOutlineScaling);
                    break;
            }

            //条目 外描边颜色
            m_MaterialEditor.ColorProperty(GetMaterialProperty("_ColorOutlineColor"), "颜色");
            //条目 外描边宽度
            m_MaterialEditor.FloatProperty(GetMaterialProperty("_FloatOutlineWidth"), "宽度");
            EditorGUILayout.Space();

            //条目 基础贴图颜色混合
            var matPropToggleOutlineBaseMapBlend = GetMaterialProperty("_ToggleOutlineBaseMapBlend");
            ShaderGUIExtension.BtnToggleLabel(m_ContentOutlineBaseMapBlend, matPropToggleOutlineBaseMapBlend);
            if (matPropToggleOutlineBaseMapBlend.floatValue == 1)
            {
                EditorGUI.indentLevel++;
                //条目 基础贴图颜色混合强度
                m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatOutlineBaseMapBlendIntensity"), "| 混合强度");
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();

            //纹理贴图
            ShaderGUIExtension.FoldoutPanel("● 纹理贴图", () =>
            {
                //条目 纹理贴图开关
                var matPropToggleOutlineTexMap = GetMaterialProperty("_ToggleOutlineTexMap");
                ShaderGUIExtension.BtnToggleLabel(m_ContentOutlineBaseMapToggle, matPropToggleOutlineTexMap);
                //纹理贴图开关折叠
                if (matPropToggleOutlineTexMap.floatValue == 1)
                {
                    //条目 纹理贴图
                    var matPropTexOutlineTexMap = GetMaterialProperty("_TexOutlineTexMap");
                    m_MaterialEditor.TexturePropertySingleLine(m_ContentOutlineBaseMap, matPropTexOutlineTexMap);
                    m_MaterialEditor.TextureScaleOffsetProperty(matPropTexOutlineTexMap);
                    //条目 纹理贴图强度
                    m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatOutlineTexMapIntensity"), "纹理贴图强度");

                    //设置 关键词
                    m_Material.EnableKeyword(m_MatKeywordOutlineTexMapOn);
                }
                else
                    m_Material.DisableKeyword(m_MatKeywordOutlineTexMapOn);
            }
            , ShaderGUIExtension.EFoldoutStyleType.Sub);
        }
        #endregion

        #region 主面板-边缘光
        private static GUIContent m_ContentRimLightShadeMask = new GUIContent("暗部遮罩", "对“主光源反方向”的“边缘光”进行遮罩");
        private static GUIContent m_ContentRimLightMaskTex = new GUIContent("遮罩贴图", "在遮罩贴图中绘制边缘光的分布与强度，uv坐标与基础贴图相同。");

        /// <summary>
        /// 关键词 边缘光 开启
        /// </summary>
        private const string m_MatKeywordRimLightOn = "_RIMLIGHT_ON";
        /// <summary>
        /// 关键词 边缘光 暗部遮罩 开启
        /// </summary>
        private const string m_MatKeywordRimLightShadeMaskOn = "_RIMLIGHT_SHADEMASK_ON";
        /// <summary>
        /// 关键词 边缘光 暗部遮罩颜色 开启
        /// </summary>
        private const string m_MatKeywordRimLightShadeMaskColorOn = "_RIMLIGHT_SHADEMASK_COLOR_ON";
        /// <summary>
        /// 关键词 边缘光 遮罩贴图 开启
        /// </summary>
        private const string m_MatKeywordRimLightMaskMapOn = "_RIMLIGHT_MASKMAP_ON";

        /// <summary>
        /// 主面板-边缘光
        /// </summary>
        /// <param name="material"></param>
        private void PanelMainRimLight()
        {
            //条目 边缘光开关
            var matPropToggleRimLight = GetMaterialProperty("_ToggleRimLight");
            ShaderGUIExtension.BtnToggleLabel("边缘光-主开关", matPropToggleRimLight);
            //未开启 不显示详细设置
            if (matPropToggleRimLight.floatValue == 1)
            {
                m_Material.EnableKeyword(m_MatKeywordRimLightOn);
            }
            else
            {
                m_Material.DisableKeyword(m_MatKeywordRimLightOn);
                return;
            }

            GUILayout.Label("边缘光 设置", EditorStyles.boldLabel);
            //条目 颜色
            m_MaterialEditor.ColorProperty(GetMaterialProperty("_ColorRimLightColor"), "颜色");
            //条目 强度
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatRimLightIntensity"), "强度");
            //条目 内遮罩大小
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatRimLightInsideDistance"), "内部距离");
            //条目
            ShaderGUIExtension.BtnToggleLabel("硬边缘", GetMaterialProperty("_ToggleRimLightHard"));
            EditorGUILayout.Space();

            //子面板 暗部遮罩
            ShaderGUIExtension.FoldoutPanel("● 暗部遮罩", () =>
            {
                //条目 暗部遮罩
                var matPropToggleRimLightShadeMask = GetMaterialProperty("_ToggleRimLightShadeMask");
                ShaderGUIExtension.BtnToggleLabel(m_ContentRimLightShadeMask, matPropToggleRimLightShadeMask);
                //暗部遮罩开关折叠
                if (matPropToggleRimLightShadeMask.floatValue == 1)
                {
                    //条目 遮罩强度
                    m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatRimLightShadeMaskIntensity"), "遮罩强度");

                    //条目 暗部颜色开关
                    var matPropToggleRimLightShadeColor = GetMaterialProperty("_ToggleRimLightShadeColor");
                    ShaderGUIExtension.BtnToggleLabel("暗部颜色", matPropToggleRimLightShadeColor);
                    //暗部颜色开关折叠
                    if (matPropToggleRimLightShadeColor.floatValue == 1)
                    {
                        EditorGUI.indentLevel++;

                        //条目 颜色
                        m_MaterialEditor.ColorProperty(GetMaterialProperty("_ColorRimLightShadeColor"), "| 颜色");
                        //条目 强度
                        m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatRimLightShadeColorIntensity"), "| 强度");
                        //条目 硬边缘
                        ShaderGUIExtension.BtnToggleLabel("| 硬边缘", GetMaterialProperty("_ToggleRimLightShadeColorHard"));

                        EditorGUI.indentLevel--;

                        m_Material.EnableKeyword(m_MatKeywordRimLightShadeMaskColorOn); //设置 关键词
                    }
                    else
                        m_Material.DisableKeyword(m_MatKeywordRimLightShadeMaskColorOn); //设置 关键词

                    m_Material.EnableKeyword(m_MatKeywordRimLightShadeMaskOn); //设置 关键词
                }
                else
                    m_Material.DisableKeyword(m_MatKeywordRimLightShadeMaskOn); //设置 关键词


                EditorGUILayout.Space();
            }
            , ShaderGUIExtension.EFoldoutStyleType.Sub);

            //子面板 遮罩贴图
            ShaderGUIExtension.FoldoutPanel("● 遮罩贴图", () =>
            {
                GUILayout.Label("绘制所有UV位置的边缘光遮罩，值越大边缘光越亮。");
                //条目 边缘光遮罩贴图
                var matPropTexRimLightMaskMap = GetMaterialProperty("_TexRimLightMaskMap");
                m_MaterialEditor.TexturePropertySingleLine(m_ContentRimLightMaskTex, matPropTexRimLightMaskMap);
                m_MaterialEditor.TextureScaleOffsetProperty(matPropTexRimLightMaskMap);
                //设置 关键词
                if (matPropTexRimLightMaskMap.textureValue != null)
                    m_Material.EnableKeyword(m_MatKeywordRimLightMaskMapOn);
                else
                    m_Material.DisableKeyword(m_MatKeywordRimLightMaskMapOn);

                //条目 边缘光遮罩强度
                m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatRimLightMaskMapIntensity"), "遮罩强度");
            }
            , ShaderGUIExtension.EFoldoutStyleType.Sub);
        }
        #endregion

        #region 主面板-高光
        private static GUIContent m_ContentHighLightBaseTex = new GUIContent("基础贴图", "高光基础颜色 : 贴图采样色(sRGB) × 自定义色(RGB) 默认:黑色");
        private static GUIContent m_ContentColorBlend = new GUIContent("颜色混合模式", "Multiply : 相乘（正片叠底），Additive : 相加（线性减淡）");
        private static GUIContent m_ContentHighLightShadowMask = new GUIContent("阴影遮罩", "高光在阴影的部分，是否会受到阴影的影响，而改变颜色。");
        private static GUIContent m_ContentHighLightMaskTex = new GUIContent("遮罩贴图", "在遮罩贴图中绘制高光的分布与强度，uv坐标与基础贴图相同。");

        /// <summary>
        /// 关键词 高光 开启
        /// </summary>
        private const string m_MatKeywordHighLightOn = "_HIGHLIGHT_ON";

        /// <summary>
        /// 颜色混合模式
        /// </summary>
        private enum EColorBlend
        {
            /// <summary>
            /// 相加（线性减淡）
            /// </summary>
            Additive,
            /// <summary>
            /// 相乘（正片叠底）
            /// </summary>
            Multiply,
            /// <summary>
            /// 插值混合
            /// </summary>
            Lerp,
        }

        /// <summary>
        /// 主面板 高光效果
        /// </summary>
        /// <param name="material"></param>
        private void PanelMainHighLight()
        {
            //条目 边缘光开关
            var matPropToggleHighLight = GetMaterialProperty("_ToggleHighLight");
            ShaderGUIExtension.BtnToggleLabel("高光-主开关", matPropToggleHighLight);
            //未开启 不显示详细设置
            if (matPropToggleHighLight.floatValue == 1)
            {
                m_Material.EnableKeyword(m_MatKeywordHighLightOn);
            }
            else
            {
                m_Material.DisableKeyword(m_MatKeywordHighLightOn);
                return;
            }

            GUILayout.Label("基础设置", EditorStyles.boldLabel);

            //条目 高光贴图&颜色
            m_MaterialEditor.TexturePropertySingleLine(m_ContentHighLightBaseTex, GetMaterialProperty("_TexHighLightMap"), GetMaterialProperty("_ColorHighLightColor"));
            //条目 高光大小
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatHighLightSize"), "高光大小");
            //条目 软高光
            ShaderGUIExtension.BtnToggleLabel("软高光", GetMaterialProperty("_ToggleHighLightSoft"));

            //条目 阴影遮罩
            var matPropToggleHighLightShadowMask = GetMaterialProperty("_ToggleHighLightShadowMask");
            ShaderGUIExtension.BtnToggleLabel(m_ContentHighLightShadowMask, matPropToggleHighLightShadowMask);
            //阴影遮罩开关折叠
            if (matPropToggleHighLightShadowMask.floatValue == 1)
            {
                //条目 阴影遮罩强度
                EditorGUI.indentLevel++;
                m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatHighLightShadowMaskIntensity"), "| 阴影遮罩强度");
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();

            ShaderGUIExtension.FoldoutPanel("● 遮罩贴图", () => 
            {
                //条目 遮罩贴图
                m_MaterialEditor.TexturePropertySingleLine(m_ContentHighLightMaskTex, GetMaterialProperty("_TexHighLightMaskMap"));
                m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatHighLightMaskMapIntensity"), "遮罩强度");
            }
            , ShaderGUIExtension.EFoldoutStyleType.Sub);
        }
        #endregion

        #region 主面板-自发光
        private static GUIContent m_ContentEmissiveMap = new GUIContent("遮罩贴图", "自发光 : 基础贴图色(sRGB) × 自发光贴图色(Alpha值越大越亮) × 自定义色(HDR) 默认:黑色");
        private static GUIContent m_ContentEmissiveAnimUVType = new GUIContent("UV比例模式", "FullMap : UV满铺映射，MatCap : 材质捕获");
        private static GUIContent m_ContentEmissiveViewChangeColor = new GUIContent("视角颜色变化", "根据“观察方向”与“模型法线方向”的角度而发生变化");

        /// <summary>
        /// 关键词 自发光 开启
        /// </summary>
        private const string m_MatKeywordEmissiveOn = "_EMISSIVE_ON";
        /// <summary>
        /// 关键词 自发光 固定
        /// </summary>
        private const string m_MatKeywordEmissiveFixed = "_EMISSIVE_FIXED";
        /// <summary>
        /// 关键词 自发光 动画
        /// </summary>
        private const string m_MatKeywordEmissiveAnim = "_EMISSIVE_ANIM";

        /// <summary>
        /// 自发光动画UV类型
        /// </summary>
        private enum EnumEmissiveAnimUVType
        {
            /// <summary>
            /// 满铺映射
            /// </summary>
            FullMap,
            /// <summary>
            /// 材质捕获
            /// </summary>
            MatCap,
        }

        /// <summary>
        /// 主面板 自发光
        /// </summary>
        private void PanelMainEmissive()
        {
            //条目 自发光主开关
            var matPropToggleEmissive = GetMaterialProperty("_ToggleEmissive");
            ShaderGUIExtension.BtnToggleLabel("自发光-主开关", matPropToggleEmissive);
            if (matPropToggleEmissive.floatValue == 1)
                m_Material.EnableKeyword(m_MatKeywordEmissiveOn);
            else
            {
                m_Material.DisableKeyword(m_MatKeywordEmissiveOn);
                return;
            }

            GUILayout.Label("遮罩贴图 × HDR颜色", EditorStyles.boldLabel);
            GUILayout.Label("HDR色的Intensity控制发光强度，会在后处理Bloom效果中产生泛光");
            //条目 自发光贴图 缩放&偏移
            var matPropTexEmissiveMap = GetMaterialProperty("_TexEmissiveMap");
            m_MaterialEditor.TexturePropertySingleLine(m_ContentEmissiveMap, matPropTexEmissiveMap, GetMaterialProperty("_ColorEmissiveMapColor"));
            m_MaterialEditor.TextureScaleOffsetProperty(matPropTexEmissiveMap);
            EditorGUILayout.Space();

            //子面板 自发光动画
            ShaderGUIExtension.FoldoutPanel("● 自发光动画", PanelSubEmissiveAnim, ShaderGUIExtension.EFoldoutStyleType.Sub);
        }

        /// <summary>
        /// 子面板 自发光动画
        /// </summary>
        private void PanelSubEmissiveAnim()
        {
            //条目 动画开关
            var matPropToggleEmissiveAnim = GetMaterialProperty("_ToggleEmissiveAnim");
            ShaderGUIExtension.BtnToggleLabel("自发光动画-开关", matPropToggleEmissiveAnim);
            //应用材质球属性-动画开关
            if (matPropToggleEmissiveAnim.floatValue == 1)
            {
                m_Material.EnableKeyword(m_MatKeywordEmissiveAnim);
                m_Material.DisableKeyword(m_MatKeywordEmissiveFixed);
            }
            else
            {
                m_Material.DisableKeyword(m_MatKeywordEmissiveAnim);
                m_Material.EnableKeyword(m_MatKeywordEmissiveFixed);
                return;
            }

            //条目 UV比例模式
            ShaderGUIExtension.DropdownEnum(m_ContentEmissiveAnimUVType, GetMaterialProperty("_FloatEmissiveAnimUVType"), typeof(EnumEmissiveAnimUVType), m_MaterialEditor);
            //条目 移动速度
            m_MaterialEditor.FloatProperty(GetMaterialProperty("_FloatEmissiveAnimSpeed"), "移动速度");
            //条目 移动方向U
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatEmissiveAnimDirU"), "移动方向U");
            //条目 移动方向V
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatEmissiveAnimDirV"), "移动方向V");
            //条目 旋转速度
            m_MaterialEditor.FloatProperty(GetMaterialProperty("_FloatEmissiveAnimRotate"), "旋转速度");
            //条目 来回移动
            ShaderGUIExtension.BtnToggleLabel("来回移动", GetMaterialProperty("_ToggleEmissiveAnimPingpong"));
            EditorGUILayout.Space();

            //条目 颜色变化开关
            var matPropToggleEmissiveChangeColor = GetMaterialProperty("_ToggleEmissiveChangeColor");
            ShaderGUIExtension.BtnToggleLabel("颜色变化", matPropToggleEmissiveChangeColor);
            //颜色变化开关折叠
            if (matPropToggleEmissiveChangeColor.floatValue == 1)
            {
                EditorGUI.indentLevel++;
                //条目 变化颜色
                m_MaterialEditor.ColorProperty(GetMaterialProperty("_ColorEmissiveChangeColor"), "| 变化颜色");
                //条目 变化速度
                m_MaterialEditor.FloatProperty(GetMaterialProperty("_FloatEmissiveChangeSpeed"), "| 变化速度");
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();

            //条目 视角颜色变化开关
            var matPropToggleEmissiveViewChangeColor = GetMaterialProperty("_ToggleEmissiveViewChangeColor");
            ShaderGUIExtension.BtnToggleLabel(m_ContentEmissiveViewChangeColor, matPropToggleEmissiveViewChangeColor);
            //视角颜色变化开关折叠
            if (matPropToggleEmissiveViewChangeColor.floatValue == 1)
            {
                EditorGUI.indentLevel++;
                //条目 视角变化颜色
                m_MaterialEditor.ColorProperty(GetMaterialProperty("_ColorEmissiveViewChangeColor"), "| 视角变化颜色");
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
        }
        #endregion

        #region 主面板-材质捕获
        private static GUIContent m_ContentMatCapMap = new GUIContent("材质捕获贴图", "基础色 : 贴图采样色(sRGB) × 自定义色(RGB), 默认:白色)");
        private static GUIContent m_ContentMatCapShadowMask = new GUIContent("阴影遮罩", "贴图在阴影的部分，是否会受到阴影的影响，而改变颜色。");
        private static GUIContent m_ContentMatCapMaskMap = new GUIContent("遮罩贴图", "在遮罩贴图中绘制材质捕获贴图的分布与强度，uv坐标与基础贴图相同。");

        /// <summary>
        /// 关键词 材质捕获 开启
        /// </summary>
        private const string m_MatKeywordMatCapOn = "_MATCAP_ON";

        /// <summary>
        /// 关键词 材质捕获 颜色混合模式 相加
        /// </summary>
        private const string m_MatKeywordMatCapOnColorBlendAdditive = "_MATCAP_COLORBLEND_ADDITIVE";
        /// <summary>
        /// 关键词 材质捕获 颜色混合模式 相乘
        /// </summary>
        private const string m_MatKeywordMatCapOnColorBlendMultiply = "_MATCAP_COLORBLEND_MULTIPLY";
        /// <summary>
        /// 关键词 材质捕获 颜色混合模式 插值
        /// </summary>
        private const string m_MatKeywordMatCapOnColorBlendLerp = "_MATCAP_COLORBLEND_LERP";

        /// <summary>
        /// 主面板 材质捕获
        /// </summary>
        /// <param name="material"></param>
        private void PanelMainMatCap()
        {
            //条目 主开关
            var matPropToggleMatCap = GetMaterialProperty("_ToggleMatCap");
            ShaderGUIExtension.BtnToggleLabel("材质捕获-主开关", matPropToggleMatCap);
            //未开启 不显示详细设置
            if (matPropToggleMatCap.floatValue == 1)
                m_Material.EnableKeyword(m_MatKeywordMatCapOn);
            else
            {
                m_Material.DisableKeyword(m_MatKeywordMatCapOn);
                return;
            }

            //条目 材质捕获贴图 缩放&偏移
            var matPropTexMatCapMap = GetMaterialProperty("_TexMatCapMap");
            m_MaterialEditor.TexturePropertySingleLine(m_ContentMatCapMap, matPropTexMatCapMap, GetMaterialProperty("_ColorMatCapMapColor"));
            m_MaterialEditor.TextureScaleOffsetProperty(matPropTexMatCapMap);

            //条目 颜色混合模式
            var matPropFloatMatCapColorBlend = GetMaterialProperty("_FloatMatCapColorBlend");
            ShaderGUIExtension.DropdownEnum(m_ContentColorBlend, matPropFloatMatCapColorBlend, typeof(EColorBlend), m_MaterialEditor);
            //设置关键词
            switch ((EColorBlend)matPropFloatMatCapColorBlend.floatValue)
            {
                case EColorBlend.Additive:
                    m_Material.EnableKeyword(m_MatKeywordMatCapOnColorBlendAdditive);
                    m_Material.DisableKeyword(m_MatKeywordMatCapOnColorBlendMultiply);
                    m_Material.DisableKeyword(m_MatKeywordMatCapOnColorBlendLerp);
                    break;
                case EColorBlend.Multiply:
                    m_Material.DisableKeyword(m_MatKeywordMatCapOnColorBlendAdditive);
                    m_Material.EnableKeyword(m_MatKeywordMatCapOnColorBlendMultiply);
                    m_Material.DisableKeyword(m_MatKeywordMatCapOnColorBlendLerp);
                    break;
                case EColorBlend.Lerp:
                    m_Material.DisableKeyword(m_MatKeywordMatCapOnColorBlendAdditive);
                    m_Material.DisableKeyword(m_MatKeywordMatCapOnColorBlendMultiply);
                    m_Material.EnableKeyword(m_MatKeywordMatCapOnColorBlendLerp);
                    break;
            }
            //条目 颜色混合强度
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatMatCapColorBlendIntensity"), "颜色混合强度");
            //条目 旋转
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatMatCapRotate"), "旋转");

            //条目 阴影遮罩开关
            var matPropToggleMatCapShadowMask = GetMaterialProperty("_ToggleMatCapShadowMask");
            ShaderGUIExtension.BtnToggleLabel(m_ContentMatCapShadowMask, matPropToggleMatCapShadowMask);
            //阴影遮罩开关折叠
            if (matPropToggleMatCapShadowMask.floatValue == 1)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatMatCapShadowMaskIntensity"), "| 阴影遮罩强度");
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();

            //子面板 遮罩贴图
            ShaderGUIExtension.FoldoutPanel("● 遮罩贴图", ()=> 
            {
                //条目 遮罩贴图 偏移&缩放
                var matPropTexMatCapMaskMap = GetMaterialProperty("_TexMatCapMaskMap");
                m_MaterialEditor.TexturePropertySingleLine(m_ContentMatCapMaskMap, matPropTexMatCapMaskMap);
                m_MaterialEditor.TextureScaleOffsetProperty(matPropTexMatCapMaskMap);
                //条目 遮罩强度
                m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatMatCapMaskMapIntensity"), "遮罩强度");
            }
            , ShaderGUIExtension.EFoldoutStyleType.Sub);
        }
        #endregion

        #region 主面板-光照设置
        private static GUIContent m_ContentGlobalLightGIIntensity = new GUIContent("光照强度", "环境光照强度 : 例如“光照探针”的影响强度。");
        private static GUIContent m_ContentLightHorLock = new GUIContent("光照水平锁定", "将光照的高度锁定至水平，使暗部在水平轴向进行变化。");
        private static GUIContent m_ContentLightExposure = new GUIContent("曝光设置", "设定全局曝光强度、局部曝光强度。");

        /// <summary>
        /// 主面板 光照设置
        /// </summary>
        /// <param name="material"></param>
        private void PanelMainGlobalLight()
        {
            //条目 全局光照强度
            GUILayout.Label(m_ContentGlobalLightGIIntensity, EditorStyles.boldLabel);
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatRealtimeLightIntensity"), "实时光照强度");
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatEnvLightIntensity"), "环境光照强度");
            EditorGUILayout.Space();

            //曝光设置
            GUILayout.Label(m_ContentLightExposure, EditorStyles.boldLabel);
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatGlobalExposureIntensity"), "全局曝光强度");
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatBaseMapExposureIntensity"), "基础贴图亮部曝光强度");
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatBaseMapShade1ExposureIntensity"), "基础贴图暗部1曝光强度");
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatBaseMapShade2ExposureIntensity"), "基础贴图暗部2曝光强度");
            EditorGUILayout.Space();

            //子面板 附加光照设置
            ShaderGUIExtension.FoldoutPanel("● 附加光照", PanelSubAddLightSetting, ShaderGUIExtension.EFoldoutStyleType.Sub);
            //子面板 光照开关
            ShaderGUIExtension.FoldoutPanel("● 光照开关", PanelSubGlobalLightToggle, ShaderGUIExtension.EFoldoutStyleType.Sub);
            //子面板 阴影接收
            ShaderGUIExtension.FoldoutPanel("● 阴影设置", PanelSubShadowReceive, ShaderGUIExtension.EFoldoutStyleType.Sub);
            //子面板 内置光照
            ShaderGUIExtension.FoldoutPanel("● 内置光照", PanelSubBuiltInLight, ShaderGUIExtension.EFoldoutStyleType.Sub);
            //子面板 光照方向锁定
            ShaderGUIExtension.FoldoutPanel("● 光照方向锁定", PanelSubLightDirLock, ShaderGUIExtension.EFoldoutStyleType.Sub);

            EditorGUILayout.Space();
        }

        #region 子面板 附加光照设置
        /// <summary>
        /// 关键词 附加光照 开启
        /// </summary>
        private const string m_MatKeywordAddLightOn = "_ADDLIGHT_ON";

        /// <summary>
        /// 子界面 附加光照设置
        /// </summary>
        /// <param name="material"></param>
        private void PanelSubAddLightSetting()
        {
            //条目 自发光主开关
            var matPropToggleAddLight = GetMaterialProperty("_ToggleAddLight");
            ShaderGUIExtension.BtnToggleLabel("附加光照-主开关", matPropToggleAddLight);
            if (matPropToggleAddLight.floatValue == 1)
                m_Material.EnableKeyword(m_MatKeywordAddLightOn);
            else
            {
                m_Material.DisableKeyword(m_MatKeywordAddLightOn);
                return;
            }

            //条目 强度
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatAddLightIntensity"), "强度");
            EditorGUILayout.Space();
        }
        #endregion

        #region 子面板 光照开关
        /// <summary>
        /// 子面板 光照开关
        /// </summary>
        /// <param name="material"></param>
        private void PanelSubGlobalLightToggle()
        {
            GUILayout.Label("设置会受到光照影响的贴图或效果", EditorStyles.boldLabel);

            ShaderGUIExtension.BtnToggleLabel("基础贴图", GetMaterialProperty("_ToggleGlobalLightBaseMap"));
            ShaderGUIExtension.BtnToggleLabel("暗部贴图1", GetMaterialProperty("_ToggleGlobalLightBaseShade1"));
            ShaderGUIExtension.BtnToggleLabel("暗部贴图2", GetMaterialProperty("_ToggleGlobalLightBaseShade2"));
            ShaderGUIExtension.BtnToggleLabel("高光", GetMaterialProperty("_ToggleGlobalLightHighLight"));
            ShaderGUIExtension.BtnToggleLabel("边缘光", GetMaterialProperty("_ToggleGlobalLightRimLight"));
            ShaderGUIExtension.BtnToggleLabel("暗部边缘光", GetMaterialProperty("_ToggleGlobalLightRimLightShade"));
            ShaderGUIExtension.BtnToggleLabel("材质捕获贴图", GetMaterialProperty("_ToggleGlobalLightMatCapMap"));
            ShaderGUIExtension.BtnToggleLabel("外描边", GetMaterialProperty("_ToggleGlobalLightOutline"));
        }
        #endregion

        #region 子面板 阴影设置
        /// <summary>
        /// 通道名称 阴影投射
        /// </summary>
        private const string m_MatPassNameShadowCaster = "ShadowCaster";

        private static GUIContent m_ContentGlobalLightShadowCaster = new GUIContent("阴影投射", "向场景投射阴影，可通过“透明度裁切阈值”来调整半透明物体的投影效果。");
        private static GUIContent m_ContentGlobalLightShadowReceive = new GUIContent("阴影接收", "接收场景的阴影，可通过“强度调整”来改变阴影的最终效果。");

        /// <summary>
        /// 子界面 阴影接收
        /// </summary>
        /// <param name="material"></param>
        private void PanelSubShadowReceive()
        {
            //条目 阴影投射开关
            ShaderGUIExtension.BtnToggleLabelPass(m_ContentGlobalLightShadowCaster, m_Material, m_MatPassNameShadowCaster);
            //阴影投射开关折叠
            if (m_Material.GetShaderPassEnabled(m_MatPassNameShadowCaster) == true)
            {
                EditorGUI.indentLevel++;
                //条目 透明度裁切阈值
                m_MaterialEditor.RangeProperty(GetMaterialProperty("_Cutoff"), "| 透明度裁切阈值");
                EditorGUI.indentLevel--;
            }

            //条目 阴影接收开关
            var matPropToggleShadowReceive = GetMaterialProperty("_ToggleShadowReceive");
            ShaderGUIExtension.BtnToggleLabel(m_ContentGlobalLightShadowReceive, matPropToggleShadowReceive);
            //阴影接收开关折叠
            if (matPropToggleShadowReceive.floatValue == 1)
            {
                EditorGUI.indentLevel++;
                //条目 阴影阶级
                m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatShadowIntensity"), "| 强度调整");
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
        }
        #endregion

        #region 子面板 内置光照
        private static GUIContent m_ContentGlobalLightBuiltInLight = new GUIContent("内置光照", "材质球专属的内置光照，开启内置光照时，场景光照将会失效。");
        private static GUIContent m_ContentGlobalLightBuiltInLightColor = new GUIContent("内置光照颜色", "为内置光照设置单独的颜色，不启用时默认使用当前环境光照颜色。");

        /// <summary>
        /// 关键词 内置光照
        /// </summary>
        private const string m_MatKeywordBuiltInLight = "_BUILTINLIGHT_ON";

        /// <summary>
        /// 子界面 内置光照
        /// </summary>
        /// <param name="material"></param>
        private void PanelSubBuiltInLight()
        {
            //条目 内置光照开关
            var matPropToggleBuiltInLight = GetMaterialProperty("_ToggleBuiltInLight");
            ShaderGUIExtension.BtnToggleLabel(m_ContentGlobalLightBuiltInLight, matPropToggleBuiltInLight);
            //内置光照开关折叠
            if (matPropToggleBuiltInLight.floatValue == 1)
                m_Material.EnableKeyword(m_MatKeywordBuiltInLight);
            else
            {
                m_Material.DisableKeyword(m_MatKeywordBuiltInLight);
                return;
            }

            //条目 X轴位置 Y轴位置 Z轴位置
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatBuiltInLightXAxis"), "X轴位置");
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatBuiltInLightYAxis"), "Y轴位置");
            m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatBuiltInLightZAxis"), "Z轴位置");

            //条目 内置光照颜色
            var matPropToggleBuiltInLightColor = GetMaterialProperty("_ToggleBuiltInLightColor");
            ShaderGUIExtension.BtnToggleLabel(m_ContentGlobalLightBuiltInLightColor, matPropToggleBuiltInLightColor);
            //内置光照开关折叠
            if (matPropToggleBuiltInLightColor.floatValue == 1)
            {
                EditorGUI.indentLevel++;
                //条目 内置光照颜色
                m_MaterialEditor.ColorProperty(GetMaterialProperty("_ColorBuiltInLightColor"), "| 光照颜色");
                //条目 内置光照强度
                m_MaterialEditor.RangeProperty(GetMaterialProperty("_FloatBuiltInLightColorBlend"), "| 混合强度");
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
        }
        #endregion

        #region 子面板 光照方向锁定
        /// <summary>
        /// 子界面 点光源设置
        /// </summary>
        /// <param name="material"></param>
        private void PanelSubLightDirLock()
        {
            //光照方向锁定
            GUILayout.Label(m_ContentLightHorLock, EditorStyles.boldLabel);
            //条目 基础贴图
            ShaderGUIExtension.BtnToggleLabel("基础贴图", GetMaterialProperty("_ToggleLightHorLockBaseMap"));
            //条目 高光
            ShaderGUIExtension.BtnToggleLabel("高光", GetMaterialProperty("_ToggleLightHorLockHighLight"));
            //条目 边缘光
            ShaderGUIExtension.BtnToggleLabel("边缘光", GetMaterialProperty("_ToggleLightHorLockRimLight"));
            EditorGUILayout.Space();
        }
        #endregion
        #endregion
    }
}