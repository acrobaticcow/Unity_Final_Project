using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class Controller : MonoBehaviour
{
	public float moveSpeed = 3;
	public float runSpeed = 6;
	Rigidbody rigidBody;
	Camera viewCamera;
	Animator animator;
	Vector3 velocity;
	Vector2 direction;
	bool isRunning;
	const string VelocityId = "Velocity";

	void Awake()
	{
		InputSystem.actions.FindAction("Movement").performed += HandleMovementAction;
		InputSystem.actions.FindAction("Run").performed += HandleRunAction;
	}

	private void HandleMovementAction(InputAction.CallbackContext context)
	{
		direction = context.ReadValue<Vector2>();
	}
	private void HandleRunAction(InputAction.CallbackContext context)
	{
		isRunning = context.ReadValueAsButton();
	}

	void Start()
	{
		rigidBody = GetComponent<Rigidbody>();
		viewCamera = Camera.main;
		animator = GetComponent<Animator>();
	}
	void Update()
	{
		LookAtMouse();
		Debug.Log(direction);
		HandleMovement();
	}

	void LookAtMouse()
	{
		Vector3 mousePos = viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, viewCamera.transform.position.y));
		transform.LookAt(mousePos + Vector3.up * transform.position.y);
	}

	void HandleMovement()
	{
		animator.SetFloat(VelocityId, direction.y * (isRunning ? 1 : 0.5f));
		velocity = new Vector3(direction.x, 0, direction.y) * (isRunning ? runSpeed : moveSpeed) * Time.deltaTime;
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