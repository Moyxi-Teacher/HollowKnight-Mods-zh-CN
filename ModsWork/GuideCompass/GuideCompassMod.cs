using Modding;
using Satchel.BetterMenus;
using UnityEngine;

namespace GuideCompass;

public sealed class GuideCompassMod : Mod, ICustomMenuMod, ILocalSettings<GuideCompassLocalSettings>
{
    private GameObject _overlayObject;
    private GuideCompassLocalSettings _localSettings = new();

    public GuideCompassMod() : base("攻略指南针")
    {
    }

    public override string GetVersion() => "0.1.0.0";

    public bool ToggleButtonInsideMenu => false;

    public override string GetMenuButtonText() => "攻略指南针";

    public void OnLoadLocal(GuideCompassLocalSettings settings)
    {
        _localSettings = settings ?? new GuideCompassLocalSettings();
        bool compatibleRoute = _localSettings.RouteVersion == GuideRoute.ProgressVersion;
        GuideCompassOverlay.LoadProgress(
            compatibleRoute ? _localSettings.StepIndex : 0,
            compatibleRoute && _localSettings.RouteFinished,
            compatibleRoute ? _localSettings.KillCount : 0);
    }

    public GuideCompassLocalSettings OnSaveLocal()
    {
        if (GuideCompassOverlay.Instance != null)
        {
            _localSettings = GuideCompassOverlay.CaptureProgress();
        }

        return _localSettings;
    }

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
    {
        Menu menu = new("攻略指南针", new Element[]
        {
            new MenuButton("重置攻略进度", "将当前目标重置为路线的第一个目标。", _ =>
                GuideCompassOverlay.ResetProgress()),
        });

        return menu.GetMenuScreen(modListMenu);
    }

    public override void Initialize()
    {
        On.PlayMakerFSM.SetState += OnPlayMakerFsmSetState;
        On.PlayMakerFSM.SendEvent += OnPlayMakerFsmSendEvent;
        On.HutongGames.PlayMaker.Fsm.EnterState += OnFsmEnterState;
        On.Breakable.Break += OnBreakableBroken;
        On.HealthCocoon.SetBroken += OnHealthCocoonBroken;
        On.HealthManager.Die += OnHealthManagerDie;
        On.PlayerData.SetBenchRespawn_RespawnMarker_string_int += OnBenchRested;
        On.PlayerData.SetBenchRespawn_string_string_bool += OnBenchRested;
        On.PlayerData.SetBenchRespawn_string_string_int_bool += OnBenchRested;
        _overlayObject = new GameObject("GuideCompassOverlay");
        Object.DontDestroyOnLoad(_overlayObject);
        _overlayObject.AddComponent<GuideCompassOverlay>();
    }

    public void Unload()
    {
        On.PlayMakerFSM.SetState -= OnPlayMakerFsmSetState;
        On.PlayMakerFSM.SendEvent -= OnPlayMakerFsmSendEvent;
        On.HutongGames.PlayMaker.Fsm.EnterState -= OnFsmEnterState;
        On.Breakable.Break -= OnBreakableBroken;
        On.HealthCocoon.SetBroken -= OnHealthCocoonBroken;
        On.HealthManager.Die -= OnHealthManagerDie;
        On.PlayerData.SetBenchRespawn_RespawnMarker_string_int -= OnBenchRested;
        On.PlayerData.SetBenchRespawn_string_string_bool -= OnBenchRested;
        On.PlayerData.SetBenchRespawn_string_string_int_bool -= OnBenchRested;
        if (_overlayObject != null)
        {
            Object.Destroy(_overlayObject);
            _overlayObject = null;
        }
    }

    private static void OnPlayMakerFsmSetState(
        On.PlayMakerFSM.orig_SetState orig,
        PlayMakerFSM self,
        string stateName)
    {
        orig(self, stateName);
        GuideCompassOverlay.NotifyFsmStateEntered(self, stateName);
    }

    private static void OnPlayMakerFsmSendEvent(
        On.PlayMakerFSM.orig_SendEvent orig,
        PlayMakerFSM self,
        string eventName)
    {
        orig(self, eventName);
        GuideCompassOverlay.NotifyFsmEventSent(self, eventName);
    }

    private static void OnFsmEnterState(
        On.HutongGames.PlayMaker.Fsm.orig_EnterState orig,
        HutongGames.PlayMaker.Fsm self,
        HutongGames.PlayMaker.FsmState state)
    {
        orig(self, state);
        if (self.FsmComponent != null && state != null)
        {
            GuideCompassOverlay.NotifyFsmStateEntered(self.FsmComponent, state.Name);
        }
    }

    private static void OnBreakableBroken(
        On.Breakable.orig_Break orig,
        Breakable self,
        float angle,
        float speed,
        float damage)
    {
        orig(self, angle, speed, damage);
        GuideCompassOverlay.NotifyInteraction(GuideInteractionKind.BreakableWall, self.gameObject);
    }

    private static void OnHealthCocoonBroken(
        On.HealthCocoon.orig_SetBroken orig,
        HealthCocoon self)
    {
        orig(self);
        GuideCompassOverlay.NotifyInteraction(GuideInteractionKind.BlueBlood, self.gameObject);
    }

    private static void OnHealthManagerDie(
        On.HealthManager.orig_Die orig,
        HealthManager self,
        float? attackDirection,
        AttackTypes attackType,
        bool ignoreEvasion)
    {
        orig(self, attackDirection, attackType, ignoreEvasion);
        GuideCompassOverlay.NotifyEnemyDefeated(self.gameObject);
    }

    private static void OnBenchRested(
        On.PlayerData.orig_SetBenchRespawn_RespawnMarker_string_int orig,
        PlayerData self,
        RespawnMarker spawnMarker,
        string sceneName,
        int spawnType)
    {
        orig(self, spawnMarker, sceneName, spawnType);
        GuideCompassOverlay.NotifyBenchRested(sceneName);
    }

    private static void OnBenchRested(
        On.PlayerData.orig_SetBenchRespawn_string_string_bool orig,
        PlayerData self,
        string spawnMarker,
        string sceneName,
        bool facingRight)
    {
        orig(self, spawnMarker, sceneName, facingRight);
        GuideCompassOverlay.NotifyBenchRested(sceneName);
    }

    private static void OnBenchRested(
        On.PlayerData.orig_SetBenchRespawn_string_string_int_bool orig,
        PlayerData self,
        string spawnMarker,
        string sceneName,
        int spawnType,
        bool facingRight)
    {
        orig(self, spawnMarker, sceneName, spawnType, facingRight);
        GuideCompassOverlay.NotifyBenchRested(sceneName);
    }
}
