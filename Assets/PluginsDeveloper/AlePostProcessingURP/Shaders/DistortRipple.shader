Shader "Hidden/Custom/DistortRipple" 
{
HLSLINCLUDE
    // This file contains the "Luminance" which we use to get the grayscale value
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
    #include "Assets/PluginsDeveloper/AlePostProcessingURP/ShaderLibrary/Core.hlsl"

    TEXTURE2D_X(_MainTex);
    float4 _MainTex_ST;
	float4 _MainTex_TexelSize;

    //Distort
    half2 _DistortTiling;
    half _DistortAmount;
    half2 _DistortVelocity;
    half _GradualLengthX;
    half _GradualAnchorX;
    half _GradualLengthY;
    half _GradualAnchorY;
    //Ripple
    float3 _Center1;
    float3 _Center2;
    float3 _Center3;
    half _Height;
    half _Width;
    half _Speed;
    half _HeightAtten;
    half _WidthAtten;

    float2 hash22(float2 p)
    {
		p = float2(dot(p,float2(127.1,311.7)),dot(p,float2(269.5,183.3)));
		return -1.0 + 2.0*frac(sin(p)*43758.5453123);
	}

    //噪声函数
    float Noise_perlin(float2 p)
    {				
		float2 pi = floor(p);
		float2 pf = p - pi;
		float2 w = pf * pf*(3.0 - 2.0*pf);
		return lerp(lerp(dot(hash22(pi + float2(0.0, 0.0)), pf - float2(0.0, 0.0)),
			dot(hash22(pi + float2(1.0, 0.0)), pf - float2(1.0, 0.0)), w.x),
			lerp(dot(hash22(pi + float2(0.0, 1.0)), pf - float2(0.0, 1.0)),
			dot(hash22(pi + float2(1.0, 1.0)), pf - float2(1.0, 1.0)), w.x), w.y);
	}

    //水波纹
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
        float height = sin(innerTime / singleTime * TWO_PI  / ((len + 0.2) * _WidthAtten)) * as * (pow(end, 2) + (0.01 * max(0, _HeightAtten + 1.5 - crtTime)));
        half heights = sign(height); //正负值
        height = height * (pow(len, _HeightAtten + 2 - end) + (0.0005 * heights * max(0, _HeightAtten * 0.7 - crtTime)));
                
        return height;
    }

    //3个水波纹
    float allwave(float2 position)
    {
        return
            waveHeight(position, _Center1.xy, _Center1.z) +
            waveHeight(position, _Center2.xy, _Center2.z) +
            waveHeight(position, _Center3.xy, _Center3.z);
    }

    float4 Frag (PostProcessVaryings i) : SV_Target
    {
        //扰动采样
        float2 uv = i.texcoord * _DistortTiling + _DistortVelocity * _Time.y;
        float s = Noise_perlin(uv); //扰动纹理采样
        float sLerp = lerp(0, s, _DistortAmount);
        half2 offset = half2(sLerp, sLerp); //偏移值 插值控制强度
        //强度渐变
        offset = offset * saturate(_GradualLengthX - (max(0, _GradualAnchorX) - i.texcoord.x * _GradualAnchorX));
        offset = offset * saturate(_GradualLengthY - (max(0, _GradualAnchorY) - i.texcoord.y * _GradualAnchorY));
        //水波纹
        float centerH = allwave(i.texcoord);
        
        offset = offset + float2(centerH, centerH) * _Height;

        //主纹理采样
        float4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.texcoord + offset);

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