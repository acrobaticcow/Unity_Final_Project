using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class XBotAnimControllerInputSystemDemo : MonoBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	Animator animator;
	Vector2 direction;
	const string isWalkingId = "IsWalking";
	const string isRunningId = "IsRunning";
	bool isRunning;
	void Awake()
	{
		InputSystem.actions.FindAction("Movement").performed += ctx =>
		{
			direction = ctx.ReadValue<Vector2>();
		};
		InputSystem.actions.FindAction("Run").performed += ctx =>
		{
			isRunning = ctx.ReadValueAsButton();
		};
	}
	void Start()
	{
		animator = GetComponent<Animator>();
	}
	// Update is called once per frame
	void Update()
	{
		HandleMovement();
		HandleRotation();
	}

	private void HandleRotation()
	{
		if (direction.x > 0.01)
		{
			transform.Rotate(new(0, Mathf.PI / 2, 0));
		}
		if (direction.x < -0.01)
		{
			transform.Rotate(new(0, -Mathf.PI / 2, 0));
		}
	}

	private void HandleMovement()
	{
		animator.SetFloat(isWalkingId, direction.y);
		animator.SetBool(isRunningId, isRunning);
	}

	void OnEnable()
	{
		InputSystem.actions.FindActionMap("CharacterControl").Enable();
	}
	void OnDisable()
	{

		InputSystem.actions.FindActionMap("CharacterControl").Disable();
	}
}
