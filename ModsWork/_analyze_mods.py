import os, json, re

mods_work = "D:/KDQSHH/ModsWork"
results = []

for mod_name in sorted(os.listdir(mods_work)):
    mod_path = os.path.join(mods_work, mod_name)
    if not os.path.isdir(mod_path):
        continue
    if mod_name.startswith('_') and mod_name != '_check_mods.py':
        continue
    
    info = {
        'dir': mod_name,
        'display_name': '',
        'lang_json': None,
        'lang_xml': None,
        'has_zh_json': False,
        'has_zh_xml': False,
        'lang_hook': False,
        'menu_frame': False,
        'hardcoded_strs': False,
        'resource_files': [],
        'menu_files': [],
        'lang_hook_files': [],
        'entry_files': [],
        'no_text': True,
        'lang_key_files': [],
        'has_chinese_somewhere': False,
        'has_chinese_detail': '',
        'stage1': '',
        'trans_entrance': '',
    }
    
    # Check language.json
    for root, dirs, files in os.walk(mod_path):
        for f in files:
            if f == 'Language.json':
                fpath = os.path.join(root, f)
                info['lang_json'] = fpath.replace(mods_work+'/', '')
                try:
                    with open(fpath, 'r', encoding='utf-8', errors='ignore') as fh:
                        data = json.load(fh)
                        if isinstance(data, dict):
                            has_zh = any(k.upper().startswith('ZH') or k.lower() == 'zh' for k in data.keys())
                            info['has_zh_json'] = has_zh
                            if has_zh:
                                info['has_chinese_somewhere'] = True
                                info['has_chinese_detail'] += 'JSON '
                except:
                    pass
    
    # Check language.xml
    for root, dirs, files in os.walk(mod_path):
        for f in files:
            if f.lower() == 'language.xml':
                fpath = os.path.join(root, f)
                info['lang_xml'] = fpath.replace(mods_work+'/', '')
                try:
                    with open(fpath, 'r', encoding='utf-8', errors='ignore') as fh:
                        content = fh.read()
                        has_zh = 'ZH' in content or 'zh' in content or 'Chinese' in content
                        info['has_zh_xml'] = has_zh
                        if has_zh:
                            info['has_chinese_somewhere'] = True
                            info['has_chinese_detail'] += 'XML '
                except:
                    pass
    
    # Check CS files for hooks, menus, hardcoded strings
    cs_files_all = []
    for root, dirs, files in os.walk(mod_path):
        for f in files:
            if f.endswith('.cs'):
                fpath = os.path.join(root, f)
                cs_files_all.append(fpath.replace(mods_work+'/', ''))
                try:
                    with open(fpath, 'r', encoding='utf-8', errors='ignore') as fh:
                        content = fh.read()
                    
                    # LanguageGetHook / langStrings
                    if 'LanguageGetHook' in content or 'Language.LanguageGetHook' in content or 'langStrings' in content:
                        info['lang_hook'] = True
                        info['lang_hook_files'].append(fpath.replace(mods_work+'/', ''))
                        info['no_text'] = False
                    
                    # Menu frame
                    if any(x in content for x in ['ModMenu', 'MenuScreen', 'BetterMenus', 'CreateMenu', 'MenuBuilder', 'UIManager']):
                        info['menu_frame'] = True
                        info['menu_files'].append(fpath.replace(mods_work+'/', ''))
                        info['no_text'] = False
                    
                    # Hardcoded strings - look for dialogue/text patterns
                    if re.search(r'"[A-Z][a-z]+(?:\s+[A-Z][a-z]+)*"', content[:3000]):
                        info['hardcoded_strs'] = True
                        info['no_text'] = False
                    
                    # Resource files (.resx, .txt)
                    if f.endswith('.resx') or f.endswith('.txt'):
                        info['resource_files'].append(fpath.replace(mods_work+'/', ''))
                        info['no_text'] = False
                    
                except:
                    pass
    
    # Check for resource files
    for root, dirs, files in os.walk(mod_path):
        for f in files:
            if f.endswith('.resx') or f.endswith('.txt'):
                fpath = os.path.join(root, f)
                info['resource_files'].append(fpath.replace(mods_work+'/', ''))
                if not info['no_text'] is False:
                    pass
    
    # Readme check for Chinese
    for root, dirs, files in os.walk(mod_path):
        for f in files:
            if f.lower() == 'readme.md':
                fpath = os.path.join(root, f)
                try:
                    with open(fpath, 'r', encoding='utf-8', errors='ignore') as fh:
                        content = fh.read()
                    if any('\u4e00' <= c <= '\u9fff' for c in content):
                        if info['has_chinese_somewhere']:
                            info['has_chinese_detail'] += 'README '
                        else:
                            info['has_chinese_somewhere'] = True
                            info['has_chinese_detail'] = 'README'
                except:
                    pass
    
    # Check CS files for Chinese characters
    for root, dirs, files in os.walk(mod_path):
        for f in files:
            if f.endswith('.cs'):
                fpath = os.path.join(root, f)
                try:
                    with open(fpath, 'r', encoding='utf-8', errors='ignore') as fh:
                        content = fh.read()
                    if any('\u4e00' <= c <= '\u9fff' for c in content):
                        if info['has_chinese_somewhere']:
                            info['has_chinese_detail'] += 'CS '
                        else:
                            info['has_chinese_somewhere'] = True
                            info['has_chinese_detail'] = 'CS'
                except:
                    pass
    
    # Determine stage1 conclusion
    if info['no_text']:
        info['stage1'] = '无文本'
    elif info['has_chinese_somewhere']:
        info['stage1'] = '已有中文'
    else:
        info['stage1'] = '待汉化'
    
    # Determine translation entrance
    entrances = []
    if info['lang_json']:
        entrances.append(info['lang_json'])
    if info['lang_xml']:
        entrances.append(info['lang_xml'])
    if info['lang_hook_files']:
        for f in info['lang_hook_files'][:2]:
            entrances.append(f)
    if info['menu_files
