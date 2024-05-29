using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SphereCollider))]
public class IconForFarCamera : MonoBehaviour
{

    [SerializeField] float startDistanceForShowIcon = 125;

    [TextArea(3, 10)]
    public string description;

    private SpriteRenderer thisImg;
    private Vector3 thisEulerAngles;

    private SpriteRenderer spriteRenderer;
    private bool isMouseOver = false;

    [SerializeField] bool isMovable = false;
    SphereCollider sphereCollider;

    private void Awake()
    {
        FollowCamera.OnCameraDistance += FollowCamera_OnCameraDistance;
        FollowCamera.NewYRotation += FollowCamera_NewYRotation;
        FollowCamera.NewXRotation += FollowCamera_NewXRotation;
        thisImg = GetComponent<SpriteRenderer>();
        thisImg.gameObject.SetActive(false);
        thisEulerAngles = thisImg.transform.eulerAngles;

        spriteRenderer = GetComponent<SpriteRenderer>();
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 1.5f;
    }
    void Update()
    {
        // Создаем луч от камеры к курсору мыши
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Проверяем, попадает ли луч в объект
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                if (!isMouseOver)
                    _OnMouseEnter();
                isMouseOver = true;
            }
            else
            {
                if (isMouseOver)
                {
                   // OnMouseExit();
                }
                isMouseOver = false;
            }
        }
        else
        {
            if (isMouseOver)
            {
                //OnMouseExit();
            }
            isMouseOver = false;
        }

        if (isMovable) UpdateData();
    }

    private void _OnMouseEnter()
    {
        Debug.Log(description);
    }
    /*private void OnMouseEnter()
    {
        Debug.Log("2");
    }*/

    private void UpdateData()
    {
        thisImg.gameObject.transform.eulerAngles = thisEulerAngles;
    }

    private void FollowCamera_NewXRotation(float obj)
    {
        thisEulerAngles.x = obj;
        UpdateData();
    }

    private void FollowCamera_NewYRotation(float obj)
    {
        thisEulerAngles.y = obj;
        UpdateData();
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
}
