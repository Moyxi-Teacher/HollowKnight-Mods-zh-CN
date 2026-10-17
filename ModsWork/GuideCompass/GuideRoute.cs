using UnityEngine;

namespace GuideCompass;

internal enum GuideCompletionKind
{
    Manual,
    Interaction,
    Location,
    PlayerDataBool,
    PlayerDataBoolAny,
    PlayerDataBoolAll,
    PlayerDataIntAtLeast,
    FsmStateEntered,
    NearbyTarget,
    EnemyKills,
    BenchUse,
}

internal enum GuideInteractionKind
{
    GeoPickup,
    BreakableWall,
    BlueBlood,
    LoreMarker,
}

internal sealed class GuideStep
{
    public string Target { get; }
    public string Hint { get; }
    public Vector2 Direction { get; }
    public bool Optional { get; }
    public GuideCompletionKind CompletionKind { get; private set; }
    public string SceneName { get; private set; }
    public Vector2 Position { get; private set; }
    public float ArrivalRadius { get; private set; }
    public string PlayerDataKey { get; private set; }
    public string[] PlayerDataKeys { get; private set; }
    public int RequiredIntValue { get; private set; }
    public int RequiredKillCount { get; private set; }
    public string FsmName { get; private set; }
    public string FsmStateName { get; private set; }
    public string ObjectName { get; private set; }
    public string[] TargetObjectKeywords { get; private set; }
    public GuideInteractionKind InteractionKind { get; private set; }
    public string[] SkipPlayerDataKeys { get; private set; }

    private GuideStep(string target, string hint, Vector2 direction, bool optional)
    {
        Target = target;
        Hint = hint;
        Direction = direction.normalized;
        Optional = optional;
    }

    public static GuideStep AtLocation(
        string target,
        string hint,
        string sceneName,
        Vector2 position,
        float arrivalRadius = 2.5f,
        bool optional = false)
    {
        return new GuideStep(target, hint, Vector2.zero, optional)
        {
            CompletionKind = GuideCompletionKind.Location,
            SceneName = sceneName,
            Position = position,
            ArrivalRadius = Mathf.Max(0.1f, arrivalRadius),
        };
    }

    public static GuideStep Manual(string target, string hint, Vector2 direction, bool optional = false)
    {
        return new GuideStep(target, hint, direction, optional)
        {
            CompletionKind = GuideCompletionKind.Manual,
        };
    }

    public static GuideStep WhenInteractionOccurs(
        string target,
        string hint,
        GuideInteractionKind interactionKind,
        Vector2 direction,
        bool optional = false)
    {
        return new GuideStep(target, hint, direction, optional)
        {
            CompletionKind = GuideCompletionKind.Interaction,
            InteractionKind = interactionKind,
        };
    }

    public static GuideStep WhenPlayerDataBoolIsTrue(
        string target,
        string hint,
        string playerDataKey,
        bool optional = false,
        Vector2 direction = default)
    {
        return new GuideStep(target, hint, direction, optional)
        {
            CompletionKind = GuideCompletionKind.PlayerDataBool,
            PlayerDataKey = playerDataKey,
        };
    }

    public static GuideStep WhenAnyPlayerDataBoolIsTrue(
        string target,
        string hint,
        bool optional = false,
        Vector2 direction = default,
        params string[] playerDataKeys)
    {
        return new GuideStep(target, hint, direction, optional)
        {
            CompletionKind = GuideCompletionKind.PlayerDataBoolAny,
            PlayerDataKeys = playerDataKeys,
        };
    }

    public static GuideStep WhenAllPlayerDataBoolsAreTrue(
        string target,
        string hint,
        bool optional = false,
        Vector2 direction = default,
        params string[] playerDataKeys)
    {
        return new GuideStep(target, hint, direction, optional)
        {
            CompletionKind = GuideCompletionKind.PlayerDataBoolAll,
            PlayerDataKeys = playerDataKeys,
        };
    }

    public static GuideStep WhenPlayerDataIntReaches(
        string target,
        string hint,
        string playerDataKey,
        int requiredValue,
        bool optional = false,
        Vector2 direction = default)
    {
        return new GuideStep(target, hint, direction, optional)
        {
            CompletionKind = GuideCompletionKind.PlayerDataIntAtLeast,
            PlayerDataKey = playerDataKey,
            RequiredIntValue = requiredValue,
        };
    }

