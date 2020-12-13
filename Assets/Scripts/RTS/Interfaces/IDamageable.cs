namespace RTS.Interfaces
{
    public interface IDamageable
    {
        bool CanBeDamaged();
        void Damage(float damage);
    }
}