using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
	[HideInInspector]
	public Transform Target;
	NavMeshAgent agent;

	[HideInInspector]
	[Tooltip("How long should this agent re-target target destination. In seconds")]
	public float UpdateRate;
	// public Animator animator;
	Coroutine FollowTargetCoroutine;
	// private const string _Speed = "Speed";
	// private Vector3 lastPosition;

	void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		// lastPosition = transform.position;
	}

	// void Update() {
	// 	SyncAnimator();
	// }

	// private void SyncAnimator()
	// {
	// 	Vector3 distance = transform.position - lastPosition;
	// 	float speed = distance.magnitude / Time.deltaTime;
	// 	if (speed > 0.01f)
	// 		animator.SetFloat(_Speed, speed);

	// 	lastPosition = transform.position;
	// }

	public void StartChasing()
	{
		if (FollowTargetCoroutine == null)
			FollowTargetCoroutine = StartCoroutine(FollowTarget());
		else Debug.LogError("Why calling chase target while it is already chasing ! Likely a bug");
	}

	private IEnumerator FollowTarget()
	{
		WaitForSeconds wait = new(UpdateRate);
		while (enabled)
		{
			agent.SetDestination(Target.transform.position);
			yield return wait;
		}
	}
}
