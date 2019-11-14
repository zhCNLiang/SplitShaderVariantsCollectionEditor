# SplitShaderVariantsCollectionEditor

### 由于Unity变体收集基本都是新建一个个空GameObject并附上对应Shader的材质放到一个场景中导出，导致所有Shader变体都在一个变体收集文件中。在游戏第一次启动WarnUp时，会有严重的卡顿。所以需要把变体分的更细的粒度，通过分帧加载体现在进度条上不至于表现的卡死。

## 测试环境

### Unity 2018.4.9f

