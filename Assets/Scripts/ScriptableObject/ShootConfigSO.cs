using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/Shoot Config", order = 2)]
public class ShootConfigSO : ScriptableObject
{
	public LayerMask HitMask;
	public float MinSpread = 0.1f;
	public float MaxSpread = 0.4f;
	public float FireRate = 0.25f;
	public float MaxSpreadTime = 1f;
	public float RecoilRecoverySpeed = 1f;

	/// <param name="lerp">the ratio represent how long the player held focus</param>
	public Vector3 GetSpread(float lerp)
	{
		return Vector3.Lerp(new Vector3(Random.Range(-MaxSpread, MaxSpread), 0, Random.Range(-MaxSpread, MaxSpread)), new Vector3(Random.Range(-MinSpread, MinSpread), 0, Random.Range(-MinSpread, MinSpread)), lerp);
	}

	internal Vector3 GetSpread(object focusLerp)
	{
		throw new System.NotImplementedException();
	}
}
