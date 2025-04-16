using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
	const string movementBlendConst = "MovementBlend";
	const string moveConst = "Move";
	[HideInInspector]
	public Transform Target;
	public Enemy Enemy;
	NavMeshAgent agent;
	Animator animator;

	[HideInInspector]
	[Tooltip("How long should this agent re-target target destination. In seconds")]
	public float UpdateRate;
	Coroutine FollowCoroutine;

	[SerializeField]
	LookAtForAnimatorIK LookAtForAnimatorIK;

	Vector2 Velocity;
	Vector2 SmoothDeltaPosition;
	public Vector3[] Waypoints = new Vector3[4];
	[SerializeField]
	private int WaypointIndex = 0;
	public NavMeshTriangulation Triangulation;

	#region  Unity function
	void Awake()
	{
		agent = Enemy.Agent;
		animator = Enemy.Animator;

		animator.applyRootMotion = true;
		agent.updatePosition = false;
		agent.updateRotation = true;
	}

	void Update()
	{
		SynchronizeAnimatorAndAgent();
	}

	private void OnAnimatorMove()
	{
		Vector3 rootPosition = animator.rootPosition;
		rootPosition.y = agent.nextPosition.y;
		Enemy.transform.position = rootPosition;
		agent.nextPosition = rootPosition;
	}
	#endregion

	#region Movement Coroutines
	public void StopMovement()
	{
		if (FollowCoroutine != null)
		{
			StopCoroutine(FollowCoroutine); FollowCoroutine = null;
		}
	}

	public void StartChasing()
	{
		if (FollowCoroutine == null)
			FollowCoroutine = StartCoroutine(FollowTarget());
		else Debug.LogError("Why calling chase target while it is already <color=green>chasing</color> ! Likely a bug");
	}
	public void StartIdleMotion()
	{
		if (FollowCoroutine == null)
			FollowCoroutine = StartCoroutine(DoIdleMotion());
		else Debug.LogError("Why calling chase target while it is already <color=green>idle</color> ! Likely a bug");
	}

	public void StartPatrolMotion()
	{
		if (FollowCoroutine == null)
			FollowCoroutine = StartCoroutine(DoPatrolMotion());
		else Debug.LogError("Why calling chase target while it is already <color=green>patrol</color> ! Likely a bug");
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
	private IEnumerator DoIdleMotion()
	{
		WaitForSeconds Wait = new WaitForSeconds(UpdateRate);

		agent.speed *= Enemy.IdleMoveSpeedMultiplier;

		while (true)
		{
			if (!agent.enabled || !agent.isOnNavMesh)
			{
				yield return Wait;
			}
			else if (agent.remainingDistance <= agent.stoppingDistance)
			{
				Vector2 point = Random.insideUnitCircle * Enemy.IdleLocationRadius;

				if (NavMesh.SamplePosition(agent.transform.position + new Vector3(point.x, 0, point.y), out NavMeshHit hit, 2f, agent.areaMask))
				{
					agent.SetDestination(hit.position);
				}
			}

			yield return Wait;
		}
	}

	// TODO finish what you start
	private IEnumerator DoPatrolMotion()
	{
		WaitForSeconds Wait = new(UpdateRate);

		yield return new WaitUntil(() => agent.enabled && agent.isOnNavMesh);
		agent.SetDestination(Waypoints[WaypointIndex]);

		while (true)
		{
			if (agent.isOnNavMesh && agent.enabled && agent.remainingDistance <= agent.stoppingDistance)
			{
				WaypointIndex++;

				if (WaypointIndex >= Waypoints.Length)
				{
					WaypointIndex = 0;
				}

				agent.SetDestination(Waypoints[WaypointIndex]);
			}

			yield return Wait;
		}
	}


	#endregion
	public void SampleWaypoints()
	{
		for (int i = 0; i < Waypoints.Length; i++)
		{
			if (NavMesh.SamplePosition(Triangulation.vertices[Random.Range(0, Triangulation.vertices.Length)], out NavMeshHit Hit, 2f, agent.areaMask))
			{
				Waypoints[i] = Hit.position;
			}
			else
			{
				Debug.LogError("Unable to find position for navmesh near Triangulation vertex!");
			}
		}
	}

	private void SynchronizeAnimatorAndAgent()
	{
		Vector3 worldDeltaPosition = agent.nextPosition - Enemy.transform.position;
		worldDeltaPosition.y = 0;
		// Map 'worldDeltaPosition' to local space
		float dx = Vector3.Dot(Enemy.transform.right, worldDeltaPosition);
		float dy = Vector3.Dot(Enemy.transform.forward, worldDeltaPosition);
		Vector2 deltaPosition = new(dx, dy);

		// Low-pass filter the deltaMove
		float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
		SmoothDeltaPosition = Vector2.Lerp(SmoothDeltaPosition, deltaPosition, smooth);

		Velocity = SmoothDeltaPosition / Time.deltaTime;

		if (agent.remainingDistance <= agent.stoppingDistance)
		{
			Velocity = Vector2.Lerp(Vector2.zero, Velocity, agent.remainingDistance);
		}

		// TODO play with these values to tune stop and chase behaviour
		// ! sometimes when agent haven't reach target but really close the velocity are reduced so low that it practically stop moving. Maybe drop velocity magnitude
		bool shouldMove = Velocity.magnitude > 0.5f && agent.remainingDistance > agent.stoppingDistance;

		animator.SetBool(moveConst, shouldMove);
		animator.SetFloat(movementBlendConst, Velocity.magnitude);


		LookAtForAnimatorIK.lookAtTargetPosition = agent.steeringTarget + Enemy.transform.forward;

		// Play with these threshold to rectify the jittering when the agent is try to go to the edge of the nav mesh
		// The jittering is cause by the distance between the model and Game Object transform 
		float deltaMagnitude = worldDeltaPosition.magnitude;
		if (deltaMagnitude > agent.radius / 2)
		{
			Enemy.transform.position = Vector3.Lerp(animator.rootPosition, agent.nextPosition, smooth);
		}
	}
	private void OnDrawGizmosSelected()
	{
		for (int i = 0; i < Waypoints.Length; i++)
		{
			Gizmos.DrawWireSphere(Waypoints[i], 0.25f);
			if (i + 1 < Waypoints.Length)
			{
				Gizmos.DrawLine(Waypoints[i], Waypoints[i + 1]);
			}
			else
			{
				Gizmos.DrawLine(Waypoints[i], Waypoints[0]);
			}
		}
	}
}