    public static GuideStep WhenFsmStateIsEntered(
        string target,
        string hint,
        string sceneName,
        string fsmName,
        string fsmStateName,
        string objectName = null,
        bool optional = false)
    {
        return new GuideStep(target, hint, Vector2.zero, optional)
        {
            CompletionKind = GuideCompletionKind.FsmStateEntered,
            SceneName = sceneName,
            FsmName = fsmName,
            FsmStateName = fsmStateName,
            ObjectName = objectName,
        };
    }

    public static GuideStep NearTarget(
        string target,
        string hint,
        string sceneName,
        float arrivalRadius,
        Vector2 direction,
        params string[] targetKeywords)
    {
        return new GuideStep(target, hint, direction, false)
        {
            CompletionKind = GuideCompletionKind.NearbyTarget,
            SceneName = sceneName,
            ArrivalRadius = Mathf.Max(0.1f, arrivalRadius),
            TargetObjectKeywords = targetKeywords,
        };
    }

    public static GuideStep WhenEnemiesAreDefeated(
        string target,
        string hint,
        int requiredCount,
        Vector2 direction,
        params string[] targetKeywords)
    {
        return new GuideStep(target, hint, direction, false)
        {
            CompletionKind = GuideCompletionKind.EnemyKills,
            RequiredKillCount = Mathf.Max(1, requiredCount),
            TargetObjectKeywords = targetKeywords,
        };
    }

    public static GuideStep WhenBenchIsUsed(string target, string hint, Vector2 direction)
    {
        return new GuideStep(target, hint, direction, false)
        {
            CompletionKind = GuideCompletionKind.BenchUse,
        };
    }

    public bool IsComplete(int killCount = 0, Transform trackedTarget = null)
    {
        if (PlayerData.instance == null)
        {
            return false;
        }

        if (SkipPlayerDataKeys != null
            && System.Array.Exists(SkipPlayerDataKeys, PlayerData.instance.GetBool))
        {
            return true;
        }

        return CompletionKind switch
        {
            GuideCompletionKind.Location => GameManager.instance != null
                && HeroController.instance != null
                && string.Equals(GameManager.instance.sceneName, SceneName, System.StringComparison.Ordinal)
                && Vector2.Distance(HeroController.instance.transform.position, Position) <= ArrivalRadius,
            GuideCompletionKind.NearbyTarget => GameManager.instance != null
                && HeroController.instance != null
                && trackedTarget != null
                && (string.IsNullOrEmpty(SceneName)
                    || string.Equals(GameManager.instance.sceneName, SceneName, System.StringComparison.Ordinal))
                && Vector2.Distance(HeroController.instance.transform.position, trackedTarget.position) <= ArrivalRadius,
            GuideCompletionKind.EnemyKills => killCount >= RequiredKillCount,
            GuideCompletionKind.PlayerDataBool => PlayerData.instance.GetBool(PlayerDataKey),
            GuideCompletionKind.PlayerDataBoolAny => PlayerDataKeys != null
                && System.Array.Exists(PlayerDataKeys, PlayerData.instance.GetBool),
            GuideCompletionKind.PlayerDataBoolAll => PlayerDataKeys != null
                && PlayerDataKeys.Length > 0
                && System.Array.TrueForAll(PlayerDataKeys, PlayerData.instance.GetBool),
            GuideCompletionKind.PlayerDataIntAtLeast => PlayerData.instance.GetInt(PlayerDataKey) >= RequiredIntValue,
            _ => false,
        };
    }

    public bool MatchesFsmState(PlayMakerFSM fsm, string stateName)
    {
        if (CompletionKind == GuideCompletionKind.Interaction)
        {
            if (fsm == null)
            {
                return false;
            }

            if (InteractionKind == GuideInteractionKind.LoreMarker
                && MatchesTargetObject(fsm.gameObject)
                && IsLoreInteractionState(stateName))
            {
                return true;
            }

            if (InteractionKind == GuideInteractionKind.GeoPickup)
            {
                return string.Equals(fsm.FsmName, "Geo Rock", System.StringComparison.Ordinal)
                    && string.Equals(stateName, "Destroy", System.StringComparison.Ordinal);
            }

            return false;
        }

        if (CompletionKind != GuideCompletionKind.FsmStateEntered
            || fsm == null
            || !string.Equals(stateName, FsmStateName, System.StringComparison.Ordinal)
            || !string.Equals(fsm.FsmName, FsmName, System.StringComparison.Ordinal))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(SceneName)
            && (GameManager.instance == null
                || !string.Equals(GameManager.instance.sceneName, SceneName, System.StringComparison.Ordinal)))
        {
            return false;
        }

