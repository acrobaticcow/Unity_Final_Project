using UnityEngine;

public class XBotAnimControllerScript : MonoBehaviour
{
	Animator animator;
	float velocity;
	public float acceleration = 0.1f;
	public float deceleration = 0.5f;
	int velocityHash;
	void Awake()
	{
		animator = GetComponent<Animator>();
		velocityHash = Animator.StringToHash("Velocity");
	}

	void Update()
	{
		bool isInputForward = Input.GetKey(KeyCode.W);
		bool isLeftShiftPress = Input.GetKey(KeyCode.LeftShift);

		if (isInputForward && velocity < 1)
		{
			velocity += Time.deltaTime * acceleration;
			animator.SetFloat(velocityHash, velocity);
		}
		if (!isInputForward && velocity > 0.01f)
		{
			velocity = Mathf.Clamp(velocity - Time.deltaTime * deceleration, 0, Mathf.Infinity);
			animator.SetFloat(velocityHash, velocity);
		}
	}
}
