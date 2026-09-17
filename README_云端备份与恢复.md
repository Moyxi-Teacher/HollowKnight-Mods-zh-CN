# KDQSHH —— Hollow Knight 模组汉化工程 · 云端备份

> 本仓库是原工作目录 `D:\KDQSHH` 的完整快照，用于释放本地磁盘空间。
> 备份时间：2026-09-17

---

## 一、备份总览

| 项目 | 数值 |
|---|---|
| 原目录总大小 | **7.72 GB** |
| 文件总数 | **17570** |
| 仓库内 Git 跟踪文件 | **17556** |
| Release 附件承载文件 | **14**（合计 4.61 GB） |
| 空目录（需手工重建） | **17** |

**校验关系**：`17556 + 14 = 17570` —— 一个文件都没有丢。

---

## 二、为什么有 14 个文件放在 Release 而不是仓库里

GitHub 对单个文件设有 **100 MiB 硬性上限**，超过即拒绝推送，**私有仓库同样适用**。
本工程中有 14 个文件超过该限制，因此改由 Release 附件分发 ——
这是 GitHub 官方文档明确推荐的大文件处理方式。

> 注意：这 14 个文件在仓库中是**缺失**的，看起来目录不完整。
> 必须执行第三节的「步骤 2、3」才能拿到完整数据。

### 14 个超限文件清单

| # | 原路径 | 大小 |
|---|---|---|
| 1 | `Mods/DoodleBosses（涂鸦BOSS）/DoodleBosses.dll` | 261 MB |
| 2 | `Mods/Hallownest-Vocalized-AudioLoader（圣巢之声音频加载器）/Hallownest-Vocalized-AudioLoader.dll` | 404 MB |
| 3 | `Mods/Pale Court（苍白宫廷）/PaleCourt.dll` | 342 MB |
| 4 | `ModsWork/HkRefs/Mods/PaleCourt/PaleCourt.dll` | 342 MB |
| 5 | `ModsWork/PaleCourt-master/bin/Debug/net472/PaleCourt.dll` | 342 MB |
| 6 | `ModsWork/PaleCourt-master/bin/Publish/PaleCourt/PaleCourt.dll` | 342 MB |
| 7 | `ModsWork/PaleCourt-master/bin/Publish/PaleCourt/PaleCourt.zip` | 341 MB |
| 8 | `ModsWork/PaleCourt-master/bin/Release/net472/PaleCourt.dll` | 342 MB |
| 9 | `ModsWork/obj/FiveKnights/Debug/net472/PaleCourt.dll` | 342 MB |
| 10 | `ModsWork/obj/FiveKnights/Release/net472/PaleCourt.dll` | 342 MB |
| 11 | `TempTest/Backup_before_PaleCourt_menuStyle_fix/PaleCourt_dll_phase77_backup.dll` | 342 MB |
| 12 | `TempTest/decompile/AudioLoader/Hallownest-Vocalized-AudioLoader.dll` | 404 MB |
| 13 | `TempTest/png_37_253x250.png` | 327 MB |
| 14 | `modzip/DoodleBosses-master.zip` | 254 MB |

---

## 三、完整还原步骤

### 步骤 1：克隆仓库

```bash
git clone https://github.com/Moyxi/KDQSHH.git "D:/KDQSHH"
```

得到 17554 个文件。

### 步骤 2：下载大文件附件

```bash
gh release download v1.0-full --repo Moyxi/KDQSHH --dir "D:/重要文件/Downloads/temp/kdqshh_release"
```

（若未安装 `gh`，可在仓库 Release 页面手动下载全部附件）

### 步骤 3：解压回工程根目录

```bash
cd "D:/重要文件/Downloads/temp/kdqshh_release"
7z x "KDQSHH_BigFiles.7z.001" -o"D:/KDQSHH"
```

分卷压缩包已保留原始相对路径，解压后 14 个文件会自动回到上表所列位置。

### 步骤 4：重建空目录

Git **不跟踪空目录**，clone 后下列 17 个目录会消失，按需重建：

```bash
cd "D:/KDQSHH"
xargs -a "_空目录清单.txt" -I{} mkdir -p "{}"
```

### 步骤 5：完整性校验

还原完成后应当满足：

```bash
cd "D:/KDQSHH"
find . -type f -not -path "./.git/*" | wc -l    # 应为 17570
```

---

## 四、重要提醒

### 1. 本仓库包含版权材料，请勿转为公开

仓库内含：

- `ModsWork/HkRefs/` —— 从游戏目录复制的**原版 Hollow Knight DLL**（Team Cherry 版权）
- `modzip/` —— 60 个**第三方模组作者的源码压缩包**
- `字体资产工作/` —— 游戏资源文件

**保持仓库为 Private。** 一旦转为公开，即构成对外再分发他人版权材料。

### 2. GitHub 不是备份服务

GitHub 官方文档明确说明其存储针对代码托管优化，**免费账号无可用性承诺**。
若本仓库成为唯一副本，存在以下风险：

- 版权投诉导致仓库被下架
- 账号异常导致数据不可访问
- 仓库体积过大（官方强烈建议 <5 GB）被要求整改

**建议**：至少把不可再生的核心资产（`.cs` 源码 88 MB + 文档 4.7 MB）在别处另存一份。

### 3. 关于行尾转换

本仓库已通过 `.git/info/attributes` 的 `* -text` 规则**关闭一切行尾转换**，
保证 clone 回来的文件与原文件**字节级一致**。

⚠️ 但该规则存放在 `.git/` 目录内，**不会随仓库同步**。
如果在另一台机器 clone 后要再次提交，需重新执行：

```bash
cd "D:/KDQSHH"
git config core.autocrlf false
git config core.quotepath false
printf '* -text\n' > .git/info/attributes
```

否则模组源码自带的 32 个 `.gitattributes` 会重新启用 CRLF 转换，改写文件哈希。

---

## 五、工程背景

《Hollow Knight》60 个模组的汉化与编译工程，包含：

- 汉化后的全部模组源码（`.cs`）
- 编译交付的中文 DLL（`Mods/`）
- 原版参考 DLL 集合（`ModsWork/HkRefs/`）
- 完整的阶段进度记录（`任务进度.txt`）
- 构建工具链配置（`ModsWork/Directory.Build.props` / `.targets`）
