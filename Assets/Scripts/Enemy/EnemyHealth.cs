using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    int _Health = 100;

    int _MaxHealth = 100;
    public int CurrentHealth
    {
        get => _Health;
        private set => _Health = value;
    }
    public int MaxHealth
    {
        get => _MaxHealth;
        private set => _MaxHealth = value;
    }

    public void SetUp(int MaxHealth)
    {
        _MaxHealth = MaxHealth;
        _Health = _MaxHealth;
    }

    public virtual void OnEnable()
    {
        CurrentHealth = MaxHealth;
    }

    public event IDamageable.TakeDamageEvent OnTakeDamage;
    public event IDamageable.DeathEvent OnDeath;

    public void TakeDamage(int Damage)
    {
        int damageTaken = Mathf.Clamp(Damage, 0, CurrentHealth);
        Debug.Log("Damage Taken" + damageTaken);

        CurrentHealth -= damageTaken;

        if (damageTaken != 0)
        {
            OnTakeDamage?.Invoke(damageTaken);
            Debug.Log("Damage Taken" + damageTaken);
        }

        if (CurrentHealth == 0 && damageTaken != 0)
        {
            OnDeath?.Invoke(transform.position);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
