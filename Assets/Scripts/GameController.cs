using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    [SerializeField] private GameObject drone;
    [SerializeField] private GameObject fallingCrate;
    [SerializeField] private float explosionForce = 500;
    [SerializeField] private float explosionRadius = 4;
    
    [SerializeField] private List<Camera> cameras;
    private int _pointer;
    private WaypointPathScript _waypointPathScript;
    
    private void OnEnable() {
        _waypointPathScript = drone.transform.GetComponent<WaypointPathScript>();
        _waypointPathScript.WaypointReached += OnWaypointReached;
    }

    private void OnDisable() {
        _waypointPathScript.WaypointReached -= OnWaypointReached;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.S)) {
            bool movementStopped = _waypointPathScript.Speed == 0;
            _waypointPathScript.Speed = movementStopped ? 4 : 0;
        }

        if (Input.GetKeyDown(KeyCode.G)) {
            Debug.Log("We are applying a force");
            Vector3 explosionPosition = fallingCrate.transform.position + Vector3.left * 2;
            fallingCrate.transform.GetComponent<Rigidbody>().AddExplosionForce(
                explosionForce, 
                explosionPosition, 
                explosionRadius);
        }
        
        //CheckForManualCameraSelection();
    }

    private void CheckForManualCameraSelection() {
        if (Input.GetKeyDown(KeyCode.H)) {
            _pointer = _pointer < cameras.Count - 1 ? ++_pointer : 0;
            cameras.ForEach(cameraElement => cameraElement.enabled = false);
            cameras[_pointer].enabled = true;
        } else if (Input.GetKeyDown(KeyCode.G)) {
            _pointer = _pointer > 0 ? --_pointer : cameras.Count - 1;
            cameras.ForEach(cameraElement => cameraElement.enabled = false);
            cameras[_pointer].enabled = true;
        }
    }

    private void OnWaypointReached(object sender, WaypointPathScript.WaypointReachedEventArgs eventArgs) {
        Debug.Log("We reached a waypoint");
        if(SceneManager.GetActiveScene().name.Equals("BetterScenario1")) return;
        
        cameras[_pointer].enabled = false;
        _pointer++;
        cameras[_pointer].enabled = true;
    }
}
