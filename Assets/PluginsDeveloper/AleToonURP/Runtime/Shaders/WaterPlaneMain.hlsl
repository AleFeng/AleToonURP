//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 属性声明 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
    //水波
    TEXTURE2D(_BumpMap); SAMPLER(sampler_BumpMap);
    //边缘泡沫
    TEXTURE2D(_EdgeThresholdMap); SAMPLER(sampler_EdgeThresholdMap);
    //反射
    TEXTURECUBE(_ReflectCubeMap); SAMPLER(sampler_ReflectCubeMap);
    //摄像机帧缓冲贴图
    SAMPLER(_CameraColorTexture);

    CBUFFER_START(UnityPerMaterial)
    
    //基础
    half4 _ColorShallow; //颜色 浅水
    half4 _ColorDeep; //颜色 深水

    //水波
    float3 _BumpMap_ST;     
    half _BumpScale; //法线强度
    float _BumpScaleFir; //贴图缩放 首要
    float _BumpScaleSec; //贴图缩放 次要
    half _FloatWaveSpeedY; //速度 X轴
    half _FloatWaveSpeedX; //速度 Y轴

    //水面反射
    half _ReflectIntensity; //反射强度
    half _ReflectBulrIntensity; //反射模糊强度
    half _FresnelFactor; //菲涅尔系数
    half _FresnelIntensity; //菲涅尔强度

    //水面折射
    half _RefractIntensity; //折射强度

    //边缘泡沫
    float4 _EdgeThresholdMap_ST; //阈值贴图
    half4 _EdgeFoamColor; //泡沫颜色
    half _EdgeFoamThresholdCutoff; //阈值裁切强度
    half _EdgeFoamDis; //边缘泡沫 深度距离
    half _EdgeFoamBlur; //边缘泡沫 模糊

    CBUFFER_END
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 属性声明 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 结构体定义 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
    struct Attributes
    {
        float4 positionOS : POSITION;
        float2 uv         : TEXCOORD0;
        float3 normalOS   : NORMAL;
        float4 tangentOS  : TANGENT;
    };

    struct Varyings
    {  
        float4 positionCS : SV_POSITION;
        float2 uv         : TEXCOORD0;
        float2 uvNoise    : TEXCOORD1; //uv噪声纹理
        float3 positionWS : TRXCOORD2; //世界坐标
        float4 positionSS : TEXCOORD3; //屏幕坐标
        float3 normalWS   : TRXCOORD4; //法线 世界空间
        float4 tangentWS  : TANGENT;
    };
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 属性声明 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

    //顶点着色器
    Varyings vert(Attributes IN)
    {
        Varyings OUT;

        OUT.uv = IN.uv;
        OUT.uvNoise = TRANSFORM_TEX(IN.uv, _EdgeThresholdMap);

        //坐标系转换
        VertexPositionInputs Attributes = GetVertexPositionInputs(IN.positionOS.xyz);
        VertexNormalInputs normalInput = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);

        OUT.positionCS = Attributes.positionCS;
        OUT.positionWS = Attributes.positionWS;
        OUT.positionSS = ComputeScreenPos(Attributes.positionCS); //屏幕坐标
        OUT.normalWS = normalInput.normalWS;
        OUT.tangentWS = half4(normalInput.tangentWS, IN.tangentOS.w * GetOddNegativeScale()); //世界空间的切线

        return OUT;
    }
    
    //片元着色器
    float4 frag(Varyings IN) : SV_TARGET
    {
        //观察方向
        float3 viewDirWS = GetWorldSpaceNormalizeViewDir(IN.positionWS); //观察方向
        
        //主光照
        Light lightMain = GetMainLight(TransformWorldToShadowCoord(IN.positionWS));
        float3 hdir = normalize(lightMain.direction + viewDirWS); //光方向 + 视方向

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ uv动画偏移 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        float2 uvOffset = IN.uv;
        //uv偏移
        float2 moveSpeed = float2(_FloatWaveSpeedY * _Time.x, _FloatWaveSpeedX * _Time.x); //移动速度
        uvOffset -= moveSpeed;
        //交叉采样 重叠法线纹理 使水流效果更丰富
        uvOffset += SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, float2(IN.uv.y, IN.uv.x) + moveSpeed * 0.5).xy;
        float2 offset = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap,  uvOffset)).xy * _BumpScaleSec;
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ uv动画偏移 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 法线贴图采样 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        //切线空间转世界空间
        float2 uvNormal = IN.uv * _BumpScaleFir; //uv缩放
        //交叉采样 时水流效果更丰富
        half3 normalDirTS1 = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uvNormal + offset));
        half3 normalDirTS2 = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uvNormal - offset));
        half3 normalDirTS3 = normalize(normalDirTS1 + normalDirTS2) * _BumpScale;
        //将法线转换至 世界空间
        half3 binormalWS = cross(IN.normalWS, normalDirTS3.xyz) * IN.tangentWS.w; //副切线 世界空间
        half3 normalWS = normalize(mul(normalDirTS3, half3x3(IN.tangentWS.xyz, binormalWS, IN.normalWS)));
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 法线贴图采样 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 深度图采样 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        float2 posSS = IN.positionSS.xy / IN.positionSS.w;
        float depth = LinearEyeDepth(SampleSceneDepth(posSS), _ZBufferParams);
        float depthDis = saturate(depth - IN.positionSS.w); //深度距离
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 深度图采样 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 水颜色 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        //水颜色 深度插值
        float4 colorWater = lerp(_ColorShallow, _ColorDeep, depthDis);
        //颜色混合
	    half4 colorFinal = colorWater;
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 水颜色 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 折射 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        float2 uvRefrac = posSS;
        //自定义折射强度
        uvRefrac.y = uvRefrac.y * (1 + _RefractIntensity) - 0.4 * _RefractIntensity;
        //摄像机贴图采样
        half3 colorRefract = tex2D(_CameraColorTexture, uvRefrac).rgb;
        //颜色混合
	    colorFinal.rgb += colorRefract;
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 折射 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 反射 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        //反射
        float3 viewDirReflect = reflect(-viewDirWS, normalWS); //反射方向
        //反射贴图采样
        half3 colorReflect = SAMPLE_TEXTURECUBE_LOD(_ReflectCubeMap, sampler_ReflectCubeMap, viewDirReflect, _ReflectBulrIntensity).rgb;
        //菲涅尔强度
        half fresnel =  pow(saturate(1 - dot(normalWS, viewDirWS)), 5 - _FresnelFactor * 0.5);
        //最终反射颜色 反射强度 + 菲涅尔强度
        colorReflect = colorReflect * _ReflectIntensity + colorReflect * fresnel * _FresnelIntensity;
        //颜色混合
	    colorFinal.rgb += colorReflect;
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 反射 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 边缘泡沫 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
        //交叉采样
        float thresholdIntensity = SAMPLE_TEXTURE2D(_EdgeThresholdMap, sampler_EdgeThresholdMap, IN.uvNoise + offset).r + SAMPLE_TEXTURE2D(_EdgeThresholdMap, sampler_EdgeThresholdMap, IN.uvNoise - offset).r;
        //边缘泡沫强度
        float foamdepthDis = saturate(depthDis / _EdgeFoamDis); //自定义 边缘泡沫的深度距离
        float thresholdCutoff = foamdepthDis * _EdgeFoamThresholdCutoff; //自定义 裁切阈值
        //平滑插值 自定义边界模糊
        float foamIntensity = smoothstep(thresholdCutoff, thresholdCutoff + _EdgeFoamBlur, thresholdIntensity);
        //边缘泡沫颜色
        half4 colorEdgeFoam = half4(_EdgeFoamColor.rgb, _EdgeFoamColor.a * foamIntensity);
        //颜色混合
        colorFinal = colorFinal * (1 - colorEdgeFoam.a) + colorEdgeFoam * colorEdgeFoam.a;
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 边缘泡沫 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
  	
        return colorFinal;
    }