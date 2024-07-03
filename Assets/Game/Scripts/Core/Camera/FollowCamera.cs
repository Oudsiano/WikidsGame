using System;
using UnityEngine;

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

        // Метод вызывается перед первым обновлением кадра
        void Start()
        {
            mainCam = Camera.main; // Получаем главную камеру
            defaultCameraTransform = transform; // Сохраняем начальное положение камеры
            target = MainPlayer.Instance.transform; // Получаем цель (обычно игрока)
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
        }

        public void ZoomUpdate(float MSW)
        {
            // Получаем количество изменения масштаба
            zoomAmt = MSW * zoomSpeed * Time.deltaTime;
            // Прибавляем это изменение к общему изменению масштаба
            zoomTotal += zoomAmt;
            // Ограничиваем общее изменение масштаба
            zoomTotal = Mathf.Clamp(zoomTotal, minZoom, maxZoom); // Ограничение zoomTotal не более 13

            // Получаем новую позицию для масштабирования
            Vector3 newZoomPos = mainCam.transform.position + (mainCam.transform.forward * zoomAmt);

            // Масштабируем камеру, если она находится в пределах допустимого масштабирования
            if (zoomTotal > minZoom && zoomTotal < maxZoom)
            {
                newZoomPos = target.position + (mainCam.transform.forward * (zoomTotal * 0.3f));
                mainCam.transform.position = newZoomPos;

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
            Vector3 targetPos = target.position + new Vector3Int(0, 1, 0);

            var direction = (targetPos - mainCam.transform.position).normalized;
            RaycastHit hit;

            //Debug.DrawRay(targetPos, direction * 200, Color.red);

            // Проверка, есть ли препятствие между камерой и целью
            if (Physics.Raycast(mainCam.transform.position, direction, out hit, Vector3.Distance(mainCam.transform.position, targetPos), obstacleMask))
            {
                if (hit.transform.gameObject.name != "Player")
                {
                    ZoomUpdate(0.2f);
                    //Debug.Log(hit.transform.gameObject.name);
                }
            }
        }
    }
}
