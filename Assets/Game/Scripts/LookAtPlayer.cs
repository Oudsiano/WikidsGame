using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    //private Transform _camera;

    // Update is called once per frame
    void Awake()
    {
        //_camera = Camera.main.transform;
        RPG.Core.FollowCamera.NewYRotation += newYrotate;
        RPG.Core.FollowCamera.NewXRotation += newXrotate;
    }

    float keepYRotate=220;
    float keepXRotate=60;

    private void newYrotate(float y) => keepYRotate = y;
    private void newXrotate(float x) => keepXRotate = x;
    

    void LateUpdate()
    {
        //Vector3 relativePos = _camera.position - transform.position;

        transform.eulerAngles = new Vector3(keepXRotate, keepYRotate, 0);
        // the second argument, upwards, defaults to Vector3.up
        //Quaternion rotation = Quaternion.LookRotation(relativePos, new Vector3(0, 1, 0));
        //transform.rotation = rotation * Quaternion.Euler(0, 0, 0);

        //transform.LookAt(_camera);
    }

    private void OnDestroy()
    {
        RPG.Core.FollowCamera.NewYRotation -= newYrotate;
        RPG.Core.FollowCamera.NewXRotation -= newXrotate;
    }
}
