using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Player.MovingBetweenPoints
{
    public class PointViewContainer : MonoBehaviour
    {
        [SerializeField] private PointView _pointViewPrefab;
        [SerializeField] private GameObject _parent;

        private List<PointView> _spawnedButtons = new();
        private PointClickHandler _handler;

        public void Construct(PointView[] originalPointViews, PointClickHandler handler, UIManager uiManager)
        {
            _handler = handler;
            ClearButtons();

            foreach (var sourceView in originalPointViews)
            {
                var instance = Instantiate(_pointViewPrefab, _parent.transform);
                instance.name = "PointView_" + sourceView.name;
                
                var sourceRect = sourceView.GetComponent<RectTransform>();
                var instanceRect = instance.GetComponent<RectTransform>();

                instanceRect.anchoredPosition = sourceRect.anchoredPosition;
                instanceRect.sizeDelta = sourceRect.sizeDelta;
                instanceRect.localRotation = sourceRect.localRotation;

                // Передаём точку назначения
                instance.Construct(_handler, sourceView.TransformPoint);

                // Закрытие карты при клике
                var button = instance.GetComponent<Button>();
                
                button.onClick.AddListener(() =>
                {
                    _handler.HandleClick(instance.TransformPoint);
                    uiManager.OnClickBtnCloseMap();
                });

                _spawnedButtons.Add(instance);
            }
        }

        public void ClearButtons()
        {
            foreach (var view in _spawnedButtons)
            {
                Destroy(view.gameObject);
            }

            _spawnedButtons.Clear();
        }
    }
}