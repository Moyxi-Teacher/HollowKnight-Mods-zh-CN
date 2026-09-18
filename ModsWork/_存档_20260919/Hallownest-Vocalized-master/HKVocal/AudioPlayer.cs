namespace HKVocals;
public static class AudioPlayer
{
    /*public static List<string> PaleCourtKeys = new()
    {

    };*/

    public static bool TryPlayAudioFor(string convName, float removeTime = 0f, AudioSource asrc = null)
    {
        HKVocals.instance.LogDebug($"Trying to play audio for {convName}");

        /*if (PaleCourtKeys.Contains(convName) && HKVocals._globalSettings.paleCourt == false)
        {
            HKVocals.instance.LogWarn("Pale Court audio is not enabled!");
            return false;
        }*/

        if (HasAudioFor(convName))
        {
            if (removeTime != 0f)
            {
                PlayAudioWithTrim(GetAudioClip(convName), removeTime);
                return true;
            }
            
            PlayAudio(GetAudioClip(convName), asrc);
            return true;
        }
        else
        {
            HKVocals.instance.LogWarn($"Audio doesn't exist {convName}");
            return false;
        }
    }

    private static AudioClip GetAudioClip(string convoName) => GetAudioFor(convoName.ToLower());
    
    // 裁剪结果缓存：同样的「片段 + 裁剪时长」只创建一次。
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
    private static void PlayAudio(AudioClip clip, AudioSource asrc = null)
    {
        //if supplied is null, use default
        if (asrc == null)
        {
            asrc = HKVocals.instance.audioSource;
        }
        //if its still null create new audio source go
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
        }

        CheckForAchivements(clip.name);


        asrc.Stop();
        asrc.transform.localPosition = HeroController.instance != null 
            ? HeroController.instance.transform.localPosition :
            new Vector3(15f, 10f, 1f); //for monomon audio

        asrc.SetMixerGroup();

        MixerLoader.SetMixerVolume();
        asrc.PlayOneShot(clip, 1f);
    }

    private static void CheckForAchivements(string clip) {
        HKVocals._globalSettings.FinishedNPCDialoge.RemoveAll(v => v.Equals(clip, StringComparison.OrdinalIgnoreCase));
        HKVocals._globalSettings.FinishedDNailDialoge.RemoveAll(v => v.Equals(clip, StringComparison.OrdinalIgnoreCase));
        HKVocals._globalSettings.FinishedLoreTabletDialoge.RemoveAll(v => v.Equals(clip, StringComparison.OrdinalIgnoreCase));
        HKVocals._globalSettings.FinishedUIDialoge.RemoveAll(v => v.Equals(clip, StringComparison.OrdinalIgnoreCase));

        if (!GameManager.instance.IsAchievementAwarded("Consideration") && HKVocals._globalSettings.FinishedNPCDialoge.Count == 0)
            GameManager.instance.AwardAchievement("Consideration");

        if (!GameManager.instance.IsAchievementAwarded("Ambition") && HKVocals._globalSettings.FinishedDNailDialoge.Count == 0)
            GameManager.instance.AwardAchievement("Ambition");
            
        if (!GameManager.instance.IsAchievementAwarded("Chronology") && HKVocals._globalSettings.FinishedLoreTabletDialoge.Count == 0)
            GameManager.instance.AwardAchievement("Chronology");

        if (!GameManager.instance.IsAchievementAwarded("Acquisition") && HKVocals._globalSettings.FinishedUIDialoge.Count == 0)
            GameManager.instance.AwardAchievement("Acquisition");

        if (
            GameManager.instance.IsAchievementAwarded("Consideration") &&
            GameManager.instance.IsAchievementAwarded("Ambition") &&
            GameManager.instance.IsAchievementAwarded("Chronology") &&
            GameManager.instance.IsAchievementAwarded("Acquisition") &&
            
            !GameManager.instance.IsAchievementAwarded("CompendiumVocalization")
        ) GameManager.instance.AwardAchievement("CompendiumVocalization");
    }

    public static bool IsPlaying() => HKVocals.instance.audioSource.isPlaying;
    public static void StopPlaying() => HKVocals.instance.audioSource.Stop();
    public static bool HasAudioFor(string convName) => AudioAPI.HasAudioFor(convName);
    public static AudioClip GetAudioFor(string convName) => AudioAPI.GetAudioFor(convName);
}
