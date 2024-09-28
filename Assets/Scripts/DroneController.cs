using UnityEngine;

public class DroneController : MonoBehaviour {
    public float speed = 5.0f;
    public float rotationSpeed = 100.0f;

    private Transform _centerPiece;

    private void Start() {
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
    }
}