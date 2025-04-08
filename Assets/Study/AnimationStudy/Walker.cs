using UnityEngine;

public class Walker : MonoBehaviour
{
	public Transform LeftFootTarget;
	public Transform RightFootTarget;
	AnimationCurve horizontalCurve;
	AnimationCurve verticalCurve;
	public WalkerSO walkerSO;
	Vector3 leftFootOffset;
	Vector3 rightFootOffset;
	float lastLeftLegHorizontalValue;
	float lastRightLegHorizontalValue;
	LayerMask groundLayer;
	void Start()
	{
		horizontalCurve = walkerSO.HorizontalCurve;
		verticalCurve = walkerSO.VerticalCurve;
		leftFootOffset = LeftFootTarget.localPosition;
		rightFootOffset = RightFootTarget.localPosition;
		groundLayer = LayerMask.GetMask("Ground");
	}

	// Update is called once per frame
	void Update()

	{
		float leftLegHorizontalValue = horizontalCurve.Evaluate(Time.time);
		float rightLegHorizontalValue = horizontalCurve.Evaluate(Time.time - 0.5f);

		LeftFootTarget.localPosition = leftFootOffset + transform.InverseTransformVector(transform.forward) * leftLegHorizontalValue + verticalCurve.Evaluate(Time.time) * transform.InverseTransformVector(transform.up);
		RightFootTarget.localPosition = rightFootOffset + transform.InverseTransformVector(transform.forward) * rightLegHorizontalValue + verticalCurve.Evaluate(Time.time - 0.5f) * transform.InverseTransformVector(transform.up);


		float leftLegDirection = leftLegHorizontalValue - lastLeftLegHorizontalValue;
		float rightLegDirection = rightLegHorizontalValue - lastRightLegHorizontalValue;

		RaycastHit hit;

		if (leftLegDirection < 0 && Physics.Raycast(transform.position + transform.right * LeftFootTarget.localPosition.x, Vector3.down, out hit, Mathf.Infinity, groundLayer.value))
		{
			LeftFootTarget.position = hit.point;
		}
		if (rightLegDirection < 0 && Physics.Raycast(transform.position + transform.right * RightFootTarget.localPosition.x, Vector3.down, out hit, Mathf.Infinity, groundLayer.value))
		{
			RightFootTarget.position = hit.point;
		}

		lastLeftLegHorizontalValue = leftLegHorizontalValue;
		lastRightLegHorizontalValue = rightLegHorizontalValue;
	}
}
