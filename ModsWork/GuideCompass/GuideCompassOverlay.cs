using UnityEngine;
using System.Collections.Generic;

namespace GuideCompass;

internal sealed class GuideCompassOverlay : MonoBehaviour
{
    private const KeyCode PreviousKey = KeyCode.F6;
    private const KeyCode NextKey = KeyCode.F7;
    private const KeyCode ToggleKey = KeyCode.F8;

    private static readonly Color AccentColor = new(1f, 0.78f, 0.24f, 1f);
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

        if (GuideRoute.Steps[instance._stepIndex].MatchesInteractionEvent(fsm, eventName))
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
        float width = Mathf.Min(580f * scale, Screen.width - 24f);
        float padding = 14f * scale;
        float contentWidth = width - padding * 2f;
        float textWidth = Mathf.Max(80f, contentWidth);
        string title = $"{GuideRoute.Title} · {_stepIndex + 1}/{GuideRoute.Steps.Length}";
        string tag = step.Optional ? "（可选）" : "";
        string target = routeComplete ? "本期路线已完成" : $"下一目标：{step.Target}{tag}";
        Vector2 direction = GetCurrentDirection(step, out bool targetTracked);
        string status = routeComplete
            ? "已取得城市纹章。"
            : GetCompassStatus(step, targetTracked);
        string description = routeComplete
            ? status
            : $"{status}\n{GetCompletionDescription(step)}";
        string footer = "F6 上一步   F7 下一步   F8 隐藏/显示";
        float gap = 6f * scale;
        float titleHeight = _titleStyle.CalcHeight(new GUIContent(title), textWidth);
        float targetHeight = _bodyStyle.CalcHeight(new GUIContent(target), textWidth);
        float descriptionHeight = _smallStyle.CalcHeight(new GUIContent(description), textWidth);
        float footerHeight = _smallStyle.CalcHeight(new GUIContent(footer), textWidth);
        float height = padding * 2f + titleHeight + targetHeight + descriptionHeight + footerHeight + gap * 3f;
        Rect panel = new(Screen.width - width - 12f, 14f, width, height);

        float y = panel.y + padding;
        GUI.Label(new Rect(panel.x + padding, y, textWidth, titleHeight), title, _titleStyle);
        y += titleHeight + gap;
        GUI.Label(new Rect(panel.x + padding, y, textWidth, targetHeight), target, _bodyStyle);
        y += targetHeight + gap;
        GUI.Label(new Rect(panel.x + padding, y, textWidth, descriptionHeight), description, _smallStyle);
        y += descriptionHeight + gap;
        GUI.Label(new Rect(panel.x + padding, y, textWidth, footerHeight), footer, _smallStyle);

