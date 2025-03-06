using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SavedRect
{
    // Присутствуют ли полезные данные в этом объекте
    [SerializeField] private bool _isInitialized = false; 

    // Поля для хранения данных из RectTransform
    [SerializeField] private Vector3 _anchoredPosition;
    [SerializeField] private Vector2 _sizeDelta;
    [SerializeField] private Vector2 _minAnchor;
    [SerializeField] private Vector2 _maxAnchor;
    [SerializeField] private Vector2 _pivot;
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private Vector3 _scale;

    /// <summary>
    /// Сохраняет данные из RectTransform в этот объект.
    /// </summary>
    /// <param name="rect"></param>
    public void SaveDataFromRectTransform(RectTransform rect)
    {
        if (rect == null) // null игнорируем 
            return;

        _isInitialized = true; // теперь полезные данные есть

        // сохраняем данные
        _anchoredPosition = rect.anchoredPosition3D;
        _sizeDelta = rect.sizeDelta;
        _minAnchor = rect.anchorMin;
        _maxAnchor = rect.anchorMax;
        _pivot = rect.pivot;
        _rotation = rect.localEulerAngles;
        _scale = rect.localScale;
    }

    /// <summary>
    /// Выгружает данные из этого объекта в RectTransform
    /// </summary>
    /// <param name="rect"></param>
    public void PutDataToRectTransform(RectTransform rect)
    {
        // игнорируем null или пустые данные
        if (rect == null || !_isInitialized) 
            return;

        // выгружаем данные
        rect.anchoredPosition3D = _anchoredPosition;
        rect.sizeDelta = _sizeDelta;
        rect.anchorMin = _minAnchor;
        rect.anchorMax = _maxAnchor;
        rect.pivot = _pivot;
        rect.localEulerAngles = _rotation;
        rect.localScale = _scale;
    }
}
