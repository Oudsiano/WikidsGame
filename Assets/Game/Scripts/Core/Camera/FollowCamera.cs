using System;
using AINavigation;
using Core.Player;
using DG.Tweening;
using SceneManagement;
using SceneManagement.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

// Пространство имен для ядра игры
namespace Core.Camera
{
    // Класс для управления камерой, следующей за целью
    public class FollowCamera : MonoBehaviour
    {
        private MainPlayer _player;
        private PlayerController _playerController;
        
        [SerializeField] private float rotationSpeed = 10f; // Скорость вращения камеры
        [SerializeField] private float zoomSpeed = 10f; // Скорость приближения/удаления камеры

        [FormerlySerializedAs("minZoomDefault")] [SerializeField]
        private float _minZoomDefault = -500f;

        [FormerlySerializedAs("maxZoomDefault")] [SerializeField]
        private float _maxZoomDefault = -20f;

        [FormerlySerializedAs("trackPlayer")] private bool _trackPlayer = true; // Флаг для отслеживания игрока
        [FormerlySerializedAs("mainCam")] private UnityEngine.Camera _mainCamera;

        private Transform _target;
        private Transform _defaultCameraTransform;

        private bool _canRotate = true;
        private bool _canZoom = true;
        private bool _commonZoomUpdate = false;

        private float _camXRotation = 40;
        private float _camYRotation = 0;

        private float _minZoom;
        private float _maxZoom;
        private float _zoomTotal = -35; // Общее изменение масштаба

        [FormerlySerializedAs("zoomLevels")] private float[] _zoomLevels = new float[]
            { -20f, -35f, -100f, -200f, -300f, -400f, -500f, -600f };

        private float _zoomAmount;
        [FormerlySerializedAs("obstacleMask")] private LayerMask _obstacleLayerMask;
        private float _autoZoomForReturn;

        private allScenes _sceneId = allScenes.emptyScene;
        
        // TODO static and rename
        public static event Action OnCameraRotation; // Для обучения
        public static event Action OnCameraScale; // Для обучения
        public static event Action<float> OnCameraDistance;
        public static event Action<Vector3> OnupdateEulerAngles;
        public static event Action<float> NewYRotation;
        public static event Action<float> NewXRotation;

        public float AutoZoomForReturn => _autoZoomForReturn;

        public bool CommonZoomUpdate => _commonZoomUpdate;
        
        public void Construct(MainPlayer player, PlayerController playerController)
        {
            Debug.Log("FollowCamera constructed");
            _player = player;
            _playerController = playerController;
            _maxZoom = _maxZoomDefault;
            _minZoom = _minZoomDefault;

            _mainCamera = UnityEngine.Camera.main; 
            _defaultCameraTransform = transform; 
            _target = _player.transform; 
            _autoZoomForReturn = _zoomTotal;
            Rotate();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void LateUpdate()
        {
            Follow();

            if (_commonZoomUpdate)
            {
                CommonZoom();
            }
        }
        
        private void Update() // TODO magic numbers
        {
            if (_sceneId == allScenes.emptyScene || _sceneId == allScenes.regionSCene)
            {
                return;
            }

            if (PauseClass.GetPauseState())
            {
                return;
            }

            if (_zoomTotal < _minZoom)
            {
                MinZoom();
            }

            /*float delta = Math.Abs(camYRotation - _playerController.transform.rotation.eulerAngles.y); // TODO not used code
            float deltaX = camYRotation - _playerController.transform.rotation.eulerAngles.y;
            float delta2 = Mathf.Sin(deltaX * Mathf.Deg2Rad);
            Debug.Log(camYRotation + " " + _playerController.transform.rotation.eulerAngles.y + " " + delta + " " + deltaX + " " + delta2);
            */

            float step = 1f; // TODO magic numbers
            Vector3 targetPos = _target.position + new Vector3Int(0, 1, 0);
            Vector3 tempV1 = _target.position + (_mainCamera.transform.forward * ((_zoomTotal - step * 2) * 0.2f));
            var direction = (targetPos - tempV1).normalized;
            RaycastHit hit;

            // Проверка, есть ли препятствие между камерой и целью
            if (Physics.Raycast(tempV1, direction, out hit, Vector3.Distance(tempV1, targetPos) - 7,
                    _obstacleLayerMask))
            {
                if (hit.transform.gameObject.name != "Player") // TODO findGO with name
                {
                    ZoomUpdateByZoomTotal(step);
                    //Debug.Log(hit.transform.gameObject.name);
                }
            }
            else if (_zoomTotal > AutoZoomForReturn)
            {
                Vector3 tempV2 = _target.position + (_mainCamera.transform.forward * ((_zoomTotal - step * 3) * 0.2f));
                if (Physics.Raycast(tempV2, direction, out hit,
                        Vector3.Distance(_mainCamera.transform.position, targetPos), _obstacleLayerMask) == false)
                {
                    ZoomUpdateByZoomTotal(-step);
                }
            }
        }

        public void MaxZoom()
        {
            float targetZoom = _maxZoom;

            for (int i = _zoomLevels.Length - 1; i >= 0; i--)
            {
                if (_zoomLevels[i] > _zoomTotal) // TODO sure:?
                {
                    targetZoom = _zoomLevels[i];
                    break;
                }
            }
            
            if (targetZoom > _maxZoom)
            {
                targetZoom = _maxZoom;
            }

            DOTween.To(() => _zoomTotal, x =>
            {
                _zoomTotal = x;
                _autoZoomForReturn = _zoomTotal;
                _commonZoomUpdate = true;
            }, targetZoom, 0.5f);
        }

        public void MinZoom()
        {
            float targetZoom = _minZoom;
            
            for (int i = 0; i < _zoomLevels.Length; i++)
            {
                if (_zoomLevels[i] < _zoomTotal)
                {
                    targetZoom = _zoomLevels[i];
                    break;
                }
            }

            if (targetZoom < _minZoom)
            {
                targetZoom = _minZoom;
            }

            DOTween.To(() => _zoomTotal, x =>
            {
                _zoomTotal = x;
                _autoZoomForReturn = _zoomTotal;
                _commonZoomUpdate = true;
            }, targetZoom, 0.5f); // TODO magic numbers
        }
        
        public void ActivateCommonZoomUpdate() => _commonZoomUpdate = true;
        public void DeactivateCommonZoomUpdate() => _commonZoomUpdate = false;

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            _minZoom = _minZoomDefault;
            _maxZoom = _maxZoomDefault;
            SceneComponent sceneComponent = FindObjectOfType<SceneComponent>(); // TODO change getcomp

            if (sceneComponent != null)
            {
                if (sceneComponent.newMinZoomCamera != 0) // TODO Change 
                {
                    _minZoom = sceneComponent.newMinZoomCamera;
                }

                if (sceneComponent.newMaxZoomCamera != 0)
                {
                    _maxZoom = sceneComponent.newMaxZoomCamera;
                }

                _sceneId = sceneComponent.IdScene;
            }
        }

