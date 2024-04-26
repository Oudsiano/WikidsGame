using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    //public Transform camera;
    // Update is called once per frame
    void Awake()
    {
        //camera = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
    }
}
