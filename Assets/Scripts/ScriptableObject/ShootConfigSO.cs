using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/Shoot Config", order = 2)]
public class ShootConfigSO : ScriptableObject
{
    public LayerMask HitMask;
    public float MinSpread = 0.1f;
    public float FireRate = 0.25f;
    public float RecoilRecoverySpeed = 1f;

    [Range(0f, 1f)]
    public float RecoilStrength;
    float maxSpread;

    public void Init()
    {
        maxSpread = Mathf.Tan(Player._aimAngle * 0.5f * Mathf.Deg2Rad);
    }

    public float BiasedRandom(float value)
    {
        // total span = 2*MinSpread
        float totalSpan = value * 2f;
        // width of each tail (20% of totalSpan)
        float tailWidth = totalSpan * 0.2f; // = 0.4f * MinSpread

        // roll a probability
        if (Random.value < 0.8f)
        {
            // 80% → sample from one of the two tails:
            // left tail:   [-MinSpread, -MinSpread + tailWidth]
            // right tail:  [ MinSpread - tailWidth,  MinSpread]
            if (Random.value < 0.5f)
                return Random.Range(-value, -value + tailWidth);
            else
                return Random.Range(value - tailWidth, value);
        }
        else
        {
            // 20% → sample from the middle:
            // [-MinSpread + tailWidth, MinSpread - tailWidth]
            return Random.Range(-value + tailWidth, value - tailWidth);
        }
    }

    public Vector3 GetSpread(float lerp)
    {
        return Vector3.Lerp(
            new Vector3(BiasedRandom(maxSpread), 0, 0),
            new Vector3(Random.Range(-MinSpread, MinSpread), 0, 0),
            lerp
        );
    }
}
