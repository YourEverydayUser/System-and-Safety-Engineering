using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

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

    public class NextWaypoint : EventArgs {
        public Vector3 Position { get; }

        public NextWaypoint(Vector3 position) {
            Position = position;
        }
    }

    public event EventHandler<WaypointReachedEventArgs> WaypointReached;
    public event EventHandler<NextWaypoint> NextWaypointEvent; 

    private void Start() {
        _waypoints = new Dictionary<Vector3, Quaternion>();
        //_waypoints.Add(new Vector3(20,0,6), Quaternion.Euler(0, 90, 0));
        
        //waypoints for scenario1
        // _waypoints.Add(new Vector3(10, 0, 6), Quaternion.Euler(0, -90, 0));
        // _waypoints.Add(new Vector3(5.2f, 0, 6), Quaternion.Euler(0, -90, 0));
        // _waypoints.Add(new Vector3(5.2f, 0, 15), Quaternion.Euler(0, 0, 0));
        
        //waypoints for scenario2 bad
        _waypoints.Add(new Vector3(6, 0, 6), Quaternion.Euler(0, 90, 0));
        _waypoints.Add(new Vector3(8.2f, 0, 2), Quaternion.Euler(0, 120, 0));
        _waypoints.Add(new Vector3(6.3f, 0, 5), Quaternion.Euler(0, -80, 0));
        _waypoints.Add(new Vector3(5.2f, 0, 7), Quaternion.Euler(0, 160, 0));
        _waypoints.Add(new Vector3(5.6f, 0, 4), Quaternion.Euler(0, -20, 0));
        _waypoints.Add(new Vector3(7f, 0, 6), Quaternion.Euler(0, 180, 0));
        
        AddRandomPoints();
        AddRandomPoints();
        AddRandomPoints();
        AddRandomPoints();
        
        _waypointEnumerator = _waypoints.GetEnumerator();
        MoveToNextWaypoint();
    }

    private void AddRandomPoints() {
        Dictionary<Vector3, Quaternion> newWaypoints = new Dictionary<Vector3, Quaternion>();
        foreach (Vector3 waypoint in _waypoints.Keys) {
            newWaypoints.TryAdd(waypoint + new Vector3(Random.Range(0, 10), 0, Random.Range(0, 7)),
                Quaternion.Euler(0, Random.Range(0, 360), 0));
        }
        foreach (var keyValuePair in newWaypoints) {
            _waypoints.TryAdd(keyValuePair.Key, keyValuePair.Value);
        }
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
    }

    private void MoveTowardsCurrentWaypoint() {
        if (Vector3.Distance(transform.position, _currentTarget) > threshold) {
            transform.position = Vector3.MoveTowards(transform.position, _currentTarget, Speed * Time.deltaTime);
            //transform.rotation = Quaternion.Slerp(transform.rotation, _currentTargetRotation, rotationSpeed * Time.deltaTime);
        } else {
            OnWaypointReached(_currentTarget, _currentTargetRotation);
            MoveToNextWaypoint();
        }
    }

    private void MoveToNextWaypoint() {
        if (_waypointEnumerator.MoveNext()) {
            _currentTarget = _waypointEnumerator.Current.Key;
            _currentTargetRotation = _waypointEnumerator.Current.Value;
            NextWaypointEvent?.Invoke(this, new NextWaypoint(_currentTarget));
        } else {
            _waypointEnumerator = null;
        }
    }
    
    protected virtual void OnWaypointReached(Vector3 position, Quaternion rotation) {
        WaypointReached?.Invoke(this, new WaypointReachedEventArgs(position, rotation));
        Speed = 0f;
    }
}
