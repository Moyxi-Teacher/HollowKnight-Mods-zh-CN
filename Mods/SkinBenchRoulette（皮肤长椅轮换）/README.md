# Skin Bench Roulette（皮肤长椅轮换）

- **模组原名**：Skin Bench Roulette
- **版本**：1.2.0汉化
- **目标**：汉化模组显示名、菜单文本
- **汉化内容**：
  - 模组显示名 → 皮肤长椅轮换
  - 菜单选项名称 → 使用皮肤长椅轮换、轮换周期 等
  - 菜单选项说明 → 在休息长椅时，根据你已加载的皮肤自动随机更换小骑士的皮肤
- **不改动项**：CustomKnight 皮肤切换逻辑、长椅检测钩子、轮换随机算法
- **Assembly 一致性**：InformationalVersion=1.2.0汉化，AssemblyVersion=1.2.0
- **语言回退**：无额外语言包，代码内嵌中文字符串

---

## 原版 README（翻译）

Skin Bench Roulette 是一个空洞骑士模组，每次你坐在长椅上时会随机更换 Custom Knight 的皮肤！

### 功能
- **完全自动化**：自动扫描你的 `Custom Knight/Skins` 目录。你再也不需要在代码中硬编码皮肤名称了！
- **延迟保护**：等待 Knight 完全坐下后再触发，避免动画 glitches。
- **开放源代码**：使用 Reflection 实现轻量级，独立于特定程序集版本。

### 依赖
- [CustomKnight](https://github.com/hk-modding/modlinks)

## 原始说明的翻译

在长椅界面中轮换皮肤选项。