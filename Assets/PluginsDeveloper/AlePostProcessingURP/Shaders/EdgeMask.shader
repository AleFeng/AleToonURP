Shader "Hidden/Custom/EdgeMask"
{
HLSLINCLUDE
    // This file contains the "Luminance" which we use to get the grayscale value
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
	#include "Assets/PluginsDeveloper/AlePostProcessingURP/ShaderLibrary/Core.hlsl"

    TEXTURE2D_X(_MainTex);
    float4 _MainTex_TexelSize;

    half4 _Color;
    half _Distance;
	half _Sharpness;
    half _Radian;
	half2 _Center;
    half2 _Toggle;

    float4 Frag(PostProcessVaryings i) : SV_Target
    {
        float4 srcColor = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.texcoord.xy);
		
        //计算距离中心点长度
		half disY = abs(i.texcoord.y - _Center.y);
        half disX = abs(i.texcoord.x - _Center.x);
        half disMask = max(0.0001, _Distance);
        //Y轴方向
        half percentY = (disY + disX * sin(disX) * _Radian) / disMask * _Toggle.y;
        //X轴方向
        half percentX = (disX + disY * sin(disY) * _Radian) / disMask * _Toggle.x;
        //插值混合颜色
        half indensity = smoothstep((_Distance - _Sharpness) / disMask * 0.999, 1, max(percentX, percentY));
		half3 finalColor = lerp(srcColor.rgb, _Color.rgb, indensity * _Color.a);
		
		return float4(finalColor.rgb, 1);
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
