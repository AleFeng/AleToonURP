Shader "Hidden/Custom/Ripple"
{
HLSLINCLUDE
    // This file contains the "Luminance" which we use to get the grayscale value
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
	#include "Assets/PluginsDeveloper/AlePostProcessingURP/ShaderLibrary/Core.hlsl"

    TEXTURE2D_X(_MainTex);
    float4 _MainTex_TexelSize;

    half3 _Center1;
    half3 _Center2;
    half3 _Center3;
    half _Height;
    half _Width;
    half _Speed;
    half _HeightAtten;
    half _WidthAtten;

    float waveHeight(float2 position, float2 center, float time)
    {
        //距离起始点长度
        float2 vec = position - center;
        vec.y = vec.y * _MainTex_TexelSize.w / _MainTex_TexelSize.z;
        float len = length(vec);

        //当前时间
        float crtTime = _Time.y - time;
        //到达时间
        float arrivalTime = len / _Speed;
        //周期时间
        float singleTime = _Width / _Speed;
        //结束时间
        float endTime = arrivalTime + singleTime;

        //范围限制
        _HeightAtten = _HeightAtten + 0.01;
        float arrival = max(0, crtTime - arrivalTime);
        half as = sign(arrival);
        float end = max(0, _HeightAtten - crtTime);
        half es = sign(end);

        //计算偏移高度
        float innerTime = crtTime - arrivalTime;
        float height = sin(innerTime / singleTime * TWO_PI / ((len + 0.2) * _WidthAtten)) * as * (pow(end, 2) + (0.01 * max(0, _HeightAtten + 1.5 - crtTime)));
        half heights = sign(height); //正负值
        height = height * (pow(len, _HeightAtten + 2 - end) + (0.0005 * heights * max(0, _HeightAtten * 0.7 - crtTime)));
        return height;
    }

    float allwave(float2 position)
    {
        return
            waveHeight(position, _Center1.xy, _Center1.z) +
            waveHeight(position, _Center2.xy, _Center2.z) +
            waveHeight(position, _Center3.xy, _Center3.z);
    }

    float4 Frag(PostProcessVaryings i) : SV_Target
    {
        float2 uv = UnityStereoTransformScreenSpaceTex(i.texcoord);
        float centerH = allwave(uv);
        float2 offset = float2(centerH, centerH) * _Height;
        float4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, (uv + offset));

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
