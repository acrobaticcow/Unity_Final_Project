using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{

	[SerializeField]
	Transform Target;
	[SerializeField]
	Camera Camera;
	[SerializeField]
	Vector3 Offset;

	// TODO smoothen and reduce jittering
	void Update()
	{
		Camera.transform.position = Target.transform.position + Offset;
	}
}
