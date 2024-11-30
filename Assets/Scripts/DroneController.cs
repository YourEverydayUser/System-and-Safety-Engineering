using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour {
    public float speed = 5.0f;
    public float rotationSpeed = 100.0f;

    [SerializeField] private WaypointPathScript waypointPathScript;
    [SerializeField] private GameObject explosionPrefab;
    
    private Transform _centerPiece;
    private Collider _ownCollider;
    private ChildCollider[] _childColliderScripts;

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
        if (_ownCollider) {
            //PlayExplosion();
            Debug.Log("We collided. Disabling main collider and stopping movement!");
            waypointPathScript.Speed = 0;
            _ownCollider.enabled = false;
        }
    }

    private void PlayExplosion() {
        GameObject explosionInstance = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosionInstance, 4f);
    }
}