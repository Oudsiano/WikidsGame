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

    [SerializeField] float scaleThis = 3f;

    private void Awake()
    {
        FollowCamera.OnCameraDistance += FollowCamera_OnCameraDistance;
        FollowCamera.NewYRotation += FollowCamera_NewYRotation;
        FollowCamera.NewXRotation += FollowCamera_NewXRotation;
        FollowCamera.OnupdateEulerAngles += FollowCamera_OnupdateEulerAngles;
        thisImg = GetComponent<SpriteRenderer>();
        thisImg.gameObject.SetActive(false);
        thisEulerAngles = thisImg.transform.eulerAngles;

        spriteRenderer = GetComponent<SpriteRenderer>();
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 0.5f;
    }

    private void FollowCamera_OnupdateEulerAngles(Vector3 obj)
    {
        thisEulerAngles.x = obj.x;
        thisEulerAngles.y = obj.y;
        UpdateData();
    }

    private void OnDestroy()
    {
        FollowCamera.OnCameraDistance -= FollowCamera_OnCameraDistance;
        FollowCamera.NewYRotation -= FollowCamera_NewYRotation;
        FollowCamera.NewXRotation -= FollowCamera_NewXRotation;
        FollowCamera.OnupdateEulerAngles -= FollowCamera_OnupdateEulerAngles;
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
                    _OnMouseExit();
                }
                isMouseOver = false;
            }
        }
        else
        {
            if (isMouseOver)
            {
                _OnMouseExit();
            }
            isMouseOver = false;
        }

        if (isMovable) UpdateData();
    }

    private void _OnMouseEnter()
    {
        IGame.Instance.UIManager.UpdateIconMapPanel(description);
    }
    private void _OnMouseExit()
    {
        IGame.Instance.UIManager.UpdateIconMapPanel("");
    }

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
        if (thisImg != null)
        {
            if (obj > startDistanceForShowIcon)
            {
                thisImg.gameObject.SetActive(true);
                Color newColor = thisImg.color;
                newColor.a = Mathf.Min(((obj - startDistanceForShowIcon) / 50f), 1);
                thisImg.color = newColor;

                float _scale = ((obj - startDistanceForShowIcon) / 100f + 1) * scaleThis / thisImg.sprite.bounds.size.magnitude;
                thisImg.transform.localScale = new Vector3(_scale, _scale, _scale);
            }
            else
            {
                _OnMouseExit();
                thisImg.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("Не найдена картинка, хотя ожидалась");
        }
    }
}
