using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class Targeting : MonoBehaviour
    {
        private bool _lookForTarget;

        [SerializeField]
        private LayerMask layer;
        [SerializeField]
        private float range;

        public Transform Target { get; private set; }
        public TargetInfo TargetInfo { get; private set; }

        private Transform _transform;

        private void Awake()
        {
            _lookForTarget = true;
            _transform ??= GetComponent<Transform>();
        }

        private void Update()
        {
            if (_lookForTarget)
                LookForTarget();
        }

        public void Disable()
        {
            _lookForTarget = false;
            Target = null;
            TargetInfo = null;
        }

        private void LookForTarget()
        {
            Target = null;
            var checkCenter = _transform.position + Vector3.up;
            var candidates = Physics.OverlapSphere(checkCenter, range, layer);
            var targets = new List<Transform>();
            foreach (var candidate in candidates)
            {
                var candidateController = candidate.GetComponent<HumanoidController>();
                if (candidate.gameObject != gameObject && candidateController != null && candidateController.IsAlive)
                    targets.Add(candidate.transform);
            }

            if (targets.Count == 0)
                return;

            var minDistance = float.MaxValue;
            foreach (var target in targets)
            {
                var distance = Vector3.Distance(_transform.position, target.position);
                if (target == _transform || distance >= minDistance)
                    continue;
                minDistance = distance;
                if (Target == null || target != Target)
                {
                    OnTargetChanged(target);
                }

                Target = target;
            }
        }

        private void OnTargetChanged(Component newTarget)
        {
            if (newTarget == null) return;
            TargetInfo = newTarget.gameObject.GetComponent<HumanoidController>().Info;
        }

        public bool HasTarget()
        {
            return Target != null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}