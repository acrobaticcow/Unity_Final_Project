using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;

[RequireComponent(typeof(Animator))]
public class Controller_Deprecated : MonoBehaviour
{
	public float moveSpeed = 2f;
	public float runSpeed = 6;
	Camera viewCamera;
	Animator animator;
	Vector2 direction;
	bool isRunning;
	bool isWalking;

	#region Animation Parameter Names
	const string HorizontalConst = "Horizontal";
	const string VerticalConst = "Vertical";
	const string RotateConst = "Rotate";
	#endregion
	float lerp;
	public Transform HeadTrackingObj;
	public Transform HeadTrackingObjOffset;


	void Awake()
	{
		InputSystem.actions.FindAction("Movement").performed += HandleMovementAction;
		InputSystem.actions.FindAction("Run").performed += HandleRunAction;
	}
	void Start()
	{
		viewCamera = Camera.main;
		animator = GetComponent<Animator>();
	}

	private void HandleMovementAction(InputAction.CallbackContext context)
	{
		var rawInput = context.ReadValue<Vector2>();
		if (rawInput != Vector2.zero) isWalking = true;
		else isWalking = false;

		if (!isRunning && isWalking)
		{
			rawInput = math.remap(-1, 1, -0.5f, 0.5f, rawInput);
		}
		if (direction != rawInput) // direction change
		{
			lerp = 0;
		}
		direction = rawInput;
	}
	private void HandleRunAction(InputAction.CallbackContext context)
	{
		if (isWalking)
			isRunning = context.ReadValueAsButton();
		else isRunning = false;
	}

	void Update()
	{
		// transform.rotation = Quaternion.AngleAxis(-90, Vector3.up);
		LookAtMouse();
		HandleMovement();
		// var test = Vector3.SignedAngle(Vector3.forward, Vector3.back, Vector3.up);
		// Debug.Log(test);
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

			// Calculate the angle to the mouse
			Vector3 characterToMouse = mousePos - transform.position;
			characterToMouse.y = 0; // Ignore height difference
			characterToMouse.Normalize(); // Normalize the vector

			HandleRotation(characterToMouse);

			HeadTrackingObj.position = characterToMouse + Vector3.up * HeadTrackingObjOffset.position.y;
		}
	}

	private void HandleRotation(Vector3 characterToMouse)
	{
		var angleToMouse = Vector3.SignedAngle(transform.forward, characterToMouse, Vector3.up);
		Debug.Log("angleToMouse" + angleToMouse);
		float dot = Vector3.Dot(transform.forward, Vector3.forward);
		// Debug.Log("dot" + dot);
		var isParallel = Mathf.Abs(dot) > 0.9f;
		// Debug.Log("isPerpendicular" + isParallel);
		// if (isParallel)
		// {
		// 	// stop the rotation
		// 	animator.SetLayerWeight(1, 0);
		// 	animator.SetFloat(RotateConst, 0);
		// 	return;
		// }
		// Mathf.Abs(dot) == 1 || Mathf.Abs(dot) == 0
		if (Mathf.Abs(angleToMouse) < 10)
		{
			// stop the rotation
			animator.SetLayerWeight(1, 0);
			animator.SetFloat(RotateConst, 0);
		}
		else if (Mathf.Abs(angleToMouse) > 90)
		{
			// start the rotation
			animator.SetLayerWeight(1, 0.1f);
			animator.SetFloat(RotateConst, Mathf.Sign(angleToMouse));
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + transform.forward * 3);
	}

	void HandleMovement()
	{
		lerp += Time.deltaTime * moveSpeed;
		var angleToDirection = Vector3.SignedAngle(transform.forward, new(direction.x, 0, direction.y), Vector3.up);
		var newDirection = isWalking ? Quaternion.AngleAxis(angleToDirection, Vector3.up) * Vector3.forward : Vector3.zero;
		// todo di chuyển chéo hiện giờ đang bị nhanh hơn 
		animator.SetFloat(HorizontalConst, Mathf.Lerp(animator.GetFloat(HorizontalConst), newDirection.x, lerp));
		animator.SetFloat(VerticalConst, Mathf.Lerp(animator.GetFloat(VerticalConst), newDirection.z, lerp));
		if (lerp < 1)
		{
			lerp += Time.deltaTime * moveSpeed;
		}
		// velocity = new Vector3(direction.x, 0, direction.y) * (isRunning ? runSpeed : moveSpeed) * Time.deltaTime;
	}

	// void FixedUpdate()
	// {
	// 	rigidBody.MovePosition(rigidBody.position + velocity * Time.fixedDeltaTime);
	// }
	void OnEnable()
	{
		InputSystem.actions.FindActionMap("CharacterController").Enable();
	}
	void OnDisable()
	{

		InputSystem.actions.FindActionMap("CharacterController").Disable();
		InputSystem.actions.FindAction("Movement").performed -= HandleMovementAction;
		InputSystem.actions.FindAction("Run").performed -= HandleRunAction;
	}
}