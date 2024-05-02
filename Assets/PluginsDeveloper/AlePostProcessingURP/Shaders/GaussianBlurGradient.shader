Shader "Hidden/Custom/GaussianBlurGradient"
{
HLSLINCLUDE
    // This file contains the "Luminance" which we use to get the grayscale value
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
	#include "Assets/PluginsDeveloper/AlePostProcessingURP/ShaderLibrary/Core.hlsl"

	TEXTURE2D_X(_MainTex);
	float4 _MainTex_TexelSize;

	half4 _BlurOffset; //模糊的颜色采样距离

	//渐变模糊
	float2 _GradientCenter; //渐变中心点
	float2 _GradientRangeX; //渐变开始与结束的范围 X轴
	float2 _GradientRangeY; //渐变开始与结束的范围 Y轴

	struct v2f
	{
		float4 pos: POSITION;
		float2 uv: TEXCOORD0;
		float4 uv01: TEXCOORD1;
		float4 uv23: TEXCOORD2;
		float4 uv45: TEXCOORD3;
	};

	v2f VertGaussianBlur(FullScreenTrianglePostProcessAttributes v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
    	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.pos = GetFullScreenTriangleVertexPosition(v.vertexID);
        o.uv = GetFullScreenTriangleTexCoord(v.vertexID);

//Y轴翻转
//#if UNITY_UV_STARTS_AT_TOP
		//o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
//#endif

		//o.uv = TransformStereoScreenSpaceTex(o.uv, 1.0);

		o.uv01 = o.uv.xyxy + _BlurOffset.xyxy * float4(1, 1, -1, -1);
		o.uv23 = o.uv.xyxy + _BlurOffset.xyxy * float4(1, 1, -1, -1) * 2.0;
		o.uv45 = o.uv.xyxy + _BlurOffset.xyxy * float4(1, 1, -1, -1) * 6.0;

		return o;
	}

	float4 FragGaussianBlur(v2f i) : SV_Target
	{
		half4 color = float4(0, 0, 0, 0);

		//渐变模糊
		float distanceX = abs(i.uv.x - _GradientCenter.x);
		float intensityX = smoothstep(_GradientRangeX.x, _GradientRangeX.y, distanceX);
		float distanceY = abs(i.uv.y - _GradientCenter.y);
		float intensityY = smoothstep(_GradientRangeY.x, _GradientRangeY.y, distanceY);
		float intensity = max(intensityX, intensityY);

		color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv);
		half4 colorBlur = color;
		if(intensity > 0) //模糊强度大于0 才进行模糊采样
		{
			colorBlur = 0.40 * color;
			colorBlur += 0.15 * SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv01.xy);
			colorBlur += 0.15 * SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv01.zw);
			colorBlur += 0.10 * SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv23.xy);
			colorBlur += 0.10 * SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv23.zw);
			colorBlur += 0.05 * SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv45.xy);
			colorBlur += 0.05 * SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv45.zw);
		}

		return lerp(color, colorBlur, intensity);
	}

	float4 FragCombine(PostProcessVaryings i) : SV_Target
	{
		return SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.texcoord);
	}
ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
		HLSLPROGRAM
			#pragma vertex VertGaussianBlur
			#pragma fragment FragGaussianBlur
		ENDHLSL
		}

		Pass
		{
		HLSLPROGRAM
			#pragma vertex FullScreenTrianglePostProcessVertexProgram
			#pragma fragment FragCombine
		ENDHLSL
		}
	}
}