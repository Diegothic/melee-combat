using System.Collections;
using Character;
using UnityEngine;

namespace Camera.Transition
{
    public class CameraTransitionController : MonoBehaviour
    {
        [SerializeField]
        private float transitionTime;
        private bool _playerHadTarget;
        private Coroutine _transitionRoutine;
        private bool _isTransitioning;
        private Vector3 _desiredPosition;

        private Targeting _targeting;

        private void Awake()
        {
            _targeting ??= GameObject.FindGameObjectWithTag("Player").GetComponent<Targeting>();
        }

        public bool IsTransitioning()
        {
            return _isTransitioning;
        }

        public void UpdateDesiredPosition(Vector3 newPosition)
        {
            _desiredPosition = newPosition;
        }

        public void CheckForTransition()
        {
            if (!ShouldTransition())
                return;

            StartTransitioning();
        }

        private bool ShouldTransition()
        {
            var playerHasTarget = _targeting.HasTarget();
            var shouldTransition = _playerHadTarget != playerHasTarget;
            _playerHadTarget = playerHasTarget;
            return shouldTransition;
        }

        private void StartTransitioning()
        {
            _isTransitioning = true;
            if (_transitionRoutine != null)
            {
                StopCoroutine(_transitionRoutine);
            }

            _transitionRoutine = StartCoroutine(TransitionCamera());
        }

        private IEnumerator TransitionCamera()
        {
            var startingPosition = transform.position;
            var elapsedTime = 0.0f;

            while (elapsedTime < transitionTime)
            {
                var percentage = elapsedTime / transitionTime;
                transform.position = Vector3.Lerp(startingPosition, _desiredPosition, percentage);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            OnTransitionFinish();
        }

        private void OnTransitionFinish()
        {
            _isTransitioning = false;
        }
    }
}