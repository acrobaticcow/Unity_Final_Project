using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

[CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
public class GunSO : ScriptableObject
{
	public GunType Type;
	public string Name;
	public GameObject ModelPrefab;
	public Vector3 SpawnPoint; // local 
	public Vector3 SpawnRotation; // local

	public TrailConfigSO TrailConfig;
	public ShootConfigSO ShootConfig;


	MonoBehaviour ActiveMonoBehaviour;
	public GameObject Model;
	ParticleSystem ShootSystem;
	ObjectPool<TrailRenderer> TrailPool;
	float LastShootTime;

	float InitialClickTime;
	bool LastFrameWantedToShoot;

	float StopShootingTime;


	public void Spawn(Transform parent, MonoBehaviour activeMonoBehaviour)
	{
		ActiveMonoBehaviour = activeMonoBehaviour;
		LastShootTime = 0;
		TrailPool = new(CreateTrail);

		Model = Instantiate(ModelPrefab);
		Model.transform.SetParent(parent, false);
		Model.transform.SetLocalPositionAndRotation(SpawnPoint, Quaternion.Euler(SpawnRotation));

		ShootSystem = Model.GetComponentInChildren<ParticleSystem>();
	}

	/// <summary>
	/// Expected to be called every frame
	/// </summary>
	/// <param name="WantsToShoot">Whether or not the player is trying to shoot</param>
	public void Tick(bool WantsToShoot)
	{
		Model.transform.localRotation = Quaternion.Lerp(
			Model.transform.localRotation,
			Quaternion.Euler(SpawnRotation),
			Time.deltaTime * ShootConfig.RecoilRecoverySpeed
		);

		if (WantsToShoot)
		{
			LastFrameWantedToShoot = true;
			TryToShoot();
		}

		if (!WantsToShoot && LastFrameWantedToShoot)
		{
			StopShootingTime = Time.time;
			LastFrameWantedToShoot = false;
		}
	}

	public void TryToShoot()
	{
		if (Time.time - LastShootTime - ShootConfig.FireRate > Time.deltaTime)
		{
			float lastDuration = Mathf.Clamp(
				0,
				StopShootingTime - InitialClickTime,
				ShootConfig.MaxSpreadTime
			);
			float lerpTime = (ShootConfig.RecoilRecoverySpeed - (Time.time - StopShootingTime))
							 / ShootConfig.RecoilRecoverySpeed;

			InitialClickTime = Time.time - Mathf.Lerp(0, lastDuration, Mathf.Clamp01(lerpTime));
		}

		if (Time.time > ShootConfig.FireRate + LastShootTime)
		{
			LastShootTime = Time.time;
			ShootSystem.Play();

			Vector3 spreadAmount = ShootConfig.GetSpread(Time.time - InitialClickTime);
			// Vector3 spreadOvertime = Vector3.Lerp(Vector3.zero, spread, );

			Vector3 shootDirection = ShootSystem.transform.forward;
			Model.transform.forward += Model.transform.TransformDirection(spreadAmount);
			shootDirection.Normalize();
			if (Physics.Raycast(ShootSystem.transform.position, shootDirection, out RaycastHit hit, float.MaxValue, ShootConfig.HitMask))
			{
				ActiveMonoBehaviour.StartCoroutine(PlayTrail(
					ShootSystem.transform.position, hit.point, hit
				));
			}
			else
			{
				ActiveMonoBehaviour.StartCoroutine(PlayTrail(
					ShootSystem.transform.position, ShootSystem.transform.position + (shootDirection * TrailConfig.MissDistance), new RaycastHit()
				));
			}
		}
	}


	private IEnumerator PlayTrail(Vector3 StartPoint, Vector3 EndPoint, RaycastHit Hit)
	{
		TrailRenderer instance = TrailPool.Get();
		instance.gameObject.SetActive(true);
		instance.transform.position = StartPoint;
		yield return null; // avoid position carry-over from last frame if reused

		instance.emitting = true;

		float distance = Vector3.Distance(StartPoint, EndPoint);
		float remainingDistance = distance;
		while (remainingDistance > 0)
		{
			instance.transform.position = Vector3.Lerp(
				StartPoint,
				EndPoint,
				Mathf.Clamp01(1 - (remainingDistance / distance))
			);
			remainingDistance -= TrailConfig.SimulationSpeed * Time.deltaTime;

			yield return null;
		}

		instance.transform.position = EndPoint;

		yield return new WaitForSeconds(TrailConfig.Duration);
		yield return null;
		instance.emitting = false;
		instance.gameObject.SetActive(false);
		TrailPool.Release(instance);
	}

	private TrailRenderer CreateTrail()
	{
		GameObject instance = new("Trail Renderer");
		TrailRenderer trail = instance.AddComponent<TrailRenderer>();

		trail.colorGradient = TrailConfig.Color;
		trail.material = TrailConfig.Material;
		trail.widthCurve = TrailConfig.WidthCurve;
		trail.time = TrailConfig.Duration;
		trail.minVertexDistance = TrailConfig.MinVertexDistance;

		trail.emitting = false;
		trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

		return trail;
	}
}
