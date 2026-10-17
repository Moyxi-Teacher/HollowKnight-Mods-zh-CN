using UnityEngine;
using System.Collections.Generic;

namespace GuideCompass;

internal sealed class GuideCompassOverlay : MonoBehaviour
{
    private const KeyCode PreviousKey = KeyCode.F6;
    private const KeyCode NextKey = KeyCode.F7;
    private const KeyCode ToggleKey = KeyCode.F8;

    private static readonly Vector2 ArrowUpperTail = new(4f, 4f);
    private static readonly Vector2 ArrowTip = new(28f, 16f);
    private static readonly Vector2 ArrowLowerTail = new(4f, 28f);
    private static readonly Vector2 ArrowUpperBaseEnd = new(4f, 11f);
    private static readonly Vector2 ArrowLowerBaseStart = new(4f, 21f);
    private static int _loadedStepIndex;
    private static bool _loadedRouteFinished;
    private static int _loadedKillCount;
    private static bool _hasLoadedProgress;

    private Texture2D _arrowTexture;
    private GUIStyle _titleStyle;
    private GUIStyle _bodyStyle;
    private GUIStyle _smallStyle;
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

        TryAdvanceIfComplete();
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

    internal static void LoadProgress(int stepIndex, bool routeFinished, int killCount)
    {
        _loadedStepIndex = stepIndex;
        _loadedRouteFinished = routeFinished;
        _loadedKillCount = killCount;

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
            ? new GuideCompassLocalSettings { RouteVersion = GuideRoute.ProgressVersion }
            : new GuideCompassLocalSettings
            {
                StepIndex = instance._stepIndex,
                RouteFinished = instance._routeFinished,
                KillCount = instance._killCount,
                RouteVersion = GuideRoute.ProgressVersion,
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

    internal static void NotifyFsmEventSent(PlayMakerFSM fsm, string eventName)
    {
        GuideCompassOverlay instance = Instance;
        if (instance == null || instance._routeFinished)
        {
            return;
        }

        if (GuideRoute.Steps[instance._stepIndex].MatchesInteractionEvent(fsm, eventName)
            && instance.IsCurrentTargetObject(fsm.gameObject))
        {
            instance.AdvanceStep();
        }
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

        DrawSectionHeader(new Rect(contentX, top, contentWidth, headerHeight), "当前任务：", scale);
        float targetY = top + headerHeight + 5f * scale;
        GUI.Label(new Rect(contentX, targetY, contentWidth, targetHeight), target, _bodyStyle);
        float hintHeaderY = targetY + targetHeight + gap;
        DrawSectionHeader(new Rect(contentX, hintHeaderY, contentWidth, headerHeight), "现阶段建议：", scale);
        float hintY = hintHeaderY + headerHeight + 5f * scale;
        GUI.Label(new Rect(contentX, hintY, contentWidth, hintHeight), hint, _smallStyle);

        Vector2 direction = GetCurrentDirection(step, out _);
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
        if (step.TargetObjectKeywords == null || step.TargetObjectKeywords.Length == 0)
        {
            return false;
        }

        string sceneName = GameManager.instance.sceneName;
        bool contextChanged = _cachedTargetStep != _stepIndex
            || !string.Equals(_cachedTargetScene, sceneName, System.StringComparison.Ordinal);
        bool targetUnavailable = _cachedTarget == null || !_cachedTarget.gameObject.activeInHierarchy;
        if (contextChanged)
        {
            UpdateTrackedTarget(step, sceneName);
        }
        else if (targetUnavailable && Time.unscaledTime >= _nextTargetScanTime)
        {
            UpdateTrackedTarget(step, sceneName);
        }

        if (_cachedTarget == null || !_cachedTarget.gameObject.activeInHierarchy)
        {
            return false;
        }

        position = _cachedTarget.position;
        return true;
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
        float bestDistance = float.MaxValue;
        Vector2 heroPosition = HeroController.instance.transform.position;
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

            float distance = ((Vector2)candidate.position - heroPosition).sqrMagnitude;
            Vector2 offset = (Vector2)candidate.position - heroPosition;
            float alignment = step.Direction.sqrMagnitude > 0.01f && offset.sqrMagnitude > 0.01f
                ? Vector2.Dot(step.Direction, offset.normalized)
                : 0f;
            if (step.Direction.sqrMagnitude > 0.01f && alignment < -0.25f)
            {
                continue;
            }

            int targetScore = matchScore + Mathf.RoundToInt(alignment * 100);
            if (targetScore > bestTargetScore || (targetScore == bestTargetScore && distance < bestDistance))
            {
                _cachedTarget = candidate;
                _cachedTargetName = candidate.name;
                bestTargetScore = targetScore;
                bestDistance = distance;
            }
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
                score = Mathf.Max(score, keyword.Length + 1000);
            }
            else if (!string.IsNullOrEmpty(keyword)
                && objectName.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                score = Mathf.Max(score, keyword.Length);
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
