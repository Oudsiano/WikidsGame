using System;
using UnityEngine;

// Пространство имен для ядра игры
namespace RPG.Core
{
    // Класс для управления камерой, следующей за целью
    public class FollowCamera : MonoBehaviour
    {
        private Transform target; // Цель, за которой следует камера
        [SerializeField] private float rotationSpeed; // Скорость вращения камеры
        [SerializeField] private float zoomSpeed; // Скорость приближения/удаления камеры
        [SerializeField] private float speedRotationFollowPLayer = 3;
        public bool trackPlayer = true; // Флаг для отслеживания игрока

        // Ссылки на камеру и начальное положение камеры
        private Transform defaultCameraTransform;
        private Camera mainCam;

        // Флаги для определения возможности двигаться/масштабироваться/вращаться камере
        private bool canRotate;
        private bool canZoom;

        // Переменные для вращения камеры
        private float camXRotation = 0;
        private float camYRotation = 0;
        private float targetCamYRotationPLayer = 0;
        private float currentCamYRPLayer = 0;

        // Пределы масштабирования
        [SerializeField] private float minZoom;
        [SerializeField] private float maxZoom;
        private float zoomTotal; // Общее изменение масштаба
        private float zoomAmt; // Количество изменения масштаба

        // Метод вызывается перед первым обновлением кадра
        void Start()
        {
            mainCam = Camera.main; // Получаем главную камеру
            defaultCameraTransform = transform; // Сохраняем начальное положение камеры
            canRotate = true; // Устанавливаем возможность вращения
            canZoom = true; // Устанавливаем возможность масштабирования
            target = MainPlayer.Instance.transform; // Получаем цель (обычно игрока)
        }

        // Метод вызывается один раз за кадр, после Update
        void LateUpdate()
        {
            cameraMovement(); // Вызываем метод для управления камерой
        }

        private bool rotationDirectLeft(float rstart, float rtarget)
        {
            rstart = rstart % 360;
            rtarget = rtarget % 360;

            float delta = 0;

            if (rstart < 0) delta = Math.Abs(rstart); else delta = rstart;
            if (rtarget < 0) delta += Math.Abs(rtarget); else delta = rtarget;

            delta = delta % 360;


            if (delta > 180)
                return true;
            else return false;
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
            else
            {
                targetCamYRotationPLayer = IGame.Instance.playerController.transform.localEulerAngles.y;
                currentCamYRPLayer = transform.localEulerAngles.y;


                float deltaAngle = Mathf.DeltaAngle(currentCamYRPLayer, targetCamYRotationPLayer);




                if (Math.Abs(deltaAngle) <= speedRotationFollowPLayer)
                    newCameraRY = targetCamYRotationPLayer;
                else
                {
                    if (deltaAngle <= 0) newCameraRY = currentCamYRPLayer - speedRotationFollowPLayer;
                    else newCameraRY = currentCamYRPLayer + speedRotationFollowPLayer;
                }

                camYRotation = 0;
            }
            // Применяем вращение к камере
            transform.localEulerAngles = new Vector3(camXRotation, newCameraRY + camYRotation, 0);

            if (canZoom)
            {
                zoomMovement(); // Вызываем метод для масштабирования
            }
        }

        // Метод для вращения камеры
        private void RotationMovement()
        {
            // Получаем значения вращения по осям X и Y
            camYRotation += (Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime);
            camXRotation += (Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime);

            // Ограничиваем вращение по оси X
            camXRotation = Mathf.Clamp(camXRotation, -10, 10);
            // Ограничиваем вращение по оси Y
            camYRotation = Mathf.Clamp(camYRotation, -90, 90);
        }

        // Метод для масштабирования камеры
        private void zoomMovement()
        {
            // Получаем количество изменения масштаба
            zoomAmt = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
            // Прибавляем это изменение к общему изменению масштаба
            zoomTotal += zoomAmt;
            // Ограничиваем общее изменение масштаба
            zoomTotal = Mathf.Clamp(zoomTotal, minZoom, maxZoom);

            // Получаем новую позицию для масштабирования
            Vector3 newZoomPos = mainCam.transform.position + (mainCam.transform.forward * zoomAmt);

            // Масштабируем камеру, если она находится в пределах допустимого масштабирования
            if (zoomTotal > minZoom && zoomTotal < maxZoom)
                mainCam.transform.position = newZoomPos;
        }
    }
}
