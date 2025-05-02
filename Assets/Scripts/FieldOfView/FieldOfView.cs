using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Range(0, 360)]
    public float ViewAngle;
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

    [Header("Circle Mesh")]
    public int segments = 100;
    public float radius = 1f;
    public MeshFilter circleMeshFilter;
    Mesh circleMesh;

    void Start()
    {
        viewMesh = new() { name = "View Cast Mesh" };
        viewMeshFilter.mesh = viewMesh;

        circleMesh = new() { name = "Circle Mesh" };
        circleMeshFilter.mesh = circleMesh;
        DrawCircleMesh();

        StartCoroutine(FindTargetWithDelay(0.2f));
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
        Collider[] targetsInViewRadius = Physics.OverlapSphere(
            transform.position,
            viewRadius,
            targetMask
        );
        foreach (Collider target in targetsInViewRadius)
        {
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < ViewAngle / 2)
            {
                float disToTarget = Vector3.Distance(transform.position, target.transform.position);
                if (!Physics.Raycast(transform.position, dirToTarget, disToTarget, obstaclesMask))
                {
                    visibleTargets.Add(target.transform);
                }
            }
        }
    }

    void DrawCircleMesh()
    {
        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;
        float angleStep = 360f / segments;

        for (int i = 1; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * angleStep * i;
            vertices[i] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
        }

        for (int i = 0; i < segments; i++)
        {
            int current = i + 1;
            int next = (i + 1) % segments + 1;

            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = next;
            triangles[i * 3 + 2] = current;
        }

        circleMesh.vertices = vertices;
        circleMesh.triangles = triangles;
        circleMesh.RecalculateNormals();
    }

    void DrawFieldOfView()
    {
        // casting line
        int stepCount = Mathf.RoundToInt(ViewAngle * meshResolution);
        float lineAngleSize = ViewAngle / stepCount;
        List<Vector3> viewPoints = new();
        ViewPointInfo oldViewCast = new();

        for (int i = 0; i <= stepCount; i++)
        {
            float globalAngle = transform.eulerAngles.y - ViewAngle / 2 + lineAngleSize * i;
            ViewPointInfo newViewCast = ViewCast(globalAngle);

            if (i > 0)
            {
                bool isEdgeDstThresholdExceed =
                    Mathf.Abs(oldViewCast.dis - newViewCast.dis) > edgeDistThreshold;
                // if (oldViewCast.hit && newViewCast.hit)
                // {
                // 	Debug.Log("Find Edge ~ min view cast dis: " + oldViewCast.dis);
                // 	Debug.Log("Find Edge ~ max view cast dis: " + newViewCast.dis);
                // 	Debug.Log("isEdgeDstThresholdExceed" + isEdgeDstThresholdExceed);
                // 	Debug.Log("min max dis" + Mathf.Abs(oldViewCast.dis - newViewCast.dis));
                // 	Debug.Log("----------");
                // }
                if (
                    oldViewCast.hit != newViewCast.hit
                    || (oldViewCast.hit && newViewCast.hit && isEdgeDstThresholdExceed)
                )
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                        // Debug.DrawLine(transform.position, edge.pointA, Color.green);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                        // Debug.DrawLine(transform.position, edge.pointB, Color.green);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            // Debug.DrawLine(transform.position, newViewCast.point, Color.red);
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
        bool hit = Physics.Raycast(
            transform.position,
            dir,
            out RaycastHit hitInfo,
            viewRadius,
            obstaclesMask
        );
        if (hit)
        {
            return new ViewPointInfo(
                _hit: hit,
                _angle: globalAngle,
                _point: hitInfo.point,
                _dis: hitInfo.distance
            );
        }
        else
        {
            return new(
                _hit: hit,
                _angle: globalAngle,
                _point: transform.position + dir * viewRadius,
                _dis: viewRadius
            );
        }
    }

    EdgeInfo FindEdge(ViewPointInfo minViewCast, ViewPointInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolvedIterations; i++)
        {
            float angle = (maxAngle + minAngle) / 2;
            ViewPointInfo newViewCast = ViewCast(angle);
            bool isEdgeDstThresholdExceed =
                Mathf.Abs(minViewCast.dis - newViewCast.dis) > edgeDistThreshold;
            // Debug.Log("Find Edge ~ min view cast dis: " + minViewCast.dis);
            // Debug.Log("Find Edge ~ max view cast dis: " + maxViewCast.dis);
            // Debug.Log("isEdgeDstThresholdExceed" + isEdgeDstThresholdExceed);
            // Debug.Log("----------");
            if (newViewCast.hit == minViewCast.hit && !isEdgeDstThresholdExceed)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new(minPoint, maxPoint);
    }

    public struct ViewPointInfo
    {
        public readonly bool hit;
        public readonly float angle;
        public readonly Vector3 point;
        public readonly float dis;

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
        public readonly Vector3 pointA;
        public readonly Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
