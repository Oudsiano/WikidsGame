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
    }

    float keepYRotate;

    private void newYrotate(float obj) => keepYRotate = obj;
    

    void LateUpdate()
    {
        //Vector3 relativePos = _camera.position - transform.position;

        var rot_parenr = transform.parent.transform.localEulerAngles;

        transform.localEulerAngles = new Vector3(40- rot_parenr.x, keepYRotate - rot_parenr.y, 0);

        // the second argument, upwards, defaults to Vector3.up
        //Quaternion rotation = Quaternion.LookRotation(relativePos, new Vector3(0, 1, 0));
        //transform.rotation = rotation * Quaternion.Euler(0, 0, 0);

        //transform.LookAt(_camera);
    }

    private void OnDestroy()
    {
        RPG.Core.FollowCamera.NewYRotation -= newYrotate;
    }
}
