using UnityEngine;

public class IKFootSolver : MonoBehaviour
{
	LayerMask groundLayer;
	public Transform body;
	public float footSpacing;
	public float offset;
	public float stepDistance = 0.5f;
	Vector3 newPosition;
	Vector3 currentPosition;
	Vector3 oldPosition;
	float lerp;
	public float stepHeight = 0.2f;
	public float speed = 5f;
	public Transform otherFoot;
	public float othersFootCastingDistance = 0.2f;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		groundLayer = LayerMask.GetMask("Ground");
		newPosition = transform.position;
	}

	// Update is called once per frame
	void Update()
	{
		if (!Physics.Raycast(otherFoot.position, Vector3.down, othersFootCastingDistance, groundLayer.value)) return;

		transform.position = currentPosition + Vector3.up * offset;

		Ray ray = new(body.position + (body.right * footSpacing), Vector3.down);
		if (Physics.Raycast(ray, out RaycastHit info, 10, groundLayer.value))
		{
			// transform.position = info.point + Vector3.up * offset;
			if (Vector3.Distance(newPosition, info.point) > stepDistance)
			{
				lerp = 0;
				newPosition = info.point;
			}
		}

		if (lerp < 1)
		{
			Vector3 footPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
			footPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

			currentPosition = footPosition;
			lerp += Time.deltaTime * speed;
		}
		else { oldPosition = newPosition; }
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(newPosition, 0.05f);
	}
}