        DrawArrowBesideHero(direction, scale);
    }

    private string GetCompletionDescription(GuideStep step)
    {
        string trigger = step.CompletionKind switch
        {
            GuideCompletionKind.Manual => "完成此操作后按 F7 继续；也可随时按 F6 返回上一步。",
            GuideCompletionKind.Interaction => step.InteractionKind switch
            {
                GuideInteractionKind.GeoPickup => "钱袋完全打碎后自动推进；拾取普通吉欧不会推进。",
                GuideInteractionKind.BreakableWall => "隐藏墙被打破后自动推进。",
                GuideInteractionKind.BlueBlood => "打破蓝血容器后自动推进。",
                GuideInteractionKind.LoreMarker => "与印记互动并触发提示后自动推进。",
                _ => "完成指定交互后自动推进。",
            },
            GuideCompletionKind.Location => $"到达 {step.SceneName} 的指定坐标（半径 {step.ArrivalRadius:0.0}）后自动推进。",
            GuideCompletionKind.PlayerDataBool => $"游戏标志 {step.PlayerDataKey} 变为 true 后自动推进。",
            GuideCompletionKind.PlayerDataBoolAny => $"任一游戏标志变为 true 后自动推进：{string.Join(" / ", step.PlayerDataKeys)}。",
            GuideCompletionKind.PlayerDataBoolAll => $"以下游戏标志全部变为 true 后自动推进：{string.Join("、", step.PlayerDataKeys)}。",
            GuideCompletionKind.PlayerDataIntAtLeast => $"游戏数据 {step.PlayerDataKey} 达到 {step.RequiredIntValue} 后自动推进。",
            GuideCompletionKind.FsmStateEntered => $"场景/对象的 FSM「{step.FsmName}」进入「{step.FsmStateName}」后自动推进。",
            GuideCompletionKind.NearbyTarget => $"进入目标「{string.Join(" / ", step.TargetObjectKeywords)}」附近后自动推进。",
            GuideCompletionKind.EnemyKills => $"击败 {step.RequiredKillCount} 个指定敌人后自动推进（当前 { _killCount}/{step.RequiredKillCount}）。",
            GuideCompletionKind.BenchUse => "使用长椅并触发存档点后自动推进。",
            _ => "完成条件未配置。",
        };

        return $"{trigger}\n{step.Hint}";
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

    private string GetCompassStatus(GuideStep step, bool targetTracked)
    {
        if (step.CompletionKind == GuideCompletionKind.Location)
        {
            return targetTracked
                ? "指南针实时指向本场景的攻略坐标。"
                : "目标位于其他场景，当前显示路线提示方向。";
        }

        return targetTracked
            ? $"指南针实时指向：{_cachedTargetName}。"
            : "当前场景未发现目标对象，显示路线提示方向。";
    }

    private bool TryGetTrackedTarget(GuideStep step, out Vector2 position)
    {
        position = default;
        if (step.TargetObjectKeywords == null || step.TargetObjectKeywords.Length == 0)
        {
            return false;
        }

        string sceneName = GameManager.instance.sceneName;
        if (_cachedTargetStep != _stepIndex
            || !string.Equals(_cachedTargetScene, sceneName, System.StringComparison.Ordinal)
            || Time.unscaledTime >= _nextTargetScanTime
            || _cachedTarget == null
            || !_cachedTarget.gameObject.activeInHierarchy)
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
        _nextTargetScanTime = Time.unscaledTime + 0.75f;

        if (string.IsNullOrEmpty(sceneName) || HeroController.instance == null)
        {
            return;
        }

        int bestMatchScore = 0;
        float bestDistance = float.MaxValue;
        Vector2 heroPosition = HeroController.instance.transform.position;
        foreach (GameObject candidateObject in UnityEngine.Object.FindObjectsOfType<GameObject>())
        {
            if (!string.Equals(candidateObject.scene.name, sceneName, System.StringComparison.Ordinal))
            {
                continue;
            }

            Transform candidate = candidateObject.transform;
            int matchScore = GetTargetMatchScore(candidate, step.TargetObjectKeywords);
            if (matchScore == 0)
            {
                continue;
            }

            float distance = ((Vector2)candidate.position - heroPosition).sqrMagnitude;
            Vector2 offset = (Vector2)candidate.position - heroPosition;
            float alignment = step.Direction.sqrMagnitude > 0.01f && offset.sqrMagnitude > 0.01f
                ? Vector2.Dot(step.Direction, offset.normalized)
                : 0f;
            if (alignment < -0.25f)
            {
                continue;
            }

            int directionalScore = matchScore + Mathf.RoundToInt(alignment * 100f);
            if (directionalScore > bestMatchScore || (directionalScore == bestMatchScore && distance < bestDistance))
            {
                _cachedTarget = candidate;
                _cachedTargetName = candidate.name;
                bestMatchScore = directionalScore;
                bestDistance = distance;
            }
        }
    }

    private static int GetTargetMatchScore(Transform candidate, string[] keywords)
    {
        int score = GetTargetMatchScore(candidate.name, keywords);
        PlayMakerFSM fsm = candidate.GetComponent<PlayMakerFSM>();
        return fsm == null ? score : Mathf.Max(score, GetTargetMatchScore(fsm.FsmName, keywords));
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
            Screen.height - heroScreenPosition.y - size - 16f * scale,
            size,
            size);
        DrawArrow(rect, direction);
    }

    private void CreateStyles(float scale)
    {
        _styleScale = scale;
        _titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(20f * scale),
            fontStyle = FontStyle.Bold,
            wordWrap = true,
            normal = { textColor = new Color(1f, 0.78f, 0.24f, 0.9f) }
        };
        _bodyStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(19f * scale),
            fontStyle = FontStyle.Bold,
            wordWrap = true,
            normal = { textColor = new Color(1f, 1f, 1f, 0.9f) }
        };
        _smallStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(16f * scale),
            wordWrap = true,
            normal = { textColor = new Color(0.86f, 0.9f, 0.94f, 0.82f) }
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
        Color arrowColor = new(1f, 0.78f, 0.12f, 1f);
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
