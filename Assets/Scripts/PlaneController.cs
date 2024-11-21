using UnityEngine;

public class PlaneController : MonoBehaviour
{
    public float forwardSpeed = 10f; // Constant forward velocity
    public float rotationSpeed = 50f; // Rotation acceleration factor
    public float maxRotationSpeed = 100f; // Max rotation speed
    public float rotationDampening = 2f; // Damping factor to slow rotation over time

    private Vector3 rotationVelocity; // Tracks current rotation velocity (pitch, yaw, roll)

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Move the plane forward constantly
        transform.Translate(Vector3.left * forwardSpeed * Time.deltaTime);

        // Get input for rotation
        float pitchInput = Input.GetKey(KeyCode.W) ? 1f : Input.GetKey(KeyCode.S) ? -1f : 0f;
        float rollInput = Input.GetKey(KeyCode.Q) ? -1f : Input.GetKey(KeyCode.E) ? 1f : 0f;
        float yawInput = Input.GetKey(KeyCode.A) ? -1f : Input.GetKey(KeyCode.D) ? 1f : 0f;

        // Apply acceleration to rotation
        rotationVelocity.x += rollInput * rotationSpeed * Time.deltaTime; // Pitch
        rotationVelocity.y += yawInput * rotationSpeed * Time.deltaTime; // Pitch
        rotationVelocity.z += pitchInput * rotationSpeed * Time.deltaTime; // Yaw

        // Clamp rotation velocity to max rotation speed
        rotationVelocity = Vector3.ClampMagnitude(rotationVelocity, maxRotationSpeed);

        // Apply rotation to the plane
        transform.Rotate(rotationVelocity * Time.deltaTime);

        // Apply damping to rotation velocity
        rotationVelocity = Vector3.Lerp(rotationVelocity, Vector3.zero, rotationDampening * Time.deltaTime);
    }
}
