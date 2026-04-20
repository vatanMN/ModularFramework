namespace MiniGame.TowerDefense
{
    public interface IDamageable
    {
        float MaxHealth { get; }
        float CurrentHealth { get; }
        bool IsAlive { get; }
        void TakeDamage(float amount);
    }
}
