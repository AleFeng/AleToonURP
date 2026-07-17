# AleToonURP

面向 Unity URP 的高性能 `NPR`（二次元卡通）渲染管线 & Shader。包含功能丰富的 `AleToonURP/Lit` 卡通 Shader
（暗部 / 外描边 / 边缘光 / 高光 / 自发光 / MatCap / 光照设置）与 `AleToonURP/Water Plane` NPR 水面 Shader，
并提供折叠式的自定义 ShaderGUI。

- 📖 完整文档（中文 / English / 日本語）：https://github.com/AleFeng/AleToonURP
- 📝 更新日志：见 [CHANGELOG.md](CHANGELOG.md)

## 环境要求
- Unity `2022.3` 或更高版本
- `Universal Render Pipeline (URP) 14.x`

## 安装（UPM git URL）
在 Unity 中打开 `Window → Package Manager`，点击左上角 `+ → Add package from git URL...`，输入：

```
https://github.com/AleFeng/AleToonURP.git?path=Assets/PluginsDeveloper/AleToonURP
```

## 示例（Samples）
安装后，在 Package Manager 的本包页面展开 `Samples`，点击 `Example Materials & Textures` 的 `Import`，
即可将示例材质与贴图导入到你的工程中。

## 使用
1. 新建材质球，Shader 选择 `AleToonURP/Lit` 或 `AleToonURP/Water Plane`。
2. 在 Inspector 的折叠面板中逐项调节各渲染功能。
3. 详细的参数说明请参考完整文档中的用户手册。

## 许可证
本项目基于 [MIT License](LICENSE.md) 开源。
