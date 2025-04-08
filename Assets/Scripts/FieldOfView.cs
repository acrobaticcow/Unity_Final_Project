using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FieldOfView : MonoBehaviour
{
	[Range(0, 360)]
	public float viewAngle;
	public float viewRadius;

	[HideInInspector]
	public List<Transform> visibleTargets = new();

	public LayerMask targetMask;
	public LayerMask obstaclesMask;
	public float meshResolution;
	public int edgeResolvedIterations;
	public float edgeDistThreshold;
	public MeshFilter viewMeshFilter;
	Mesh viewMesh;


	void Start()
	{
		viewMesh = new Mesh()
		{
			name = "View Cast Mesh"
		};
		viewMeshFilter.mesh = viewMesh;

		StartCoroutine("FindTargetWithDelay", .2f);

	}
	void LateUpdate()
	{
		DrawFieldOfView();
	}

	IEnumerator FindTargetWithDelay(float delay)
	{
		while (true)
		{
			yield return new WaitForSeconds(delay);
			FindVisibleTarget();
		}
	}
	public Vector3 AngleToDir(float degrees, bool isGlobalAngle = false)
	{
		if (!isGlobalAngle)
		{
			degrees += transform.eulerAngles.y;
		}
		return new(Mathf.Sin(degrees * Mathf.Deg2Rad), 0, Mathf.Cos(degrees * Mathf.Deg2Rad));
	}
	void FindVisibleTarget()
	{
		visibleTargets.Clear();
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
		foreach (Collider target in targetsInViewRadius)
		{
			Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
			if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
			{
				float disToTarget = Vector3.Distance(transform.position, target.transform.position);
				if (!Physics.Raycast(transform.position, dirToTarget, disToTarget, obstaclesMask))
				{
					visibleTargets.Add(target.transform);
				}
			}
		}
	}

	void DrawFieldOfView()
	{

		// casting line
		int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
		float lineAngleSize = viewAngle / stepCount;
		List<Vector3> viewPoints = new();
		ViewPointInfo oldViewCast = new();

		for (int i = 0; i <= stepCount; i++)
		{
			float globalAngle = transform.eulerAngles.y - viewAngle / 2 + lineAngleSize * i;
			ViewPointInfo newViewCast = ViewCast(globalAngle);

			if (i > 0)
			{
				bool isEdgeDstThresholdExceed = Mathf.Abs(oldViewCast.dis - newViewCast.dis) > edgeDistThreshold;
				// if (oldViewCast.hit && newViewCast.hit)
				// {
				// 	Debug.Log("Find Edge ~ min view cast dis: " + oldViewCast.dis);
				// 	Debug.Log("Find Edge ~ max view cast dis: " + newViewCast.dis);
				// 	Debug.Log("isEdgeDstThresholdExceed" + isEdgeDstThresholdExceed);
				// 	Debug.Log("min max dis" + Mathf.Abs(oldViewCast.dis - newViewCast.dis));
				// 	Debug.Log("----------");
				// }
				if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && isEdgeDstThresholdExceed))
				{
					EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
					if (edge.pointA != Vector3.zero)
					{
						viewPoints.Add(edge.pointA);
						Debug.DrawLine(transform.position, edge.pointA, Color.green);
					}
					if (edge.pointB != Vector3.zero)
					{
						viewPoints.Add(edge.pointB);
						Debug.DrawLine(transform.position, edge.pointB, Color.green);
					}
				}
			}

			viewPoints.Add(newViewCast.point);
			Debug.DrawLine(transform.position, newViewCast.point, Color.red);
			oldViewCast = newViewCast;

		}

		// for (int i = 0; i < viewPoints.Count; i++)
		// {
		// 	Debug.DrawLine(transform.position, viewPoints[i], Color.red);
		// }

		// creating mesh
		int vertexCount = viewPoints.Count + 1;
		Vector3[] vertices = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount - 2) * 3];

		vertices[0] = Vector3.zero;
		for (int i = 0; i < vertexCount - 1; i++)
		{
			vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

			if (i < vertexCount - 2)
			{
				int originPnt = i * 3;
				triangles[originPnt] = 0;
				triangles[originPnt + 1] = i + 1;
				triangles[originPnt + 2] = i + 2;
			}
		}

		viewMesh.Clear();
		viewMesh.vertices = vertices;
		viewMesh.triangles = triangles;
		viewMesh.RecalculateNormals();
	}

	ViewPointInfo ViewCast(float globalAngle)
	{
		Vector3 dir = AngleToDir(globalAngle, true);
		RaycastHit hitInfo;
		bool hit = Physics.Raycast(transform.position, dir, out hitInfo, viewRadius, obstaclesMask);
		if (hit)
		{
			return new ViewPointInfo(
				_hit: hit, _angle: globalAngle,
				 _point: hitInfo.point, _dis: hitInfo.distance
			);
		}
		else
		{
			return new(_hit: hit, _angle: globalAngle,
				 _point: transform.position + dir * viewRadius, _dis: viewRadius);
		}
	}

	EdgeInfo FindEdge(ViewPointInfo minViewCast, ViewPointInfo maxViewCast)
	{
		float minAngle = minViewCast.angle;
		float maxAngle = maxViewCast.angle;
		Vector3 minPoint = Vector3.zero;
		Vector3 maxPoint = Vector3.zero;
		ViewPointInfo betweenViewCast;

		for (int i = 0; i < edgeResolvedIterations; i++)
		{
			float betweenAngle = (maxAngle + minAngle) / 2;
			betweenViewCast = ViewCast(betweenAngle);
			bool isEdgeDstThresholdExceed = Mathf.Abs(minViewCast.dis - maxViewCast.dis) > edgeDistThreshold;
			// Debug.Log("Find Edge ~ min view cast dis: " + minViewCast.dis);
			// Debug.Log("Find Edge ~ max view cast dis: " + maxViewCast.dis);
			// Debug.Log("isEdgeDstThresholdExceed" + isEdgeDstThresholdExceed);
			// Debug.Log("----------");
			if (betweenViewCast.hit == minViewCast.hit && !isEdgeDstThresholdExceed)
			{
				minAngle = betweenAngle;
				minPoint = betweenViewCast.point;
			}
			else
			{
				maxAngle = betweenAngle;
				maxPoint = betweenViewCast.point;
			}
		}

		return new(minPoint, maxPoint);
	}

	public struct ViewPointInfo
	{
		readonly public bool hit;
		readonly public float angle;
		readonly public Vector3 point;
		readonly public float dis;

		public ViewPointInfo(bool _hit, float _angle, Vector3 _point, float _dis)
		{
			hit = _hit;
			angle = _angle;
			point = _point;
			dis = _dis;
		}
	}
	public struct EdgeInfo
	{
		readonly public Vector3 pointA;
		readonly public Vector3 pointB;

		public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
		{
			pointA = _pointA;
			pointB = _pointB;
		}
	}
}
