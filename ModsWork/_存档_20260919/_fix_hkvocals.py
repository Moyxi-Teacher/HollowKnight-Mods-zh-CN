# -*- coding: utf-8 -*-
"""修复「圣巢之声」语音偶发不播放。

根因（代码审查所得）：
  1. PlayAudioWithTrim 每播一句就 AudioClip.Create 一个全新片段且从不销毁 ——
     AudioClip 是 UnityEngine.Object，不受 GC 回收，会持续累积；
     累积后 Create/SetData 失败或产出空片段，表现为"玩越久越容易不发声"。
  2. 裁剪无边界判断：removeTime 换算出的采样偏移一旦 >= 总采样数，
     size 变成 0 或负数，造出来的片段自然无声。
  3. clip.GetData 的返回值没检查（流式/压缩加载可能读不到采样）。
  4. PlayAudio 中音频源为空时，CreateAudioSource() 之后局部变量仍是 null，
     紧接着的 asrc.Stop() 抛空引用，整句语音被吞。
  5. AudioAPI.HasAudioFor 用原始大小写匹配（大写）缓存名单，不一致时误判"音频不存在"。
"""
import io, os, re

ROOT = r"D:\KDQSHH\ModsWork\Hallownest-Vocalized-master\HKVocal"

# ─────────────── AudioPlayer.cs：按正则替换，避免被尾随空格绊住 ───────────────
ap = os.path.join(ROOT, "AudioPlayer.cs")
s = io.open(ap, encoding="utf-8-sig").read().replace("\r\n", "\n")

pattern = re.compile(
    r"[ \t]*private static void PlayAudioWithTrim\(AudioClip clip, float removeTime\)\s*\{.*?\n[ \t]*\}[ \t]*\n",
    re.S)
new_trim = '''    // 裁剪结果缓存：同样的「片段 + 裁剪时长」只创建一次。
    // 原实现每播一句就新建一个 AudioClip，而 AudioClip 属于 UnityEngine.Object，
    // 不受 GC 回收，长时间游戏会不断累积，最终导致创建失败或片段无声。
    private static readonly Dictionary<string, AudioClip> _trimmedCache = new();
    private const int TrimCacheLimit = 64;

    private static void PlayAudioWithTrim(AudioClip clip, float removeTime)
    {
        if (clip == null)
        {
            HKVocals.instance.LogWarn("PlayAudioWithTrim got a null clip");
            return;
        }

        string cacheKey = clip.name + "|" + removeTime.ToString("F2");
        if (_trimmedCache.TryGetValue(cacheKey, out AudioClip cached) && cached != null)
        {
            PlayAudio(cached);
            return;
        }

        HKVocals.instance.LogDebug($"Trimming {clip.name}");

        int remove = (int)(clip.frequency * removeTime);

        // 边界保护：裁掉的长度必须落在片段范围内，
        // 否则 size 会变成 0 或负数，造出一个无声片段 —— 听感就是"这句没播"。
        if (remove <= 0 || remove >= clip.samples)
        {
            PlayAudio(clip);
            return;
        }

        int size = clip.samples - remove;
        float[] samples = new float[size * clip.channels];

        // GetData 对流式/压缩加载的片段可能取不到数据，失败就退回原片段整句播放
        if (!clip.GetData(samples, remove))
        {
            HKVocals.instance.LogWarn($"Could not read samples of {clip.name}, playing untrimmed");
            PlayAudio(clip);
            return;
        }

        AudioClip newclip = AudioClip.Create(clip.name, size, clip.channels, clip.frequency, false);
        newclip.SetData(samples, 0);

        // 缓存上限，超出后销毁重建，避免无限增长
        if (_trimmedCache.Count >= TrimCacheLimit)
        {
            foreach (AudioClip c in _trimmedCache.Values)
            {
                if (c != null) UnityEngine.Object.Destroy(c);
            }
            _trimmedCache.Clear();
        }
        _trimmedCache[cacheKey] = newclip;

        PlayAudio(newclip);
    }
'''
s, n1 = pattern.subn(new_trim, s, count=1)
assert n1 == 1, "PlayAudioWithTrim 替换失败"

pattern2 = re.compile(
    r"(//if its still null create new audio source go\s*\n[ \t]*if \(asrc == null\)\s*\n[ \t]*\{\s*\n[ \t]*HKVocals\.instance\.CreateAudioSource\(\);\s*\n[ \t]*\})",
    re.S)
new_asrc = '''//if its still null create new audio source go
        if (asrc == null)
        {
            HKVocals.instance.CreateAudioSource();
            // 原实现创建后没有把新音频源取回来，局部变量仍是 null，
            // 紧接着的 asrc.Stop() 会抛空引用，把这一句语音整个吞掉。
            asrc = HKVocals.instance.audioSource;
        }
        if (asrc == null)
        {
            HKVocals.instance.LogWarn("PlayAudio has no audio source available");
            return;
        }
        if (clip == null)
        {
            HKVocals.instance.LogWarn("PlayAudio got a null clip");
            return;
        }'''
s, n2 = pattern2.subn(new_asrc, s, count=1)
assert n2 == 1, "PlayAudio 音频源兜底替换失败"

io.open(ap, "wb").write(b"\xef\xbb\xbf" + s.encode("utf-8"))
print("AudioPlayer.cs 已修复（裁剪缓存 + 边界保护 + 音频源兜底 + 空片段保护）")

# ─────────────── AudioAPI.cs ───────────────
aa = os.path.join(ROOT, "AudioAPI.cs")
t = io.open(aa, encoding="utf-8-sig").read().replace("\r\n", "\n")

p3 = re.compile(r"[ \t]*public static bool HasAudioFor\(string convName\)\s*\{.*?\n[ \t]*\}[ \t]*\n", re.S)
new_has = '''    public static bool HasAudioFor(string convName)
    {
        UpdateAudioNames();
        // 缓存的名单由 GetAudioNames() 统一转成大写；
        // 这里若不做同样处理，混合大小写的对话名会被误判为"音频不存在"而整句跳过。
        return CachedAudioNames.Contains(convName)
            || CachedAudioNames.Contains(convName.ToUpperInvariant());
    }
'''
t, n3 = p3.subn(new_has, t, count=1)
assert n3 == 1, "HasAudioFor 替换失败"

p4 = re.compile(
    r"[ \t]*public virtual AudioClip GetAudioClip\(string convName\) => Bundle\.LoadAsset<AudioClip>\(convName\);[ \t]*\n",
    re.S)
new_load = '''    public virtual AudioClip GetAudioClip(string convName)
    {
        // AssetBundle 内的资源名大小写不固定：先按原名找，找不到再试全小写 / 全大写
        AudioClip clip = Bundle.LoadAsset<AudioClip>(convName);
        if (clip == null) clip = Bundle.LoadAsset<AudioClip>(convName.ToLowerInvariant());
        if (clip == null) clip = Bundle.LoadAsset<AudioClip>(convName.ToUpperInvariant());
        return clip;
    }
'''
t, n4 = p4.subn(new_load, t, count=1)
assert n4 == 1, "GetAudioClip 替换失败"

io.open(aa, "wb").write(b"\xef\xbb\xbf" + t.encode("utf-8"))
print("AudioAPI.cs 已修复（大小写一致 + 资源名多重回退）")
