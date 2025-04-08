using UnityEngine;

public class XBotAnimControllerScript2D : MonoBehaviour
{
	Animator animator;
	float velocityX;
	float velocityZ;
	int velocityXHash;
	int velocityZHash;
	public float acceleration = 0.2f;
	public float deceleration = 0.5f;
	void Awake()
	{
		animator = GetComponent<Animator>();
		velocityXHash = Animator.StringToHash("Velocity X");
		velocityZHash = Animator.StringToHash("Velocity Z");
	}

	void Update()
	{
		bool forwardPressed = Input.GetKey(KeyCode.W);
		bool leftStrafePressed = Input.GetKey(KeyCode.A);
		bool rightStrafePressed = Input.GetKey(KeyCode.D);
		bool leftShiftPressed = Input.GetKey(KeyCode.LeftShift);

		if (forwardPressed && velocityZ <= 2.0f)
		{
			velocityZ = Mathf.Clamp(velocityZ + Time.deltaTime * acceleration, 0, 2);
		}
		if (!forwardPressed && velocityZ > 0.01f)
		{
			velocityZ = Mathf.Clamp(velocityZ - Time.deltaTime * deceleration, 0, 2);
		}

		if (!rightStrafePressed)
		{

			if (leftStrafePressed && velocityX >= -2.0f)
			{
				velocityX = Mathf.Clamp(velocityX - Time.deltaTime * acceleration, -2, 2);
			}
			else if (!leftStrafePressed && velocityX < 0.01f)
			{
				velocityX = Mathf.Clamp(velocityX + Time.deltaTime * acceleration, -2, 2);
			}
		}

		if (!leftStrafePressed)
		{
			if (rightStrafePressed && velocityX <= 2.0f)
			{
				velocityX = Mathf.Clamp(velocityX + Time.deltaTime * acceleration, -2, 2);
			}
			else if (!rightStrafePressed && velocityX > 0.01f)
			{
				velocityX = Mathf.Clamp(velocityX - Time.deltaTime * acceleration, -2, 2);
			}
		}

		animator.SetFloat(velocityXHash, velocityX);
		animator.SetFloat(velocityZHash, velocityZ);
	}
}
