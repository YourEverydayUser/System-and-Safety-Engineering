using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class WaypointPathScript : MonoBehaviour {

    public float Speed { get; set; }
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float threshold = 0.1f;
    [SerializeField] private Vector3 startingPoint;

    private Dictionary<Vector3, Quaternion> _waypoints;

    private IEnumerator<KeyValuePair<Vector3, Quaternion>> _waypointEnumerator;
    private Vector3 _currentTarget;
    private Quaternion _currentTargetRotation;

    public class WaypointReachedEventArgs : EventArgs {
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public WaypointReachedEventArgs(Vector3 position, Quaternion rotation) {
            Position = position;
            Rotation = rotation;
        }
    }

    public event EventHandler<WaypointReachedEventArgs> WaypointReached;

    private void Start() {
        _waypoints = new Dictionary<Vector3, Quaternion>();
        _waypoints.Add(new Vector3(10, 0, 6), Quaternion.Euler(0, 0, 0));
        _waypoints.Add(new Vector3(5.2f, 0, 6), Quaternion.Euler(0, -90, 0));
        _waypoints.Add(new Vector3(5.2f, 0, 15), Quaternion.Euler(0, 0, 0));
        
        _waypointEnumerator = _waypoints.GetEnumerator();
        MoveToNextWaypoint();
    }

    private void Update() {
        if (_waypointEnumerator == null) return;
        if (Input.GetKeyDown(KeyCode.R)) {
            transform.position = startingPoint;
            Start();
        }
        
        MoveTowardsCurrentWaypoint();
    }

    private void OnCollisionEnter(Collision other) {
        Debug.Log($"Collided with {other.gameObject.name}");
        Speed = 0;
    }

    private void MoveTowardsCurrentWaypoint() {
        if (Vector3.Distance(transform.position, _currentTarget) > threshold) {
            transform.position = Vector3.MoveTowards(transform.position, _currentTarget, Speed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, _currentTargetRotation, rotationSpeed * Time.deltaTime);
        } else {
            OnWaypointReached(_currentTarget, _currentTargetRotation);
            MoveToNextWaypoint();
        }
    }

    private void MoveToNextWaypoint() {
        if (_waypointEnumerator.MoveNext()) {
            _currentTarget = _waypointEnumerator.Current.Key;
            _currentTargetRotation = _waypointEnumerator.Current.Value;
        } else {
            _waypointEnumerator = null;
        }
    }
    
    protected virtual void OnWaypointReached(Vector3 position, Quaternion rotation) {
        WaypointReached?.Invoke(this, new WaypointReachedEventArgs(position, rotation));
    }
}
