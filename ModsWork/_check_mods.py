import os, json, sys

mods_work = "/d/KDQSHH/ModsWork"
results = []

for mod_name in sorted(os.listdir(mods_work)):
    mod_path = os.path.join(mods_work, mod_name)
    if not os.path.isdir(mod_path) or mod_name.startswith('HKMP') == False and not any(f.endswith('.csproj') for root,_,files in os.walk(mod_path) for f in files):
        # skip non-project dirs
        continue
    
    # Find readme
    readme = None
    for root, dirs, files in os.walk(mod_path):
        if "README.md" in files:
            readme = os.path.join(root, "README.md")
            break
    
    # Find language.json
    lang_json = None
    has_zh = False
    for root, dirs, files in os.walk(mod_path):
        if "Language.json" in files:
            lang_json = os.path.join(root, "Language.json")
            try:
                with open(lang_json, 'r', encoding='utf-8', errors='ignore') as f:
                    data = json.load(f)
                    has_zh = "ZH" in data or "zh" in data
            except:
                pass
            break
    
    # Find csproj for deps
    csproj = None
    for root, dirs, files in os.walk(mod_path):
        for f in files:
            if f.endswith('.csproj'):
                csproj = os.path.join(root, f)
                break
        if csproj:
            break
    
    deps = []
    if csproj:
        try:
            with open(csproj, 'r', errors='ignore') as f:
                content = f.read()
                import re
                # Look for HintPath
                for m in re.finditer(r'HintPath>([^<]+)<', content):
                    deps.append(m.group(1).strip())
        except:
            pass
    
    # Check for LanguageGetHook in .cs files
    has_lang_hook = False
    has_menu = False
    menu_files = []
    cs_files_checked = 0
    for root, dirs, files in os.walk(mod_path):
        for f in files:
            if f.endswith('.cs'):
                cs_files_checked += 1
                fpath = os.path.join(root, f)
                try:
                    with open(fpath, 'r', errors='ignore') as fh:
                        content = fh.read()
                        if 'LanguageGetHook' in content or 'langStrings' in content:
                            has_lang_hook = True
                        if 'ModMenu' in content or 'MenuScreen' in content or 'MenuItem' in content or 'BetterMenus' in content or 'CreateMenu' in content or 'MenuBuilder' in content:
                            has_menu = True
                            menu_files.append(f)
                except:
                    pass
    
    results.append({
        'mod': mod_name,
        'readme': os.path.basename(os.path.dirname(readme)) if readme else 'NONE',
        'lang_json': 'YES' if lang_json else 'NO',
        'has_zh': 'YES' if has_zh else 'NO',
        'lang_hook': 'YES' if has_lang_hook else 'NO',
        'has_menu': 'YES' if has_menu else 'NO',
        'deps_count': len(deps),
        'cs_files': cs_files_checked,
        'csproj_name': os.path.basename(csproj) if csproj else 'NONE'
    })

# Print table
print(f"{'Mod':<45} {'LangJSON':<8} {'ZH':<4} {'Hook':<6} {'Menu':<6} {'Deps':<6} {'CS':<5} {'Proj'}")
print("-" * 120)
for r in results:
    print(f"{r['mod']:<45} {r['lang_json']:<8} {r['has_zh']:<4} {r['lang_hook']:<6} {r['has_menu']:<6} {r['deps_count']:<6} {r['cs_files']:<5} {r['csproj_name']}")
print(f"\nTotal mods with .csproj: {len(results)}")
