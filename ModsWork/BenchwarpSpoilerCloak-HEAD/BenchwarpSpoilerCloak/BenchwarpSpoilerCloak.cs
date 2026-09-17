using Modding;

namespace BenchwarpSpoilerCloak
{
    public class BenchwarpSpoilerCloak : Mod, ITogglableMod, IMenuMod
    {
        public BenchwarpSpoilerCloak() : base("长椅传送保护") { }

        public override void Initialize()
        {
            Benchwarp.Events.OnGetBenchName += HideUnvisitedBenchNames;
            Benchwarp.Events.OnGetBenchSceneName += HideUnvisitedBenchNames;
        }

        public void Unload()
        {
            Benchwarp.Events.OnGetBenchName -= HideUnvisitedBenchNames;
            Benchwarp.Events.OnGetBenchSceneName -= HideUnvisitedBenchNames;
        }

        public bool ToggleButtonInsideMenu => true;

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new List<IMenuMod.MenuEntry>
            {
                new(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values, "在长椅传送界面隐藏未访问的长椅名称。", toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader)
            };
        }

        private void HideUnvisitedBenchNames(Benchwarp.Bench bench, ref string name)
        {
            if (!bench.HasVisited())
            {
                name = "???";
            }
        }

        public override string GetVersion() => "1.0";
    }
}
