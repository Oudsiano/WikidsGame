using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Пространство имен для ядра игры
namespace RPG.Core
{
    // Класс для управления камерой, следующей за целью
    public class FollowCamera : MonoBehaviour
    {
        public static event Action OnCameraRotation; // Для обучения
        public static event Action OnCameraScale; // Для обучения
        public static event Action<float> OnCameraDistance;
        public static event Action<Vector3> OnupdateEulerAngles;
        public static event Action<float> NewYRotation;
        public static event Action<float> NewXRotation;

        private Transform target; // Цель, за которой следует камера
        [SerializeField] private float rotationSpeed = 10f; // Скорость вращения камеры
        [SerializeField] private float zoomSpeed = 10f; // Скорость приближения/удаления камеры
        public bool trackPlayer = true; // Флаг для отслеживания игрока

        // Ссылки на камеру и начальное положение камеры
        private Transform defaultCameraTransform;
        private Camera mainCam;

        // Флаги для определения возможности двигаться/масштабироваться/вращаться камере
        private bool canRotate = true;
        private bool canZoom = true;

        // Переменные для вращения камеры
        private float camXRotation = 40;
        private float camYRotation = 0;

        // Пределы масштабирования
        [SerializeField] private float minZoom = 5f;
        [SerializeField] private float maxZoom = 20f;
        private float zoomTotal = -35; // Общее изменение масштаба

        private float zoomAmt; // Количество изменения масштаба

        public LayerMask obstacleMask; // Слой, который обозначает препятствия
        private float autoZoomForReturn;

        public float AutoZoomForReturn
        {
            get => autoZoomForReturn;
            set
            {
                autoZoomForReturn = value;
                Debug.Log(autoZoomForReturn);
            }
        }

        // Метод вызывается перед первым обновлением кадра
        void Start()
        {
            mainCam = Camera.main; // Получаем главную камеру
            defaultCameraTransform = transform; // Сохраняем начальное положение камеры
            target = MainPlayer.Instance.transform; // Получаем цель (обычно игрока)
            AutoZoomForReturn = zoomTotal;
            RotationMovement();
        }

        // Метод вызывается один раз за кадр, после Update
        void LateUpdate()
        {
            cameraMovement(); // Вызываем метод для управления камерой
        }

        // Метод для управления камерой
        private void cameraMovement()
        {
            // Переключаем отслеживание игрока при нажатии клавиши LeftControl
            if (Input.GetKeyDown(KeyCode.LeftControl))
                trackPlayer = !trackPlayer;

            if (trackPlayer)
                transform.position = target.position; // Позиционируем камеру на цели

            float newCameraRY = 0;
            // Обрабатываем вращение камеры при нажатии правой кнопки мыши
            if (canRotate && Input.GetMouseButton(1))
            {
                RotationMovement(); // Вызываем метод для вращения
                newCameraRY = IGame.Instance.playerController.transform.localEulerAngles.y;
            }

            if (canZoom)
            {
                zoomMovement(); // Вызываем метод для масштабирования
            }
        }

        // Метод для вращения камеры
        private void RotationMovement()
        {
            var MX = Input.GetAxis("Mouse X");
            //if (MX == 0) return;

            // Получаем значения вращения по осям X и Y
            camYRotation = transform.localEulerAngles.y + (MX * rotationSpeed * Time.deltaTime);
            camXRotation += (Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime);

            // Ограничиваем вращение по оси X
            camXRotation = Mathf.Clamp(camXRotation, 45, 90);

            transform.localEulerAngles = new Vector3(camXRotation, camYRotation, 0);
            OnupdateEulerAngles?.Invoke(transform.localEulerAngles);
            OnCameraRotation?.Invoke();
            NewYRotation?.Invoke(camYRotation);
            //NewXRotation?.Invoke(camXRotation);
        }

        // Метод для масштабирования камеры
        private void zoomMovement()
        {
            var MSW = Input.GetAxis("Mouse ScrollWheel");

            if (MSW == 0) return;

            ZoomUpdate(MSW);
            AutoZoomForReturn = zoomTotal;
        }

        public void ZoomUpdate(float MSW)
        {
            // Получаем количество изменения масштаба
            zoomAmt = MSW * zoomSpeed * Time.deltaTime;
            // Прибавляем это изменение к общему изменению масштаба
            zoomTotal += zoomAmt;
            // Ограничиваем общее изменение масштаба
            zoomTotal = Mathf.Clamp(zoomTotal, minZoom, maxZoom);

            // Получаем новую позицию для масштабирования
            CommonZoom();
        }

        private void ZoomUpdateByZoomTotal(float delta)
        {
            zoomTotal += delta;
            CommonZoom();
        }

        private void CommonZoom()
        {
            Vector3 newZoomPos;//= mainCam.transform.position + (mainCam.transform.forward * zoomAmt);
            if (zoomTotal < minZoom) zoomTotal = minZoom;
            if (zoomTotal > maxZoom) zoomTotal = maxZoom;
            // Масштабируем камеру, если она находится в пределах допустимого масштабирования
            {
                newZoomPos = target.position + (mainCam.transform.forward * (zoomTotal * 0.3f));
                mainCam.transform.position = newZoomPos;
                //Debug.Log(mainCam.transform.position);

                OnupdateEulerAngles?.Invoke(transform.localEulerAngles);
                OnCameraDistance?.Invoke(Math.Abs(zoomTotal));

                //Debug.Log($"Zoom Total: {zoomTotal}"); // Отладочное сообщение для отслеживания zoomTotal
            }

            OnCameraScale?.Invoke();
        }

        public void LockCamera()
        {
            canRotate = false;
            canZoom = false;
        }

        public void UnlockCamera()
        {
            canRotate = true;
            canZoom = true;
        }

        void Update()
        {
            if (pauseClass.GetPauseState()) return;

            float step = 1f;
            Vector3 targetPos = target.position + new Vector3Int(0, 1, 0);
            Vector3 tempV1 = target.position + (mainCam.transform.forward * ((zoomTotal - step * 2) * 0.3f));
            var direction = (targetPos - tempV1).normalized;
            RaycastHit hit;

            // Проверка, есть ли препятствие между камерой и целью
            if (Physics.Raycast(tempV1, direction, out hit, Vector3.Distance(tempV1, targetPos), obstacleMask))
            {
                if (hit.transform.gameObject.name != "Player")
                {
                    ZoomUpdateByZoomTotal(step);
                    //Debug.Log(hit.transform.gameObject.name);
                }
            }
            else if (zoomTotal > AutoZoomForReturn)
            {
                Vector3 tempV2 = target.position + (mainCam.transform.forward * ((zoomTotal - step * 3) * 0.3f));
                if (!(Physics.Raycast(tempV2, direction, out hit, Vector3.Distance(mainCam.transform.position, targetPos), obstacleMask)))
                {
                    ZoomUpdateByZoomTotal(-step);
                }
            }
        }

        public void MaxZoom()
        {
            zoomTotal = maxZoom;
            CommonZoom();
            AutoZoomForReturn = zoomTotal;
        }

        public void MinZoom()
        {
            zoomTotal = minZoom;
            CommonZoom();
            AutoZoomForReturn = zoomTotal;
        }
    }
}
