using Character;
using UnityEngine;

namespace ScriptableObjects.Brains
{
    public abstract class HumanoidBrain : ScriptableObject
    {
        protected HumanoidController Controller { get; private set; }
        protected Targeting Targeting { get; private set; }
        protected AIControls Controls { get; private set; }

        public void Init(HumanoidController humanoidController, Targeting targeting, AIControls controls)
        {
            Controller = humanoidController;
            Targeting = targeting;
            Controls = controls;
        }

        public abstract void Think();
    }
}