using Character.Combat;
using ScriptableObjects.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class OutOfStaminaIndicator : MonoBehaviour
    {
        [SerializeField]
        private HumanoidCombat combat;
        [SerializeField]
        private Image indicator;
        [SerializeField]
        private float displayTime;

        private float _currentDisplayTime;

        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private AudioEvent audioEvent;

        private void Awake()
        {
            combat ??= GetComponentInParent<HumanoidCombat>();
            if (combat == null)
                return;
            combat.OnOutOfStamina += HandleOutOfStamina;
        }

        private void Update()
        {
            UpdateIndicator();
        }

        private void UpdateIndicator()
        {
            if (_currentDisplayTime <= 0.0f)
            {
                SetIndicatorAlpha(0.0f);
                return;
            }

            _currentDisplayTime -= Time.deltaTime;
            var percent = _currentDisplayTime / displayTime;
            SetIndicatorAlpha(percent);
        }

        private void SetIndicatorAlpha(float value)
        {
            indicator.color = new Color(indicator.color.r, indicator.color.g, indicator.color.b, value);
        }

        private void HandleOutOfStamina()
        {
            if (indicator == null)
                return;
            _currentDisplayTime = displayTime;

            if (audioSource != null && audioEvent != null)
                audioEvent.Play(audioSource);
        }
    }
}