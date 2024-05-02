Shader "Hidden/Custom/Distort"
{
HLSLINCLUDE
    // This file contains the "Luminance" which we use to get the grayscale value
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
    #include "Assets/PluginsDeveloper/AlePostProcessingURP/ShaderLibrary/Core.hlsl"

    TEXTURE2D_X(_MainTex);
    float4 _MainTex_TexelSize;

    sampler2D _DistortTex;
    half4 _DistortTex_ST;
    half2 _DistortTiling;
    half _DistortAmount;
    half2 _DistortVelocity;
    half _GradualLengthX;
    half _GradualAnchorX;
    half _GradualLengthY;
    half _GradualAnchorY;

    //水面反光
    float4 _SpecularColor;
    float _SpecularAmount;
    float _SpecularScale;

    float4 Frag(PostProcessVaryings i) : SV_Target
    {
        //折射偏移
        float2 samp = tex2D(_DistortTex, i.texcoord * _DistortTiling + _DistortVelocity * _Time.y).xy ; //扰动纹理采样
        half2 offset = lerp(half2(0, 0), samp - half2(0.5, 0.5), _DistortAmount); //偏移值 插值控制强度

        //强度渐变
        offset = offset * saturate(_GradualLengthX - (max(0, _GradualAnchorX) - i.texcoord.x * _GradualAnchorX));
        offset = offset * saturate(_GradualLengthY - (max(0, _GradualAnchorY) - i.texcoord.y * _GradualAnchorY));

        //主纹理采样
        float4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.texcoord + offset);
        
        //水面反光
        if(_SpecularAmount < 1)
        {
            float2 samp1 = tex2D(_DistortTex, i.texcoord * _DistortTiling * _SpecularScale - _DistortVelocity * 1.5 * _Time.y).xy;
            float s = samp1.x + samp1.y;
            s = s * samp.x;
            float minNum = max(0, s - _SpecularAmount); //范围限制
            float minS = sign(minNum);
            float specular1 = s * minS;

            float2 samp2 = tex2D(_DistortTex, i.texcoord * _DistortTiling * _SpecularScale - float2(-_DistortVelocity.x, _DistortVelocity.y) * 1.5 * _Time.y).xy;
            float s2 = samp2.x + samp2.y;
            s2 = s2 * tex2D(_DistortTex, i.texcoord * _DistortTiling + float2(_DistortVelocity.x, -_DistortVelocity.y) * 1.5 * _Time.y).x;
            minNum = max(0, s2 - _SpecularAmount);
            minS = sign(minNum);
            float specular2 = s2 * minS;

            float4 specularColor = (specular1 + specular2) * _SpecularColor;
            color = color + specularColor * _SpecularColor.a;
        }

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
