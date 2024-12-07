using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DroneController : MonoBehaviour {
    public float speed = 5f;
    public float rotationSpeed = 100.0f;

    [SerializeField] private WaypointPathScript waypointPathScript;
    [SerializeField] private GameObject explosionPrefab;
    
    private Transform _centerPiece;
    private Collider _ownCollider;
    private ChildCollider[] _childColliderScripts;
    private bool _collisionEventNotTriggeredYet = true;
    private bool _sensorsOff;

    private void Start() {
        _ownCollider = transform.GetComponent<Collider>();
        _childColliderScripts = transform.GetComponentsInChildren<ChildCollider>();
        
        foreach (var childColliderScript in _childColliderScripts) {
            if (childColliderScript) {
                childColliderScript.OnChildCollision += HandleChildCollision;
            }
        }
        
        _centerPiece = transform.Find("Center");
        if (!_centerPiece) {
            Debug.LogError("Center piece not found in the drone's children objects.");
        }
    }

    private void Update() {
        ListenForMovementInput();
        ListenForRotationInput();
    }

    private void ListenForMovementInput() {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        transform.Translate(movement * speed * Time.deltaTime, Space.Self);
    }

    private void ListenForRotationInput() {
        
        if (Input.GetKey(KeyCode.E)) {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);
        }

        if (Input.GetKey(KeyCode.Q)) {
            transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0, Space.World);
        }

        if (Input.GetKey(KeyCode.S)) {
            speed = 0;
        }
    }

    private void HandleChildCollision(Collision collision) {
        Debug.Log($"Child collider triggered by {collision.gameObject.name}, activating main drone collider.");
        if (collision.gameObject.name.Equals("Character_C")) {
            Animator animator = collision.gameObject.GetComponent<Animator>();
            animator.SetBool("Collided", true);
        }
        if (collision.gameObject.name.Equals("Ramp") && _collisionEventNotTriggeredYet) {
            _collisionEventNotTriggeredYet = false;
            StartSlowDown();
        }
        if (_ownCollider) {
            if (!SceneManager.GetActiveScene().name.Equals("WorseScenario1")) return;
            Debug.Log("We collided. Disabling main collider and stopping movement!");
            waypointPathScript.Speed = 0;
            _ownCollider.enabled = false;
        }
    }
    
    // This method will start the slowdown process
    private IEnumerator SlowDownCoroutine(float duration) {
        float initialSpeed = waypointPathScript.Speed;
        float targetSpeed = 0f; // Target speed when the object comes to a halt

        // Duration over which to slow down
        float timeElapsed = 0f;

        // Gradually reduce speed over the specified duration
        while(timeElapsed < duration) {
            // Lerp between initial speed and target speed (0) over time
            waypointPathScript.Speed = Mathf.Lerp(initialSpeed, targetSpeed, timeElapsed / duration);
            timeElapsed += Time.deltaTime; // Increment time passed

            if (waypointPathScript.Speed < 2) {
                TurnOffSensors();
            }

            yield return null; // Wait until the next frame
        }

        // Ensure the speed reaches exactly 0 at the end
        waypointPathScript.Speed = targetSpeed;
    }

    private void TurnOffSensors() {
        if (_sensorsOff) return;
        _sensorsOff = true;
        LaserSensor[] sensors = GetComponentsInChildren<LaserSensor>();
        foreach (LaserSensor laserSensor in sensors) {
            laserSensor.TurnOff();
        } 
    }

    // Call this method where needed
    public void StartSlowDown() {
        if (SceneManager.GetActiveScene().name.Equals("Scenario2")) {
            StartCoroutine(SlowDownCoroutine(2f)); // Gradually slow down over 5 seconds
        }
    }


    private void PlayExplosion() {
        GameObject explosionInstance = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosionInstance, 4f);
    }
}