using UnityEngine;

public class FollowPlayer : MonoBehaviour {
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, -5);

    private void LateUpdate() {
        transform.position = target.position + offset;
        transform.LookAt(target);
    }
}