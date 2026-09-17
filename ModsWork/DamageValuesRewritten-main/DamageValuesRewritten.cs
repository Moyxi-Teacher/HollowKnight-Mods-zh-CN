using Modding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DamageValuesRewritten
{
    public class DamageValuesRewritten : Mod, IGlobalSettings<Settings>, IMenuMod, ITogglableMod
    {
        public static readonly string ImagesDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); // the path where the mod dll is located
        private List<Sprite> _sprites = new();
        private static int _tileSize;
        private static Settings _settings = new Settings();
        private bool furyActive = false;

        public override string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public DamageValuesRewritten() : base("伤害数值") { }

        public override void Initialize()
        {
            var files = Directory.EnumerateFiles(ImagesDir).Where(path => path.EndsWith("png")).ToList();
            if (!files.Any())
            {
                Modding.Logger.Log("Sprites for damage value text does not exists.");
                return;
            }
            var spriteImage = files.First();
            if (spriteImage == null)
            {
                Modding.Logger.Log("Sprites for damage value text does not exists.");
                return;
            }

            _tileSize = _settings.TileSize;

            byte[] imageBytes = File.ReadAllBytes(Path.Combine(ImagesDir, spriteImage));
            var spriteTexture = new Texture2D(2, 2);
            spriteTexture.LoadImage(imageBytes);
            int k = 0;
            for (int i = spriteTexture.height - _tileSize; i >= 0; i -= _tileSize)
            {
                for (int j = 0; j < spriteTexture.width; j += _tileSize)
                {
                    var sprite = Sprite.Create(spriteTexture, new Rect(j, i, _tileSize, _tileSize), new Vector2(0.5f, 0.5f), _tileSize / 2);
                    sprite.name = k.ToString();
                    _sprites.Add(sprite);
                    k++;
                }
            }

            On.HealthManager.TakeDamage += OnTakeDamage;
            On.HealthManager.ApplyExtraDamage += OnTickDamage;
            On.NailSlash.SetFury += SetFuryActive;
        }

        public bool ToggleButtonInsideMenu => true;

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new List<IMenuMod.MenuEntry>
            {
                new(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values, "显示敌人受到的伤害数值，支持普通伤害与 tick 伤害。", toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader)
            };
        }

        public void Unload()
        {
            On.HealthManager.TakeDamage -= OnTakeDamage;
            On.HealthManager.ApplyExtraDamage -= OnTickDamage;
            On.NailSlash.SetFury -= SetFuryActive;
        }

        private void OnTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            orig(self, hitInstance);

            int damage = (int)(hitInstance.DamageDealt * hitInstance.Multiplier);
            var damageValue = new GameObject("Damage Value");
            int numDigits = 0;
            do
            {
                int i = damage % 10;
                var digit = new GameObject("Digit " + i);
                digit.transform.SetParent(damageValue.transform, false);
                digit.transform.position += Vector3.left * numDigits;
                var sr = digit.AddComponent<SpriteRenderer>();
                sr.sprite = _sprites[i];
                sr.material = new Material(Shader.Find("Sprites/Default"));
                sr.sortingOrder = 1;

                damage /= 10;
                numDigits++;
            }
            while (damage > 0);

            damageValue.AddComponent<DamageValue>();
            damageValue.transform.SetPosition2D(self.transform.position + Vector3.right * (numDigits - 1) / 2 + Vector3.up * 2);
            if (furyActive) damageValue.GetComponent<DamageValue>()._color = new Color(1f, 0.53f, 0.55f);
        }

        private void OnTickDamage(On.HealthManager.orig_ApplyExtraDamage orig, HealthManager self, int amount)
        {
            orig(self, amount);

            int damage = amount;
            var damageValue = new GameObject("Tick Value");
            int numDigits = 0;
            do
            {
                int i = damage % 10;
                var digit = new GameObject("Digit " + i);
                digit.transform.SetParent(damageValue.transform, false);
                digit.transform.position += Vector3.left * numDigits;
                var sr = digit.AddComponent<SpriteRenderer>();
                sr.sprite = _sprites[i];
                sr.material = new Material(Shader.Find("Sprites/Default"));
                sr.sortingOrder = 1;

                damage /= 10;
                numDigits++;
            }
            while (damage > 0);

            damageValue.AddComponent<TickValue>();
            damageValue.transform.SetPosition2D(self.transform.position + Vector3.right * (numDigits - 1) / 2 + Vector3.up * 0.8f
                + new Vector3(UnityEngine.Random.Range(-1.2f, 1.2f), UnityEngine.Random.Range(-0.6f, 0.6f), 0f));
            damageValue.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        }

        private void SetFuryActive(On.NailSlash.orig_SetFury orig, NailSlash self, bool set)
        {
            orig(self, set);

            furyActive = set;
        }

        public void OnLoadGlobal(Settings s) => _settings = s;

        public Settings OnSaveGlobal() => _settings;
    }
}
