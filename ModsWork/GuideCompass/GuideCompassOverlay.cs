using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GuideCompass;

internal sealed class GuideCompassOverlay : MonoBehaviour
{
    private const KeyCode PreviousKey = KeyCode.F6;
    private const KeyCode NextKey = KeyCode.F7;
    private const KeyCode ToggleKey = KeyCode.F8;
    private const KeyCode TeleportToTargetKey = KeyCode.F9;

    private static readonly Vector2 ArrowUpperTail = new(4f, 4f);
    private static readonly Vector2 ArrowTip = new(28f, 16f);
    private static readonly Vector2 ArrowLowerTail = new(4f, 28f);
    private static readonly Vector2 ArrowUpperBaseEnd = new(4f, 11f);
    private static readonly Vector2 ArrowLowerBaseStart = new(4f, 21f);
    private static int _loadedStepIndex;
    private static bool _loadedRouteFinished;
    private static int _loadedKillCount;
    private static bool _hasLoadedProgress;
    private static Vector2? _observedTutorialLoreMarkerPosition;
    private static readonly HashSet<string> LoggedLoreDiagnostics = new();

    private Texture2D _arrowTexture;
    private GUIStyle _titleStyle;
    private GUIStyle _bodyStyle;
    private GUIStyle _smallStyle;
    private GUIStyle _shortcutStyle;
    private float _styleScale;
    private float _nextTargetScanTime;
    private int _cachedTargetStep = -1;
    private string _cachedTargetScene;
    private string _cachedTargetName;
    private Transform _cachedTarget;
    private int _stepIndex;
    private int _killCount;
    private readonly HashSet<int> _countedEnemyIds = new();
    private bool _visible = true;
    private bool _routeFinished;
    private bool _restorePending = true;
    private bool _pendingTeleport;
    private int _pendingTeleportStepIndex = -1;
    private string _pendingTeleportScene;
    private float _pendingTeleportStartedAt;

    public static GuideCompassOverlay Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        if (_hasLoadedProgress)
        {
            _stepIndex = Mathf.Clamp(_loadedStepIndex, 0, GuideRoute.Steps.Length - 1);
            _routeFinished = _loadedRouteFinished;
            _killCount = Mathf.Max(0, _loadedKillCount);
            _hasLoadedProgress = false;
        }

