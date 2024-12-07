using UnityEngine;

namespace DefaultNamespace {

    public class LaserSensor : MonoBehaviour {
        public Transform target;           // The target to detect (e.g., the object the laser is aimed at)
        public float laserLength = 50f;    // Maximum length of the laser
        public float laserWidth = 0.1f;    // Width of the laser
        public Color laserColor = Color.red; // Color of the laser
    
        private LineRenderer lineRenderer;
        private Light spotLight;
        private bool _turnedOff;

        void Start() {
            // Get the LineRenderer component
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.startWidth = laserWidth;
            lineRenderer.endWidth = laserWidth;
            lineRenderer.positionCount = 2; // Two points: start and end of the laser
            lineRenderer.startColor = laserColor;
            lineRenderer.endColor = laserColor;

            // Set the material to make the laser glow (Optional, can use a shader with emissive properties)
            lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // You can assign a glowing material here

            // Get the Spot Light component
            spotLight = GetComponent<Light>();
            spotLight.type = LightType.Spot;
            spotLight.color = laserColor;
            spotLight.range = laserLength;
            spotLight.spotAngle = 10f; // Adjust the spread angle to match your desired laser width
        }

        void Update() {
            if(_turnedOff) return;
            
            // Update the line renderer to simulate the laser ray
            Vector3 laserDirection = (target.position - transform.position).normalized;
            Vector3 laserEndPosition = transform.position + laserDirection * laserLength;

            lineRenderer.SetPosition(0, transform.position);  // Start of the laser (emitter position)
            lineRenderer.SetPosition(1, laserEndPosition);    // End of the laser (target or max distance)

            // Optionally, update the Spot Light's position to match the laser
            spotLight.transform.position = transform.position;
            spotLight.transform.LookAt(target);
        }

        public void TurnOff() {
            _turnedOff = true;
            Destroy(lineRenderer);
            Destroy(spotLight);
        }
    }

}