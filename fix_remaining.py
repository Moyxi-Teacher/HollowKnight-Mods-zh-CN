# -*- coding: utf-8 -*-
"""
彻底修复 PaleCourt Language.json ZH 区残留术语
- 所有"利刃"→"骨钉"（Nail 的官方简体中文译名）
- "利刃匠人"→"骨钉匠人"
"""
import json
import sys

ZH_FILE = r"D:\KDQSHH\ModsWork\PaleCourt-master\assets\Language.json"

def deep_replace(obj, old, new):
    """递归替换所有字符串中的 old → new"""
    if isinstance(obj, str):
        return obj.replace(old, new)
    elif isinstance(obj, dict):
        return {k: deep_replace(v, old, new) for k, v in obj.items()}
    elif isinstance(obj, list):
        return [deep_replace(item, old, new) for item in obj]
    return obj

def main():
    with open(ZH_FILE, 'r', encoding='utf-8') as f:
        data = json.load(f)

    zh = data['ZH']

    # 先做精确的复合词替换，再做通用的单字替换
    # 注意顺序：先替换包含"利刃"的复合词，再替换剩余的"利刃"

    # 1. 精确复合词替换（如果还有残留）
    zh = deep_replace(zh, "利刃匠人", "骨钉匠人")
    zh = deep_replace(zh, "纯净利刃", "纯净骨钉")
    zh = deep_replace(zh, "利刃标记的", "骨钉标记的")
    zh = deep_replace(zh, "连续命中利刃", "连续命中骨钉")
    zh = deep_replace(zh, "被利刃标记的敌人灵魂", "被骨钉标记的敌人灵魂")

    # 2. 通用替换：所有剩余的"利刃"→"骨钉"
    zh = deep_replace(zh, "利刃", "骨钉")

    # 验证
    all_zh = json.dumps(zh, ensure_ascii=False)
    remaining_old = all_zh.count("利刃")
    new_count = all_zh.count("骨钉")

    print(f"=== 替换结果 ===")
    print(f"利刃 残留: {remaining_old}")
    print(f"骨钉 出现次数: {new_count}")

    # 检查其他术语
    for term in ["赫格莫尔","海格默","黛莉娅","德莱娅","伊丝玛","伊思玛",
                 "蜂巢之国","圣巢","器皿","容器","空无","虚空",
                 "泽莫尔","奥格瑞姆"]:
        c = all_zh.count(term)
        if c > 0:
            print(f"  {term}: {c}")

    # 写回
    data['ZH'] = zh
    with open(ZH_FILE, 'w', encoding='utf-8') as f:
        json.dump(data, f, ensure_ascii=False, indent=2)

    if remaining_old == 0:
        print("\n全部替换完成！无残留。")
    else:
        print(f"\n警告：仍有 {remaining_old} 处利刃残留！")

if __name__ == "__main__":
    main()