        _arrowTexture = CreateArrowTexture();
    }

    private void Update()
    {
        if (_pendingTeleport)
        {
            if (Time.realtimeSinceStartup - _pendingTeleportStartedAt > 120f)
            {
                Modding.Logger.Log("[攻略指南针] 跨场景传送超时：场景尚未完成加载，已取消待执行传送。");
                ClearPendingTeleport();
            }
            else
            {
                return;
            }
        }

        if (HeroController.instance == null || GameManager.instance == null)
        {
            return;
        }

        if (_restorePending)
        {
            RestoreProgress();
        }

        if (Input.GetKeyDown(PreviousKey))
        {
            if (_routeFinished)
            {
                _routeFinished = false;
                _stepIndex = Mathf.Max(0, _stepIndex - 1);
            }
            else
            {
                _stepIndex = Mathf.Max(0, _stepIndex - 1);
            }
            ResetCurrentTarget();
            _killCount = 0;
            _countedEnemyIds.Clear();
        }
        else if (Input.GetKeyDown(NextKey))
        {
            AdvanceStep();
        }
        else if (Input.GetKeyDown(ToggleKey))
        {
            _visible = !_visible;
        }
        else if (Input.GetKeyDown(TeleportToTargetKey))
        {
            TeleportToCurrentTarget();
        }

        TryAdvanceIfComplete();
    }

    private void TeleportToCurrentTarget()
    {
        GuideStep step = GuideRoute.Steps[_stepIndex];
        string targetScene = step.GetTeleportSceneName();
        string currentScene = GameManager.instance.sceneName;
        if (!string.IsNullOrEmpty(targetScene)
            && !string.Equals(currentScene, targetScene, System.StringComparison.Ordinal))
        {
            BeginCrossSceneTeleport(targetScene);
            return;
        }

        Vector2 targetPosition;
        if (step.CompletionKind == GuideCompletionKind.Location)
        {
            if (!string.Equals(currentScene, step.SceneName, System.StringComparison.Ordinal))
            {
                Modding.Logger.Log($"[攻略指南针] 无法传送：目标位于场景 {step.SceneName}，当前场景为 {GameManager.instance.sceneName}。");
                return;
            }

            targetPosition = step.Position;
        }
        else if (!TryGetTrackedTarget(step, out targetPosition))
        {
            Modding.Logger.Log($"[攻略指南针] 无法传送：当前目标“{step.Target}”没有可定位的场景坐标。");
            return;
        }

        TeleportHeroToTarget(step, targetPosition);
    }

    private void BeginCrossSceneTeleport(string targetScene)
    {
        if (_pendingTeleport)
        {
            Modding.Logger.Log("[攻略指南针] 已有跨场景传送正在执行；请等待它完成后再按 F9。");
            return;
        }

        _pendingTeleport = true;
        _pendingTeleportStepIndex = _stepIndex;
        _pendingTeleportScene = targetScene;
        _pendingTeleportStartedAt = Time.realtimeSinceStartup;
        Modding.Logger.Log($"[攻略指南针] 正在加载目标场景“{targetScene}”，加载完成后将重新定位“{GuideRoute.Steps[_stepIndex].Target}”并传送。");
        GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
        {
            SceneName = targetScene,
            EntryGateName = string.Empty,
            Visualization = GameManager.SceneLoadVisualizations.Default,
        });
    }

    internal static void NotifySceneEntered()
    {
        GuideCompassOverlay instance = Instance;
        if (instance == null || !instance._pendingTeleport)
        {
            return;
        }

        if (GameManager.instance == null
            || !string.Equals(
                GameManager.instance.sceneName,
                instance._pendingTeleportScene,
                System.StringComparison.Ordinal))
        {
            Modding.Logger.Log(
                $"[攻略指南针] 跨场景传送已取消：加载完成的场景与目标场景“{instance._pendingTeleportScene}”不一致。");
            instance.ClearPendingTeleport();
            return;
        }

        instance.StartCoroutine(instance.CompletePendingTeleport());
    }

    private IEnumerator CompletePendingTeleport()
    {
        int stepIndex = _pendingTeleportStepIndex;
        string targetScene = _pendingTeleportScene;
        yield return new WaitForSecondsRealtime(0.25f);

        if (!_pendingTeleport)
        {
            yield break;
        }

        if (_stepIndex != stepIndex)
        {
            Modding.Logger.Log("[攻略指南针] 跨场景传送已取消：加载期间当前攻略目标发生变化。");
            ClearPendingTeleport();
            yield break;
        }

        if (GameManager.instance == null
            || !string.Equals(GameManager.instance.sceneName, targetScene, System.StringComparison.Ordinal))
        {
            Modding.Logger.Log($"[攻略指南针] 跨场景传送失败：当前未处于目标场景“{targetScene}”。");
            ClearPendingTeleport();
            yield break;
        }

        GuideStep step = GuideRoute.Steps[stepIndex];
        Vector2 targetPosition;
        bool targetResolved;
        if (step.CompletionKind == GuideCompletionKind.Location)
        {
            targetResolved = string.Equals(step.SceneName, targetScene, System.StringComparison.Ordinal);
            targetPosition = step.Position;
        }
        else
        {
            ResetCurrentTarget();
            targetResolved = TryGetTrackedTarget(step, out targetPosition);
        }

        if (!targetResolved)
        {
            Modding.Logger.Log(
                $"[攻略指南针] 跨场景传送失败：场景“{targetScene}”加载完成，但无法唯一定位目标“{step.Target}”。");
            ClearPendingTeleport();
            yield break;
        }

        ClearPendingTeleport();
        TeleportHeroToTarget(step, targetPosition);
    }

    private void ClearPendingTeleport()
    {
        _pendingTeleport = false;
        _pendingTeleportStepIndex = -1;
        _pendingTeleportScene = null;
        _pendingTeleportStartedAt = 0f;
    }

    private static void TeleportHeroToTarget(GuideStep step, Vector2 targetPosition)
    {
        HeroController hero = HeroController.instance;
        if (hero == null)
        {
            Modding.Logger.Log($"[攻略指南针] 无法传送：当前目标“{step.Target}”已定位，但骑士对象不可用。");
            return;
        }

        Vector2 destination = targetPosition + Vector2.up * 1.5f;
        hero.transform.position = new Vector3(destination.x, destination.y, hero.transform.position.z);
        Rigidbody2D body = hero.GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.velocity = Vector2.zero;
        }

        Modding.Logger.Log(
            $"[攻略指南针] 已传送到当前目标“{step.Target}”附近：({destination.x:F2}, {destination.y:F2})。");
    }

    internal static void ResetProgress()
    {
        if (Instance == null)
        {
            return;
        }

        Instance._stepIndex = 0;
        Instance._killCount = 0;
        Instance._countedEnemyIds.Clear();
        Instance._visible = true;
        Instance._routeFinished = false;
        Instance._restorePending = false;
    }

    internal static void LoadProgress(
        int stepIndex,
        bool routeFinished,
        int killCount,
        bool hasLoreMarkerPosition,
        float loreMarkerX,
        float loreMarkerY)
    {
        _loadedStepIndex = stepIndex;
        _loadedRouteFinished = routeFinished;
        _loadedKillCount = killCount;
        _observedTutorialLoreMarkerPosition = hasLoreMarkerPosition
            ? new Vector2(loreMarkerX, loreMarkerY)
            : null;

        GuideCompassOverlay instance = Instance;
        if (instance == null)
        {
            _hasLoadedProgress = true;
            return;
        }

        _hasLoadedProgress = false;
        instance._stepIndex = Mathf.Clamp(stepIndex, 0, GuideRoute.Steps.Length - 1);
        instance._routeFinished = routeFinished;
        instance._killCount = Mathf.Max(0, killCount);
        instance._restorePending = true;
    }

    internal static GuideCompassLocalSettings CaptureProgress()
    {
        GuideCompassOverlay instance = Instance;
        return instance == null
            ? new GuideCompassLocalSettings
            {
                RouteVersion = GuideRoute.ProgressVersion,
                HasTutorialLoreMarkerPosition = _observedTutorialLoreMarkerPosition.HasValue,
                TutorialLoreMarkerX = _observedTutorialLoreMarkerPosition?.x ?? 0f,
                TutorialLoreMarkerY = _observedTutorialLoreMarkerPosition?.y ?? 0f,
            }
            : new GuideCompassLocalSettings
            {
                StepIndex = instance._stepIndex,
                RouteFinished = instance._routeFinished,
                KillCount = instance._killCount,
                RouteVersion = GuideRoute.ProgressVersion,
                HasTutorialLoreMarkerPosition = _observedTutorialLoreMarkerPosition.HasValue,
                TutorialLoreMarkerX = _observedTutorialLoreMarkerPosition?.x ?? 0f,
                TutorialLoreMarkerY = _observedTutorialLoreMarkerPosition?.y ?? 0f,
            };
    }

    private void RestoreProgress()
    {
        _restorePending = false;
        AdvancePastCompletedSteps();
    }

    private void TryAdvanceIfComplete()
    {
        AdvancePastCompletedSteps();
    }

    private void AdvancePastCompletedSteps()
    {
        while (_stepIndex < GuideRoute.Steps.Length - 1
            && IsCurrentStepComplete())
        {
            AdvanceStep();
        }

        if (_stepIndex == GuideRoute.Steps.Length - 1 && IsCurrentStepComplete())
        {
            _routeFinished = true;
        }
    }

    private bool IsCurrentStepComplete()
    {
        GuideStep step = GuideRoute.Steps[_stepIndex];
        if (step.TargetObjectKeywords != null && step.TargetObjectKeywords.Length > 0)
        {
            TryGetTrackedTarget(step, out _);
        }

        return step.IsComplete(_killCount, _cachedTarget);
    }

    internal static void NotifyFsmStateEntered(PlayMakerFSM fsm, string stateName)
    {
        GuideCompassOverlay instance = Instance;
        if (instance == null || instance._routeFinished)
        {
            return;
        }

        GuideStep step = GuideRoute.Steps[instance._stepIndex];
        if (step.MatchesFsmState(fsm, stateName)
            && (step.CompletionKind != GuideCompletionKind.Interaction
                || step.InteractionKind != GuideInteractionKind.GeoPickup
                || instance.IsCurrentTargetObject(fsm.gameObject)))
        {
            instance.AdvanceStep();
        }
    }

    internal static void NotifyLoreMarkerActivated(AudioEvent audioEvent, AudioSource source, Vector3 position)
    {
        GuideCompassOverlay instance = Instance;
        if (audioEvent.Clip == null
            || !string.Equals(audioEvent.Clip.name, "Lore_Tablet_activate_temp", System.StringComparison.Ordinal)
            || GameManager.instance == null
            || !string.Equals(GameManager.instance.sceneName, "Tutorial_01", System.StringComparison.Ordinal))
        {
            return;
        }

        _observedTutorialLoreMarkerPosition = new Vector2(position.x, position.y);
        Vector3 sourcePosition = source == null ? position : source.transform.position;
        Modding.Logger.Log(
            $"[攻略指南针] 记录最近检查的教程碑文：音效位置={FormatPosition(position)}，" +
            $"AudioSource位置={FormatPosition(sourcePosition)}，骑士位置=" +
            $"{(HeroController.instance == null ? "<none>" : FormatPosition(HeroController.instance.transform.position))}");

        if (instance == null || instance._routeFinished || HeroController.instance == null)
        {
            return;
        }

        GuideStep step = GuideRoute.Steps[instance._stepIndex];
        if (step.CompletionKind == GuideCompletionKind.Interaction
            && step.InteractionKind == GuideInteractionKind.LoreMarker
            && Vector2.Distance(HeroController.instance.transform.position, position) <= 6f)
        {
            instance.AdvanceStep();
        }
    }

    internal static void NotifyLoreMarkerFsmEvent(PlayMakerFSM fsm, string eventName)
    {
        GuideCompassOverlay instance = Instance;
        if (instance == null
            || instance._routeFinished
            || fsm == null
            || GameManager.instance == null
            || HeroController.instance == null
            || !string.Equals(GameManager.instance.sceneName, "Tutorial_01", System.StringComparison.Ordinal))
        {
            return;
        }

        GuideStep step = GuideRoute.Steps[instance._stepIndex];
        if (step.CompletionKind != GuideCompletionKind.Interaction
            || step.InteractionKind != GuideInteractionKind.LoreMarker
            || Vector2.Distance(fsm.transform.position, HeroController.instance.transform.position) > 10f)
        {
            return;
        }

        LogLoreDiagnostic(
            $"FSM事件={eventName}，FSM={GetFsmDescription(fsm)}，位置={FormatPosition(fsm.transform.position)}");
        if (!IsLoreInteractionSignal(eventName))
        {
            return;
        }

        LogLoreDiagnostic(
            $"候选碑文 FSM 事件={eventName}，FSM={GetFsmDescription(fsm)}，位置={FormatPosition(fsm.transform.position)}");
    }

    internal static void NotifyInteraction(GuideInteractionKind interactionKind, GameObject source)
    {
        GuideCompassOverlay instance = Instance;
        if (instance == null || instance._routeFinished)
        {
            return;
        }

        GuideStep step = GuideRoute.Steps[instance._stepIndex];
        if (step.MatchesInteraction(interactionKind, source)
            && (interactionKind is not (GuideInteractionKind.GeoPickup or GuideInteractionKind.BreakableWall)
                || instance.IsCurrentTargetObject(source)))
        {
            instance.AdvanceStep();
        }
    }

    internal static void NotifyEnemyDefeated(GameObject source)
    {
        GuideCompassOverlay instance = Instance;
        if (instance == null || instance._routeFinished)
        {
            return;
        }

        GuideStep step = GuideRoute.Steps[instance._stepIndex];
        if (step.MatchesEnemyDeath(source)
            && instance.IsCurrentTargetObject(source)
            && instance._countedEnemyIds.Add(source.GetInstanceID()))
        {
            instance._killCount++;
            if (instance._killCount >= step.RequiredKillCount)
            {
                instance.AdvanceStep();
            }
        }
    }

    internal static void NotifyBenchRested(string sceneName)
    {
        GuideCompassOverlay instance = Instance;
        if (instance == null || instance._routeFinished)
        {
            return;
        }

        if (GuideRoute.Steps[instance._stepIndex].MatchesBenchRested(sceneName))
        {
            instance.AdvanceStep();
        }
    }

    private bool IsCurrentTargetObject(GameObject source)
    {
        if (_cachedTarget == null || _cachedTargetStep != _stepIndex)
        {
            TryGetTrackedTarget(GuideRoute.Steps[_stepIndex], out _);
        }

        if (source == null || _cachedTarget == null)
        {
            return false;
        }

        Transform sourceTransform = source.transform;
        return sourceTransform == _cachedTarget
            || sourceTransform.IsChildOf(_cachedTarget)
            || _cachedTarget.IsChildOf(sourceTransform);
    }

    private void AdvanceStep()
    {
        if (_stepIndex < GuideRoute.Steps.Length - 1)
        {
            _stepIndex++;
            _killCount = 0;
            _countedEnemyIds.Clear();
            ResetCurrentTarget();
        }
        else
        {
            _routeFinished = true;
        }
    }

    private void ResetCurrentTarget()
    {
        _cachedTarget = null;
        _cachedTargetStep = -1;
        _cachedTargetScene = null;
        _cachedTargetName = null;
    }

    private void OnGUI()
    {
        if (!_visible
            || Event.current.type != EventType.Repaint
            || HeroController.instance == null
            || GameManager.instance == null)
        {
            return;
        }

        float scale = Mathf.Clamp(Screen.height / 720f, 1f, 1.7f);
        if (_titleStyle == null || !Mathf.Approximately(_styleScale, scale))
        {
            CreateStyles(scale);
        }

        GuideStep step = GuideRoute.Steps[_stepIndex];
        bool routeComplete = _routeFinished || (_stepIndex == GuideRoute.Steps.Length - 1 && step.IsComplete());
        float width = Mathf.Min(360f * scale, Screen.width * 0.42f);
        float contentX = Mathf.Max(12f, Screen.width - width - 48f * scale);
        float contentWidth = Mathf.Max(80f, width - 12f * scale);
        float top = 48f * scale;
        float gap = 12f * scale;
        string target = routeComplete
            ? "本期路线已完成"
            : step.Target + (step.Optional ? "（可选）" : "");
        string hint = routeComplete ? "本期路线已完成。" : step.Hint;
        float headerHeight = _titleStyle.CalcHeight(new GUIContent("现阶段建议："), contentWidth);
        float targetHeight = _bodyStyle.CalcHeight(new GUIContent(target), contentWidth);
        float hintHeight = _smallStyle.CalcHeight(new GUIContent(hint), contentWidth);
        const string shortcuts = "F6 上一步　F7 下一步/跳过可选\nF8 隐藏/显示　F9 传送到目标";
        float shortcutsHeight = _shortcutStyle.CalcHeight(new GUIContent(shortcuts), contentWidth);

        DrawSectionHeader(new Rect(contentX, top, contentWidth, headerHeight), "当前任务：", scale);
        float targetY = top + headerHeight + 5f * scale;
        GUI.Label(new Rect(contentX, targetY, contentWidth, targetHeight), target, _bodyStyle);
        float hintHeaderY = targetY + targetHeight + gap;
        DrawSectionHeader(new Rect(contentX, hintHeaderY, contentWidth, headerHeight), "现阶段建议：", scale);
        float hintY = hintHeaderY + headerHeight + 5f * scale;
        GUI.Label(new Rect(contentX, hintY, contentWidth, hintHeight), hint, _smallStyle);
        float shortcutsY = hintY + hintHeight + 8f * scale;
        GUI.Label(new Rect(contentX, shortcutsY, contentWidth, shortcutsHeight), shortcuts, _shortcutStyle);

        Vector2 direction = GetCurrentDirection(step, out bool targetTracked);
        if (step.TargetObjectKeywords != null
            && step.TargetObjectKeywords.Length > 0
            && !targetTracked)
        {
            return;
        }

        DrawArrowBesideHero(direction, scale);
    }

    private void DrawSectionHeader(Rect rect, string label, float scale)
    {
        Color previousColor = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, 0.72f);
        GUI.DrawTexture(
            new Rect(rect.x, rect.y + rect.height * 0.58f, 40f * scale, Mathf.Max(1f, scale)),
            Texture2D.whiteTexture);
        GUI.color = previousColor;
        GUI.Label(
            new Rect(rect.x + 48f * scale, rect.y, rect.width - 48f * scale, rect.height),
            label,
            _titleStyle);
    }

    private Vector2 GetCurrentDirection(GuideStep step, out bool targetTracked)
    {
        if (step.CompletionKind == GuideCompletionKind.Interaction
            && step.InteractionKind == GuideInteractionKind.LoreMarker
            && string.Equals(GameManager.instance.sceneName, "Tutorial_01", System.StringComparison.Ordinal))
        {
            Vector2 loreDelta = GetTutorialLoreMarkerPosition() - (Vector2)HeroController.instance.transform.position;
            targetTracked = true;
            return loreDelta.sqrMagnitude > 0.01f ? loreDelta.normalized : step.Direction;
        }

        if (step.CompletionKind == GuideCompletionKind.Location
            && string.Equals(GameManager.instance.sceneName, step.SceneName, System.StringComparison.Ordinal))
        {
            Vector2 delta = step.Position - (Vector2)HeroController.instance.transform.position;
            if (delta.sqrMagnitude > 0.01f)
            {
                targetTracked = true;
                return delta.normalized;
            }
        }

        if (TryGetTrackedTarget(step, out Vector2 targetPosition))
        {
            Vector2 delta = targetPosition - (Vector2)HeroController.instance.transform.position;
            if (delta.sqrMagnitude > 0.01f)
            {
                targetTracked = true;
                return delta.normalized;
            }
        }

        targetTracked = false;
        return step.Direction;
    }

    private bool TryGetTrackedTarget(GuideStep step, out Vector2 position)
    {
        position = default;
        if (step.CompletionKind == GuideCompletionKind.Interaction
            && step.InteractionKind == GuideInteractionKind.LoreMarker)
        {
            if (string.Equals(GameManager.instance.sceneName, "Tutorial_01", System.StringComparison.Ordinal))
            {
                position = GetTutorialLoreMarkerPosition();
                return true;
            }

            return false;
        }

        if (step.TargetObjectKeywords == null || step.TargetObjectKeywords.Length == 0)
        {
            return false;
        }

        string sceneName = GameManager.instance.sceneName;
        bool contextChanged = _cachedTargetStep != _stepIndex
            || !string.Equals(_cachedTargetScene, sceneName, System.StringComparison.Ordinal);
        bool targetUnavailable = !IsTrackedTargetAvailable(step);
        if (contextChanged)
        {
            UpdateTrackedTarget(step, sceneName);
        }
        else if (targetUnavailable && Time.unscaledTime >= _nextTargetScanTime)
        {
            UpdateTrackedTarget(step, sceneName);
        }

        if (!IsTrackedTargetAvailable(step))
        {
            return false;
        }

        position = _cachedTarget.position;
        return true;
    }

    private static Vector2 GetTutorialLoreMarkerPosition()
    {
        return _observedTutorialLoreMarkerPosition ?? GuideRoute.TutorialLoreMarkerPosition;
    }

    private static bool IsLoreInteractionSignal(string eventName)
    {
        return !string.IsNullOrEmpty(eventName)
            && (eventName.IndexOf("inspect", System.StringComparison.OrdinalIgnoreCase) >= 0
                || eventName.IndexOf("interact", System.StringComparison.OrdinalIgnoreCase) >= 0
                || eventName.IndexOf("read", System.StringComparison.OrdinalIgnoreCase) >= 0
                || eventName.IndexOf("activate", System.StringComparison.OrdinalIgnoreCase) >= 0
                || eventName.IndexOf("convo", System.StringComparison.OrdinalIgnoreCase) >= 0
                || eventName.IndexOf("talk", System.StringComparison.OrdinalIgnoreCase) >= 0
                || eventName.IndexOf("lore", System.StringComparison.OrdinalIgnoreCase) >= 0);
    }

    private static void LogLoreDiagnostic(string detail)
    {
        if (LoggedLoreDiagnostics.Count >= 80 || !LoggedLoreDiagnostics.Add(detail))
        {
            return;
        }

        Modding.Logger.Log($"[攻略指南针] 白王印记附近交互诊断：{detail}");
    }

    private static string GetFsmDescription(PlayMakerFSM fsm)
    {
        return fsm == null ? "<none>" : $"{fsm.gameObject.name}/{fsm.FsmName}";
    }

    private static string FormatPosition(Vector3 position)
    {
        return $"({position.x:F2}, {position.y:F2})";
    }

    private bool IsTrackedTargetAvailable(GuideStep step)
    {
        if (_cachedTarget == null)
        {
            return false;
        }

        if (step.CompletionKind == GuideCompletionKind.Interaction
            && step.InteractionKind == GuideInteractionKind.LoreMarker)
        {
            return _cachedTarget.gameObject.scene.IsValid()
                && string.Equals(_cachedTarget.gameObject.scene.name, "Tutorial_01", System.StringComparison.Ordinal);
        }

        return _cachedTarget.gameObject.activeInHierarchy;
    }

    private void UpdateTrackedTarget(GuideStep step, string sceneName)
    {
        _cachedTarget = null;
        _cachedTargetName = null;
        _cachedTargetStep = _stepIndex;
        _cachedTargetScene = sceneName;
        _nextTargetScanTime = Time.unscaledTime + 2f;
        if (string.IsNullOrEmpty(sceneName) || HeroController.instance == null)
        {
            return;
        }

        int bestTargetScore = int.MinValue;
        bool ambiguousTarget = false;
        Transform bestTarget = null;
        string bestTargetName = null;
        HashSet<int> matchedTargets = new();
        Transform[] candidates;
        if (step.CompletionKind == GuideCompletionKind.Interaction
            && step.InteractionKind == GuideInteractionKind.GeoPickup)
        {
            GeoRock[] geoRocks = UnityEngine.Object.FindObjectsOfType<GeoRock>();
            candidates = new Transform[geoRocks.Length];
            for (int i = 0; i < geoRocks.Length; i++)
            {
                candidates[i] = geoRocks[i].transform;
            }
        }
        else
        {
            PlayMakerFSM[] fsms = UnityEngine.Object.FindObjectsOfType<PlayMakerFSM>();
            candidates = new Transform[fsms.Length];
            for (int i = 0; i < fsms.Length; i++)
            {
                candidates[i] = fsms[i].transform;
            }
        }

        foreach (Transform candidateObject in candidates)
        {
            if (candidateObject == null
                || !string.Equals(candidateObject.gameObject.scene.name, sceneName, System.StringComparison.Ordinal)
                || !TryGetTargetMatch(candidateObject, step.TargetObjectKeywords, out Transform candidate, out int matchScore)
                || IsDestroyedGeoRock(candidate))
            {
                continue;
            }

            if (!matchedTargets.Add(candidate.GetInstanceID()))
            {
                continue;
            }

            if (matchScore > bestTargetScore)
            {
                bestTarget = candidate;
                bestTargetName = candidate.name;
                bestTargetScore = matchScore;
                ambiguousTarget = false;
            }
            else if (matchScore == bestTargetScore)
            {
                ambiguousTarget = true;
            }
        }

        if (!ambiguousTarget)
        {
            _cachedTarget = bestTarget;
            _cachedTargetName = bestTargetName;
        }
    }

    private static bool TryGetTargetMatch(
        Transform candidate,
        string[] keywords,
        out Transform matchedTarget,
        out int bestScore)
    {
        matchedTarget = null;
        bestScore = 0;
        for (Transform current = candidate; current != null; current = current.parent)
        {
            int score = GetTargetMatchScore(current.name, keywords);
            PlayMakerFSM fsm = current.GetComponent<PlayMakerFSM>();
            if (fsm != null)
            {
                score = Mathf.Max(score, GetTargetMatchScore(fsm.FsmName, keywords));
            }

            if (score > bestScore)
            {
                bestScore = score;
                matchedTarget = current;
            }
        }

        return matchedTarget != null;
    }

    private static bool IsDestroyedGeoRock(Transform candidate)
    {
        for (Transform current = candidate; current != null; current = current.parent)
        {
            PlayMakerFSM fsm = current.GetComponent<PlayMakerFSM>();
            if (fsm != null
                && string.Equals(fsm.FsmName, "Geo Rock", System.StringComparison.Ordinal)
                && string.Equals(fsm.Fsm.ActiveStateName, "Destroy", System.StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static int GetTargetMatchScore(string objectName, string[] keywords)
    {
        if (string.IsNullOrEmpty(objectName))
        {
            return 0;
        }

        int score = 0;
        foreach (string keyword in keywords)
        {
            if (!string.IsNullOrEmpty(keyword)
                && string.Equals(objectName, keyword, System.StringComparison.OrdinalIgnoreCase))
            {
                score = 1;
                break;
            }
        }

        return score;
    }

    private void DrawArrow(Rect rect, Vector2 direction)
    {
        Vector2 center = rect.center;
        float angle = Mathf.Atan2(-direction.y, direction.x) * Mathf.Rad2Deg;
        Matrix4x4 previousMatrix = GUI.matrix;
        Color previousColor = GUI.color;
        GUIUtility.RotateAroundPivot(angle, center);
        GUI.color = Color.white;
        GUI.DrawTexture(rect, _arrowTexture);
        GUI.matrix = previousMatrix;
        GUI.color = previousColor;
    }

    private void DrawArrowBesideHero(Vector2 direction, float scale)
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            return;
        }

        Vector3 heroScreenPosition = camera.WorldToScreenPoint(HeroController.instance.transform.position);
        if (heroScreenPosition.z <= 0f)
        {
            return;
        }

        float size = 19f * scale;
        Rect rect = new(
            heroScreenPosition.x - size / 2f,
            Screen.height - heroScreenPosition.y - size - 42f * scale,
            size,
            size);
        DrawArrow(rect, direction);
    }

    private void CreateStyles(float scale)
    {
        _styleScale = scale;
        _titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(18f * scale),
            wordWrap = true,
            normal = { textColor = new Color(0.94f, 0.95f, 0.98f, 0.9f) }
        };
        _bodyStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(21f * scale),
            fontStyle = FontStyle.Bold,
            wordWrap = true,
            normal = { textColor = new Color(0.2f, 0.88f, 0.42f, 0.95f) }
        };
        _smallStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(17f * scale),
            wordWrap = true,
            normal = { textColor = new Color(0.92f, 0.93f, 0.96f, 0.9f) }
        };
        _shortcutStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.UpperRight,
            fontSize = Mathf.RoundToInt(14f * scale),
            wordWrap = true,
            normal = { textColor = new Color(0.82f, 0.85f, 0.89f, 0.78f) }
        };
    }

    private static Texture2D CreateArrowTexture()
    {
        const int size = 32;
        Texture2D texture = new(size, size, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Bilinear,
            hideFlags = HideFlags.HideAndDontSave
        };

        Color shadowColor = new(0.025f, 0.03f, 0.04f, 0.9f);
        Color arrowColor = new(0.24f, 1f, 0.42f, 1f);
        const int samplesPerAxis = 4;
        const float arrowStrokeRadius = 1.7f;
        const float shadowStrokeRadius = 2.8f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int arrowSamples = 0;
                int borderSamples = 0;
                for (int sampleY = 0; sampleY < samplesPerAxis; sampleY++)
                {
                    for (int sampleX = 0; sampleX < samplesPerAxis; sampleX++)
                    {
                        float px = x + (sampleX + 0.5f) / samplesPerAxis;
                        float py = y + (sampleY + 0.5f) / samplesPerAxis;
                        Vector2 sample = new(px, py);
                        float nearestDistanceSquared = Mathf.Min(
                            Mathf.Min(
                                DistanceToSegmentSquared(sample, ArrowUpperTail, ArrowTip),
                                DistanceToSegmentSquared(sample, ArrowTip, ArrowLowerTail)),
                            Mathf.Min(
                                DistanceToSegmentSquared(sample, ArrowUpperTail, ArrowUpperBaseEnd),
                                DistanceToSegmentSquared(sample, ArrowLowerBaseStart, ArrowLowerTail)));

                        float squaredArrowRadius = arrowStrokeRadius * arrowStrokeRadius;
                        float squaredShadowRadius = shadowStrokeRadius * shadowStrokeRadius;
                        if (nearestDistanceSquared <= squaredArrowRadius)
                        {
                            arrowSamples++;
                        }
                        else if (nearestDistanceSquared <= squaredShadowRadius)
                        {
                            borderSamples++;
                        }
                    }
                }

                int coveredSamples = arrowSamples + borderSamples;
                Color pixel = Color.Lerp(
                    shadowColor,
                    arrowColor,
                    coveredSamples == 0 ? 0f : arrowSamples / (float)coveredSamples);
                pixel.a = coveredSamples / (float)(samplesPerAxis * samplesPerAxis);
                texture.SetPixel(x, y, pixel);
            }
        }

        texture.Apply();
        return texture;
    }

    private static float DistanceToSegmentSquared(Vector2 point, Vector2 start, Vector2 end)
    {
        Vector2 segment = end - start;
        float segmentLengthSquared = segment.sqrMagnitude;
        if (segmentLengthSquared <= Mathf.Epsilon)
        {
            return (point - start).sqrMagnitude;
        }

        float projection = Mathf.Clamp01(Vector2.Dot(point - start, segment) / segmentLengthSquared);
        return (point - (start + projection * segment)).sqrMagnitude;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        if (_arrowTexture != null)
        {
            Destroy(_arrowTexture);
            _arrowTexture = null;
        }

    }
}
