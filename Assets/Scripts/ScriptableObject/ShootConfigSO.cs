using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/Shoot Config", order = 2)]
public class ShootConfigSO : ScriptableObject
{
	public LayerMask HitMask;
	public float Spread = 0.4f;
	public float FireRate = 0.25f;
	public float MaxSpreadTime = 1f;
	public float RecoilRecoverySpeed = 1f;

	public Vector3 GetSpread(float ShootTime = 0)
	{
		return Vector3.Lerp(
			Vector3.zero, // or any min spread
			new Vector3(
				Random.Range(-Spread, Spread),
				0,
				Random.Range(-Spread, Spread)
			),
			Mathf.Clamp01(ShootTime / MaxSpreadTime)
		);
	}
}
