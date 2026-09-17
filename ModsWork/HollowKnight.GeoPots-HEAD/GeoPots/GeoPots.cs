using Modding;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeoPots
{
    public class GeoPots : Mod, IMenuMod, ITogglableMod
    {
        internal static GeoPots Instance;

        public override string GetVersion() => "1.1.0.0";

        public GeoPots() : base("吉欧存钱罐") {}

        /// <summary>
        /// 从全局对象池中保存的吉欧矿石预制体
        /// </summary>
        private GameObject geoPrefab;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            Instance = this;

            // 从全局对象池中取出吉欧矿石预制体
            foreach (var pool in ObjectPool.instance.startupPools)
            {
                if (pool.prefab.name.Equals("Geo Small"))
                {
                    geoPrefab = pool.prefab;
                }
            }

            On.Breakable.Break += BreakPots;

            Log("Initialized");
        }

        /// <summary>
        /// 总开关位于模组菜单内（菜单中显示开关项）
        /// </summary>
        public bool ToggleButtonInsideMenu => true;

        /// <summary>
        /// 构建模组菜单数据：仅提供总开关一项
        /// </summary>
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new List<IMenuMod.MenuEntry> {
                new(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values,
                    "破坏木桶或推车时，按一定概率掉落吉欧。",
                    toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader)
            };
        }

        /// <summary>
        /// 卸载时移除钩子，确保能正确关闭模组效果
        /// </summary>
        public void Unload()
        {
            Instance = null;
            On.Breakable.Break -= BreakPots;
        }

        /// <summary>
        /// 如同塞尔达传说一般，当木桶或推车被打碎时掉落吉欸
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="flingAngleMin"></param>
        /// <param name="flingAngleMax"></param>
        /// <param name="impactMultiplier"></param>
        private void BreakPots(On.Breakable.orig_Break orig, Breakable self, float flingAngleMin, float flingAngleMax, float impactMultiplier)
        {
            //Log($"Breaking {self.gameObject.name}");
            orig(self, flingAngleMin, flingAngleMax, impactMultiplier);

            // Barrels are smaller and more common, so they will have a 50% of dropping 1 geo
            if (self.gameObject.name.ToLower().Contains("barrel") ||
                self.gameObject.name.ToLower().Contains("deep_egg_set"))
            {
                int random = UnityEngine.Random.Range(1, 101);
                //Log($"Barrel chance: {random} vs 50");
                if (random <= 50)
                {
                    FlingGeo(self.gameObject.transform.position, 1);
                }
            }

            // Carts are larger, so they will have a 75% of dropping 5 geo
            if (self.gameObject.name.ToLower().Contains("cart"))
            {
                int random = UnityEngine.Random.Range(1, 101);
                //Log($"Cart chance: {random} vs 75");
                if (random <= 75)
                {
                    FlingGeo(self.gameObject.transform.position, 5);
                }
            }
        }

        /// <summary>
        /// Flings a random range of geo in various directions
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="geoCount"></param>
        private void FlingGeo(Vector3 origin, int geoCount)
        {
            //Log($"Spawning {geoCount} geo");
            for (int i = 0; i < geoCount; i++)
            {
                GameObject gameObject = geoPrefab.Spawn(origin, Quaternion.Euler(Vector3.zero));
                Vector3 newPosition = gameObject.transform.position;
                newPosition.Set(newPosition.x + UnityEngine.Random.Range(-0.25f, 0.25f),
                                newPosition.y + UnityEngine.Random.Range(-0.25f, 0.25f),
                                newPosition.z);
                gameObject.transform.position = newPosition;

                float speed = UnityEngine.Random.Range(23, 30);
                float angle = UnityEngine.Random.Range(80, 100);
                Vector2 velocity = default;
                velocity.x = speed * Mathf.Cos(angle * ((float)Math.PI / 180f));
                velocity.y = speed * Mathf.Sin(angle * ((float)Math.PI / 180f));

                Rigidbody2D rb2d = gameObject.GetComponent<Rigidbody2D>();
                rb2d.velocity = velocity;
            }
        }
    }
}