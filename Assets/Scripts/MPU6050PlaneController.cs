using UnityEngine;
using System;
using System.IO.Ports;

public class MPU6050PlaneController : MonoBehaviour
{
    // Plane movement settings
    public float forwardSpeed = 10f; // Constant forward velocity
    //public float rotationSpeed = 0f; // Rotation acceleration factor
    public float rotationSpeed = 50f; // Rotation acceleration factor
    //public float maxRotationSpeed = 0f; // Max rotation speed
    public float maxRotationSpeed = 100f; // Max rotation speed
    //public float rotationDampening = 0f; // Damping factor to slow rotation over time
    public float rotationDampening = 2f; // Damping factor to slow rotation over time

    private Vector3 rotationVelocity; // Tracks current rotation velocity (pitch, yaw, roll)

    // Serial communication settings
    private SerialPort serialPort;

    // Sensor data
    private float accelX, accelY, accelZ;
    private float gyroX, gyroY, gyroZ;

    void Start()
    {
        // Initialize Serial Port
        try
        {
            serialPort = new SerialPort("COM3", 9600); // Replace "COM3" with your Arduino port
            serialPort.Open();
            Debug.Log("Serial port opened successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error opening serial port: {e.Message}");
        }
    }

    void Update()
    {
        // Read sensor data from the serial port
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string data = serialPort.ReadLine(); // Read data from Arduino
                string[] values = data.Split(',');

                if (values.Length == 6)
                {
                    // Parse the sensor data
                    accelX = float.Parse(values[0]);
                    accelY = float.Parse(values[1]);
                    accelZ = float.Parse(values[2]);
                    gyroX = float.Parse(values[3]);
                    gyroY = float.Parse(values[4]);
                    gyroZ = float.Parse(values[5]);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error reading serial data: {e.Message}");
            }
        }

        // Apply sensor data to control the plane
        UpdatePlaneMovement();
    }

    void UpdatePlaneMovement()
    {
        // Move the plane forward constantly
        transform.Translate(Vector3.left * forwardSpeed * Time.deltaTime);

        // Use gyroscope data for pitch, yaw, and roll
        rotationVelocity.x = gyroX * rotationSpeed * Time.deltaTime; // Pitch
        rotationVelocity.y = gyroY * rotationSpeed * Time.deltaTime; // Yaw
        rotationVelocity.z = gyroZ * rotationSpeed * Time.deltaTime; // Roll

        // Clamp rotation velocity to max rotation speed
        rotationVelocity = Vector3.ClampMagnitude(rotationVelocity, maxRotationSpeed);

        // Apply rotation to the plane
        transform.Rotate(rotationVelocity * Time.deltaTime);

        // Apply damping to rotation velocity
        rotationVelocity = Vector3.Lerp(rotationVelocity, Vector3.zero, rotationDampening * Time.deltaTime);
    }

    void OnApplicationQuit()
    {
        // Close the serial port on application exit
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
