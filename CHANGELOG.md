# 【更新日志】
**AleToonURP插件功能的新增、优化与修复**

## [1.1.0] - 2022/11/28
---
### **AleToon/Lit**
#### 新增
- 支持SRP-Batcher
- **[阴影投射]** 开启与关闭，透明物体投射阴影的透明度阈值。
#### 优化
- Shader宏命令优化。提升Shader变体性能。
#### 修复
- **[材质捕获 MatCap]** 遮罩贴图的遮罩效果不正确。

<br/>
<br/>

## [1.2.0] - 2022/12/2
---
### **AleToon/Lit**
#### 新增
- 示例材质球。
#### 优化
- Shader文件命名规范化。
#### 修复
- **[法线贴图 NormalMap]** 法线贴图效果不正确。

### **AleToon/WaterPlane**
#### 新增
- NPR水面渲染Shader。
- 示例材质球。
- 自定义ShaderGUI界面。
- **[基础 Basic]** 水体的深水颜色与浅水颜色，会根据深度进行插值过渡。
- **[水波 Wave]** 水面的波浪表现、强度、速度等。
- **[反射 Reflect]** 水面的反射贴图、强度、模糊、菲涅尔效果等。
- **[折射 Refract]** 水底物体的折射效果。
- **[边缘泡沫 EdgeFoam]** 水面与岸边接触的边缘位置的泡沫，设置阈值表现、距离、模糊、透明度等。

<br/>
<br/>

## [1.2.1] - 2022/12/4
---
### **AleToon/Lit**
#### 优化
- **[自发光 Emissive]** 限制最小自发光强度。
#### 修复
- **[自发光 Emissive]** 动画时，自发光区域的uv坐标不正确。

<br/>
<br/>

## [1.2.2] - 2026/07/18
---
### **AleToon/Lit**
#### 优化
- 精简 Shader 关键词：移除未使用的 URP 模板关键词，删除 URP14 已失效的 `_CLUSTERED_RENDERING` 与无用的 `_OUTLINE_THRESHOLDMAP_ON`，减少 Shader 变体。
- 清理死代码：未使用的 `_DETAIL` 宏分支、`hash21` 函数与冗余赋值。
#### 修复
- **[光照 Light]** 附加光照累加变量未初始化，在无附加光源时可能产生异常亮度或噪点。
- **[外描边 Outline]** 描边 Pass 在 GPU Instancing / XR 立体渲染变体下的编译问题（片元变量名错误、结构体缺少 instancing/stereo 成员）。

### **AleToon/WaterPlane**
#### 修复
- **[折射 Refract]** 改用 URP 标准的 `SampleSceneColor` 采样摄像机不透明纹理（原 `tex2D(_CameraColorTexture)` 为非规范、跨平台不可用的写法），折射效果现可正确生效。
- **[基础 Basic]** 修正 SubShader 的 `RenderPipeline` 标签为 `UniversalPipeline`。
- 修正法线贴图 `_ST` 的 CBUFFER 类型（`float3` → `float4`，恢复 SRP-Batcher 兼容），以及非规范的插值器语义（`TRXCOORD`/`TANGENT` → `TEXCOORD`）。

### **ShaderGUI（编辑器）**
#### 优化
- 材质面板每帧使用最新属性重建，避免 Undo/切换材质后属性对象过期。
- 缓存折叠面板 GUIStyle，减少编辑器 GC 分配。
- 修正 `LabelTextColor` 未使用传入样式参数的问题。