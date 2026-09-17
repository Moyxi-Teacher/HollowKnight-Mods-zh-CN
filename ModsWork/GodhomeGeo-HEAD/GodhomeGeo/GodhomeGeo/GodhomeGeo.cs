using System.Collections.Generic;
using Modding;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace GodhomeGeo;

internal class GodhomeGeo : Mod, IGlobalSettings<GhGeoGs>, IMenuMod
{
    internal static GodhomeGeo Instance { get; private set; }

    public GodhomeGeo() : base("神堂吉欧")
    {
    }

    public override string GetVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }

    public override void Initialize()
    {
        Log("Initializing");

        Instance = this;

        Log("Initialized");

        IL.GeoControl.OnEnable += il =>
        {
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before,
                    instr => instr.OpCode == OpCodes.Call
                )) return;
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4_0);
        };

        On.HealthManager.Start += (orig, self) =>
        {
            orig(self);
            if (!_settings.DisableVanillaEnemyGeo || self.name.StartsWith("[Architect]")) return;
            self.SetGeoLarge(0);
            self.SetGeoMedium(0);
            self.SetGeoSmall(0);
        };
    }

    private static GhGeoGs _settings = new();

    public void OnLoadGlobal(GhGeoGs s)
    {
        _settings = s;
    }

    public GhGeoGs OnSaveGlobal()
    {
        return _settings;
    }
    
    public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
    {
        return [
            new IMenuMod.MenuEntry
            {
                Name = "禁用原版吉欧掉落",
                Description = "禁用在神居内部的敌人吉欧掉落（原版行为）。",
                Values = ["已禁用", "已启用"],
                Saver = opt => _settings.DisableVanillaEnemyGeo = opt == 1,
                Loader = () => _settings.DisableVanillaEnemyGeo ? 1 : 0
            }
        ];
    }

    public bool ToggleButtonInsideMenu { get; }
}