using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class IconForFarCamera : MonoBehaviour
{

    [SerializeField] float startDistanceForShowIcon = 125;
    private SpriteRenderer thisImg;
    private Vector3 thisEulerAngles;

    private void Awake()
    {
        FollowCamera.OnCameraDistance += FollowCamera_OnCameraDistance;
        FollowCamera.NewYRotation += FollowCamera_NewYRotation;
        FollowCamera.NewXRotation += FollowCamera_NewXRotation;
        thisImg = GetComponent<SpriteRenderer>();
        thisImg.gameObject.SetActive(false);
        thisEulerAngles = thisImg.transform.eulerAngles;

    }

    private void FollowCamera_NewXRotation(float obj)
    {
        thisEulerAngles.x = obj;
        thisImg.gameObject.transform.eulerAngles = thisEulerAngles;
    }

    private void FollowCamera_NewYRotation(float obj)
    {
        thisEulerAngles.y = obj;
        thisImg.gameObject.transform.eulerAngles = thisEulerAngles;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void FollowCamera_OnCameraDistance(float obj)
    {
        if (obj > startDistanceForShowIcon)
        {
            thisImg.gameObject.SetActive(true);
            Color newColor = thisImg.color;
            newColor.a = Mathf.Min(((obj - startDistanceForShowIcon) / 50f), 1);
            thisImg.color = newColor;

            float _scale = (obj - startDistanceForShowIcon) / 100f + 1;

            thisImg.transform.localScale = new Vector3(_scale, _scale, _scale);
        }
        else
        {
            thisImg.gameObject.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
