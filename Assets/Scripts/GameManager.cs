using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MPU6050Controller mpuController;
    public PlaneController planeController;

    void Start()
    {
        if (mpuController == null || planeController == null)
        {
            Debug.LogError("Missing references to MPU6050Controller or PlaneController!");
        }
    }

    void Update()
    {
        // You can add global logic here if needed
    }
}