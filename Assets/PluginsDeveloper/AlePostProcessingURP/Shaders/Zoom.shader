Shader "Hidden/Custom/Zoom"
{
HLSLINCLUDE
    // This file contains the "Luminance" which we use to get the grayscale value
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
	#include "Assets/PluginsDeveloper/AlePostProcessingURP/ShaderLibrary/Core.hlsl"

    TEXTURE2D_X(_MainTex);
    float4 _MainTex_TexelSize;

    float2 _Pos; //中心点
    float _ZoomFactor; //缩放强度
    float _Size; //半径

    float4 Frag(PostProcessVaryings i) : SV_Target
    {
        float2 uv = UnityStereoTransformScreenSpaceTex(i.texcoord);
        //缩放比例
        float2 scale = float2(_ScreenParams.x / _ScreenParams.y, 1);
        //到透镜中心点距离
        float2 center = _Pos;
        float2 dir = center - uv;
        //缩放后的距离
        float disScale = length(dir * scale);
        //透镜采样计算
        float factorFade = 1 + smoothstep(_Size * 0.8, _Size, disScale) * 0.5; //边缘渐变强度 透镜边缘折射率更大
        float atZoomArea = 1 - step(_Size, disScale); //是否在半径内
        float4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv + dir * _ZoomFactor * factorFade * atZoomArea);

        return color;
    }
ENDHLSL

    SubShader
    {
       Cull Off ZWrite Off ZTest Always
       Pass
        {
        HLSLPROGRAM
            #pragma vertex FullScreenTrianglePostProcessVertexProgram
            #pragma fragment Frag
        ENDHLSL
        }
    }
 }

