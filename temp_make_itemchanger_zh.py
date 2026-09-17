from pathlib import Path
import re

source = Path(r"D:\KDQSHH\ModsWork\HollowKnight.ItemChanger-master\ItemChanger\Resources\language.xml")
target = Path(r"D:\重要文件\Downloads\temp\ItemChanger.language.zh.xml")
translations = {}
text = r'''ITEMCHANGER_GET_COLLECTOR_MAP	别把这当成一张地图。把它当成一份关于你是怎样一个人的声明吧。
ITEMCHANGER_NAME_LURIEN	守望者卢瑞恩
ITEMCHANGER_DESC_LURIEN	这东西的价格至少该是现在的十倍。
ITEMCHANGER_NAME_MONOMON	教师莫诺蒙
ITEMCHANGER_DESC_MONOMON	这东西的价格至少该是现在的十倍。
ITEMCHANGER_NAME_HERRAH	野兽赫拉
ITEMCHANGER_DESC_HERRAH	这东西的价格至少该是现在的十倍。
ITEMCHANGER_NAME_DREAMER	守梦者
ITEMCHANGER_DESC_DREAMER	这东西的价格至少该是现在的十倍。
ITEMCHANGER_NAME_WORLD_SENSE	世界感知
ITEMCHANGER_DESC_WORLD_SENSE	容器。纵然被束缚，你仍将知晓世界的状态。圣巢终将再次完整。
ITEMCHANGER_GET_WORLD_SENSE_1	容器。纵然被束缚，你仍将知晓世界的状态。
ITEMCHANGER_GET_WORLD_SENSE_2	圣巢终将再次完整。
ITEMCHANGER_NAME_ESSENCE	精华
ITEMCHANGER_DESC_ESSENCE	新鲜有机梦境植物球体，产自本地独立梦境植物农场。请支持辛勤工作的邻里梦蛾们。
ITEMCHANGER_NAME_GRIMMKIN_FLAME	格林之子火焰
ITEMCHANGER_DESC_GRIMMKIN_FLAME	为你的随机模式带来的火热新成员！
ITEMCHANGER_NAME_LIFEBLOOD_COCOON_SMALL	2 格生命血面具
ITEMCHANGER_NAME_LIFEBLOOD_COCOON_LARGE	3 格生命血面具
ITEMCHANGER_DESC_LIFEBLOOD_COCOON	限量版生命血面具——趁还有货赶紧拿！
ITEMCHANGER_DESC_GEO	今天我们正在清仓甩卖。快把你的返款拿走吧。
ITEMCHANGER_NAME_SOUL	灵魂补充
ITEMCHANGER_DESC_SOUL	附带杯子、杯盖和吸管。不提供额外续杯。
ITEMCHANGER_NAME_LORE	知识石板
ITEMCHANGER_DESC_LORE	这块石板上会藏着什么秘密呢？
ITEMCHANGER_STAG_NAME_DIRTMOUTH	德特茅斯鹿角站
ITEMCHANGER_STAG_NAME_CROSSROADS	遗忘十字路鹿角站
ITEMCHANGER_STAG_NAME_GREENPATH	苍绿之径鹿角站
ITEMCHANGER_STAG_NAME_QUEENSSTATION	王后驿站鹿角站
ITEMCHANGER_STAG_NAME_QUEENSGARDENS	王后花园鹿角站
ITEMCHANGER_STAG_NAME_STOREROOMS	城市仓库鹿角站
ITEMCHANGER_STAG_NAME_KINGSSTATION	国王驿站鹿角站
ITEMCHANGER_STAG_NAME_RESTINGGROUNDS	安息之地鹿角站
ITEMCHANGER_STAG_NAME_DEEPNEST	遥远村庄鹿角站
ITEMCHANGER_STAG_NAME_HIDDENSTATION	隐藏车站鹿角站
ITEMCHANGER_STAG_NAME_STAGNEST	鹿角虫巢穴鹿角站
ITEMCHANGER_STAG_DESC	我不知道，不过要我说，它看起来相当有用。
ITEMCHANGER_NAME_GRUB	一只幼虫
ITEMCHANGER_DESC_GRUB	一只装在瓶里的幼虫。非常适合榨汁、制酱，或做成任何美味佳肴。只是目前只供应绿色的。
ITEMCHANGER_MAP_NAME_DIRTMOUTH	德特茅斯地图
ITEMCHANGER_MAP_NAME_CROSSROADS	遗忘十字路地图
ITEMCHANGER_MAP_NAME_GREENPATH	苍绿之径地图
ITEMCHANGER_MAP_NAME_FOG_CANYON	雾之峡谷地图
ITEMCHANGER_MAP_NAME_FUNGAL_WASTES	真菌荒地地图
ITEMCHANGER_MAP_NAME_DEEPNEST	深邃巢穴地图
ITEMCHANGER_MAP_NAME_ABYSS	古老盆地地图
ITEMCHANGER_MAP_NAME_OUTSKIRTS	王国边缘地图
ITEMCHANGER_MAP_NAME_CITY	泪水之城地图
ITEMCHANGER_MAP_NAME_WATERWAYS	皇家水道地图
ITEMCHANGER_MAP_NAME_CLIFFS	呼啸悬崖地图
ITEMCHANGER_MAP_NAME_MINES	水晶山峰地图
ITEMCHANGER_MAP_NAME_ROYAL_GARDENS	王后花园地图
ITEMCHANGER_MAP_NAME_RESTING_GROUNDS	安息之地地图
ITEMCHANGER_NAME_FOCUS	聚集
ITEMCHANGER_DESC_FOCUS	长按慢速施法键来治疗。
ITEMCHANGER_SHOP_DESC_FOCUS	哈哈哈哈哈哈哈哈哈哈哈。不行。
ITEMCHANGER_NAME_SWIM	游泳
ITEMCHANGER_DESC_SWIM	浮力的力量现在归你所有。
ITEMCHANGER_SHOP_DESC_SWIM	我看起来可能不像，但我其实是持证游泳教练。一次简单付款，我把你扔进深水区，然后嘛，你会自己想办法学会的。
ITEMCHANGER_NAME_LEFTSLASH	左劈
ITEMCHANGER_NAME_RIGHTSLASH	右劈
ITEMCHANGER_NAME_UPSLASH	上劈
ITEMCHANGER_NAME_DOWNSLASH	下劈
ITEMCHANGER_SHOP_DESC_LEFTSLASH	你的角色看起来有点太弱了，对吧。我们来解决这个问题。
ITEMCHANGER_SHOP_DESC_RIGHTSLASH	你的角色看起来有点太弱了，对吧。我们来解决这个问题。
ITEMCHANGER_SHOP_DESC_UPSLASH	你的角色看起来有点太弱了，对吧。我们来解决这个问题。
ITEMCHANGER_SHOP_DESC_DOWNSLASH	你的角色看起来有点太弱了，对吧。我们来解决这个问题。
ITEMCHANGER_NAME_LEFT_CLAW	左半螳螂爪
ITEMCHANGER_DESC_LEFT_CLAW	在墙壁上滑行时跳跃。
ITEMCHANGER_DESC_LEFT_CLAW_2	不过它只在左侧生效！
ITEMCHANGER_SHOP_DESC_LEFT_CLAW	由骨头雕成的一部分爪子。让佩戴者能攀附左侧墙壁，并从墙上跃离。
ITEMCHANGER_NAME_RIGHT_CLAW	右半螳螂爪
ITEMCHANGER_DESC_RIGHT_CLAW	在墙壁上滑行时跳跃。
ITEMCHANGER_DESC_RIGHT_CLAW_2	不过它只在右侧生效！
ITEMCHANGER_SHOP_DESC_RIGHT_CLAW	由骨头雕成的一部分爪子。让佩戴者能攀附右侧墙壁，并从墙上跃离。
ITEMCHANGER_SHOP_NAME_WALLJUMP	螳螂爪
ITEMCHANGER_SHOP_DESC_WALLJUMP	由骨头雕成的爪子。让佩戴者能攀附墙壁，并从墙上跃离。
ITEMCHANGER_NAME_LEFT_CLOAK	左半蛾翼披风
ITEMCHANGER_DESC_LEFT_CLOAK	向左冲刺。
ITEMCHANGER_DESC_LEFT_CLOAK_2	使用披风在地面或空中快速冲刺。
ITEMCHANGER_SHOP_DESC_LEFT_CLOAK	以蛾翼丝线织成的披风。让佩戴者能在地面或空中向左冲刺。
ITEMCHANGER_NAME_RIGHT_CLOAK	右半蛾翼披风
ITEMCHANGER_DESC_RIGHT_CLOAK	向右冲刺。
ITEMCHANGER_DESC_RIGHT_CLOAK_2	使用披风在地面或空中快速冲刺。
ITEMCHANGER_SHOP_DESC_RIGHT_CLOAK	以蛾翼丝线织成的披风。让佩戴者能在地面或空中向右冲刺。
ITEMCHANGER_SHOP_NAME_DASH	蛾翼披风
ITEMCHANGER_SHOP_DESC_DASH	以蛾翼丝线织成的披风。让佩戴者能在地面或空中向前冲刺。
ITEMCHANGER_SHOP_NAME_SHADOWDASH	暗影披风
ITEMCHANGER_SHOP_DESC_SHADOWDASH	由深渊物质形成的披风。让佩戴者能穿过敌人及其攻击而不受伤害。
ITEMCHANGER_SHOP_NAME_LEFT_SHADOWDASH	左半暗影披风
ITEMCHANGER_SHOP_NAME_RIGHT_SHADOWDASH	右半暗影披风
ITEMCHANGER_SHOP_DESC_LEFT_SHADOWDASH	由深渊物质形成的披风。让佩戴者能向左穿过敌人及其攻击而不受伤害。
ITEMCHANGER_SHOP_DESC_RIGHT_SHADOWDASH	由深渊物质形成的披风。让佩戴者能向右穿过敌人及其攻击而不受伤害。
ITEMCHANGER_NAME_LEFT_SUPERDASH	左半水晶之心
ITEMCHANGER_GET_LEFT_SUPERDASH_1	在地面上或攀附墙壁时蓄力。
ITEMCHANGER_GET_LEFT_SUPERDASH_2	松开按键，向左猛冲并飞越空中。
ITEMCHANGER_SHOPDESC_LEFT_SUPERDASH	古老采矿傀儡能量核心的一部分，以强力水晶制成。可引导水晶能量，让持有者以危险的高速向左发射。
ITEMCHANGER_NAME_RIGHT_SUPERDASH	右半水晶之心
ITEMCHANGER_GET_RIGHT_SUPERDASH_1	在地面上或攀附墙壁时蓄力。
ITEMCHANGER_GET_RIGHT_SUPERDASH_2	松开按键，向右猛冲并飞越空中。
ITEMCHANGER_SHOPDESC_RIGHT_SUPERDASH	古老采矿傀儡能量核心的一部分，以强力水晶制成。可引导水晶能量，让持有者以危险的高速向右发射。
CHEST_CONTENTS	宝箱内容物
FREE	免费
PURCHASED	已购买
OBTAINED	已获得
???	???
UNKNOWN_ITEM	未知物品
SECRET_COST_SHOPDESC	这件物品的价格是秘密！
NOTHING	无
TRACKER_NAME	追踪器
TRACKER_CAN_FOCUS	可以聚集。
TRACKER_CANNOT_FOCUS	无法聚集。
TRACKER_CAN_SWIM	可以游泳。
TRACKER_CANNOT_SWIM	无法游泳。
TRACKER_HAS_ELEVATOR_PASS	可以乘坐大型升降梯。
TRACKER_NO_ELEVATOR_PASS	无法乘坐大型升降梯。
TRACKER_ESSENCE	你拥有 {0} 点精华。
TRACKER_FLAMES	你有 {0} 团未使用的格林之子火焰。（共 {1} 团）
TRACKER_FLAMES_NO_GFM	你有 {0} 团未使用的格林之子火焰。
TRACKER_GRUBS	至今已救出 {0} 只幼虫！
TRACKER_NO_DREAMERS	你找到了 0 位守梦者。
TRACKER_DREAMERS	你找到了 {0} 位守梦者，包括：
TRACKER_LURIEN	守望者卢瑞恩
TRACKER_MONOMON	教师莫诺蒙
TRACKER_HERRAH	野兽赫拉
TRACKER_DUPLICATE_DREAMERS	重复的守梦者
TRACKER_LEFT	左
TRACKER_RIGHT	右
TRACKER_UP	上
TRACKER_DOWN	下
TRACKER_CAN_NAIL	可以向{0}挥动。
TRACKER_CANNOT_NAIL	无法挥动。
FIREBALL1_TOGGLE	按攻击键升级为暗影之魂。
FIREBALL2_TOGGLE	按攻击键降级为复仇之魂。
FALSE	否
TRUE	是
COMMA_AND	，以及
COMMA_SPACE	，
ITEMCHANGERMOD	物品替换器模组
PRELOAD_SETTINGS_NAME	预加载设置
PRELOAD_SETTINGS_DESC	重新加载游戏后，预加载设置的改动才会生效。
LOCATION_SETTINGS_NAME	地点设置
LOCATION_SETTINGS_DESC	地点设置的改动不会影响旧存档。
AVOID_NPC_ITEM_DIALOGUE_NAME	避免 NPC 物品对话
AVOID_BLUGGSACS_NAME	始终替换布鲁格萨克
RETAIN_TABLETS_ON_REPLACE_NAME	替换时保留石板图像
PRELOAD_GEO_ROCKS_NAME	预加载吉欧岩石
PRELOAD_SOUL_TOTEMS_NAME	预加载灵魂图腾
PRELOAD_GRUB_NAME	预加载幼虫
PRELOAD_MIMIC_NAME	预加载拟态怪
PRELOAD_LEVEL_FULL_NAME	完整
PRELOAD_LEVEL_REDUCED_NAME	精简
PRELOAD_LEVEL_NONE_NAME	无
EGGSHOP_GREET	啊，你好。近来过得怎么样？你是来带些美味东西给我的吗？让我看看……
EGGSHOP_DECLINE	哦？既然你不想拿走我的物品，我也帮不了你。
EGGSHOP_YN	把你所有的东西都交给吉吉吗？
TRIAL_HINT_BRONZE	勇士的试炼。为 {0} 和吉欧而战。&lt;br&gt;留下印记并开始试炼吗？
TRIAL_HINT_SILVER	征服者的试炼。为 {0} 和吉欧而战。&lt;br&gt;留下印记并开始试炼吗？
TRIAL_HINT_GOLD	愚人的试炼。为 {0} 和吉欧而战。&lt;br&gt;留下印记并开始试炼吗？
TRIAL_NULLHINT_BRONZE	勇士的试炼。为吉欧而战。&lt;br&gt;留下印记并开始试炼吗？
TRIAL_NULLHINT_SILVER	勇士的试炼。为吉欧而战。&lt;br&gt;留下印记并开始试炼吗？
TRIAL_NULLHINT_GOLD	愚人的试炼。为吉欧而战。&lt;br&gt;留下印记并开始试炼吗？
DIVINE_HAS_CHARM	啊啊啊啊啊啊啊啊！&lt;page&gt;那个 {0}……真美！最珍贵的东西！小可爱，能让我看看它吗？一定要给我看！
DIVINE_NO_CHARM	&lt;page&gt;我想要它……{0} 的气味……我想要！
GEO_RELIEVE_HINT	将所有吉欧投入喷泉，以换取 {0} 吗？
BRUMM_DEEPNEST_3_HINT	所以我们侍奉……向来如此。是吗？&lt;page&gt;那就拿走 {0} 吧，这正是你来这里的原因。
GODSEEKER_COFFIN_NOKEY_HINT	一个用简单锁链住的茧。你能感知到里面的 {0}。
GODSEEKER_COFFIN_KEY_HINT	一个装有 {0} 的茧。&lt;br&gt;使用简单钥匙吗？
XUN_OFFER_HINT	明知只能得到一件糟糕的 {0}，也要接受这份礼物吗？
BRETTA_DIARY_POSTSCRIPT	&lt;page&gt;少女的珍宝&lt;br&gt;思索着该送什么给救命恩人，这位少女想起了自己房间下方珍藏的 {0}。虽然难以割舍，但她没有更好的东西能用来感谢对方。
DUSK_KNIGHT_CORPSE_HINT	一具身穿白色盔甲的尸体。你能清楚看见它拿着的 {0}，但不知为何，你感觉自己得穿过一段不必要地漫长、布满尖刺与锯刃的考验，才能把它拿到手。
DUSK_KNIGHT_CORPSE_OBTAINED	一具身穿白色盔甲的尸体。它拿着的东西你已经拿到了。
TUK_REMINDER	&lt;page&gt;要是你戴着 {0}，情况就不一样了……
ESSENCE	{0} 点精华
GEO	{0} 吉欧
GRUB	一只幼虫！（#{0}）
FLAME	格林之子火焰（#{0}）
PAY_GEO	支付 {0} 吉欧
REQUIRES_ESSENCE	需要 {0} 点精华
REQUIRES_GRUBS	需要 {0} 只幼虫
REQUIRES_GRUB	需要 1 只幼虫
REQUIRES_FLAMES	需要 {0} 团格林之子火焰
PAY_FLAMES	支付 {0} 团格林之子火焰
PAY_EGGS	支付 {0} 枚腐臭蛋
PAY_EGG	支付 1 枚腐臭蛋
CHARM_DESC_36_C_ALT	从前隐藏在内心的虚空，现已不受拘束。让佩戴者能以自身意志统合虚空。&lt;br&gt;可以卸下此护符，使暗影和同胞再次变得敌对。
PAYWALL_TEXT	……&lt;br&gt;你已达到每月免费文章阅读上限。订阅此限时优惠即可继续阅读！
PAYWALL_LIMIT	125'''
for line in text.splitlines():
    key, value = line.split('\t', 1)
    translations[key] = value
original = source.read_text(encoding='utf-8')
keys = re.findall(r'<entry\s+sheet="[^"]+"\s+key="([^"]+)">', original)
assert len(keys) == 189, len(keys)
assert set(keys) == set(translations), (set(keys)-set(translations), set(translations)-set(keys))
def replace(match):
    return match.group(1) + translations[match.group(2)] + match.group(3)
result, count = re.subn(r'(<entry\s+sheet="[^"]+"\s+key="([^"]+)">).*?(</entry>)', replace, original)
assert count == 189, count
target.parent.mkdir(parents=True, exist_ok=True)
target.write_text(result, encoding='utf-8', newline='')
print(f'{target}\nentries={count}')
