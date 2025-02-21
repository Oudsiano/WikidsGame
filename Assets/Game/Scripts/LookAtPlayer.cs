using Core.Camera;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private float _keepYRotate=220;
    private float _keepXRotate=60;
    
    private void Awake()
    {
        //_camera = Camera.main.transform;
        FollowCamera.NewYRotation += SetupYrotate;
        FollowCamera.NewXRotation += SetupXrotate;
    }
    
    private void LateUpdate()
    {
        //Vector3 relativePos = _camera.position - transform.position;

        transform.eulerAngles = new Vector3(_keepXRotate, _keepYRotate, 0);
        // the second argument, upwards, defaults to Vector3.up
        //Quaternion rotation = Quaternion.LookRotation(relativePos, new Vector3(0, 1, 0));
        //transform.rotation = rotation * Quaternion.Euler(0, 0, 0);

        //transform.LookAt(_camera);
    }

    private void OnDestroy()
    {
        FollowCamera.NewYRotation -= SetupYrotate;
        FollowCamera.NewXRotation -= SetupXrotate;
    }
    
    private void SetupYrotate(float y) => _keepYRotate = y;
    private void SetupXrotate(float x) => _keepXRotate = x;
}
