using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyLineOfSightCheck : MonoBehaviour
{
	public SphereCollider Collider;
	public float FieldOfView = 90f;
	public LayerMask LineOfSightLayers;

	public delegate void GainSightEvent(Controller controller);
	public GainSightEvent OnGainSight;
	public delegate void LoseSightEvent(Controller controller);
	public LoseSightEvent OnLoseSight;

	private Coroutine CheckForLineOfSightCoroutine;

	private void Awake()
	{
		Collider = GetComponent<SphereCollider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out Controller controller))
		{
			if (!CheckLineOfSight(controller))
			{
				CheckForLineOfSightCoroutine = StartCoroutine(CheckForLineOfSight(controller));
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent(out Controller controller))
		{
			OnLoseSight?.Invoke(controller);
			if (CheckForLineOfSightCoroutine != null)
			{
				StopCoroutine(CheckForLineOfSightCoroutine);
			}
		}
	}

	private bool CheckLineOfSight(Controller controller)
	{
		Vector3 Direction = (controller.transform.position - transform.position).normalized;
		float DotProduct = Vector3.Dot(transform.forward, Direction);
		if (DotProduct >= Mathf.Cos(FieldOfView))
		{
			if (Physics.Raycast(transform.position, Direction, out RaycastHit Hit, Collider.radius, LineOfSightLayers))
			{
				if (Hit.transform.GetComponent<Controller>() != null)
				{
					OnGainSight?.Invoke(controller);
					return true;
				}
			}
		}

		return false;
	}

	private IEnumerator CheckForLineOfSight(Controller controller)
	{
		WaitForSeconds Wait = new(0.1f);

		while (!CheckLineOfSight(controller))
		{
			yield return Wait;
		}
	}
}
