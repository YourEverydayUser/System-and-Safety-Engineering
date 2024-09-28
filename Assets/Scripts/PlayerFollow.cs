using UnityEngine;

public class FollowPlayer : MonoBehaviour {
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, -5);
    public float positionSmoothTime = 0.3f;
    public float rotationSmoothTime = 10f;

    private Vector3 _velocity = Vector3.zero;

    void LateUpdate() {
        Vector3 desiredPosition = target.TransformPoint(offset);
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, positionSmoothTime);

        Quaternion desiredRotation = target.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothTime * Time.deltaTime);
    }
}