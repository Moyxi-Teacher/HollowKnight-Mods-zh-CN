# KDQSHH —— Hollow Knight 模组汉化工程 · 云端备份

> 本仓库是原工作目录 `D:\KDQSHH` 的完整快照，用于释放本地磁盘空间。
> 快照时间：2026-09-17

---

## 一、备份总览

| 项目 | 数值 |
|---|---|
| 原目录文件总数 | 17586 |
| 原目录占用（不含 `.git`） | 7.8 GB |
| 仓库内 Git 跟踪文件 | **13041** |
| Release 附件承载文件 | **8**（合计 2.61 GB） |
| 排除：`bin/` `obj/` 编译产物 | 3557 个 |
| 排除：Visual Studio 缓存 `.vs/` | 980 个 |

**文件数校验**：`13041 + 8 + 3557 + 980 = 17586` —— 一个文件都没丢。

**排除理由**

- `bin/`、`obj/`：全部为 `dotnet build` 产物与引用程序集副本，可由源码重建。
  已抽查确认：其中的 `.cs` 均为 `*.AssemblyInfo.cs` / `*.GlobalUsings.g.cs` /
  `*.AssemblyAttributes.cs` 等自动生成文件，`.props`/`.targets` 为 NuGet 恢复产物。
- `.vs/`：Visual Studio 索引、Copilot 快照、DesignTimeBuild 缓存等 IDE 私有数据。

---

## 二、Release 附件（8 个超 100 MiB 文件）

GitHub 对单个文件设 **100 MiB 硬上限**（私有仓库同样适用），以下文件改由 Release 附件承载。
**附件名已改为 ASCII 安全名，下载后请按下表改回原名并放回对应位置。**

| 附件名 | 大小 | 应放回的位置 |
|---|---|---|
| `01_Mods_DoodleBosses.dll` | 261 MB | `Mods/DoodleBosses（涂鸦BOSS）/DoodleBosses.dll` |
| `02_Mods_AudioLoader.dll` | 403 MB | `Mods/Hallownest-Vocalized-AudioLoader（圣巢之声音频加载器）/Hallownest-Vocalized-AudioLoader.dll` |
| `03_Mods_PaleCourt.dll` | 342 MB | `Mods/Pale Court（苍白宫廷）/PaleCourt.dll` |
| `04_HkRefs_PaleCourt.dll` | 342 MB | `ModsWork/HkRefs/Mods/PaleCourt/PaleCourt.dll` |
| `05_backup_PaleCourt_phase77.dll` | 342 MB | `TempTest/Backup_before_PaleCourt_menuStyle_fix/PaleCourt_dll_phase77_backup.dll` |
| `06_TempTest_AudioLoader_decompiled.dll` | 403 MB | `TempTest/decompile/AudioLoader/Hallownest-Vocalized-AudioLoader.dll` |
| `07_png_37_253x250.png` | 327 MB | `TempTest/png_37_253x250.png` |
| `08_modzip_DoodleBosses-master.zip` | 254 MB | `modzip/DoodleBosses-master.zip` |

> 另有 6 个超 100 MiB 文件位于 `bin/`、`obj/` 下（PaleCourt 的编译产物，约 2.05 GB）。
> 按「不上传编译产物」的要求，这些**未纳入备份**，需要时可由源码重新编译生成。

---

## 三、完整还原步骤

### 步骤 1：克隆仓库

```bash
git clone https://github.com/Moyxi/KDQSHH.git "D:/KDQSHH"
```

（远程 `main` 由多个提交组成，是为避开 GitHub 单次 push 体积限制而分批推送的，克隆时自动全部拉取。）

### 步骤 2：下载大文件附件

```bash
gh release download v1.0-full --repo Moyxi/KDQSHH --dir "D:/重要文件/Downloads/temp/kdqshh_release"
```

未安装 `gh` 时，可在仓库 Release 页面手动下载全部附件。

### 步骤 3：按下表放回 8 个文件

见第二节表格，把附件名改回其对应的原始路径与文件名。

### 步骤 4：重建空目录

Git 不跟踪空目录，clone 后下列目录会消失：

```bash
cd "D:/KDQSHH"
grep -v '^#' "_空目录清单.txt" | grep -v '^$' | xargs -I{} mkdir -p "{}"
```

### 步骤 5：恢复行尾设置（**重要**）

`.git/info/attributes` 的 `* -text` 规则**存放在 `.git/` 内，不会随仓库同步**。
若要在新机器上继续提交，必须先重设：

```bash
cd "D:/KDQSHH"
git config core.autocrlf false
git config core.quotepath false
printf '* -text\n' > .git/info/attributes
```

否则模组源码自带的 32 个 `.gitattributes` 会重新启用 CRLF 转换，改写文件哈希，破坏「下载回来字节级一致」。

### 步骤 6：需要时重新编译

被排除的 `bin/obj` 产物可随时重新生成——编译工具链配置（`ModsWork/Directory.Build.props`、
`ModsWork/Directory.Build.targets`、`HkRefs` 参考 DLL 集合）**都已在仓库内**，
安装 dotnet SDK 后执行 `dotnet build` 即可。

---

## 四、重要提醒

### 1. 本仓库包含版权材料，必须保持 Private

- `ModsWork/HkRefs/` —— 从游戏目录复制的**原版 Hollow Knight DLL**（Team Cherry 版权）
- `modzip/` —— 60 个**第三方模组作者的源码压缩包**
- `字体资产工作/` —— 游戏资源文件

一旦转为公开，即构成对外再分发他人版权材料。

### 2. GitHub 不是备份服务

GitHub 官方文档明确说明其存储针对代码托管优化，**免费账号无可用性承诺**。
若本仓库成为唯一副本，存在以下风险：版权投诉导致下架、账号异常导致不可访问、
仓库体积过大被要求整改。

**建议**：把不可再生的核心资产（`.cs` 源码 + 文档）在别处另存一份——
例如 `D:\重要文件\Documents\空洞骑士模组项目备份\`（纯源码/文档镜像，116 MB、10113 个文件）。

### 3. 仓库体积

Git 跟踪部分约 2.18 GB（13041 个文件），Release 附件 2.61 GB。
GitHub 官方强烈建议仓库体积 < 5 GB。

---

## 五、工程背景

《Hollow Knight》60 个模组的汉化与编译工程，包含：

- 汉化后的全部模组源码（`.cs`）
- 编译交付的中文 DLL（`Mods/`）
- 原版参考 DLL 集合（`ModsWork/HkRefs/`）
- 完整的阶段进度记录（`任务进度.txt`）
- 构建工具链配置（`ModsWork/Directory.Build.props` / `.targets`）
