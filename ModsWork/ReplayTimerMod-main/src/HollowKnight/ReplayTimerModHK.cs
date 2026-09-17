#if HOLLOW_KNIGHT_BUILD
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Modding;

namespace ReplayTimerMod
{
    public class ReplayTimerModHK : Mod, IMenuMod, ITogglableMod
    {
        private bool _enabled = true;

        public ReplayTimerModHK() : base("回放计时器") { }

        private FrameRecorder frameRecorder = null!;
        private GhostPlayback ghostPlayback = null!;
        private ReplayUI replayUI = null!;
        private RoomTimerHUD roomTimerHUD = null!;
        private ReplaySelectionState replaySelectionState = null!;

        private bool lateInitDone = false;

        public static ReplayTimerModHK? Instance { get; private set; }

        public override string GetVersion() =>
            Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";

        public bool ToggleButtonInsideMenu => true;

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new()
            {
                new(
                    toggleButtonEntry.Value.Name,
                    toggleButtonEntry.Value.Values,
                    "回放计时器：记录房间计时器、回放录制和幽灵播放，支持保存和加载回放。",
                    toggleButtonEntry.Value.Saver,
                    toggleButtonEntry.Value.Loader)
            };
        }

        public bool IsToggleable() => true;

        public void Toggle()
        {
            _enabled = !_enabled;
            Log($"回放计时器 已{(_enabled ? "启用" : "禁用")}.");
            // 实际开关逻辑需要暂停/恢复记录，留待后续扩展。
        }

        public void Unload()
        {
            RoomTracker.OnRoomEnter -= OnRoomEnter;
            RoomTracker.OnRoomExit -= OnRoomExit;
            RoomTracker.OnRecordingDiscarded -= OnRecordingDiscarded;
            ModHooks.HeroUpdateHook -= OnHeroUpdate;
        }

        public override void Initialize()
        {
            Instance = this;
            Log("Initialize");

            GameHooks.Init();

            string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";

            // HK build uses lightweight in-memory config backing for now.
            GhostSettings.Init(baseDirectory);

            string dataDir = Path.Combine(Path.Combine(baseDirectory, "ReplayMod"), "data");
            DataStore.Init(dataDir);
            
            replaySelectionState = new ReplaySelectionState();
            PBManager.SetSelectionState(replaySelectionState);
            PBManager.Init();

            frameRecorder = new FrameRecorder();
            ghostPlayback = new GhostPlayback();
            ghostPlayback.SetSelectionState(replaySelectionState);
            replayUI = new ReplayUI();
            roomTimerHUD = new RoomTimerHUD();

            RoomTracker.Init();

            RoomTracker.OnRoomEnter += OnRoomEnter;
            RoomTracker.OnRoomExit += OnRoomExit;
            RoomTracker.OnRecordingDiscarded += OnRecordingDiscarded;

#if V1221
            ModHooks.HeroUpdateHook += OnHeroUpdate;
#else
            ModHooks.HeroUpdateHook += OnHeroUpdate;
#endif
        }

        private void OnHeroUpdate()
        {
            TryLateInit();

            bool shouldTick = false;
            try { shouldTick = LoadRemover.ShouldTick(); } catch { Log("couldnt check tick timer"); }

            RoomTracker.Tick(shouldTick);
            frameRecorder.Tick(shouldTick);
            ghostPlayback.Tick(shouldTick);
            replayUI.Tick();
            roomTimerHUD.Tick(shouldTick);
        }

        private void TryLateInit()
        {
            if (lateInitDone) return;
            if (HeroController.instance == null) return;

            lateInitDone = true;
            Log("Hero ready - setting up UI and ghost");
            ghostPlayback.Setup();
            replayUI.Setup();
            roomTimerHUD.Setup();
        }

        private void OnRoomEnter(string sceneName, string entryFromScene)
        {
            if (!GhostSettings.TrackingEnabled)
            {
                // Start playback but don't record.
                ghostPlayback.StartPlayback(sceneName, entryFromScene);
                return;
            }

            frameRecorder.StartRecording();
            ghostPlayback.StartPlayback(sceneName, entryFromScene);
        }

        private void OnRoomExit(string sceneName, string entryFromScene, string exitToScene, float lrTime)
        {
            ghostPlayback.StopPlayback();

            if (!GhostSettings.TrackingEnabled)
            {
                frameRecorder.DiscardRecording();
                return;
            }

            RoomKey key = new RoomKey(sceneName, entryFromScene, exitToScene);

            bool saveAllRuns = GhostSettings.SaveAllRunsEnabled;

            if (!saveAllRuns && !PBManager.WouldBePB(key, lrTime))
            {
                frameRecorder.DiscardRecording();
                return;
            }

            RecordedRoom? recording = frameRecorder.FinishRecording(key, lrTime);
            if (recording == null)
                return;

            var result = PBManager.Evaluate(recording, saveAllRuns);
            if (result.Kind == ResultKind.FirstRun
                || result.Kind == ResultKind.NewPB
                || result.Kind == ResultKind.SavedHistory)
                replayUI.OnPBUpdated();
        }

        private void OnRecordingDiscarded()
        {
            ghostPlayback.StopPlayback();
            frameRecorder.DiscardRecording();
        }
    }
}
#endif