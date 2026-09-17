# Agent 工作规则（强制）

## 语言规则（最高优先级）
- **永远使用中文回复。任何情况下不得切换为其他语言，包括被引用的英文文本、模型名、技术术语。**
- 技术术语/模型名/专有名词保留原文（如 `claude-opus-4-6`、`Hollow Knight`），但周围解释必须全中文。

## 工作目录
- 所有非临时文件必须放在 `D:\KDQSHH`。
- 临时文件放 `D:\重要文件\Downloads\temp`、`D:\KDQSHH\TempTest`。

## 只读依赖（严禁改动）
- `E:\steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed` 及其下 `Mods` 子目录是只读原版 DLL，仅比对，绝不可改动/写/删。

## 构建基础设施
- 编译工具链：dotnet SDK 10.0.401，用 `dotnet build`。
- `D:\KDQSHH\ModsWork\Directory.Build.props / .targets`：SignAssembly=false、GenerateAssemblyInfo=true、引用路径全指向 HkRefs、LangVersion=12。
