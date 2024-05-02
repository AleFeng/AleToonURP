Shader "Hidden/Custom/CyberFault"
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

    //像素块剔除
    float _CullAmount; //剔除数
    float _CullRandom; //剔除随机值

    //色彩分离
    float _ColorSplitMainTexAmount; //色彩分离强度 主纹理
    float _ColorSplitPixelAmount; //色彩分离强度 像素块内
    float _ColorSplitAddRandom; //色彩分离强度增加

    //动画
    float _AnimEnable; //动画开关
    float _AnimSpeed; //动画速度
    float _AnimPixelScaleRandom; //动画 像素块尺寸随机

    //不连续噪声函数
    float randomNoise(float2 seed)
    {
        return frac(sin(dot(seed + floor((_Time.y % 1.0) * _AnimEnable * _AnimSpeed), float2(127.1, 311.7))) * 43758.5453123);
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
        float uvOffsetX = randomNoise(uvPixel.yy * _PixelRandom); //采样随机 像素块X轴偏移
        interval.x = interval.x * uvOffsetX; //像素块X轴缩放
        //动画 像素块尺寸随机
        if(_AnimEnable == 1)
        {
            interval *= 1 + _AnimPixelScaleRandom * randomNoise(17.3);
        }
        uvPixel = floor((i.texcoord + uvOffsetX) / interval) * interval;

        //偏移采样
        float2 offset = randomNoise(frac(uvPixel * _MainTexOffestRandom)) * _MainTexOffestIntensity;

        //随机偏移方向
        offset.x *= sign(randomNoise(uvPixel * _MainTexOffestRandom * 3) - 0.5);
        offset.y *= sign(randomNoise(uvPixel * _MainTexOffestRandom * 5) - 0.5);

        //随机缩放
        float scale =  1 + randomNoise(uvPixel * 7) * 0.5 * _MainTexOffestIntensity;

        //像素块剔除
        float2 uvMain = i.texcoord;
        float colorSplitAmount = _ColorSplitMainTexAmount;
        if((randomNoise(uvPixel * _CullRandom).x - _CullAmount) > 0)
        {
            uvMain = frac(i.texcoord * scale + offset);
            //像素块内 色彩分离强度随机
            colorSplitAmount = _ColorSplitPixelAmount + randomNoise(uvPixel * 7) * _ColorSplitAddRandom;
        }

        //色彩分离 主纹理采样
        float colorSplitOffset = colorSplitAmount + randomNoise(2.6) * _ColorSplitAddRandom;
        float4 colorR = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uvMain);
        float4 colorG = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uvMain + float2(colorSplitOffset, 0));
        float4 colorB = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uvMain - float2(colorSplitOffset, 0));
        float4 color = float4(colorR.r, lerp(colorR.g, colorG.g, 0.5), lerp(colorR.b, colorB.b, 0.5), colorR.a + colorG.a + colorB.a);

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
