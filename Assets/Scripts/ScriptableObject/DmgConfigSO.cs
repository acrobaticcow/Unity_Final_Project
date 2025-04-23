using UnityEngine;
using static UnityEngine.ParticleSystem;

[CreateAssetMenu(fileName = "Damage Config", menuName = "Guns/Damage Config", order = 1)]
public class DmgConfigSO : ScriptableObject
{
    public MinMaxCurve DamageCurve;

    private void Reset()
    {
        DamageCurve.mode = ParticleSystemCurveMode.Curve;
    }

    public int GetDamage(float Distance = 0, float DamageMultiplier = 1)
    {
        return Mathf.CeilToInt(DamageCurve.Evaluate(Distance, Random.value) * DamageMultiplier);
    }
}
