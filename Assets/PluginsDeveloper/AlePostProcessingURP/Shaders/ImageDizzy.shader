Shader "Hidden/Custom/ImageDizzy"
{
HLSLINCLUDE
    // This file contains the "Luminance" which we use to get the grayscale value
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
	#include "Assets/PluginsDeveloper/AlePostProcessingURP/ShaderLibrary/Core.hlsl"

    TEXTURE2D_X(_MainTex);
    float4 _MainTex_TexelSize;

    float _Range;
    float _Speed;

    float4 Frag(PostProcessVaryings i) : SV_Target
    {
        float offset = sin(_Time.y * _Speed) * _Range;
        float4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.texcoord);
        float4 colorL = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.texcoord + float2(offset, 0));
        float4 colorR = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.texcoord - float2(offset, 0));
        color.rgb = (color.rgb + colorL.rgb + colorR.rgb) / 3;
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
