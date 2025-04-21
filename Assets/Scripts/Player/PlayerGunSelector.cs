using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class PlayerGunSelector : MonoBehaviour
{
	public Camera Camera;
	[field: SerializeField] public GunType Gun { get; private set; }

	[SerializeField] private Transform GunParent;
	[field: SerializeField] public List<GunSO> Guns { get; private set; }
	// [SerializeField] private PlayerIK InverseKinematics;

	[Space][Header("Runtime Filled")] public GunSO ActiveGun;
	[field: SerializeField] public GunSO ActiveBaseGun { get; private set; }

	/// <summary>
	/// If you are not using the demo AttachmentController, you may want it to initialize itself on start.
	/// If you are configuring this separately using <see cref="SetupGun"/> then set this to false.
	/// </summary>
	[SerializeField] private bool InitializeOnStart = false;

	private void Start()
	{
		if (InitializeOnStart)
		{
			GunSO gun = Guns.Find(gun => gun.Type == Gun);

			if (gun == null)
			{
				Debug.LogError($"No GunScriptableObject found for GunType: {gun}");
				return;
			}

			ActiveGun = gun;
			gun.Spawn(GunParent, this);

			// TODO inverse kinematic here
		}
	}

	// Update is called once per frame
	void Update()
	{

	}
}
