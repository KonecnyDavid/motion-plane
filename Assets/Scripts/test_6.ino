#include <Wire.h>

const int MPU_ADDR = 0x68; // I2C address of the MPU-6050. If AD0 pin is HIGH, the address is 0x69.

int16_t accelerometer_x, accelerometer_y, accelerometer_z; // Accelerometer raw data
int16_t gyro_x, gyro_y, gyro_z;                           // Gyroscope raw data
int16_t temperature;                                      // Temperature raw data

void setup() {
  Serial.begin(9600);        // Initialize Serial communication
  Wire.begin();              // Initialize I2C communication
  Wire.beginTransmission(MPU_ADDR); // Begin communication with MPU-6050
  Wire.write(0x6B);          // PWR_MGMT_1 register
  Wire.write(0);             // Set to zero (wake up the MPU-6050)
  Wire.endTransmission(true);
}

void loop() {
  // Request sensor data
  Wire.beginTransmission(MPU_ADDR);
  Wire.write(0x3B); // Start with ACCEL_XOUT_H register (0x3B)
  Wire.endTransmission(false); // Keep connection active
  Wire.requestFrom(MPU_ADDR, 14, true); // Request 14 bytes (accelerometer, temperature, gyroscope)

  // Read accelerometer data
  accelerometer_x = Wire.read() << 8 | Wire.read(); // ACCEL_XOUT_H and ACCEL_XOUT_L
  accelerometer_y = Wire.read() << 8 | Wire.read(); // ACCEL_YOUT_H and ACCEL_YOUT_L
  accelerometer_z = Wire.read() << 8 | Wire.read(); // ACCEL_ZOUT_H and ACCEL_ZOUT_L

  // Read temperature data
  temperature = Wire.read() << 8 | Wire.read(); // TEMP_OUT_H and TEMP_OUT_L

  // Read gyroscope data
  gyro_x = Wire.read() << 8 | Wire.read(); // GYRO_XOUT_H and GYRO_XOUT_L
  gyro_y = Wire.read() << 8 | Wire.read(); // GYRO_YOUT_H and GYRO_YOUT_L
  gyro_z = Wire.read() << 8 | Wire.read(); // GYRO_ZOUT_H and GYRO_ZOUT_L

  // Send data to Unity via Serial
  Serial.print(accelerometer_x); Serial.print(",");
  Serial.print(accelerometer_y); Serial.print(",");
  Serial.print(accelerometer_z); Serial.print(",");
  Serial.print(gyro_x); Serial.print(",");
  Serial.print(gyro_y); Serial.print(",");
  Serial.println(gyro_z);

  delay(5); // Adjust the delay to control data transmission speed
}