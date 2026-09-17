using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using Modding;

namespace CollisionGlitchRemover {
    public class CollisionGlitchRemover : Mod, ITogglableMod, IMenuMod {
        public static CollisionGlitchRemover instance;

        public CollisionGlitchRemover() : base("碰撞故障修正") => instance = this;

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public override void Initialize() {
            Log("Initializing");

            On.HutongGames.PlayMaker.Actions.CheckCollisionSide.OnExit += OnCheckCollisionSideExit;

            Log("Initialized");
        }

        public void Unload() {
            On.HutongGames.PlayMaker.Actions.CheckCollisionSide.OnExit -= OnCheckCollisionSideExit;
        }

        public bool ToggleButtonInsideMenu => true;

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry) {
            return new List<IMenuMod.MenuEntry> {
                new(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values, "修复 CheckCollisionSide 碰撞检测在中空态后未能及时重置导致的卡墙故障。", toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader)
            };
        }

        /*
        The CheckCollisionSide action does not properly reset itself after leaving the midair state at times,
        so the next time the midair state is entered, it immediately triggers the collision transition instead of
        waiting for an actual collision.

        Credit to shownyoung for finding the cause of this bug.
        */
        public void OnCheckCollisionSideExit(On.HutongGames.PlayMaker.Actions.CheckCollisionSide.orig_OnExit orig, CheckCollisionSide self) {
            orig(self);
            GameManager.instance.StartCoroutine(ClearCollisions(self));
        }

        public IEnumerator ClearCollisions(CheckCollisionSide instance) {
            yield return null; // Wait for one frame to ensure values are no longer being updated

            instance.topHit = false;
            instance.rightHit = false;
            instance.bottomHit = false;
            instance.leftHit = false;
        }
    }
}
