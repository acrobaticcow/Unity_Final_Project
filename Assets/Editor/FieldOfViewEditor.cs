using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{

	void OnSceneGUI()
	{
		FieldOfView fow = (FieldOfView)target;
		Handles.color = Color.white;
		Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);
		Vector3 viewAngle01 = fow.AngleToDir(-fow.viewAngle / 2);
		Vector3 viewAngle02 = fow.AngleToDir(fow.viewAngle / 2);
		Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngle01 * fow.viewRadius);
		Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngle02 * fow.viewRadius);

		Handles.color = Color.red;
		foreach (var visibleTarget in fow.visibleTargets)
		{
			Handles.DrawLine(fow.transform.position, visibleTarget.position);
		}
	}

}

