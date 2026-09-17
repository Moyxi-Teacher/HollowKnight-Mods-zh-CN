using System.Collections.Generic;
using System.Reflection;
using Modding;
using UnityEngine;

namespace BenchWarpFix;

public class BenchWarpFix : Mod, IMenuMod, ITogglableMod
{
    public BenchWarpFix() : base("长椅传送修正") { }

    internal static BenchWarpFix Instance;

    public override string GetVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }

    public override void Initialize()
    {
        Instance = this;

        On.HeroController.FixedUpdate += this.OnHeroControllerFixedUpdate;
    }

    public bool ToggleButtonInsideMenu => true;

    public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
    {
        return new List<IMenuMod.MenuEntry>
        {
            new(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values, "当英雄掉出地图边界时，自动传送回德特茅斯以防止卡在外层空间。", toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader)
        };
    }

    public void Unload()
    {
        On.HeroController.FixedUpdate -= this.OnHeroControllerFixedUpdate;
    }

    private void OnHeroControllerFixedUpdate(On.HeroController.orig_FixedUpdate orig, HeroController self)
    {
        orig(self);

        var heroPos = self.transform.position;

        if (heroPos.x >= -10000.0f && heroPos.y >= -10000.0f)
        {
            return;
        }

        var failSafe = new GameObject("FAILSAFE");
        failSafe.transform.SetPosition2D(heroPos);
        var tp = failSafe.AddComponent<TransitionPoint>();

        var bc = failSafe.AddComponent<BoxCollider2D>();
        bc.size = new Vector2(10f, 10f);
        bc.isTrigger = true;
        tp.SetTargetScene("Town");
        tp.entryPoint = "top1";

        var rm = new GameObject("Hazard Respawn Marker");
        rm.transform.parent = failSafe.transform;
        rm.transform.SetPosition2D(new Vector2(0f, 0f));
        var tmp = rm.AddComponent<HazardRespawnMarker>();
        tmp.respawnFacingRight = false;
        tp.respawnMarker = rm.GetComponent<HazardRespawnMarker>();
        tp.sceneLoadVisualization = GameManager.SceneLoadVisualizations.Default;
    }
}