using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class Controller : MonoBehaviour, IDamageable
{
	public Animator animator;
	const string velocityConst = "Velocity";
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public Transform SpineTrackingSrc;
	public Transform Model;
	public float SpineTrackingVerticalOffset;
	Camera viewCamera;
	private Rigidbody rb;

	private Vector3 currentVelocity; // for SmoothDamp
	private Vector3 inputVector;

	[Header("Stat")]
	public int Health = 100;
	public float MaxSpeed = 5f;
	public float AccelerationTime = 0.2f;
	public float DecelerationTime = 0.1f;
	[Header("Collider")]
	public LayerMask layerMask;
	readonly int maxBounces = 5;
	readonly float skinWidth = 0.02f;
	new CapsuleCollider collider;
	Bounds bounds;
	Vector3 smoothedVel;

	[Header("Gun")]
	[SerializeField] PlayerGunSelector gunSelector;
	[Header("Aim")]
	public FieldOfView FieldOfView;
	public float AimViewAngle;
	public float AimSpeed;
	float defaultViewAngle;
	float tAim;
	public FOVVisualization FOVVisualization;


	void Awake()
	{
		InputSystem.actions.FindAction("Movement").performed += HandleMovementEvent;
		rb = GetComponent<Rigidbody>();
		collider = GetComponent<CapsuleCollider>();
		bounds = collider.bounds;
	}
	void Start()
	{
		viewCamera = Camera.main;
		bounds.Expand(-2 * skinWidth);
		defaultViewAngle = FieldOfView.ViewAngle;
	}

	void Update()
	{
		gunSelector.ActiveGun.Tick(
			Application.isFocused && Mouse.current.leftButton.isPressed && gunSelector.ActiveGun != null
		);
		LookAtMouse();
		HandleMovementAnimation();
		Aim();
	}
	void FixedUpdate()
	{
		HandleMovement();
	}
	bool prevRightButtonPressed;
	void Aim()
	{
		bool isPressed = Mouse.current.rightButton.isPressed;
		float endAngle = isPressed ? AimViewAngle : defaultViewAngle;
		float startAngle = isPressed ? defaultViewAngle : AimViewAngle;
		if (isPressed != prevRightButtonPressed)
		{
			tAim = 0;
		}
		if (tAim < 1 && FieldOfView.ViewAngle != endAngle)
		{
			tAim += Time.deltaTime * AimSpeed;
			float t = Mathf.Clamp01(tAim);
			FieldOfView.ViewAngle = Mathf.Lerp(startAngle, endAngle, EaseOutCirc(t));
		}
		else { tAim = 0; }
		prevRightButtonPressed = isPressed;
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Vector3 from = gunSelector.ActiveGun.Model.transform.position + (0.5f * Vector3.up);
		float spread = gunSelector.ActiveGun.ShootConfig.Spread;
		Vector3 dir1 = (FOVVisualization.transform.forward + FOVVisualization.transform.right * spread).normalized;
		Vector3 dir2 = (FOVVisualization.transform.forward - FOVVisualization.transform.right * spread).normalized;
		float length = 5;
		Gizmos.DrawRay(from, dir1 * length);
		Gizmos.DrawRay(from, dir2 * length);
	}

	float EaseOutCirc(float t) { return Mathf.Sqrt(1 - Mathf.Pow(t - 1, 2)); }
	void LookAtMouse()
	{
		// Create a plane at the character's height
		Plane plane = new(Vector3.up, transform.position);

		// Raycast from the mouse position into the scene
		Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);

		// Determine the hit point
		if (plane.Raycast(ray, out float distance))
		{
			Vector3 mousePos = ray.GetPoint(distance);

			SpineTrackingSrc.position = mousePos + Vector3.up * SpineTrackingVerticalOffset;
		}
	}
	#region Movement
	void HandleMovementAnimation()
	{
		if (inputVector != Vector3.zero)
			Model.transform.forward = inputVector;
		if (inputVector != Vector3.zero)
			animator.SetFloat(velocityConst, 1);
		else
			animator.SetFloat(velocityConst, 0);
	}

	void HandleMovementEvent(InputAction.CallbackContext context)
	{
		var value = context.ReadValue<Vector2>();
		inputVector = new Vector3(value.x, 0, value.y);
	}

	void HandleMovement()
	{// Smooth movement with acceleration/deceleration
		Vector3 targetVel = inputVector * MaxSpeed;
		smoothedVel = Vector3.SmoothDamp(
		   rb.linearVelocity,
		   targetVel,
		   ref currentVelocity,
		   inputVector.magnitude > 0 ? AccelerationTime : DecelerationTime
	   );
		var collide = CollideAndSlide(smoothedVel, collider.center, 0, smoothedVel);
		rb.MovePosition(rb.position + collide * Time.fixedDeltaTime);
	}
	#endregion

	// TODO Right now character can clip through moving collider. Find away to improve collider detection 
	#region  Collider
	Vector3 CollideAndSlide(Vector3 vel, Vector3 center, int depth, Vector3 velInit)
	{
		if (depth >= maxBounces) return Vector3.zero;

		float dist = vel.magnitude * Time.fixedDeltaTime + skinWidth;

		Vector3 p1 = transform.position + center + -collider.height * 0.5f * Vector3.up;
		Vector3 p2 = p1 + Vector3.up * collider.height;


		if (Physics.CapsuleCast(p1, p2, bounds.extents.x, vel.normalized, out RaycastHit hit, dist, layerMask))
		{
			Vector3 snapToSurface = vel.normalized * (hit.distance - skinWidth);
			Vector3 leftover = vel - snapToSurface;

			if (snapToSurface.magnitude <= skinWidth)
			{
				snapToSurface = Vector3.zero;
			}

			float scale = 1 - Vector3.Dot(new Vector3(hit.normal.x, 0, hit.normal.z).normalized, -new Vector3(velInit.x, 0, velInit.z).normalized);// use to scale character velocity down along side the wall. it would stop the character when he facing directly to the wall

			float mag = leftover.magnitude;
			leftover = Vector3.ProjectOnPlane(leftover, hit.normal).normalized;
			leftover *= mag * scale;

			return snapToSurface + CollideAndSlide(leftover, center + snapToSurface, depth + 1, velInit);
		}

		return vel;
	}
	#endregion

	void OnEnable()
	{
		InputSystem.actions.FindActionMap("CharacterController").Enable();
	}
	void OnDisable()
	{

		InputSystem.actions.FindActionMap("CharacterController").Disable();
		InputSystem.actions.FindAction("Movement").performed -= HandleMovementEvent;
	}

	public void TakeDamage(int Damage)
	{
		Health -= Damage;

		if (Health <= 0)
		{
			gameObject.SetActive(false);
		}
	}

	public Transform GetTransform()
	{
		return transform;
	}
}
