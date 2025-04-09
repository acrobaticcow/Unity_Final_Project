using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;

[RequireComponent(typeof(Animator))]
public class Controller : MonoBehaviour
{
	public float moveSpeed = 3;
	public float runSpeed = 6;
	Camera viewCamera;
	Animator animator;
	Vector2 direction;
	bool isRunning;
	bool isWalking;
	const string HorizontalConst = "Horizontal";
	const string VerticalConst = "Vertical";


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
		Vector3 mousePos = viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, viewCamera.transform.position.y));
		Debug.Log("mousePos: " + mousePos);
		transform.LookAt(mousePos + Vector3.up * transform.position.y);
	}

	void HandleMovement()
	{
		Debug.Log("direction" + direction);
		var angleToDirection = Vector3.SignedAngle(transform.forward, new(direction.x, 0, direction.y), Vector3.up);
		Debug.Log("angleToDirection" + angleToDirection);
		var newDirection = isWalking ? Quaternion.AngleAxis(angleToDirection, Vector3.up) * Vector3.forward : Vector3.zero;
		Debug.Log("New direction:" + newDirection);
		animator.SetFloat(HorizontalConst, newDirection.x);
		animator.SetFloat(VerticalConst, newDirection.z);
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