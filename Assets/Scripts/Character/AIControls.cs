using System.Collections;
using Character.Combat;
using UnityEngine;

namespace Character
{
    public class AIControls : MonoBehaviour
    {
        public bool IsInRoutine { get; private set; }
        public bool IsInAction { get; private set; }

        private HumanoidCombat _combat;
        private HumanoidController _controller;

        private void Awake()
        {
            _combat ??= GetComponent<HumanoidCombat>();
            _controller ??= GetComponent<HumanoidController>();
        }

        private void Update()
        {
            IsInAction = _combat.IsInAction;
        }

        public void WalkTowardsTarget()
        {
            if (IsInAction || IsInRoutine) return;

            StartCoroutine(CWalkTowardsTarget(Random.Range(0.1f, 1.0f)));
        }

        public void Strafe(bool right)
        {
            if (IsInAction || IsInRoutine) return;

            StartCoroutine(CStrafe(right, Random.Range(0.1f, 1.0f)));
        }

        public void Retreat()
        {
            if (IsInAction || IsInRoutine) return;

            StartCoroutine(CRetreat(Random.Range(0.1f, 1.0f)));
        }

        public void LightAttack()
        {
            if (IsInAction && !_combat.IsInCombo) return;

            StopAllCoroutines();
            IsInRoutine = false;
            _controller.Move(Vector2.zero);
            _controller.LightAttack();
        }

        public void HeavyAttack(float strength)
        {
            if (IsInAction && !_combat.IsInCombo) return;

            StopAllCoroutines();
            IsInRoutine = false;
            _controller.Move(Vector2.zero);
            StartCoroutine(CHeavyAttack(strength));
        }

        private IEnumerator CHeavyAttack(float strength)
        {
            IsInRoutine = true;
            var elapsedTime = 0.0f;
            var time = 1.0f + strength;
            _controller.ChargeHeavyAttack();

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _controller.HeavyAttack();
            IsInRoutine = false;
        }

        public void DodgeForward()
        {
            if (IsInAction) return;

            StopAllCoroutines();
            Dodge(0.0f);
        }

        public void DodgeBackwards()
        {
            if (IsInAction) return;

            StopAllCoroutines();
            Dodge(-180.0f);
        }

        public void DodgeLeft()
        {
            if (IsInAction) return;

            StopAllCoroutines();
            Dodge(-90.0f);
        }

        public void DodgeRight()
        {
            if (IsInAction) return;

            StopAllCoroutines();
            Dodge(90.0f);
        }

        private void Dodge(float angle)
        {
            IsInRoutine = false;
            _controller.Move(Vector2.zero);
            _controller.Dodge(angle);
        }

        public void StartBlocking()
        {
            _controller.Move(Vector2.zero);
            _controller.Block();
            StartCoroutine(CBlock(Random.Range(1.0f, 3.0f)));
        }

        private IEnumerator CBlock(float time)
        {
            var elapsedTime = 0.0f;
            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            StopBlocking();
        }

        public void StopBlocking()
        {
            _controller.StopBlocking();
        }

        public void ForceRetreat()
        {
            if (IsInAction) return;

            IsInRoutine = false;
            StopAllCoroutines();
            StartCoroutine(CRetreat(1.0f));
        }

        private IEnumerator CWalkTowardsTarget(float time)
        {
            IsInRoutine = true;
            var elapsedTime = 0.0f;
            var direction = new Vector2(0.0f, 1.0f);

            while (elapsedTime < time)
            {
                _controller.Move(direction);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            IsInRoutine = false;
        }

        private IEnumerator CStrafe(bool right, float time)
        {
            IsInRoutine = true;
            var elapsedTime = 0.0f;
            var direction = new Vector2(right ? 1.0f : -1.0f, 0.0f);

            while (elapsedTime < time)
            {
                _controller.Move(direction);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            IsInRoutine = false;
        }

        private IEnumerator CRetreat(float time)
        {
            IsInRoutine = true;
            var elapsedTime = 0.0f;
            var direction = new Vector2(0.0f, -1.0f);

            while (elapsedTime < time)
            {
                _controller.Move(direction);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            IsInRoutine = false;
        }

        public void Wait()
        {
            if (IsInAction || IsInRoutine) return;

            StartCoroutine(CWait(Random.Range(0.1f, 1.0f)));
        }

        private IEnumerator CWait(float time)
        {
            IsInRoutine = true;
            var elapsedTime = 0.0f;
            var direction = new Vector2(0.0f, 0.0f);

            while (elapsedTime < time)
            {
                _controller.Move(direction);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            IsInRoutine = false;
        }

        public void WalkRandomly()
        {
            if (IsInAction || IsInRoutine) return;

            var time = Random.Range(0.5f, 2.0f);
            StartCoroutine(CWalkRandomly(time));
        }

        private IEnumerator CWalkRandomly(float time)
        {
            IsInRoutine = true;
            var elapsedTime = 0.0f;
            var direction = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(0.4f, 0.5f));

            while (elapsedTime < time)
            {
                _controller.Move(direction);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            IsInRoutine = false;
        }
    }
}