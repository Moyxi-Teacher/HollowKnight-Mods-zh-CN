using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using BepInEx.Configuration;

namespace HollowKnightHaptics;

public class HapticsMenu : MonoBehaviour
{
    private Canvas _canvas;
    private GameObject _panel;
    private RectTransform _content;
    private bool _visible;
    private readonly KeyCode _toggleKey = KeyCode.F4;

    void Awake() { CreateUI(); }

    void Update()
    {
        if (Input.GetKeyDown(_toggleKey))
        {
            if (UnityEngine.EventSystems.EventSystem.current == null)
            {
                var es = new GameObject("HapticsEventSystem");
                DontDestroyOnLoad(es);
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
            _visible = !_visible;
            _panel.SetActive(_visible);
        }
    }

    private void CreateUI()
    {
        var go = new GameObject("HapticsMenuCanvas");
        DontDestroyOnLoad(go);
        _canvas = go.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 9999;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920,1080);
        scaler.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();

        _panel = new GameObject("Panel");
        _panel.transform.SetParent(go.transform,false);
        var rt = _panel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f,0.5f);
        rt.anchorMax = new Vector2(0.5f,0.5f);
        rt.pivot = new Vector2(0.5f,0.5f);
        rt.sizeDelta = new Vector2(460f, 880f);

        var bg = _panel.AddComponent<Image>();
        bg.color = new Color(0.07f,0.07f,0.09f,0.96f);

        var scroll = _panel.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 50f;

        var viewport = new GameObject("Viewport");
        viewport.transform.SetParent(_panel.transform,false);
        var vrt = viewport.AddComponent<RectTransform>();
        vrt.anchorMin = Vector2.zero;
        vrt.anchorMax = Vector2.one;
        vrt.pivot = new Vector2(0.5f,0.5f);
        vrt.offsetMin = new Vector2(10f,10f);
        vrt.offsetMax = new Vector2(-10f,-10f);
        viewport.AddComponent<RectMask2D>();
        scroll.viewport = vrt;

        var contentGo = new GameObject("Content");
        contentGo.transform.SetParent(viewport.transform,false);
        _content = contentGo.AddComponent<RectTransform>();
        _content.anchorMin = new Vector2(0f,1f);
        _content.anchorMax = new Vector2(1f,1f);
        _content.pivot = new Vector2(0.5f,1f);
        _content.sizeDelta = Vector2.zero;

        var layout = contentGo.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(14,14,14,14);
        layout.spacing = 8;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childForceExpandWidth = true;
        layout.childControlHeight = false;

        var fitter = contentGo.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        scroll.content = _content;

        AddTitle("PS5 触觉 参数");
        AddSlider("主增益", HapticsPlugin.MasterGain, 0f, 2f);

        foreach (var kv in HapticsPlugin.GetSortedClipGains())
        {
            string label = HapticsPlugin.GetClipDisplayName(kv.Key);
            AddSlider(label, kv.Value, 0f, 3f);
        }

        _panel.SetActive(false);
    }

    private void AddTitle(string title)
    {
        var go = new GameObject("Title");
        go.transform.SetParent(_content,false);
        var t = go.AddComponent<Text>();
        t.text = title;
        t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        t.fontSize = 20;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        var trt = go.GetComponent<RectTransform>();
        trt.sizeDelta = new Vector2(0f,34f);
    }

    private void AddSlider(string label, ConfigEntry<float> cfg, float min, float max)
    {
        var row = new GameObject("Row_" + label);
        row.transform.SetParent(_content,false);
        var rowRT = row.AddComponent<RectTransform>();
        rowRT.sizeDelta = new Vector2(0f,52f);

        var vgroup = row.AddComponent<VerticalLayoutGroup>();
        vgroup.spacing = 3;
        vgroup.childControlWidth = true;
        vgroup.childForceExpandWidth = true;

        var lbl = new GameObject("Label");
        lbl.transform.SetParent(row.transform,false);
        var lblT = lbl.AddComponent<Text>();
        lblT.text = label + ": " + cfg.Value.ToString("F2");
        lblT.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        lblT.fontSize = 14;
        lblT.color = Color.white;
        var lblRT = lbl.GetComponent<RectTransform>();
        lblRT.sizeDelta = new Vector2(0f,18f);

        var sliderObj = new GameObject("Slider");
        sliderObj.transform.SetParent(row.transform,false);
        var slider = sliderObj.AddComponent<Slider>();
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = cfg.Value;
        slider.wholeNumbers = false;

        var sliderImg = sliderObj.AddComponent<Image>();
        sliderImg.color = new Color(0.2f,0.2f,0.2f);

        var fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(slider.transform,false);
        var fillImg = fillObj.AddComponent<Image>();
        fillImg.color = new Color(0.4f,0.7f,1f);
        var fillRT = fillObj.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.offsetMin = Vector2.zero;
        fillRT.offsetMax = Vector2.zero;
        slider.fillRect = fillRT;

        var handleObj = new GameObject("Handle");
        handleObj.transform.SetParent(slider.transform,false);
        var handleImg = handleObj.AddComponent<Image>();
        handleImg.color = Color.white;
        var handleRT = handleObj.GetComponent<RectTransform>();
        handleRT.sizeDelta = new Vector2(20f,20f);
        slider.handleRect = handleRT;

        var valueObj = new GameObject("Value");
        valueObj.transform.SetParent(row.transform,false);
        var valT = valueObj.AddComponent<Text>();
        valT.text = cfg.Value.ToString("F2");
        valT.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        valT.fontSize = 12;
        valT.color = Color.gray;
        valT.alignment = TextAnchor.MiddleLeft;
        var valRT = valT.GetComponent<RectTransform>();
        valRT.sizeDelta = new Vector2(0f,16f);

        slider.onValueChanged.AddListener(v =>
        {
            cfg.Value = v;
            lblT.text = label + ": " + v.ToString("F2");
            valT.text = v.ToString("F2");
        });
    }
}
