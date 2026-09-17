using System.Collections.Generic;
using ItemChanger;
using ItemChanger.Tags;
using ItemChanger.UIDefs;
using Modding;
using UnityEngine;

namespace PaleCourtCharms
{
    public class HonourUpgradeItem : AbstractItem
    {
        private const string HonourKey = "Kings_Honour";

        public HonourUpgradeItem()
        {
            name = HonourKey;

            UIDef = new MsgUIDef
            {
                name = new LanguageString("UI", "CHARM_NAME_HONOUR"),
                shopDesc = new LanguageString("RANDO", "SHOP_DESCRIPTION_HONOUR"),
                sprite = new ICShiny.EmbeddedSprite { key = HonourKey }
            };
        }

        protected override void OnLoad()
        {
            Events.OnStringGet += AddNotchCostToCharmName;
        }

        protected override void OnUnload()
        {
            Events.OnStringGet -= AddNotchCostToCharmName;
        }

        private void AddNotchCostToCharmName(StringGetArgs args)
        {
            if (args.Source is LanguageString ls && ls.key == "CHARM_NAME_HONOUR")
            {
                args.Current = args.Current.Replace("-9999", $"{PlayerData.instance.charmCost_10}");
            }
        }

        public override void GiveImmediate(GiveInfo info)
        {
            PaleCourtCharms.Settings.upgradedCharm_10 = true;
            PlayerData.instance.SetBool("upgradedCharm_10", true);
            GameManager.instance.SaveGame();
        }

        public override bool Redundant()
        {
            return PlayerData.instance.GetBool("gotCharm_10") &&
                   PaleCourtCharms.Settings.upgradedCharm_10;
        }
    }
}

