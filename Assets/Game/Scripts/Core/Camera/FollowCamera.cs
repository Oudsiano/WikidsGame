using System;
using AINavigation;
using Core.Player;
using DG.Tweening;
using SceneManagement;
using SceneManagement.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Utils;

// Пространство имен для ядра игры
namespace Core.Camera
{
    // Класс для управления камерой, следующей за целью
    public class FollowCamera : MonoBehaviour
    {
        private const string MOUSE_X_NAME = "Mouse X";
        private const string MOUSE_Y_NAME = "Mouse Y";
        private const string MOUSE_SCROLLWHEEL_NAME = "Mouse ScrollWheel";

        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float zoomSpeed = 10f;

        [FormerlySerializedAs("minZoomDefault")] [SerializeField]
        private float _minZoomDefault = -500f;

        [FormerlySerializedAs("maxZoomDefault")] [SerializeField]
        private float _maxZoomDefault = -20f;

        [FormerlySerializedAs("trackPlayer")] [SerializeField]
        private bool _trackPlayer = true; // Флаг для отслеживания игрока

        [FormerlySerializedAs("obstacleMask")] [SerializeField]
        private LayerMask _obstacleLayerMask;

        private string _sceneName;
        private UnityEngine.Camera _mainCamera;
        private MainPlayer _player;
        private PlayerController _playerController;

        private Transform _target;
        private Transform _cameraTransform;

        private float[] _zoomLevels = { -20f, -35f, -100f, -200f, -300f, -400f, -500f, -600f };

        private float _zoomAmount;
        private float _minZoom;
        private float _maxZoom;
        private float _zoomTotal = -35;
        private float _camXRotation = 40;
        private float _camYRotation = 0;
        private float _step = 1f;

        private bool _canRotate = true;
        private bool _canZoom = true;
        private bool _commonZoomUpdate = false;
        private float _autoZoomForReturn;
        

        public void Construct(MainPlayer player, PlayerController playerController)
        {
            Debug.Log("FollowCamera constructed");

            _player = player;
            _playerController = playerController;
            _maxZoom = _maxZoomDefault;
            _minZoom = _minZoomDefault;

            _mainCamera = UnityEngine.Camera.main;
            _cameraTransform = transform;
            _target = _player.transform;
            _autoZoomForReturn = _zoomTotal;
            Rotate();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        // TODO static and rename
        public static event Action OnCameraRotation; // Для обучения
        public static event Action OnCameraScale; // Для обучения
        public static event Action<float> OnCameraDistance;
        public static event Action<Vector3> OnupdateEulerAngles;
        public static event Action<float> NewYRotation;
        public static event Action<float> NewXRotation;

        public float AutoZoomForReturn => _autoZoomForReturn;

        public bool CommonZoomUpdate => _commonZoomUpdate;

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
            if (_sceneName == Constants.Scenes.OpenScene ||
                _sceneName == Constants.Scenes.MapScene ||
                _sceneName == Constants.Scenes.BootstrapScene)
            {
                return;
            }

            if (PauseClass.GetPauseState())
                return;

            if (_zoomTotal < _minZoom)
            {
                MinZoom();
            }
            
            Vector3 targetPos = _target.position + new Vector3Int(0, 1, 0);
            Vector3 tempV1 = _target.position + (_mainCamera.transform.forward * ((_zoomTotal - _step * 2) * 0.2f));
            var direction = (targetPos - tempV1).normalized;
            RaycastHit hit;

            if (Physics.Raycast(tempV1, direction, out hit, Vector3.Distance(tempV1, targetPos) - 7, _obstacleLayerMask))
            {
                if (hit.transform.gameObject.TryGetComponent<MainPlayer>(out _))
                {
                    ZoomUpdateByZoomTotal(_step);
                }
            }
            else if (_zoomTotal > _autoZoomForReturn)
            {
                Vector3 tempV2 = _target.position + (_mainCamera.transform.forward * ((_zoomTotal - _step * 3) * 0.2f));
                if (!Physics.Raycast(tempV2, direction, out hit, Vector3.Distance(_mainCamera.transform.position, targetPos), _obstacleLayerMask))
                {
                    ZoomUpdateByZoomTotal(-_step);
                }
            }
        }

