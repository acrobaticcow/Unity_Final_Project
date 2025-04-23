using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField]
    Transform Target;

    [SerializeField]
    Camera Camera;

    [SerializeField]
    Vector3 Offset;

    public float smoothSpeed = 0.125f; // Expose in inspector for tuning

    // TODO smoothen and reduce jittering
    void LateUpdate()
    {
        Vector3 desiredPosition = Target.transform.position + Offset;
        Camera.transform.position = Vector3.Lerp(
            Camera.transform.position,
            desiredPosition,
            smoothSpeed
        );
    }
}
