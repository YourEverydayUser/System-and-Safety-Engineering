using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace {
    public class HumanMovement : MonoBehaviour {
        public Transform pointA;
        public Transform pointB;
        public float speed = 2f;
        public float rotationSpeed = 2f;

        private Transform _target;
        private Animator _animator;

        private void Start() {
            _target = pointB;
            _animator = GetComponent<Animator>();
            _animator.SetBool("IsWalking", true);
            _animator.SetBool("IsWalkingBackwards", false);
        }

        private void Update() {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _target.position, step);

            if (Vector3.Distance(transform.position, _target.position) < 0.1f) {
                bool isMovingBack = _target == pointA;
                _target = isMovingBack ? pointB : pointA;
                StartCoroutine(Rotate(_target)); // Start rotation coroutine
                _animator.SetBool("IsWalking", !isMovingBack);
                _animator.SetBool("IsWalkingBackwards", isMovingBack);
            }
        }

        private IEnumerator Rotate(Transform newTarget) {
            Vector3 targetDirection = (newTarget.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            
            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f) {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                yield return null;
            }

            transform.rotation = targetRotation;
        }
    }
}