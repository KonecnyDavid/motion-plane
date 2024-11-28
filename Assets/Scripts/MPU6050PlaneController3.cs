using UnityEngine;
using System;
using System.IO.Ports;

public class MPU6050PlaneController3 : MonoBehaviour
{
    // Plane movement settings
    [Header("Plane Movement Settings")]
    public float forwardSpeed = 10f; // Constant forward velocity
    public float rotationSpeed = 50f; // Scaling factor for rotation
    public float maxRotationSpeed = 100f; // Maximum allowable rotation speed
    public float rotationDampening = 2f; // Damping factor to slow rotation over time

    // Input processing
    [Header("Input Processing")]
    public float inputScalingFactor = 1000f; // Divides sensor values for stabilization
    public float deadZoneThreshold = 2f; // Minimum value to register input (e.g., ignore noise)
    public float maxRotationAngle = 45f; // Maximum allowed rotation angle for better control

    private Vector3 rotationVelocity; // Current rotation velocity (pitch, yaw, roll)
    private Vector3 smoothedInput;    // Smoothed and scaled sensor input

    // Serial communication settings
    [Header("Serial Communication Settings")]
    public string portName = "COM3"; // Replace with your Arduino port
    public int baudRate = 9600;
    private SerialPort serialPort;

    // Sensor data
    private float accelX, accelY, accelZ;
    private float gyroX, gyroY, gyroZ;

    void Start()
    {
        InitializeSerialPort();
    }

    void Update()
    {
        ReadSensorData();
        UpdatePlaneMovement();
    }

    private void InitializeSerialPort()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.Open();
            Debug.Log($"Serial port {portName} opened successfully at {baudRate} baud.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error initializing serial port: {e.Message}");
        }
    }

    private void ReadSensorData()
    {
        if (serialPort == null || !serialPort.IsOpen)
        {
            Debug.LogWarning("Serial port is not open. Skipping sensor data read.");
            return;
        }

        try
        {
            string data = serialPort.ReadLine(); // Read data from Arduino
            string[] values = data.Split(',');

            if (values.Length == 6)
            {
                // Parse and scale sensor data
                accelX = float.Parse(values[0]) / inputScalingFactor;
                accelY = float.Parse(values[1]) / inputScalingFactor;
                accelZ = float.Parse(values[2]) / inputScalingFactor;
                gyroX = -1*float.Parse(values[5]) / inputScalingFactor;
                gyroY = float.Parse(values[4]) / inputScalingFactor;
                gyroZ = -1*float.Parse(values[3]) / inputScalingFactor;

                SmoothInput();
                ApplyDeadZone();

                DebugSensorData(); // Optional: Debug log sensor values
            }
            else
            {
                Debug.LogWarning($"Unexpected data format: {data}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error reading serial data: {e.Message}");
        }
    }

    private void SmoothInput()
    {
        smoothedInput.x = Mathf.Lerp(smoothedInput.x, gyroX, 0.1f); // Adjust smoothing factor as needed
        smoothedInput.y = Mathf.Lerp(smoothedInput.y, gyroY, 0.1f);
        smoothedInput.z = Mathf.Lerp(smoothedInput.z, gyroZ, 0.1f);
    }

    private void ApplyDeadZone()
    {
        if (Mathf.Abs(smoothedInput.x) < deadZoneThreshold) smoothedInput.x = 0;
        if (Mathf.Abs(smoothedInput.y) < deadZoneThreshold) smoothedInput.y = 0;
        if (Mathf.Abs(smoothedInput.z) < deadZoneThreshold) smoothedInput.z = 0;
    }

    private void UpdatePlaneMovement()
    {
        // Move the plane forward constantly
        transform.Translate(Vector3.left * forwardSpeed * Time.deltaTime);

        // Map smoothed and scaled gyroscope data to rotation
        rotationVelocity.x = Mathf.Clamp(smoothedInput.x * rotationSpeed, -maxRotationAngle, maxRotationAngle); // Pitch
        rotationVelocity.y = Mathf.Clamp(smoothedInput.y * rotationSpeed, -maxRotationAngle, maxRotationAngle); // Yaw
        rotationVelocity.z = Mathf.Clamp(smoothedInput.z * rotationSpeed, -maxRotationAngle, maxRotationAngle); // Roll

        // Apply rotation to the plane
        transform.Rotate(rotationVelocity * Time.deltaTime);

        // Apply damping to rotation
        rotationVelocity = Vector3.Lerp(rotationVelocity, Vector3.zero, rotationDampening * Time.deltaTime);
    }

    private void DebugSensorData()
    {
        Debug.Log($"Accel: ({accelX:F3}, {accelY:F3}, {accelZ:F3}) | Gyro: ({gyroX:F3}, {gyroY:F3}, {gyroZ:F3})");
    }

    private void CloseSerialPort()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial port closed.");
        }
    }

    void OnApplicationQuit()
    {
        CloseSerialPort();
    }
}
