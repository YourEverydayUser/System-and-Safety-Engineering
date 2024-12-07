using System.Collections;
using UnityEngine;

public class SensorController : MonoBehaviour {
    private DroneController _droneController;
    public float lerpDuration = 1f;              // Duration to lerp to the new position
    private bool isLerping = false;              // To avoid multiple coroutines running at once

    void Start() {
        // Subscribe to the WaypointReached event
        GetComponentInParent<WaypointPathScript>().NextWaypointEvent += OnWaypointReached;
        _droneController = GetComponentInParent<DroneController>();
    }

    // Triggered when the waypoint is reached
    private void OnWaypointReached(object sender, WaypointPathScript.NextWaypoint args) {
        if (!isLerping) {
            StartCoroutine(LerpToNewPositionAndRotation(args.Position));
        }
    }

    // Coroutine to smoothly move the controller to the new position and align rotation
    private IEnumerator LerpToNewPositionAndRotation(Vector3 targetPosition) {
        isLerping = true;

        // Record the starting position and rotation
        Vector3 startPosition = transform.position;
        Quaternion startRotation = _droneController.transform.rotation;

        // Calculate the target rotation to face the new position
        Vector3 directionToTarget = (targetPosition - _droneController.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration) {
            // Interpolate position
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / lerpDuration);

            // Interpolate rotation
            _droneController.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / lerpDuration);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        // Ensure the final position and rotation are set correctly
        transform.position = targetPosition;
        _droneController.transform.rotation = targetRotation;

        isLerping = false;
    }
}