        private void Follow()
        {
            if (PauseClass.GetPauseState())
            {
                return;
            }

            // Переключаем отслеживание игрока при нажатии клавиши LeftControl
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                _trackPlayer = !_trackPlayer;
            }

            if (_trackPlayer)
            {
                transform.position = _target.position;
            }

            float newCameraRY = 0;

            if (_canRotate && Input.GetMouseButton(1))
            {
                Rotate();
                newCameraRY = _playerController.transform.localEulerAngles.y; // TODO not used code
            }

            if (_canZoom)
            {
                Zoom();
            }
        }

        private void Rotate()
        {
            var MX = Input.GetAxis("Mouse X"); // TODO can be cached
            //if (MX == 0) return; // TODO not used code

            // Получаем значения вращения по осям X и Y
            _camYRotation = transform.localEulerAngles.y + (MX * rotationSpeed * Time.deltaTime);
            _camXRotation += (Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime); // TODO can be cached

            // Ограничиваем вращение по оси X
            _camXRotation = Mathf.Clamp(_camXRotation, 45, 90);

            transform.localEulerAngles = new Vector3(_camXRotation, _camYRotation, 0);
            OnupdateEulerAngles?.Invoke(transform.localEulerAngles);
            OnCameraRotation?.Invoke();
            NewYRotation?.Invoke(_camYRotation);

            SetLocalPosition();
            //NewXRotation?.Invoke(camXRotation); // TODO not used code
        }

        private void Zoom()
        {
            float scrollWheelValue = Input.GetAxis("Mouse ScrollWheel"); // TODO can be cached

            if (scrollWheelValue == 0)
            {
                return;
            }

            ZoomUpdate(scrollWheelValue);
            _autoZoomForReturn = _zoomTotal;
        }

        private void ZoomUpdate(float value)
        {
            // Получаем количество изменения масштаба
            _zoomAmount = value * zoomSpeed * Time.deltaTime;
            // Прибавляем это изменение к общему изменению масштаба
            _zoomTotal += _zoomAmount;
            // Ограничиваем общее изменение масштаба
            _zoomTotal = Mathf.Clamp(_zoomTotal, _minZoom, _maxZoom);

            _commonZoomUpdate = true;
        }

        private void ZoomUpdateByZoomTotal(float delta)
        {
            _zoomTotal += delta;
            _commonZoomUpdate = true;
        }

        private void CommonZoom()
        {
            _commonZoomUpdate = false;
            Vector3 newZoomPos; //= mainCam.transform.position + (mainCam.transform.forward * zoomAmt);
            if (_zoomTotal < _minZoom)
            {
                _zoomTotal = _minZoom;
            }

            if (_zoomTotal > _maxZoom)
            {
                _zoomTotal = _maxZoom;
            }

            // Масштабируем камеру, если она находится в пределах допустимого масштабирования
            newZoomPos = _target.position + (_mainCamera.transform.forward * (_zoomTotal * 0.2f)); // TODO magic numbers
            _mainCamera.transform.position = newZoomPos;

            SetLocalPosition();

            //Debug.Log(mainCam.transform.position);
            //Debug.Log(mainCam.transform.localPosition); // TODO not used code

            OnupdateEulerAngles?.Invoke(transform.localEulerAngles);
            OnCameraDistance?.Invoke(Math.Abs(_zoomTotal));

            //Debug.Log($"Zoom Total: {zoomTotal}"); // Отладочное сообщение для отслеживания zoomTotal // TODO not used code

            OnCameraScale?.Invoke();
        }

        private void SetLocalPosition() // TODO magic numbers
        {
            float delta = Math.Abs(_camYRotation - _playerController.transform.rotation.eulerAngles.y);
            float deltaX = _camYRotation - _playerController.transform.rotation.eulerAngles.y;
            float delta2 = Math.Abs(180 - delta);
            float delta2X = Mathf.Sin(deltaX * Mathf.Deg2Rad);
            float deltaCameraY = (delta2 - 90) / 90 * 0.04f * (Math.Abs(_zoomTotal) - 10);
            float deltaCameraX = -delta2X * 0.04f * (Math.Abs(_zoomTotal) - 10);
            Vector3 newZoomPos = _mainCamera.transform.localPosition;
            newZoomPos.y = deltaCameraY;
            newZoomPos.x = deltaCameraX;
            _mainCamera.transform.localPosition = newZoomPos;
        }
    }
}