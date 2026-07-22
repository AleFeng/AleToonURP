#ifndef ALETOON_LIT_SHADOW_DEPTH_INCLUDED
#define ALETOON_LIT_SHADOW_DEPTH_INCLUDED

// 阴影投射 / 深度 Pass 的自定义顶点+片元。
// 目的：让阴影与深度的裁剪轮廓与表面(LitMain)一致——使用同一套 _CLIP_* 溶解
// （_TexClipMaskMap + _FloatClipIntensity/_FloatClipTransIntensity），而不是 URP 自带
// 的 _ALPHATEST_ON + _BaseMap.a + _Cutoff（本 Shader 从不启用 _ALPHATEST_ON）。
//
// 复用 URP 的阴影偏移逻辑（ApplyShadowBias），不复制那部分数学，避免与 URP 漂移。
// 由 Pass 通过 #define ALETOON_SHADOWCASTER / ALETOON_DEPTHONLY 选择编译哪一段。
// 所需属性(_TexClipMaskMap / _FloatClip* / _BaseMap / _BaseColor / *_ST / CBUFFER)
// 由 Pass 内先行 include 的 LitPropInput.hlsl 提供。

//——与表面(LitMain 的裁剪段)完全同一口径的溶解裁剪——
//uv 为原始 texcoord（未套 ST），与 LitMain 中 OUT.uv = IN.texcoord 一致
void AleClipDissolve(float2 uv)
{
#if defined(_CLIP_DITHER) //挖孔
    half clipValue = SAMPLE_TEXTURE2D(_TexClipMaskMap, sampler_TexClipMaskMap, TRANSFORM_TEX(uv, _TexClipMaskMap));
    clip(clipValue - _FloatClipIntensity);
#elif defined(_CLIP_ALPHA) //透明度
    half clipValue = SAMPLE_TEXTURE2D(_TexClipMaskMap, sampler_TexClipMaskMap, TRANSFORM_TEX(uv, _TexClipMaskMap));
    half baseAlpha = (SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv) * _BaseColor).a;
    clipValue = lerp(clipValue, clipValue * baseAlpha, _ToggleClipTransBaseMapAlpha);
    clip(clipValue - _FloatClipTransIntensity);
#endif
    //_CLIP_OFF：不裁剪（实心投影/深度）
}

struct AleShadowDepthAttributes
{
    float4 positionOS : POSITION;
    float3 normalOS   : NORMAL;
    float2 texcoord   : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 阴影投射 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
#if defined(ALETOON_SHADOWCASTER)

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl" //ApplyShadowBias

// 阴影 Varyings（与 URP ShadowCaster 一致：不含 stereo 输出，阴影贴图非逐眼渲染）
struct AleShadowVaryings
{
    float2 uv         : TEXCOORD0;
    float4 positionCS : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

// 由 ShadowUtils 设置：方向光用 _LightDirection，点/聚光用 _LightPosition 计算逐顶点光向
float3 _LightDirection;
float3 _LightPosition;

// 复刻 URP ShadowCasterPass 的偏移逻辑（数学取自 ApplyShadowBias，无自研）
float4 AleGetShadowPositionHClip(AleShadowDepthAttributes input)
{
    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

#if _CASTING_PUNCTUAL_LIGHT_SHADOW
    float3 lightDirectionWS = normalize(_LightPosition - positionWS);
#else
    float3 lightDirectionWS = _LightDirection;
#endif

    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

#if UNITY_REVERSED_Z
    positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
#else
    positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
#endif

    return positionCS;
}

AleShadowVaryings AleShadowPassVertex(AleShadowDepthAttributes input)
{
    AleShadowVaryings output = (AleShadowVaryings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);

    output.uv = input.texcoord;
    output.positionCS = AleGetShadowPositionHClip(input);
    return output;
}

half4 AleShadowPassFragment(AleShadowVaryings input) : SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(input);
    AleClipDissolve(input.uv);
    return 0;
}

#endif
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 阴影投射 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 深度 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
#if defined(ALETOON_DEPTHONLY)

// 深度 Varyings（与 URP DepthOnly 一致：含 stereo 输出）
struct AleDepthVaryings
{
    float2 uv         : TEXCOORD0;
    float4 positionCS : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

AleDepthVaryings AleDepthOnlyVertex(AleShadowDepthAttributes input)
{
    AleDepthVaryings output = (AleDepthVaryings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    output.uv = input.texcoord;
    output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
    return output;
}

half AleDepthOnlyFragment(AleDepthVaryings input) : SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    AleClipDissolve(input.uv);
    return input.positionCS.z;
}

#endif
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 深度 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

#endif
