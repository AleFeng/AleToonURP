
Shader "AleToonURP/Lit" 
{
    Properties
    {
//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 基础设置 Basic ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        //基础
        [Enum(Off,0,On,1)] _IntSurfaceType("Surface Type", int) = 0 //表面类型 0不透明/1透明 ●仅用于记录 设置关键词开启
        [Enum(Off,0,On,1)]	_FloatSurfaceZWrite("Surface ZWrite", int) = 1 //表面 深度写入
        [Enum(Both,0,Back,1,Front,2)] _IntRenderFaceType("RenderFace Type", int) = 2 //渲染面类型 双面/反面/正面
        //渲染队列
        _ToggleRenderQueueAuto("RenderQueue Auto", int) = 1 //开关 渲染队列自动设置 ●仅用于记录在UnityEditor执行材质球参数修改
        //模板测试
        [Enum(Off,0,Discard,1,Reserve,2)] _IntStencilType("Stencil Type", int) = 0 //模板测试类型 关闭/丢弃/保留 ●仅用于记录在UnityEditor执行材质球参数修改
        _FloatStencilNum("Stencil No", Float) = 1 //模板组序号 同组材质球才会互相影响
        _FloatStencilComp("Stencil Comparison", Float) = 8 //模板 比较规则
        _FloatStencilPass("Stencil Operation", Float) = 0 //模板 通过的写入规则
        _FloatStencilFail("Stencil Operation", Float) = 0 //模板 失败的写入规则
        //裁剪
        [Enum(Off,0,Clip,1,Alpha,2)] _IntClipType ("Clip Type",int) = 0 //裁剪类型 关闭/裁剪/透明 ●仅用于记录在UnityEditor执行材质球参数修改
        _TexClipMaskMap("Clip MaskMap", 2D) = "white" {} //贴图 裁剪遮罩
        _FloatClipIntensity("Clip Intensity", Range(0, 1)) = 0 //裁剪强度
        _FloatClipTransIntensity("ClipTrans Intensity", Range(-1, 1)) = 0 //透明强度
        _ToggleClipTransBaseMapAlpha("ClipTrans BaseMapAlpha", Float) = 0 //基础贴图的A通道生效
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 基础设置 Basic ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 基础贴图 BaseMap ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        //基础纹理
        _BaseMap ("BaseMap", 2D) = "white" {} //基础贴图
        [HDR]_BaseColor ("BaseMap Color", Color) = (1, 1, 1, 1) //基础贴图 颜色

        [HDR]_ColorBaseMapBlendColor ("MainTex ColorBlend", Color) = (1, 1, 1, 1) //基础贴图 混合颜色
        _BaseMapBlendColorIntensity ("MainTex BlendIntensity", Range(0,1)) = 0 //基础贴图 混合颜色强度

        [HDR]_ColorShade1Color ("Shade1 Color", Color) = (0.7, 0.7, 0.7, 1) //暗部1颜色
        [HDR]_ColorShade2Color ("Shade2 Color", Color) = (0.4, 0.4, 0.4, 1) //暗部2颜色

        //色阶分布与模糊
        _FloatBrightShade1Step ("BaseShade1 Step", Range(0, 1)) = 0.5 //基础→暗部1 位置
        _FloatBrightShade1Blur ("BaseShade1 Blur", Range(0.0001, 3)) = 0.01 //基础→暗部1 羽化
        _FloatShade1Shade2Step ("Shade1Shade2 Step", Range(0, 1)) = 0.2 //暗部1→暗部2 位置
        _FloatShade1Shade2Blur ("Shade1Shade2 Blur", Range(0.0001, 3)) = 0.01 //暗部1→暗部2 羽化

        //暗部阈值贴图
        [Toggle(_)] _ToggleShadeThresholdMap ("Shade ThresholdMap Toggle", Float) = 0 //开关 暗部阈值贴图
        _TexShadeThresholdMap ("Shade ThresholdMap ", 2D) = "white" {} //暗部阈值贴图
        _FloatShadeThresholdMapIntensity ("Shade ThresholdMap Intensity", Range(0, 1)) = 0.5 //暗部阈值贴图 强度
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 基础贴图 BaseMap ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 法线贴图 NormalMap ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        _BumpMap ("Bump Map", 2D) = "bump" {} //法线贴图
        _BumpScale ("Bump Scale", Range(0, 1)) = 1 //强度
        //有效开关
        [Toggle(_)] _ToggleNormalMapOnBaseMap ("NormalMap On BaseMap", Float) = 0 //开关 基础贴图
        [Toggle(_)] _ToggleNormalMapOnHighLight ("NormalMap On HighLight", Float) = 0 //开关 高光
        [Toggle(_)] _ToggleNormalMapOnRimLight ("NormalMap On RimLight", Float) = 0 //开关 边缘光
        [Toggle(_)] _ToggleNormalMapOnMatCap ("NormalMap On MatCap", Float) = 0 //开关 材质捕获
        [Toggle(_)] _ToggleNormalMapOnEmissive ("NormalMap On Emissive", Float) = 0 //开关 自发光 视角变化颜色
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 法线贴图 NormalMap ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 外描边 Outline ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        [KeywordEnum(VertNormal, VertColor)] _FloatOutlineType ("Outline Type", Float) = 0 //外描边类型 ●仅用于记录 设置关键词开启
        [KeywordEnum(Same, Scaling)] _FloatOutlineWidthType ("Outline Width Type", Float) = 0 //外描边宽度类型 ●仅用于记录 设置关键词开启
        [HDR]_ColorOutlineColor ("Outline Color", Color) = (0.5,0.5,0.5,1) //颜色
        _FloatOutlineWidth ("Outline Width", Float ) = 0 //宽度
        [Toggle(_)] _ToggleOutlineBaseMapBlend ("Outline BaseMapBlend", Float ) = 0 //开关 基础贴图混合
        _FloatOutlineBaseMapBlendIntensity ("Outline BaseMapBlend Intensity", Range(0, 1) ) = 1 //基础贴图混合 强度
        //纹理贴图
        [Toggle(_)] _ToggleOutlineTexMap ("Outline TexMap Toggle", Float ) = 0 //开关 纹理贴图 ●仅用于记录 设置关键词开启
        _TexOutlineTexMap ("Outline TexMap", 2D) = "white" {} //纹理贴图
        _FloatOutlineTexMapIntensity ("Outline TexMap Intensity", Range(0, 1) ) = 1 //纹理贴图 强度
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 外描边 Outline ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 边缘光 RimLight ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        [Toggle(_)] _ToggleRimLight ("RimLight Toggle", Float ) = 0 //开关 边缘光 ●仅用于记录 设置关键词开启
        [HDR]_ColorRimLightColor ("RimLight Color", Color) = (1,1,1,1) //颜色
        _FloatRimLightIntensity ("RimLight Intensity", Range(0, 1)) = 0.1 //强度
        _FloatRimLightInsideDistance ("RimLight Inside Distance", Range(0.0001, 1)) = 0.0001 //内部距离
        [Toggle(_)] _ToggleRimLightHard ("RimLight Hard", Float ) = 0 //开关 硬边缘
        //暗部遮罩
        [Toggle(_)] _ToggleRimLightShadeMask ("RimLight ShadeMask Toggle", Float ) = 0 //开关 暗部遮罩 ●仅用于记录 设置关键词开启
        _FloatRimLightShadeMaskIntensity ("RimLight ShadeMask Intensity", Range(0, 1)) = 0 //暗部遮罩强度
        [Toggle(_)] _ToggleRimLightShadeColor ("RimLight ShadeColor", Float ) = 0 //开关 暗部颜色 ●仅用于记录 设置关键词开启
        [HDR]_ColorRimLightShadeColor ("RimLight ShadeColor", Color) = (1,1,1,1) //暗部颜色
        _FloatRimLightShadeColorIntensity ("RimLight ShadeColor Intensity", Range(0, 1)) = 0.1 //暗部颜色 强度
        [Toggle(_)] _ToggleRimLightShadeColorHard ("RimLight ShadeColor Hard", Float ) = 0 //暗部颜色 硬边缘
        //遮罩贴图
        _TexRimLightMaskMap ("RimLight MaskMap", 2D) = "white" {} //遮罩贴图
        _FloatRimLightMaskMapIntensity ("RimLight MaskMap Intensity", Range(-1, 1)) = 0 //遮罩贴图 强度
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 边缘光 RimLight ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 高光 HighLight ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        [Toggle(_)] _ToggleHighLight ("HighLight Toggle", Float ) = 0 //开关 高光 ●仅用于记录 设置关键词开启
        _TexHighLightMap ("HighLight Map", 2D) = "white" {} //高光贴图 颜色
        _ColorHighLightColor ("HighLight Color", Color) = (0,0,0,1) //高光颜色
        _FloatHighLightSize ("HighLight Size", Range(0, 1)) = 0 //高光大小
        [Toggle(_)] _ToggleHighLightSoft ("HighLight Soft", Float ) = 0 //开关 软高光
        //阴影遮罩
        [Toggle(_)] _ToggleHighLightShadowMask ("HighLight ShadowMask", Float ) = 0 //开关 阴影遮罩
        _FloatHighLightShadowMaskIntensity ("HighLight ShadowMask Intensity", Range(0, 1)) = 0 //阴影遮罩强度
        //遮罩贴图&强度
        _TexHighLightMaskMap ("HighLight MaskMap", 2D) = "white" {} //高光遮罩贴图
        _FloatHighLightMaskMapIntensity ("HighLight MaskMap Intensity", Range(-1, 1)) = 0 //高光遮罩贴图强度
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 高光 HighLight ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 自发光 Emissive ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        [Toggle(_)] _ToggleEmissive ("Emissive Toggle", Float ) = 0 //开关 自发光 ●仅用于记录 设置关键词开启
        _TexEmissiveMap ("EmissiveMap", 2D) = "white" {} //自发光贴图
        [HDR]_ColorEmissiveMapColor ("EmissiveMap Color", Color) = (0,0,0,1) //自发光 颜色
        //自发光动画
        [KeywordEnum(Fixed, Animation)] _ToggleEmissiveAnim("EmissiveAnim Toggle", Float) = 0 //开关 动画 ●仅用于记录在UnityEditor执行材质球参数修改
        _FloatEmissiveAnimUVType ("EmissiveAnim UVType", Float) = 0 //UV比例类型
        _FloatEmissiveAnimSpeed ("EmissiveAnim Speed", Float ) = 0.5 //动画速度
        _FloatEmissiveAnimDirU ("EmissiveAnim DirU", Range(-1, 1)) = 0.5 //动画方向U
        _FloatEmissiveAnimDirV ("EmissiveAnim DirV", Range(-1, 1)) = 0.5 //动画方向V
        _FloatEmissiveAnimRotate ("EmissiveAnim Rotate", Float ) = 0 //旋转速度
        [Toggle(_)] _ToggleEmissiveAnimPingpong ("EmissiveAnim Pingpong", Float) = 0 //开关 来回移动
        //变化颜色
        [Toggle(_)] _ToggleEmissiveChangeColor ("Emissive ChangeColor Toggle", Float) = 0 //开关 变化颜色
        [HDR]_ColorEmissiveChangeColor ("Emissive ChangeColor", Color) = (0,0,0,1) //变化颜色
        _FloatEmissiveChangeSpeed ("Emissive ChangeSpeed", Float ) = 0 //变化速度
        //视角变化颜色
        [Toggle(_)] _ToggleEmissiveViewChangeColor ("Emissive ViewChangeColor Toggle", Float) = 0 //开关 视角变化颜色
        [HDR]_ColorEmissiveViewChangeColor ("Emissive ViewChangeColor", Color) = (0,0,0,1) //视角变化颜色
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 自发光 Emissive ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 材质捕获 Matcap ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        [Toggle(_)] _ToggleMatCap ("MatCap Toggle", Float ) = 0 //开关 材质捕获 ●仅用于记录 设置关键词开启
        _TexMatCapMap ("MatCapMap", 2D) = "black" {} //材质捕获贴图
        [HDR]_ColorMatCapMapColor ("MatCapMap Color", Color) = (1,1,1,1) //材质捕获 颜色
        _FloatMatCapColorBlend ("MatCap ColorBlend", Float ) = 1 //颜色混合模式
        _FloatMatCapColorBlendIntensity ("MatCap ColorBlend Intensity", Range(0, 1)) = 1 //颜色混合强度
        _FloatMatCapRotate ("MatCap Rotate", Range(-1, 1)) = 0 //旋转
        //阴影遮罩
        [Toggle(_)] _ToggleMatCapShadowMask ("MatCap ShadowMask Toggle", Float ) = 0 //开关 阴影遮罩
        _FloatMatCapShadowMaskIntensity ("MatCap ShadowMask Intensity", Range(0, 1)) = 0 //阴影遮罩强度
        //遮罩贴图
        _TexMatCapMaskMap ("MatCap MaskMap", 2D) = "white" {} //遮罩贴图
        _FloatMatCapMaskMapIntensity ("MatCap MaskMap Intensity", Range(-1, 1)) = 0 //遮罩贴图强度
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 材质捕获 Matcap ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 光照设置 Light ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        _FloatRealtimeLightIntensity ("Realtime Light Intensity", Range(0, 10)) = 5 //实时光照强度
        _FloatEnvLightIntensity ("Environment Light Intensity", Range(0, 10)) = 2 //环境光照强度

		//曝光设置
		_FloatGlobalExposureIntensity ("Global Exposure Intensity", Range(0.001, 10)) = 1 //全局曝光强度
		_FloatBaseMapExposureIntensity ("BaseMap Intensity", Range(0.001, 10)) = 2 //基础贴图亮部曝光强度
		_FloatBaseMapShade1ExposureIntensity ("BaseMapShade1 Intensity", Range(0.001, 10)) = 1.2 //基础贴图暗部1曝光强度
		_FloatBaseMapShade2ExposureIntensity ("BaseMapShade2 Intensity", Range(0.001, 10)) = 1 //基础贴图暗部2曝光强度
		
		//光照水平锁定
        [Toggle(_)] _ToggleLightHorLockBaseMap ("HorizontalLock BaseMap", Float ) = 0 //光照水平锁定 基础贴图
        [Toggle(_)] _ToggleLightHorLockHighLight ("HorizontalLock HighLight", Float ) = 0 //光照水平锁定 高光
        [Toggle(_)] _ToggleLightHorLockRimLight ("HorizontalLock RimLight", Float ) = 0 //光照水平锁定 边缘光
        
        //附加光照设置
		[Toggle(_)] _ToggleAddLight ("PointLight HighLight", Float ) = 1 //开关 附加光照 ●仅用于记录 设置关键词开启
        _FloatAddLightIntensity ("PointLight Intensity", Range(0, 2)) = 1 //附加光照强度

        //光照开关
        [Toggle(_)] _ToggleGlobalLightBaseMap ("GlobalLight BaseMap Toggle", Float) = 1 //基础贴图
        [Toggle(_)] _ToggleGlobalLightBaseShade1 ("GlobalLight BaseShade1 Toggle", Float) = 1 //暗部1
        [Toggle(_)] _ToggleGlobalLightBaseShade2 ("GlobalLight BaseShade2 Toggle", Float) = 1 //暗部2
        [Toggle(_)] _ToggleGlobalLightHighLight ("GlobalLight HighLight Toggle", Float) = 1 //高光
        [Toggle(_)] _ToggleGlobalLightRimLight ("GlobalLight RimLight Toggle", Float) = 1 //边缘光
        [Toggle(_)] _ToggleGlobalLightRimLightShade ("GlobalLight RimLightShade Toggle", Float) = 1 //边缘光暗部
        [Toggle(_)] _ToggleGlobalLightMatCapMap ("GlobalLight MatCapMap Toggle", Float) = 1 //材质捕获
        [Toggle(_)] _ToggleGlobalLightOutline ("GlobalLight Outline Toggle", Float) = 1 //外描边
        
        //阴影设置
        [Toggle(_)] _ToggleShadowCaster ("ShadowCaster Toggle", Float ) = 1 //开关 阴影投射 ●仅用于记录 设置Pass开启
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5 //透明度裁切阈值
        [Toggle(_)] _ToggleShadowReceive ("ShadowReceive Toggle", Float ) = 1 //开关 阴影接收
        _FloatShadowIntensity ("Shadow Intensity", Range(-1, 1)) = 0 //阴影强度

        //内置光照
        [Toggle(_)] _ToggleBuiltInLight ("BuiltInLight Toggle", Float ) = 0 //开关 内置光照
        _FloatBuiltInLightXAxis ("BuiltInLight XAxis", Range(-1, 1)) = 1 //内置光照 水平旋转
        _FloatBuiltInLightYAxis ("BuiltInLight YAxis", Range(-1, 1)) = 1 //内置光照 垂直旋转
        _FloatBuiltInLightZAxis ("BuiltInLight ZAxis", Range(-1, 1)) = -1 //内置光照 垂直旋转
        //内置光照颜色
        [Toggle(_)] _ToggleBuiltInLightColor ("BuiltInLight Color Toggle", Float) = 0 //开关 内置光照
        [HDR]_ColorBuiltInLightColor ("BuiltInLight Color", Color) = (1,1,1,1) //内置光照颜色
        _FloatBuiltInLightColorBlend ("BuiltInLight Color Blend", Range(0, 1)) = 1 //内置光照颜色 混合强度
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 光照设置 Light ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
    }

    SubShader 
    {
        Tags 
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
        }

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 基础渲染 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        Pass 
        {
            Name "ForwardLit"
            Tags {"LightMode" = "UniversalForward"}
            ZWrite [_FloatSurfaceZWrite]
            Cull [_IntRenderFaceType]
            Blend SrcAlpha OneMinusSrcAlpha

            Stencil 
            {
                Ref[_FloatStencilNum]
                Comp[_FloatStencilComp]
                Pass[_FloatStencilPass]
                Fail[_FloatStencilFail]
            }

        HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _PARALLAXMAP
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
            #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
            #pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local_fragment _OCCLUSIONMAP
            #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #pragma shader_feature_local_fragment _SPECULAR_SETUP

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _CLUSTERED_RENDERING

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
        
            //关键词 表面类型
            #pragma shader_feature_local _SURFACETYPE_OPAQUE _SURFACETYPE_TRANSPARENT
            //关键词 裁剪模式
            #pragma shader_feature_local _CLIP_OFF _CLIP_DITHER _CLIP_ALPHA
            //关键词 暗部阈值贴图
            #pragma shader_feature_local _BASEMAP_SHADE_THRESHOLDMAP_ON
            //关键词 外描边
            #pragma shader_feature_local _OUTLINE_ON
            //关键词 边缘光
            #pragma shader_feature_local _RIMLIGHT_ON
            #pragma shader_feature_local _RIMLIGHT_SHADEMASK_ON
            #pragma shader_feature_local _RIMLIGHT_SHADEMASK_COLOR_ON
            #pragma shader_feature_local _RIMLIGHT_MASKMAP_ON
            //关键词 高光
            #pragma shader_feature_local _HIGHLIGHT_ON
            //关键词 自发光
            #pragma shader_feature_local _EMISSIVE_ON
            #pragma shader_feature_local _EMISSIVE_FIXED _EMISSIVE_ANIM
            //关键词 材质捕获
            #pragma shader_feature_local _MATCAP_ON
            #pragma shader_feature_local _MATCAP_COLORBLEND_ADDITIVE _MATCAP_COLORBLEND_MULTIPLY _MATCAP_COLORBLEND_LERP
			//关键词 附加光照
			#pragma shader_feature_local _ADDLIGHT_ON
            //关键词 内置光照
            #pragma shader_feature_local _BUILTINLIGHT_ON

            //顶点/片元着色器
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "LitPropInput.hlsl"
            #include "Function.hlsl"
            #include "LitMain.hlsl"
        ENDHLSL
        }
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 基础渲染 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 外描边 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        Pass 
        {
            Name "Outline"

            Tags 
            {
                "LightMode" = "SRPDefaultUnlit"
            }

			ZWrite On
            Cull Front
            Blend SrcAlpha OneMinusSrcAlpha

            Stencil
            {
                Ref [_FloatStencilNum]
                Comp [_FloatStencilComp]
                Pass [_FloatStencilPass]
                Fail [_FloatStencilFail]
            }

        HLSLPROGRAM
            #pragma target 2.0

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //关键词 外描边
            #pragma shader_feature_local _OUTLINE_ON
            #pragma shader_feature_local _OUTLINE_WIDTH_SAME _OUTLINE_WIDTH_SCALING
            //关键词 裁剪类型
            #pragma shader_feature_local _CLIP_OFF _CLIP_DITHER _CLIP_ALPHA
            //关键词 阈值贴图
            #pragma shader_feature_local _OUTLINE_THRESHOLDMAP_ON
            #pragma shader_feature_local _OUTLINE_TEXMAP_ON

            //顶点/片元着色器
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "LitPropInput.hlsl"
            #include "Function.hlsl"
            #include "LitOutline.hlsl"
        ENDHLSL
        }
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 外描边 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 阴影投射 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_IntRenderFaceType]

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma target 2.0

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            // -------------------------------------
            // Universal Pipeline keywords

            // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "LitPropInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 阴影投射 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 深度贴图 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull[_IntRenderFaceType]

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma target 2.0

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            
            #include "LitPropInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 深度贴图 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
    }

    //自定义Shader编辑器面板
    //CustomEditor "UnityEditor.AleToonURP.ShaderGUI.ShaderGUILit"
}
