using UnityEngine;
using System.Collections;

public class PlayerShoot : MonoBehaviour
{
	public Bullet bulletPrefab;
	[Tooltip("Per seconds")]
	public int rateOfFire = 5;
	ObjectPool bulletPool;

	void Awake()
	{
		bulletPool = ObjectPool.CreateInstance(bulletPrefab, 200);
	}
	void Start()
	{
		StartCoroutine(Shoot());
	}

	private IEnumerator Shoot()
	{
		WaitForSeconds wait = new(1f / rateOfFire);
		while (enabled)
		{
			var instance = bulletPool.GetObject();
			if (instance != null)
			{
				instance.transform.SetParent(transform, false);
				instance.transform.localPosition = Vector2.zero;
			}
			yield return wait;
		}
	}
}