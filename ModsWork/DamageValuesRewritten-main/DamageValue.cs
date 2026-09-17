using System.Collections;
using UnityEngine;

namespace DamageValuesRewritten
{
    internal class DamageValue : MonoBehaviour
    {
        private SpriteRenderer[] _renderers;
        private Rigidbody2D _rb;
        public Color _color = new Color(1, 1, 1);
        private void Awake()
        {
            _rb = gameObject.AddComponent<Rigidbody2D>();
            _rb.isKinematic = true;
            _renderers = GetComponentsInChildren<SpriteRenderer>();
            StartCoroutine(FadeOut());
        }
        private IEnumerator FadeOut()
        {
            float opacity = 1.0f;
            float fadeRate = 2;
            _rb.velocity = Vector2.up * 2.5f;
            yield return new WaitUntil(() =>
            {
                foreach (SpriteRenderer rend in _renderers)
                    rend.color = new Color(_color.r ,_color.g, _color.b, opacity);
                opacity -= Time.deltaTime * fadeRate;
                return opacity <= 0;
            });
            Destroy(gameObject);
        }
    }
    
    internal class TickValue : MonoBehaviour
    {
        private SpriteRenderer[] _renderers;
        private Rigidbody2D _rb;
        private void Awake()
        {
            _rb = gameObject.AddComponent<Rigidbody2D>();
            _rb.isKinematic = true;
            _renderers = GetComponentsInChildren<SpriteRenderer>();
            StartCoroutine(FadeOut());
        }
        private IEnumerator FadeOut()
        {
            float opacity = 1.0f;
            float fadeRate = 2;
            _rb.velocity = Vector2.up * 1f;
            yield return new WaitUntil(() =>
            {
                foreach (SpriteRenderer rend in _renderers)
                    rend.color = new Color(0.95f, 0.9f, 0.15f, opacity);
                opacity -= Time.deltaTime * fadeRate;
                return opacity <= 0;
            });
            Destroy(gameObject);
        }
    }
}
