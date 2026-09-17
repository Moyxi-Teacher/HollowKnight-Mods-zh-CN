# 模组汉化交付说明

本目录包含 Hollow Knight 模组的汉化交付文件。

## 交付目录结构

每个模组的 DLL 文件位于其对应的汉化目录下：

- `Mods\MenderbugOptions（蛀虫选项）\` - MenderbugOptions.dll
- `Mods\MylaFlower（米拉花）\` - MylaFlower.dll
- ...（其他模组）

## 汉化说明

依据**方案B**：所有模组显示名 `GetName()` 已汉化为中文，影响：
- 游戏内模组列表显示
- 存档键名（modData/ModEnabledSettings）

## 版本号

- AssemblyVersion / FileVersion = 原版数字
- InformationalVersion = 原版数字 + "汉化" 后缀
- PublicKeyToken = null（未签名）