using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
[DisallowMultipleComponent]
public class FOVVisualization : MonoBehaviour
{
	MeshRenderer MeshRenderer;
	public Transform FOVProxy;
	void Awake()
	{
		MeshRenderer = GetComponent<MeshRenderer>();
	}
	void Start()
	{
		MeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
	}

	void LateUpdate()
	{
		// transform.SetPositionAndRotation(FOVProxy.position, FOVProxy.rotation);
		transform.rotation = FOVProxy.rotation;
	}
}