        return string.IsNullOrEmpty(ObjectName)
            || string.Equals(fsm.gameObject.name, ObjectName, System.StringComparison.Ordinal);
    }

    public bool MatchesInteraction(GuideInteractionKind interactionKind, GameObject source)
    {
        return CompletionKind == GuideCompletionKind.Interaction
            && InteractionKind == interactionKind
            && MatchesTargetObject(source);
    }

    public bool MatchesEnemyDeath(GameObject source)
    {
        return CompletionKind == GuideCompletionKind.EnemyKills && MatchesTargetObject(source);
    }

    public bool MatchesBenchRested(string sceneName)
    {
        return CompletionKind == GuideCompletionKind.BenchUse
            && !string.IsNullOrEmpty(sceneName);
    }

    public bool MatchesInteractionEvent(PlayMakerFSM fsm, string eventName)
    {
        return CompletionKind == GuideCompletionKind.Interaction
            && InteractionKind == GuideInteractionKind.LoreMarker
            && fsm != null
            && MatchesTargetObject(fsm.gameObject)
            && IsLoreInteractionState(eventName);
    }

    private bool MatchesTargetObject(GameObject source)
    {
        if (source == null || TargetObjectKeywords == null || TargetObjectKeywords.Length == 0)
        {
            return source != null;
        }

        for (Transform current = source.transform; current != null; current = current.parent)
        {
            foreach (string keyword in TargetObjectKeywords)
            {
                if (!string.IsNullOrEmpty(keyword)
                    && current.name.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool IsLoreInteractionState(string name)
    {
        return !string.IsNullOrEmpty(name)
            && (name.IndexOf("inspect", System.StringComparison.OrdinalIgnoreCase) >= 0
                || name.IndexOf("interact", System.StringComparison.OrdinalIgnoreCase) >= 0
                || name.IndexOf("read", System.StringComparison.OrdinalIgnoreCase) >= 0
                || name.IndexOf("activate", System.StringComparison.OrdinalIgnoreCase) >= 0
                || name.IndexOf("convo", System.StringComparison.OrdinalIgnoreCase) >= 0);
    }

    public GuideStep WithTargetObjects(params string[] keywords)
    {
        TargetObjectKeywords = keywords;
        return this;
    }

    public GuideStep SkipWhenAnyPlayerDataBoolIsTrue(params string[] playerDataKeys)
    {
        SkipPlayerDataKeys = playerDataKeys;
        return this;
    }
}

internal static class GuideRoute
{
    public const string Title = "第一期：十字路口的小骑士";
    public const int ProgressVersion = 4;

    public static readonly GuideStep[] Steps =
    {
        BirthplaceGeoBag("出生地左侧暗门里的小钱袋", "从出生地往左直接进入暗门，将里面的小钱袋完全打碎。", Vector2.left),
        BirthplaceInteraction("白王印记", "跳上平台后向上、再往左；与白王印记互动并看到提示。", GuideInteractionKind.LoreMarker, Vector2.up)
            .WithTargetObjects("Lore", "Tablet", "King", "White", "Mark"),
        BirthplaceInteraction("打破左侧的门", "继续往左攻击并打破挡路的门。", GuideInteractionKind.BreakableWall, Vector2.left)
            .WithTargetObjects("Breakable Wall", "Break Wall", "Door"),
        BirthplaceGeoBag("左侧平台的小钱袋", "爬上平台将隐藏小钱袋完全打碎。", Vector2.left),
        BirthplaceInteraction("塔顶左角的蓝血", "回到塔形平台并一路向上，打破蓝血容器并取得蓝血。", GuideInteractionKind.BlueBlood, Vector2.up),
        BirthplaceGeoBag(
            "塔平台左侧墙缝里的小钱袋",
            "塔平台会塌陷。拿到蓝血后等平台塌下，前往塔平台左侧墙壁的缝隙，将里面的小钱袋完全打碎。",
            Vector2.left),
        BirthplaceGeoBag(
            "尖刺平台右下角的小钱袋",
            "打碎墙缝里的钱袋后下来，向右越过有尖刺的平台，再将右下角出现的小钱袋完全打碎。",
            Vector2.right + Vector2.down),
        GuideStep.WhenPlayerDataBoolIsTrue(
            "取得亡者之怒护符（可选）",
            "从尖刺平台下劈落到下方区域并取得亡者之怒护符；也可按 F7 跳过。",
            "gotCharm_6",
            optional: true),
        GuideStep.WhenPlayerDataBoolIsTrue(
            "抵达德特茅斯",
            "从出生地沿捷径离开，完成信仰之跃抵达山脚小镇；可跳过的出生地目标会自动略过。",
            "visitedDirtmouth",
            direction: Vector2.right)
            .WithTargetObjects("Dirtmouth"),
        GuideStep.WhenPlayerDataBoolIsTrue(
            "查看黑卵圣殿",
            "从德特茅斯水井下到遗忘十字路口，再往右进入黑卵圣殿；存档记录后自动推进。",
            "eggTempleVisited",
            direction: Vector2.down)
            .WithTargetObjects("Black Egg", "Egg Temple"),
        GuideStep.NearTarget(
            "到达虫爷爷的位置",
            "离开圣殿后一路往左到头，进入虫爷爷房间；到达房间即可完成，不要求对话。继续主线前准备至少 46 吉欧。",
            null,
            3f,
            Vector2.left,
            "Elderbug"),
        GuideStep.WhenPlayerDataBoolIsTrue(
            "购买遗忘十字路地图",
            "离开虫爷爷房间后往下寻找制图师 Cornifer 并购买地图；购买标志出现后自动推进。",
            "mapCrossroads",
            direction: Vector2.left)
            .WithTargetObjects("Cornifer"),
        GuideStep.WhenEnemiesAreDefeated(
            "击败酸水路上的大型敌人",
            "从地图区域往下进入左侧岔路，穿过酸水并击败大型敌人，再前往最左侧的幼虫罐。",
            1,
            Vector2.left,
            "Moss Knight"),
        GuideStep.WhenPlayerDataIntReaches(
            "救出第一只幼虫",
            "打破最左侧幼虫罐并救出幼虫；存档幼虫数达到 1 后自动推进。",
            "grubsCollected",
            1,
            direction: Vector2.left)
            .WithTargetObjects("Grub"),
        GeoBagStep("车站旁的小钱袋", "救出幼虫后一直往右到鹿角虫车站标志，再往上将隐藏小钱袋完全打碎。", Vector2.up, optional: true),
        WallStep("车站标志上方的隐藏墙", "打破车站标志上方的隐藏墙，然后穿过墙往右、再往下进入温泉。", Vector2.up, optional: true),
        GuideStep.WhenBenchIsUsed("在温泉长椅休息", "进入温泉并使用长椅存档回血；之后离开温泉继续向右。", Vector2.right),
        GuideStep.WhenEnemiesAreDefeated(
            "击败三只阿斯匹德",
            "触发小战斗并击败三只阿斯匹德。它们会瞄准当前位置射击，可利用平台躲避。",
            3,
            Vector2.right,
            "Aspid"),
        GeoBagStep("穿过陷阱后的钱袋", "打开门后向右越过陷阱和小怪，将当前路线上的目标钱袋完全打碎。", Vector2.right, optional: true),
        GuideStep.WhenPlayerDataBoolIsTrue(
            "开启遗忘十字路车站",
            "向右找到鹿角虫车站并进入站内，支付吉欧开启遗忘十字路口车站。",
            "openedCrossroads",
            direction: Vector2.right)
            .WithTargetObjects("Stag Station", "Stag"),
        GuideStep.WhenPlayerDataBoolIsTrue(
            "开启德特茅斯车站",
            "乘车返回德特茅斯并开启镇上的鹿角虫车站。",
            "openedTown",
            direction: Vector2.up)
            .WithTargetObjects("Stag Station", "Stag"),
        GuideStep.WhenPlayerDataBoolIsTrue(
            "购买羽毛笔",
            "前往制图师商店购买羽毛笔；存档标志出现后自动推进。",
            "hasQuill",
            direction: Vector2.up)
            .WithTargetObjects("Cornifer"),
        GuideStep.WhenPlayerDataBoolIsTrue(
            "购买指南针护符",
            "购买指南针护符；存档标志出现后自动推进。",
            "gotCharm_2",
            direction: Vector2.up)
            .WithTargetObjects("Iselda"),
        GuideStep.WhenBenchIsUsed("回遗忘十字路口坐长椅", "乘车返回遗忘十字路口，在长椅存档并更新地图，然后出门往下再往左。", Vector2.down),
        GeoBagStep("格鲁兹之母附近的小钱袋", "从车站区域往下再往左，路暂未开放；先打碎附近可选小钱袋，再继续向右。", Vector2.left, optional: true),
        GuideStep.WhenPlayerDataBoolIsTrue(
            "击败格鲁兹之母",
            "继续向右触发格鲁兹之母战斗；留意连续上下撞击，击败后还会出现七只小苍蝇。",
            "giantFlyDefeated",
            direction: Vector2.right)
            .WithTargetObjects("Gruz Mother", "Giant Fly", "GiantFly"),
        GuideStep.WhenPlayerDataBoolIsTrue(
            "救出斯莱",
            "击败 Boss 后往右、往下，再向左进入小房间并和斯莱交谈；存档记录后自动推进。",
            "slyRescued",
            direction: Vector2.down)
            .WithTargetObjects("Sly"),
        GuideStep.WhenPlayerDataIntReaches(
            "救出第二只幼虫",
            "离开斯莱房间后往上打开捷径，再向上并进入左侧幼虫区域；连续下劈打破幼虫罐。",
            "grubsCollected",
            2,
            direction: Vector2.up)
            .WithTargetObjects("Grub"),
        WallStep("幼虫区域的隐藏墙", "救出第二只幼虫后上行离开，遇到持盾小怪可用下劈处理；往左跳上平台并打破隐藏墙。", Vector2.left),
        GuideStep.WhenPlayerDataIntReaches(
            "救出第三只幼虫",
            "进入隐藏墙后的区域，打破幼虫罐并救出第三只幼虫。",
            "grubsCollected",
            3,
            direction: Vector2.left)
            .WithTargetObjects("Grub"),
        GuideStep.WhenPlayerDataIntReaches(
            "救出第四只幼虫",
            "攻略目标是四只幼虫，但正文只给出前三只的具体路线；参考地图上的幼虫标记，找到并救出任意第四只。",
            "grubsCollected",
            4,
            direction: Vector2.left),
        GuideStep.Manual("可选挑战精英战", "前方精英战可以跳过；若要挑战，从上方绕到龙牙哥左侧卡位，攻击后撤，躲大波时跳跃。", Vector2.left, optional: true),
        GuideStep.WhenPlayerDataBoolIsTrue(
            "击败假骑士",
            "从车站上方平台向左进入假骑士房间。留意高跳下砸、后撤蓄力发波或左右乱砸；进入下一轮时可踢落石攻击。击败后自动推进；离开房间会错过城市纹章。",
            "falseKnightDefeated",
            direction: Vector2.up)
            .WithTargetObjects("False Knight", "FalseKnight"),
        GuideStep.WhenPlayerDataBoolIsTrue(
            "取得城市纹章",
            "胜利后拾取城市纹章；拿到纹章后自动推进。",
            "hasCityKey",
            direction: Vector2.left)
            .WithTargetObjects("City Crest", "City Key"),
        GeoBagStep("假骑士左侧的大钱袋", "取得城市纹章后到左边将大钱袋完全打碎；之后往左再向上离开本期路线。", Vector2.left, optional: true),
    };

    private static GuideStep BirthplaceGeoBag(string target, string hint, Vector2 direction)
    {
        return GeoBagStep(target, hint, direction)
            .SkipWhenAnyPlayerDataBoolIsTrue("gotCharm_6", "visitedDirtmouth");
    }

    private static GuideStep BirthplaceInteraction(
        string target,
        string hint,
        GuideInteractionKind interactionKind,
        Vector2 direction)
    {
        return GuideStep.WhenInteractionOccurs(target, hint, interactionKind, direction, optional: true)
            .SkipWhenAnyPlayerDataBoolIsTrue("gotCharm_6", "visitedDirtmouth");
    }

    private static GuideStep GeoBagStep(string target, string hint, Vector2 direction, bool optional = false)
    {
        return GuideStep.WhenInteractionOccurs(
                target,
                hint,
                GuideInteractionKind.GeoPickup,
                direction,
                optional)
            .WithTargetObjects("Geo Rock");
    }

    private static GuideStep WallStep(string target, string hint, Vector2 direction, bool optional = false)
    {
        return GuideStep.WhenInteractionOccurs(
                target,
                hint,
                GuideInteractionKind.BreakableWall,
                direction,
                optional)
            .WithTargetObjects("Breakable Wall", "Break Wall");
    }
}
