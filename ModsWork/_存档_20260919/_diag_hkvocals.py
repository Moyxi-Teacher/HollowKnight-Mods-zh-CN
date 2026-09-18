# -*- coding: utf-8 -*-
"""给 AudioPlayer 加诊断日志，用于定位"该出声却无声"。

记录内容：
  · 真正查找的音频名（带页码，例如 ELDERBUG_INTRO_NORMAL_0）
  · HasAudioFor 判定结果
  · GetAudioClip 是否拿到片段（拿不到说明判定与实际不一致）
  · 片段的采样数/声道/频率（0 采样说明是空片段）
  · 裁剪是否被跳过
"""
import io, os, re

ROOT = r"D:\KDQSHH\ModsWork\Hallownest-Vocalized-master\HKVocal"
ap = os.path.join(ROOT, "AudioPlayer.cs")
s = io.open(ap, encoding="utf-8-sig").read().replace("\r\n", "\n")

# 1) TryPlayAudioFor：记录查找名与命中情况
p1 = re.compile(
    r"[ \t]*public static bool TryPlayAudioFor\(string convName, float removeTime = 0f, AudioSource asrc = null\)\s*\{.*?\n[ \t]*\}[ \t]*\n",
    re.S)
new1 = '''    public static bool TryPlayAudioFor(string convName, float removeTime = 0f, AudioSource asrc = null)
    {
        HKVocals.instance.LogDebug($"Trying to play audio for {convName}");

        bool exists = HasAudioFor(convName);
        AudioClip clip = null;

        if (exists)
        {
            clip = GetAudioClip(convName);

            // 诊断：HasAudioFor 说有、GetAudioClip 却拿不到 —— 说明名单和资源对不上
            if (clip == null)
            {
                HKVocals.instance.LogWarn($"[诊断] {convName} 名单里有但取不到片段（名单与资源不一致）");
            }
            else
            {
                HKVocals.instance.Log(
                    $"[诊断] 播放 {convName} | 采样={clip.samples} 声道={clip.channels} " +
                    $"频率={clip.frequency} 时长={(clip.frequency > 0 ? clip.samples / (float)clip.frequency : 0f):F2}s " +
                    $"裁剪={removeTime:F2}s");
            }

            if (removeTime != 0f)
            {
                PlayAudioWithTrim(clip, removeTime);
                return true;
            }

            PlayAudio(clip, asrc);
            return true;
        }
        else
        {
            HKVocals.instance.LogWarn($"Audio doesn't exist {convName}");
            return false;
        }
    }
'''
s, n1 = p1.subn(new1, s, count=1)
assert n1 == 1, "TryPlayAudioFor 替换失败"

# 2) 裁剪分支：记录是否跳过裁剪
s = s.replace(
    '''        if (remove <= 0 || remove >= clip.samples)
        {
            PlayAudio(clip);
            return;
        }''',
    '''        if (remove <= 0 || remove >= clip.samples)
        {
            HKVocals.instance.LogWarn($"[诊断] {clip.name} 裁剪偏移 {remove} 超出采样数 {clip.samples}，改为整句播放");
            PlayAudio(clip);
            return;
        }''', 1)

io.open(ap, "wb").write(b"\xef\xbb\xbf" + s.encode("utf-8"))
print("已加入诊断日志")
