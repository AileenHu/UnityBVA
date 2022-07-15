# BVA_skybox_sixSidedExtension

## 概览

天空盒是一种创建背景以使视频游戏关卡看起来比实际更大的方法。 当使用天空盒时，关卡被封闭在一个长方体中。天空、远处的山脉、远处的建筑物和其他无法到达的物体被投影到立方体的表面上（使用一种称为立方体映射的技术），从而创造出远处 3D 环境的错觉。天穹采用相同的概念，但使用球体或半球体而不是立方体。
传统上，这些是简单的立方体，最多可在面上放置六种不同的纹理。通过仔细对齐，位于天空盒正中间的观看者将感知到由这六个面孔组成的真实 3D 世界的错觉。

## 实现

Unity引擎提供了多个Skybox Shader供您使用。每个Shader使用一组不同的属性和生成技术，因此只需要导出材质。

## Properties
|              | 类型         | 描述            | 是否必需             |
|----------------|------------|---------------|----------------------|
|**material**              | `id`        | 材质使用天空盒着色器 | Yes   |