using Modding;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using Satchel;
using HutongGames.PlayMaker.Actions;
using System.Collections;

namespace DarkerTransitions
{
    public class DarkerTransitions : Mod, IMenuMod, ITogglableMod
    {
        private static DarkerTransitions? _instance;

        internal static DarkerTransitions Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException($"An instance of {nameof(DarkerTransitions)} was never constructed");
                }
                return _instance;
            }
        }

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public DarkerTransitions() : base("更暗的转场")
        {
            _instance = this;
        }

        public override void Initialize()
        {
            Log("Initializing");

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += FixHallOfGods;
            On.HeroController.Awake += DarkenFader;
            On.BossSceneController.Awake += EditGHTransitions;
            Log("Initialized");
        }

        public bool ToggleButtonInsideMenu => true;

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new List<IMenuMod.MenuEntry>
            {
                new(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values, "将战斗转场中的白光替换为黑色，视野更暗。", toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader)
            };
        }

        public void Unload()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= FixHallOfGods;
            On.HeroController.Awake -= DarkenFader;
            On.BossSceneController.Awake -= EditGHTransitions;
        }



        private void EditGHTransitions(On.BossSceneController.orig_Awake orig, BossSceneController self)
        {
            GameObject battleTransition = self.transitionPrefab;
            battleTransition.transform.Find("battle_end").Find("white_solid").GetComponent<SpriteRenderer>().color = Color.black;
            battleTransition.transform.Find("battle_end").Find("vignette_large_v01").GetComponent<SpriteRenderer>().color = Color.black;
            battleTransition.transform.Find("battle_enter").Find("white_solid (1)").GetComponent<SpriteRenderer>().color = Color.black;
            battleTransition.transform.Find("battle_enter").Find("vignette_large_v01 (1)").GetComponent<SpriteRenderer>().color = Color.black;
            battleTransition.transform.Find("battle_final").Find("white_solid (2)").GetComponent<SpriteRenderer>().color = Color.black;
            battleTransition.transform.Find("battle_final").Find("vignette_large_v01").GetComponent<SpriteRenderer>().color = Color.black;
            orig(self);
        }

        private void DarkenFader(On.HeroController.orig_Awake orig, HeroController self)
        {

            GameCameras.instance.transform.Find("HudCamera").Find("Blanker White").GetComponent<SpriteRenderer>().color = Color.black;
            orig(self);
        }

        private void FixHallOfGods(Scene From, Scene To)
        {   
            //Backup solution. Didn't work ebfore but should work now if the other method causes bugs
            /*if (To.name == "GG_Atrium" || To.name == "GG_Atrium_Roof")
            {
               
                GameObject battleTransition = GameObject.Find("gg_battle_transitions(Clone)");
                battleTransition.transform.Find("battle_end").Find("white_solid").GetComponent<SpriteRenderer>().color = Color.black;
                battleTransition.transform.Find("battle_end").Find("vignette_large_v01").GetComponent<SpriteRenderer>().color = Color.black;
                battleTransition.transform.Find("battle_enter").Find("white_solid (1)").GetComponent<SpriteRenderer>().color = Color.black;
                battleTransition.transform.Find("battle_enter").Find("vignette_large_v01 (1)").GetComponent<SpriteRenderer>().color = Color.black;
                battleTransition.transform.Find("battle_final").Find("white_solid (2)").GetComponent<SpriteRenderer>().color = Color.black;
                battleTransition.transform.Find("battle_final").Find("vignette_large_v01").GetComponent<SpriteRenderer>().color = Color.black;
            }*/
            if (To.name == "GG_Workshop")
            {
                GameObject battleTransition = GameObject.Find("GG_Statue_Gruz/Inspect").LocateMyFSM("GG Boss UI").GetState("Transition").GetAction<CreateObject>(0).gameObject.Value;
                battleTransition.transform.Find("battle_end").Find("white_solid").GetComponent<SpriteRenderer>().color = Color.black;
                battleTransition.transform.Find("battle_end").Find("vignette_large_v01").GetComponent<SpriteRenderer>().color = Color.black;
                battleTransition.transform.Find("battle_enter").Find("white_solid (1)").GetComponent<SpriteRenderer>().color = Color.black;
                battleTransition.transform.Find("battle_enter").Find("vignette_large_v01 (1)").GetComponent<SpriteRenderer>().color = Color.black;
                battleTransition.transform.Find("battle_final").Find("white_solid (2)").GetComponent<SpriteRenderer>().color = Color.black;
                battleTransition.transform.Find("battle_final").Find("vignette_large_v01").GetComponent<SpriteRenderer>().color = Color.black;
            }
           
        }
    }
}
