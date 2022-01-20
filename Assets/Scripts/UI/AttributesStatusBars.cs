using System.Collections;
using Character;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AttributesStatusBars : MonoBehaviour
    {
        [SerializeField]
        private Attributes attributes;
        [SerializeField]
        private Image healthBar;
        [SerializeField]
        private Image staminaBar;
        [SerializeField]
        private float updateTime;

        private Canvas _canvas;

        private void Awake()
        {
            attributes ??= GetComponentInParent<Attributes>();
            if (attributes == null)
                return;
            attributes.OnHealthChanged += HandleHealthChange;
            attributes.OnStaminaChanged += HandleStaminaChange;

            _canvas ??= GetComponent<Canvas>();
        }

        private void Update()
        {
            if (healthBar.fillAmount <= 0.0f)
                StartCoroutine(CDisableCanvas());
        }

        private IEnumerator CDisableCanvas()
        {
            yield return new WaitForSeconds(0.3f);
            _canvas.enabled = false;
        }

        private void HandleHealthChange(float current, float max)
        {
            if (healthBar == null)
                return;
            StartCoroutine(CUpdateHealth(current / max));
        }

        private IEnumerator CUpdateHealth(float percent)
        {
            var startAmount = healthBar.fillAmount;

            var elapsed = 0.0f;
            while (elapsed < updateTime)
            {
                if (healthBar == null)
                    break;
                elapsed += Time.deltaTime;
                healthBar.fillAmount = Mathf.Lerp(startAmount, percent, elapsed / updateTime);
                yield return null;
            }

            if (healthBar != null)
                healthBar.fillAmount = percent;
        }

        private void HandleStaminaChange(float current, float max)
        {
            if (staminaBar == null)
                return;
            StartCoroutine(CUpdateStamina(current / max));
        }

        private IEnumerator CUpdateStamina(float percent)
        {
            var startAmount = staminaBar.fillAmount;

            var elapsed = 0.0f;
            while (elapsed < updateTime)
            {
                if (staminaBar == null)
                    break;
                elapsed += Time.deltaTime;
                staminaBar.fillAmount = Mathf.Lerp(startAmount, percent, elapsed / updateTime);
                yield return null;
            }

            if (staminaBar != null)
                staminaBar.fillAmount = percent;
        }
    }
}