Shader "Hidden/Custom/Mosaic"
{
HLSLINCLUDE
    // This file contains the "Luminance" which we use to get the grayscale value
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
	#include "Assets/PluginsDeveloper/AlePostProcessingURP/ShaderLibrary/Core.hlsl"

    TEXTURE2D_X(_MainTex);
    float4 _MainTex_TexelSize;
    
    //主纹理偏移
    float _MainTexOffestIntensity; //偏移强度
    float _MainTexOffestRandom; //偏移随机

    //像素块尺寸
    float _PixelSize; //像素尺寸
    float2 _PixelRatio; //像素高宽比
    float _PixelRandom; //像素随机值 X轴偏移量 X轴缩放量

    //不连续噪声函数
    float randomNoise(float2 seed)
    {
        return frac(sin(dot(seed + floor((_Time.y % 1.0)), float2(127.1, 311.7))) * 43758.5453123);
    }

    float randomNoise(float seed)
    {
        return randomNoise(float2(seed, 1.0));
    }

    float4 Frag(PostProcessVaryings i) : SV_Target
    {
        //像素化UV
        float2 interval = _MainTex_TexelSize.xy * _PixelRatio * _PixelSize;
        float2 uvPixel = floor(i.texcoord / interval) * interval;
        //动画 像素块尺寸随机
        uvPixel = floor(i.texcoord / interval) * interval;

        //偏移采样
        float2 offset = randomNoise(frac(uvPixel * _MainTexOffestRandom)) * _MainTexOffestIntensity;

        //随机偏移方向
        offset.x *= sign(randomNoise(uvPixel * _MainTexOffestRandom * 3) - 0.5);
        offset.y *= sign(randomNoise(uvPixel * _MainTexOffestRandom * 5) - 0.5);

        float2 uvMain = uvPixel + offset;

        //主纹理采样
        float4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uvMain);

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
