namespace RTS.Controls
{
    public interface IDamageable
    {
        bool CanBeDamaged();
        void Damage(float damage);
    }
}