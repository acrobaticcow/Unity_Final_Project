using UnityEngine;
using UnityEngine.InputSystem;


// todo make camera follow the player
public class Controller : MonoBehaviour, IDamageable
{
	public Animator animator;
	const string velocityConst = "Velocity";
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public Transform SourceObj;
	public Transform SourceObjOffset;
	Camera viewCamera;
	private Rigidbody rb;

	private Vector3 currentVelocity; // for SmoothDamp
	private Vector3 inputVector;

	[Header("Stat")]
	public int Health = 100;
	public float maxSpeed = 5f;
	public float accelerationTime = 0.2f;
	public float decelerationTime = 0.1f;
	[Header("Collider")]
	public LayerMask layerMask;
	readonly int maxBounces = 5;
	readonly float skinWidth = 0.02f;
	new CapsuleCollider collider;
	Bounds bounds;
	Vector3 smoothedVel;

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
	}

	// Update is called once per frame
	void Update()
	{
		// Debug.Log("Player position" + transform.position);
		LookAtMouse();
		HandleMovementAnimation();
	}


	void FixedUpdate()
	{
		HandleMovement();
	}
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

			SourceObj.position = mousePos + Vector3.up * SourceObjOffset.position.y;
		}
	}
	#region Movement
	void HandleMovementAnimation()
	{
		if (inputVector != Vector3.zero) transform.forward = inputVector;
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
		Vector3 targetVel = inputVector * maxSpeed;
		smoothedVel = Vector3.SmoothDamp(
		   rb.linearVelocity,
		   targetVel,
		   ref currentVelocity,
		   inputVector.magnitude > 0 ? accelerationTime : decelerationTime
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
