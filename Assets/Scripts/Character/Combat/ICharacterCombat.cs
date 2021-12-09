namespace Character.Combat
{
    public interface ICharacterCombat
    {
        void Reset();

        void LightAttack();
        void StartHeavyAttack();
        void HeavyAttack();

        void StartBlocking();
        void StopBlocking();

        void Parry();

        void Dodge(float direction);
    }
}