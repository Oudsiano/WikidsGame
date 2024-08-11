using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public bool ShouldRotate = true;

    public float AngleX = 50;
    public float StartDistance = 10f;
    public float MaxDistance = 15f;
    public float MinDistance = 5f;
    public float RotationSpeed = 50f;

    private float _angleY = 0;

    private float _targetDistance;
    private float _distance;

    private void Awake()
    {
        _targetDistance = _distance = StartDistance;
    }

    private void Update()
    {
        _targetDistance += Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * -100f;
        _targetDistance = Mathf.Clamp(_targetDistance, MinDistance, MaxDistance);
        _distance = Mathf.Lerp(_distance, _targetDistance, Time.deltaTime * 5f);

        if (Input.GetMouseButton(1))
        {
            AngleX += Input.GetAxis("Mouse Y") * -90 * Time.deltaTime;
            AngleX = Mathf.Clamp(AngleX, -85, 85);

            _angleY += Input.GetAxis("Mouse X") * 90 * Time.deltaTime;
        }
        else if (ShouldRotate)
            _angleY += RotationSpeed * Time.deltaTime;

        var direction = Quaternion.Euler(AngleX, _angleY, 0) * Vector3.forward;
        transform.forward = direction;
        transform.position = -direction * _distance;
    }
}
