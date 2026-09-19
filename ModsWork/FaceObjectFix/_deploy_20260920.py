# -*- coding: utf-8 -*-
"""验证并部署「朝向动作修正」(FaceObjectFix) 到游戏模组目录。"""
import hashlib, io, os, shutil

BUILT = r"D:\KDQSHH\ModsWork\FaceObjectFix\bin\Release\net472"
GAME_MODS = r"E:\steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods"
DEST = os.path.join(GAME_MODS, "FaceObjectFix（朝向动作修正）")

LOG = io.open(r"D:\KDQSHH\ModsWork\_deploy_fof.log", "w", encoding="utf-8")


def md5(p):
    return hashlib.md5(open(p, "rb").read()).hexdigest()


def log(m):
    LOG.write(m + "\n")
    LOG.flush()


log("=== 一、产物校验 ===")
dll = os.path.join(BUILT, "FaceObjectFix.dll")
pdb = os.path.join(BUILT, "FaceObjectFix.pdb")
b = open(dll, "rb").read()
log("  DLL %d 字节  MD5 %s" % (len(b), hashlib.md5(b).hexdigest()))
u = b.decode("utf-16-le", "ignore")
for kw in ["朝向动作修正", "已拦截 FaceObject", "1.0.0.0"]:
    log("  文本「%s」：%s" % (kw, "有" if kw in u else "无"))
for kw in [b"orig_OnEnter", b"orig_DoFace", b"EndpointsValid", b"objectA", b"Finish"]:
    log("  符号 %-16s %s" % (kw.decode(), "有" if kw in b else "无"))

log("")
log("=== 二、部署到游戏模组目录 ===")
os.makedirs(DEST, exist_ok=True)
log("  目标目录：%s" % DEST)

README = r"""# FaceObjectFix（朝向动作修正）

## 模组信息

| 项目 | 内容 |
|---|---|
| 模组名称 | 朝向动作修正 |
| 英文标识 | FaceObjectFix |
| 模组版本 | 1.0.0.0汉化 |
| 模组类型 | 兼容性修正（不改变任何玩法内容） |

## 这个模组做什么

修复《空洞骑士》原版 `HutongGames.PlayMaker.Actions.FaceObject` 动作的空引用缺陷。

`FaceObject` 是原版用来做「对象 A 水平翻转朝向对象 B」的动作，大量 NPC 与敌人都在用。
它的原版实现有两处隐患：

1. `DoFace()` 第一行就读取 `objectA.Value.transform.localScale`，
   但整个方法里**只检查了 objectB，从不检查 objectA**；
2. `OnEnter()` 在找不到动画组件（`_sprite == null`）时调用了 `Finish()`，
   却**没有 return**，之后仍然访问 `objectA.Value` 并继续执行。

当动作的宿主对象被销毁（场景重载、对象提前消失等）而 FSM 仍在执行该状态时，
就会**每帧抛出 NullReferenceException**。异常会中断该动作的执行，
使 NPC / 敌人卡在朝向状态上不再响应（表现为无法交互），
同时大量异常写入日志还会拖慢游戏。

## 修复方式

在调用原方法之前先校验 `objectA` 与 `objectB` 是否都存在且未被销毁：

- 任一无效 → 结束该动作（与原版对 objectB 的处理方式一致），跳过原方法；
- 两者都有效 → 原样调用，**行为与修复前完全一致**；
- 若原方法内部仍有引用失效（如动画组件），拦截该异常并结束动作，
  避免异常继续扩散。

## 日志

模组会在拦截到内部空引用时输出一条日志（最多前 5 次，之后每 100 次一条），
便于确认是否真的发生过异常：

```
[朝向动作修正] 已拦截 FaceObject 内部空引用 N 次（宿主：对象名）
```

## 安装 / 卸载

- 安装：把本目录放入 `Steam/steamapps/common/Hollow Knight/hollow_knight_Data/Managed/Mods/`
- 卸载：在游戏内模组列表中关闭本模组，或直接删除本目录

本模组可以安全地随时开关，不会影响存档。
"""
io.open(os.path.join(DEST, "README.md"), "w", encoding="utf-8").write(README)

for src, name in ((dll, "FaceObjectFix.dll"), (pdb, "FaceObjectFix.pdb")):
    dst = os.path.join(DEST, name)
    shutil.copy2(src, dst)
    log("  %8d  %s  MD5 %s" % (os.path.getsize(dst), name, md5(dst)))

log("")
log("=== 三、复核 ===")
for src, name in ((dll, "FaceObjectFix.dll"), (pdb, "FaceObjectFix.pdb")):
    dst = os.path.join(DEST, name)
    log("  %-24s %s" % (name, "一致" if md5(src) == md5(dst) else "不一致"))

log("")
log("=== 四、目录内容 ===")
for n in sorted(os.listdir(DEST)):
    log("  %8d  %s" % (os.path.getsize(os.path.join(DEST, n)), n))

LOG.close()
print("done")
