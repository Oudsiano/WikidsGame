using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Player.MovingBetweenPoints
{
    public class PointViewContainer: MonoBehaviour
    {
        [SerializeField] private PointView _pointViewPrefab;
        [SerializeField] private GameObject _parent;
        
        private List<PointView> _spawnedButtons = new();
        private PointClickHandler _handler;
        
 public void Construct(PointView[] pointViews, PointClickHandler handler, UIManager uiManager)
        {
            _handler = handler;
            ClearButtons();

            for (int i = 0; i < pointViews.Length; i++)
            {
                var pointView = pointViews[i];
                var buttonGO = pointView.gameObject;

                // Сохраняем позицию, поворот и размеры до смены родителя
                var rectTransform = buttonGO.GetComponent<RectTransform>();
                var oldPosition = rectTransform.anchoredPosition;
                var oldSizeDelta = rectTransform.sizeDelta;
                var oldRotation = rectTransform.localRotation;

                // Перемещаем в новый родительский объект
                rectTransform.SetParent(transform, false);

                // Восстанавливаем параметры RectTransform после смены родителя
                rectTransform.anchoredPosition = oldPosition;
                rectTransform.sizeDelta = oldSizeDelta;
                rectTransform.localRotation = oldRotation;

                // ВАЖНО: Вставляем строго в том же порядке
                rectTransform.SetSiblingIndex(i);

                _spawnedButtons.Add(pointView);

                var button = buttonGO.GetComponent<Button>();
                var label = buttonGO.GetComponentInChildren<TMP_Text>();

                if (label != null)
                    label.text = pointView.name;

                var pointTransform = pointView.TransformPoint;

                button.onClick.AddListener(() =>
                {
                    _handler.HandleClick(pointTransform);
                    uiManager.OnClickBtnCloseMap();
                });
            }
        }

        private void ClearButtons()
        {
            foreach (var btn in _spawnedButtons)
            {
                btn.transform.SetParent(null); // Отключаем от контейнера перед удалением
                Destroy(btn.gameObject);
            }

            _spawnedButtons.Clear();
        }
    }
}