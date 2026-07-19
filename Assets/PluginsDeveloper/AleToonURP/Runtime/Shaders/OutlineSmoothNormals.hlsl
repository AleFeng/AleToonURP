#ifndef ALE_OUTLINE_SMOOTH_NORMALS_INCLUDED
#define ALE_OUTLINE_SMOOTH_NORMALS_INCLUDED

// ═══════════════════════════════════════════════════════════════════════
//  OutlineSmoothNormals.hlsl —— AleToonURP 外描边的平滑法线解码与外扩
//
//  这是一份【自包含】的实现，移植自 OutlineSmoothNormalsGenerator 插件的
//  同名文件（Packages/com.alefeng.outlinesmoothnormalsgenerator/Shader/
//  OutlineSmoothNormals.hlsl）。为避免与该插件产生符号冲突/硬依赖，函数
//  统一使用 AleOSN_ 前缀、独立 include 守卫。数学与来源枚举值与其保持一致，
//  因此可直接使用 OutlineSmoothNormalsGenerator 工具烘焙的平滑法线数据。
//
//  约束：纯数学，不 include 任何管线头文件；UNITY_MATRIX_VP 由调用方所在
//  Shader 已 include 的 URP Core.hlsl 提供。只用 float / half。
//
//  ── 存储格式 ─────────────────────────────────────────────────────────
//  平滑法线一律以【对象空间】完整三维方向存储，不做半球压缩：
//    顶点色     选定通道对(8-bit×2) ← 八面体编码，全球面双射、无符号歧义
//    切线       tangent.xyz          ← 直接存
//    TEXCOORDn  uv.xyz               ← 直接存
//  八面体编码在硬边角点不会像"存 XY + 重建 Z + 顶点法线定符号"那样产生
//  符号歧义，因此描边不会在角点处裂开。
// ═══════════════════════════════════════════════════════════════════════

// ── 顶点色通道对 ─────────────────────────────────────────────────────
#define ALEOSN_VC_RG 0
#define ALEOSN_VC_GB 1
#define ALEOSN_VC_BA 2

// ── 八面体编码 ───────────────────────────────────────────────────────
float2 AleOSN_OctWrap(float2 v)
{
    return (1.0 - abs(v.yx)) * (v.xy >= 0.0 ? 1.0 : -1.0);
}

/// 单位向量 → [0,1]^2。
float2 AleOSN_OctEncode(float3 n)
{
    n /= max(1e-8, abs(n.x) + abs(n.y) + abs(n.z));
    n.xy = (n.z >= 0.0) ? n.xy : AleOSN_OctWrap(n.xy);
    return n.xy * 0.5 + 0.5;
}

/// [0,1]^2 → 单位向量。
float3 AleOSN_OctDecode(float2 f)
{
    f = f * 2.0 - 1.0;
    float3 n = float3(f.x, f.y, 1.0 - abs(f.x) - abs(f.y));
    float  t = saturate(-n.z);
    n.xy += (n.xy >= 0.0) ? -t : t;
    return normalize(n);
}

// ── 顶点色解码：从选定通道对取八面体坐标 ──────────────────────────────
float3 AleOSN_DecodeVertexColor(float4 col, float vcChannel)
{
    int ch = (int)round(vcChannel);
    float2 oct;
    if      (ch == ALEOSN_VC_RG) oct = col.rg;
    else if (ch == ALEOSN_VC_GB) oct = col.gb;
    else                         oct = col.ba;
    return AleOSN_OctDecode(oct);
}

// ── 切线解码：tangent.xyz 直接就是对象空间平滑法线，w 不参与 ───────────
float3 AleOSN_DecodeTangent(float4 tangentOS)
{
    return normalize(tangentOS.xyz);
}

// ── TEXCOORD 解码：uv.xyz 直接就是对象空间平滑法线 ─────────────────────
float3 AleOSN_DecodeTexCoord(float3 uv)
{
    return normalize(uv);
}

// ── 按存储模式选择解码来源（运行时 float 分支）─────────────────────────
//  mode 与材质 _FloatOutlineNormalSource 的下拉一一对应：
//    0 顶点色（默认）  1 切线      2..5 TEXCOORD0..3
//    6 顶点法线（对照，「未使用平滑法线」的效果）
//    7..10 TEXCOORD4..7
float3 AleOSN_SelectSmoothNormalOS(float mode, float4 color, float4 tangentOS,
                                   float3 uv0, float3 uv1, float3 uv2, float3 uv3,
                                   float3 uv4, float3 uv5, float3 uv6, float3 uv7,
                                   float3 normalOS, float vcChannel)
{
    int m = (int)round(mode);
    if      (m == 1)  return AleOSN_DecodeTangent(tangentOS);
    else if (m == 2)  return AleOSN_DecodeTexCoord(uv0);
    else if (m == 3)  return AleOSN_DecodeTexCoord(uv1);
    else if (m == 4)  return AleOSN_DecodeTexCoord(uv2);
    else if (m == 5)  return AleOSN_DecodeTexCoord(uv3);
    else if (m == 6)  return normalize(normalOS);          // 顶点法线（对照）
    else if (m == 7)  return AleOSN_DecodeTexCoord(uv4);
    else if (m == 8)  return AleOSN_DecodeTexCoord(uv5);
    else if (m == 9)  return AleOSN_DecodeTexCoord(uv6);
    else if (m == 10) return AleOSN_DecodeTexCoord(uv7);
    else              return AleOSN_DecodeVertexColor(color, vcChannel);  // 0 = 顶点色（默认）
}

// ── 描边外扩：沿平滑法线在【裁剪空间】外扩，支持两种宽度模式 ────────────
//  只接受【已变换好】的 clipPos 与【逆转置变换后】的 worldNormal。
//    mode = 0（屏幕空间）：描边在屏幕上【等宽】，不随距离变化。
//    mode = 1（世界空间）：按世界单位偏移，近大远小。
//  NaN 保护（仅屏幕空间）：法线正对/背对相机时 xy≈0，未保护的 normalize
//  会产生 NaN，GPU 会丢弃整个三角形（表现为闪烁空洞）。
float4 AleOSN_ApplyOutlineOffset(float4 clipPos, float3 worldNormal, float width, float mode)
{
    float3 n = normalize(worldNormal);
    float4 clipNormal = mul(UNITY_MATRIX_VP, float4(n, 0.0));

    if (mode < 0.5)
    {
        // 屏幕空间：等宽，不随深度变化。
        float2 offsetDir = clipNormal.xy;
        float  dirLen    = length(offsetDir);
        offsetDir = (dirLen > 1e-5) ? (offsetDir / dirLen) : float2(0.0, 0.0);
        clipPos.xy += offsetDir * width * clipPos.w;
    }
    else
    {
        // 世界空间：沿世界法线移动 width 个世界单位，透视除法后近大远小。
        clipPos += clipNormal * width;
    }
    return clipPos;
}

#endif // ALE_OUTLINE_SMOOTH_NORMALS_INCLUDED
