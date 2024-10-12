using UnityEngine;

public class ChildCollider : MonoBehaviour {
    public delegate void CollisionHandler(Collision collision);
    public event CollisionHandler OnChildCollision;

    private void OnCollisionEnter(Collision collision) {
        Debug.Log($"Child collided with {collision.gameObject.name}");
        OnChildCollision?.Invoke(collision); 
    }
}