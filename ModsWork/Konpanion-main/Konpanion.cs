using Modding;
using System.Collections.Generic;
using UnityEngine;
using static Satchel.EnemyUtils;
using static Satchel.GameObjectUtils;

namespace Konpanion
{
    public class Konpanion : Mod, IMenuMod, ITogglableMod
    {
        public Konpanion() : base("伴侣") { }

        internal static Konpanion Instance;

        internal static List<GameObject> knights = new List<GameObject>();
        internal static Dictionary<ushort,GameObject> remoteKnights = new Dictionary<ushort,GameObject>();

        // 游戏内菜单显示为开关按钮
        public bool ToggleButtonInsideMenu => true;

        // 仅返回一条总开关条目（说明文写进 Description），避免重复条目导致菜单渲染时 NullReferenceException。
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            List<IMenuMod.MenuEntry> entries = new List<IMenuMod.MenuEntry>();
            if (toggleButtonEntry != null)
            {
                IMenuMod.MenuEntry entry = toggleButtonEntry.Value;
                entry.Description = "跟随在小骑士身边的伙伴，在 HKMP 多人模式下会显示远程玩家的伴侣。";
                entries.Add(entry);
            }
            return entries;
        }

        public static bool HasPouch()
        {
            var hasPouch = ModHooks.GetMod("HkmpPouch") is Mod;
            return hasPouch;
        }
        public override string GetVersion()
        {
            return "1.0";
        }

        public GameObject createKnightCompanion(GameObject ft = null){
            HeroController.instance.gameObject.SetActive(false);
            var knight = HeroController.instance.gameObject.createCompanionFromPrefab();
            HeroController.instance.gameObject.SetActive(true);
            knight.RemoveComponent<HeroController>();
            knight.RemoveComponent<HeroAnimationController>();
            knight.RemoveComponent<HeroAudioController>();
            knight.RemoveComponent<ConveyorMovementHero>();

            knight.name = "kompanion";
            knight.GetAddComponent<MeshRenderer>().enabled = true;
            knight.GetAddComponent<Rigidbody2D>().gravityScale = 1f;

            //add control and adjust parameters
            var kc = knight.GetAddComponent<CompanionControl>();
            kc.moveSpeed = 10f;
            kc.followDistance = 2f;
            kc.IdleShuffleDistance = 0.01f;

            if(ft != null){
                kc.followTarget = ft;
            }

            //fix up collider size
            var collider = knight.GetAddComponent<BoxCollider2D>();
            collider.size = new Vector2(1f,2.0f);
            collider.offset = new Vector2(0f,-0.4f);
            collider.enabled = true;

            // add animations
            kc.Animations.Add(State.Idle,"Idle");
            kc.Animations.Add(State.IdleFidget1,"Acid Death");
            kc.Animations.Add(State.IdleFidget2,"Prostrate");
            kc.Animations.Add(State.Walk,"Run");
            kc.Animations.Add(State.Turn,"Map Walk");
            kc.Animations.Add(State.Teleport,"Fall");
            kc.Animations.Add(State.Jump,"Airborne");

            knight.SetActive(true);
            return knight;
        }

        public override void Initialize()
        {
            Instance = this;
            ModHooks.HeroUpdateHook += update;
            if (Konpanion.HasPouch())
            {
                PouchIntegration.Initialize();
            }
        }

        // 关闭时移除钩子，实现真正的总开关
        public void Unload()
        {
            ModHooks.HeroUpdateHook -= update;
            Instance = null;
        }


        public GameObject GetNetworkKonpanion(ushort id){
            if(remoteKnights.TryGetValue(id,out var knight)){
                return knight;
            }
            remoteKnights[id] = createKnightCompanion();
            return remoteKnights[id];
        }
        public void update()
        {
            if(knights.Count < 1) {
                knights.Add(createKnightCompanion());
            }
        }

    }

}