        public void MaxZoom()
        {
            float targetZoom = _maxZoom;
            for (int i = _zoomLevels.Length - 1; i >= 0; i--)
            {
                if (_zoomLevels[i] > _zoomTotal)
                {
                    targetZoom = _zoomLevels[i];
                    break;
                }
            }
            targetZoom = Mathf.Min(targetZoom, _maxZoom);

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
            targetZoom = Mathf.Max(targetZoom, _minZoom);

            DOTween.To(() => _zoomTotal, x =>
            {
                _zoomTotal = x;
                _autoZoomForReturn = _zoomTotal;
                _commonZoomUpdate = true;
            }, targetZoom, 0.5f);
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

                _sceneName = sceneComponent.SceneName;
            }
        }

        private void Follow()
        {
            if (PauseClass.GetPauseState())
                return;

            if (Input.GetKeyDown(KeyCode.LeftControl))
                _trackPlayer = !_trackPlayer;

            if (_trackPlayer)
            {
                // Если FollowCamera имеет родителя, переводим мировую позицию цели в локальные координаты родителя
                if (transform.parent != null)
                {
                    if (_target == null)
                    {
                        Debug.LogError("FollowCamera target is null");
                    }
                    
                    transform.localPosition = transform.parent.InverseTransformPoint(_target.position);
                }
                else
                {
                    transform.position = _target.position;
                }
            }

            if (_canRotate && Input.GetMouseButton(1))
            {
                Rotate();
                NewYRotation?.Invoke(_camYRotation);
            }

            if (_canZoom)
            {
                Zoom();
            }
        }

        private void Rotate()
        {
            float MX = Input.GetAxis(MOUSE_X_NAME);
            _camYRotation = transform.localEulerAngles.y + (MX * rotationSpeed * Time.deltaTime);
            _camXRotation += (Input.GetAxis(MOUSE_Y_NAME) * rotationSpeed * Time.deltaTime);
            _camXRotation = Mathf.Clamp(_camXRotation, 45, 90);

            Quaternion desiredWorldRotation = Quaternion.Euler(_camXRotation, _camYRotation, 0);
            if (transform.parent != null)
            {
                transform.localRotation = Quaternion.Inverse(transform.parent.rotation) * desiredWorldRotation;
            }
            else
            {
                transform.rotation = desiredWorldRotation;
            }
            
            OnupdateEulerAngles?.Invoke(transform.eulerAngles);
            OnCameraRotation?.Invoke();
            NewYRotation?.Invoke(_camYRotation);

            SetLocalPosition();
        }

        private void Zoom()
        {
            float scrollValue = Input.GetAxis(MOUSE_SCROLLWHEEL_NAME);
            
            if (scrollValue == 0)
            {
                return;
            }
            
            ZoomUpdate(scrollValue);
            _autoZoomForReturn = _zoomTotal;
        }

        private void ZoomUpdate(float value)
        {
            _zoomAmount = value * zoomSpeed * Time.deltaTime;
            _zoomTotal += _zoomAmount;
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
            _zoomTotal = Mathf.Clamp(_zoomTotal, _minZoom, _maxZoom);
            
            if (_mainCamera.transform.parent == transform)
            {
                _mainCamera.transform.localPosition = new Vector3(0f, 0f, _zoomTotal * 0.2f);
            }
            else
            {
                Vector3 desiredPos = _target.position + transform.forward * (_zoomTotal * 0.2f);
                _mainCamera.transform.position = desiredPos;
            }
            
            SetLocalPosition();

            OnupdateEulerAngles?.Invoke(transform.eulerAngles);
            OnCameraDistance?.Invoke(Mathf.Abs(_zoomTotal));
            OnCameraScale?.Invoke();
        }

        private void SetLocalPosition() // TODO magic numbers
        {
            if (_mainCamera.transform.parent == transform)
            {
                Vector3 pos = _mainCamera.transform.localPosition;
                pos.x = 0f;
                pos.y = 0f;
                _mainCamera.transform.localPosition = pos;
            }
        }
    }
}