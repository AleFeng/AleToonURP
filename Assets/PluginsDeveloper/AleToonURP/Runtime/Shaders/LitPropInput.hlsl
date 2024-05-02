#ifndef UNIVERSAL_LIT_INPUT_INCLUDED

#define UNIVERSAL_LIT_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"

#if defined(_DETAIL_MULX2) || defined(_DETAIL_SCALED)
#define _DETAIL
#endif

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 属性声明 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
    //已在"Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"中定义
    //TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap); //基础贴图
    //TEXTURE2D(_BumpMap); SAMPLER(sampler_BumpMap); //法线贴图
    //
    TEXTURE2D(_TexClipMaskMap); SAMPLER(sampler_TexClipMaskMap); //裁剪贴图

    #if defined(_BASEMAP_SHADE_THRESHOLDMAP_ON)
    TEXTURE2D(_TexShadeThresholdMap); SAMPLER(sampler_TexShadeThresholdMap); //暗部阈值贴图
    #endif

    #if defined(_OUTLINE_ON) && defined(_OUTLINE_TEXMAP_ON)
    TEXTURE2D(_TexOutlineTexMap); SAMPLER(sampler_TexOutlineTexMap); //外描边 纹理贴图
    #endif

    #if defined(_RIMLIGHT_ON) && defined(_RIMLIGHT_MASKMAP_ON)
    TEXTURE2D(_TexRimLightMaskMap); SAMPLER(sampler_TexRimLightMaskMap); //边缘光贴图
    #endif

    #if defined(_HIGHLIGHT_ON)
    TEXTURE2D(_TexHighLightMap); SAMPLER(sampler_TexHighLightMap); //高光贴图
    TEXTURE2D(_TexHighLightMaskMap); SAMPLER(sampler_TexHighLightMaskMap); //高光遮罩贴图
    #endif

    #if defined(_EMISSIVE_ON)
    TEXTURE2D(_TexEmissiveMap); SAMPLER(sampler_TexEmissiveMap); //自发光贴图
    #endif

    #if defined(_MATCAP_ON)
    TEXTURE2D(_TexMatCapMap); SAMPLER(sampler_TexMatCapMap); //材质捕获贴图
    TEXTURE2D(_TexMatCapMaskMap); SAMPLER(sampler_TexMatCapMaskMap); //材质捕获 遮罩贴图贴图
    #endif

    CBUFFER_START(UnityPerMaterial)

    //裁剪Clip
    float4 _TexClipMaskMap_ST; //裁剪遮罩 贴图
    half _FloatClipIntensity; //裁剪强度
    half _FloatClipTransIntensity; //透明强度
    half _ToggleClipTransBaseMapAlpha; //基础贴图的A通道生效

    //基础贴图
    float4 _BaseMap_ST; //基础贴图
    half4 _BaseColor; //基础贴图 颜色
    half4 _ColorBaseMapBlendColor; //基础贴图 混合颜色
    half _BaseMapBlendColorIntensity; //基础贴图 混合颜色强度
    half4 _ColorShade1Color; //暗部1颜色
    half4 _ColorShade2Color; //暗部2颜色
    //色阶分布与模糊
    half _FloatBrightShade1Step; //基础→暗部1 位置
    half _FloatBrightShade1Blur; //基础→暗部1 羽化
    half _FloatShade1Shade2Step; //暗部1→暗部2 位置
    half _FloatShade1Shade2Blur; //暗部1→暗部2 羽化
    //暗部阈值贴图
    float4 _TexShadeThresholdMap_ST; //暗部阈值 贴图
    half _FloatShadeThresholdMapIntensity; //暗部阈值贴图 强度

    //法线贴图
    float4 _BumpMap_ST;
    half _BumpScale; //法线贴图强度
    //有效开关
    half _ToggleNormalMapOnBaseMap; //开关 基础贴图
    half _ToggleNormalMapOnHighLight; //开关 高光
    half _ToggleNormalMapOnRimLight; //开关 边缘光
    half _ToggleNormalMapOnMatCap; //开关 材质捕获
    half _ToggleNormalMapOnEmissive ; //开关 自发光

    //外描边
    half _FloatOutlineType; //外描边类型
    half4 _ColorOutlineColor; //颜色
    half _FloatOutlineWidth; //宽度
    half _ToggleOutlineBaseMapBlend; //开关 基础贴图混合
    half _FloatOutlineBaseMapBlendIntensity; //基础贴图混合强度
    //纹理贴图
    float4 _TexOutlineTexMap_ST; //纹理贴图
    half _FloatOutlineTexMapIntensity; //纹理贴图 强度

    //边缘光
    half _ToggleRimLight; //开关 边缘光
    half4 _ColorRimLightColor; //颜色
    half _FloatRimLightIntensity; //强度
    half _FloatRimLightInsideDistance; //内部距离
    half _ToggleRimLightHard; //开关 硬边缘
    //暗部遮罩
    half _FloatRimLightShadeMaskIntensity; //暗部遮罩强度
    //暗部颜色
    half4 _ColorRimLightShadeColor; //暗部颜色
    half _FloatRimLightShadeColorIntensity; //暗部颜色 强度
    half _ToggleRimLightShadeColorHard; //暗部颜色 硬边缘
    //遮罩贴图
    float4 _TexRimLightMaskMap_ST;
    half _FloatRimLightMaskMapIntensity; //遮罩贴图强度

    //高光
    float4 _TexHighLightMap_ST; //高光贴图
    half4 _ColorHighLightColor; //高光颜色
    half _FloatHighLightSize; //高光大小
    half _ToggleHighLightSoft; //开关 软高光
    //阴影遮罩
    half _ToggleHighLightShadowMask; //开关 阴影遮罩
    half _FloatHighLightShadowMaskIntensity; //阴影遮罩强度
    //遮罩贴图&强度
    float4 _TexHighLightMaskMap_ST; //高光遮罩贴图
    half _FloatHighLightMaskMapIntensity; //高光遮罩贴图强度

    //自发光
    float4 _TexEmissiveMap_ST; //自发光贴图
    half4 _ColorEmissiveMapColor; //自发光 颜色
    //自发光动画
    half _FloatEmissiveAnimUVType; //UV比例类型
    half _FloatEmissiveAnimSpeed; //动画速度
    half _FloatEmissiveAnimDirU; //动画方向U
    half _FloatEmissiveAnimDirV; //动画方向V
    half _FloatEmissiveAnimRotate; //旋转速度
    half _ToggleEmissiveAnimPingpong; //开关 来回移动
    //变化颜色
    half _ToggleEmissiveChangeColor; //开关 变化颜色
    half4 _ColorEmissiveChangeColor; //变化颜色
    half _FloatEmissiveChangeSpeed; //变化速度
    //视角变化颜色
    half _ToggleEmissiveViewChangeColor; //开关 视角变化颜色
    half4 _ColorEmissiveViewChangeColor; //视角变化颜色

    //材质捕获
    float4 _TexMatCapMap_ST; //材质捕获贴图
    half4 _ColorMatCapMapColor; //材质捕获贴图 颜色
    half _FloatMatCapColorBlendIntensity; //颜色混合强度
    half _FloatMatCapRotate; //旋转
    //阴影遮罩
    half _ToggleMatCapShadowMask; //开关 阴影遮罩
    half _FloatMatCapShadowMaskIntensity; //阴影遮罩强度
    //遮罩贴图
    float4 _TexMatCapMaskMap_ST; //遮罩贴图
    half _FloatMatCapMaskMapIntensity; //遮罩贴图强度

    //光照设置
    half _FloatRealtimeLightIntensity; //实时光照强度
    half _FloatEnvLightIntensity; //环境光照强度
    //曝光设置
    half _FloatGlobalExposureIntensity; //全局曝光强度
    half _FloatBaseMapExposureIntensity; //曝光强度
    half _FloatBaseMapShade1ExposureIntensity; //曝光强度
    half _FloatBaseMapShade2ExposureIntensity; //曝光强度
    //光照水平锁定
    half _ToggleLightHorLockBaseMap; //光照水平锁定 基础贴图
    half _ToggleLightHorLockHighLight; //光照水平锁定 高光
    half _ToggleLightHorLockRimLight; //光照水平锁定 边缘光
    //附加光照设置
    half _FloatAddLightIntensity; //附加光照强度
    //光照开关
    half _ToggleGlobalLightBaseMap; //基础贴图
    half _ToggleGlobalLightBaseShade1; //暗部1
    half _ToggleGlobalLightBaseShade2; //暗部2
    half _ToggleGlobalLightOutline; //外描边
    half _ToggleGlobalLightHighLight; //高光
    half _ToggleGlobalLightRimLight; //边缘光
    half _ToggleGlobalLightRimLightShade; //边缘光暗部
    half _ToggleGlobalLightMatCapMap; //材质捕获
    //阴影设置
    half _Cutoff; //阴影投射透明度阈值
    half _ToggleShadowReceive; //开关 阴影接收
    half _FloatShadowIntensity; //阴影接收强度
    //内置光照
    float _FloatBuiltInLightXAxis; //内置光照 X轴位置
    float _FloatBuiltInLightYAxis; //内置光照 Y轴位置
    float _FloatBuiltInLightZAxis; //内置光照 Z轴位置
    half _ToggleBuiltInLightColor; //开关 内置光照颜色
    half4 _ColorBuiltInLightColor; //内置光照颜色
    half _FloatBuiltInLightColorBlend; //内置光照颜色 混合强度

    CBUFFER_END
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 属性声明 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

#endif