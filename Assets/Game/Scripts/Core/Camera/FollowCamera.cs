using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LevelChangeObserver;

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
        public Camera mainCam;

        // Флаги для определения возможности двигаться/масштабироваться/вращаться камере
        private bool canRotate = true;
        private bool canZoom = true;
        private bool commonZoomUpdata = false;

        // Переменные для вращения камеры
        private float camXRotation = 40;
        private float camYRotation = 0;

        // Пределы масштабирования
        [SerializeField] private float minZoomDefault = -500f;
        [SerializeField] private float maxZoomDefault = -20f;
        private float minZoom;
        private float maxZoom;
        private float zoomTotal = -35; // Общее изменение масштаба
        public float[] zoomLevels = new float[] { -20f, -35f, -100f, -200f, -300f, -400f, -500f, -600f };


        private float zoomAmt; // Количество изменения масштаба

        public LayerMask obstacleMask; // Слой, который обозначает препятствия
        private float autoZoomForReturn;

        private allScenes SceneId = allScenes.emptyScene;

        public float AutoZoomForReturn
        {
            get => autoZoomForReturn;
            set
            {
                autoZoomForReturn = value;
            }
        }

        public bool CommonZoomUpdata { get => commonZoomUpdata; set => commonZoomUpdata = value; }

        // Метод вызывается перед первым обновлением кадра
        void Start()
        {
            maxZoom = maxZoomDefault;
            minZoom = minZoomDefault;

            mainCam = Camera.main; // Получаем главную камеру
            defaultCameraTransform = transform; // Сохраняем начальное положение камеры
            target = MainPlayer.Instance.transform; // Получаем цель (обычно игрока)
            AutoZoomForReturn = zoomTotal;
            RotationMovement();


            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            minZoom = minZoomDefault;
            maxZoom = maxZoomDefault;
            SceneComponent sceneComponent = FindObjectOfType<SceneComponent>();
            if (sceneComponent != null)
            {
                if (sceneComponent.newMinZoomCamera != 0)
                    minZoom = sceneComponent.newMinZoomCamera;
                if (sceneComponent.newMaxZoomCamera != 0)
                    maxZoom = sceneComponent.newMaxZoomCamera;

                SceneId = sceneComponent.IdScene;
            }
        }

        // Метод вызывается один раз за кадр, после Update
        void LateUpdate()
        {
            cameraMovement(); // Вызываем метод для управления камерой

            if (CommonZoomUpdata) CommonZoom();
        }

        // Метод для управления камерой
        private void cameraMovement()
        {
            if (pauseClass.GetPauseState()) return;

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
            setLocalPosition();
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

            CommonZoomUpdata=true;
        }

        private void ZoomUpdateByZoomTotal(float delta)
        {
            zoomTotal += delta;
            CommonZoomUpdata = true;
        }

        private void CommonZoom()
        {
            CommonZoomUpdata = false;
            Vector3 newZoomPos;//= mainCam.transform.position + (mainCam.transform.forward * zoomAmt);
            if (zoomTotal < minZoom) zoomTotal = minZoom;
            if (zoomTotal > maxZoom) zoomTotal = maxZoom;
            // Масштабируем камеру, если она находится в пределах допустимого масштабирования
            {
                newZoomPos = target.position + (mainCam.transform.forward * (zoomTotal * 0.2f));
                mainCam.transform.position = newZoomPos;

                setLocalPosition();

                //Debug.Log(mainCam.transform.position);
                //Debug.Log(mainCam.transform.localPosition);

                OnupdateEulerAngles?.Invoke(transform.localEulerAngles);
                OnCameraDistance?.Invoke(Math.Abs(zoomTotal));

                //Debug.Log($"Zoom Total: {zoomTotal}"); // Отладочное сообщение для отслеживания zoomTotal
            }

            OnCameraScale?.Invoke();
        }

        private void setLocalPosition()
        {
            float delta = Math.Abs(camYRotation - IGame.Instance.playerController.transform.rotation.eulerAngles.y);
            float deltaX = camYRotation - IGame.Instance.playerController.transform.rotation.eulerAngles.y;
            float delta2 = Math.Abs(180 - delta);
            float delta2X = Mathf.Sin(deltaX * Mathf.Deg2Rad);
            float deltaCameraY = (delta2 - 90) / 90 * 0.04f * (Math.Abs(zoomTotal) - 10);
            float deltaCameraX = -delta2X  * 0.04f * (Math.Abs(zoomTotal) - 10);
            Vector3 newZoomPos = mainCam.transform.localPosition;
            newZoomPos.y = deltaCameraY;
            newZoomPos.x = deltaCameraX;
            mainCam.transform.localPosition = newZoomPos;

        }

        void Update()
        {
            if (SceneId == allScenes.emptyScene || SceneId == allScenes.regionSCene) return;
            if (pauseClass.GetPauseState()) return;
            if (zoomTotal < minZoom) MinZoom();

            /*float delta = Math.Abs(camYRotation - IGame.Instance.playerController.transform.rotation.eulerAngles.y);
            float deltaX = camYRotation - IGame.Instance.playerController.transform.rotation.eulerAngles.y;
            float delta2 = Mathf.Sin(deltaX * Mathf.Deg2Rad);
            Debug.Log(camYRotation + " " + IGame.Instance.playerController.transform.rotation.eulerAngles.y + " " + delta + " " + deltaX + " " + delta2);
            */

            float step = 1f;
            Vector3 targetPos = target.position + new Vector3Int(0, 1, 0);
            Vector3 tempV1 = target.position + (mainCam.transform.forward * ((zoomTotal - step * 2) * 0.2f));
            var direction = (targetPos - tempV1).normalized;
            RaycastHit hit;

            // Проверка, есть ли препятствие между камерой и целью
            if (Physics.Raycast(tempV1, direction, out hit, Vector3.Distance(tempV1, targetPos)-7, obstacleMask))
            {
                if (hit.transform.gameObject.name != "Player")
                {
                    ZoomUpdateByZoomTotal(step);
                    //Debug.Log(hit.transform.gameObject.name);
                }
            }
            else if (zoomTotal > AutoZoomForReturn)
            {
                Vector3 tempV2 = target.position + (mainCam.transform.forward * ((zoomTotal - step * 3) * 0.2f));
                if (!(Physics.Raycast(tempV2, direction, out hit, Vector3.Distance(mainCam.transform.position, targetPos), obstacleMask)))
                {
                    ZoomUpdateByZoomTotal(-step);
                }
            }
        }

        public void MaxZoom()
        {
            float targetZoom = maxZoom;

            for (int i = zoomLevels.Length - 1; i >= 0; i--)
                if (zoomLevels[i] > zoomTotal)
                {
                    targetZoom = zoomLevels[i];
                    break;
                }

            // Если значение превышает максимум, устанавливаем его на максимальное значение
            if (targetZoom > maxZoom)
            {
                targetZoom = maxZoom;
            }

            DOTween.To(() => zoomTotal, x =>
            {
                zoomTotal = x;
                AutoZoomForReturn = zoomTotal;
                CommonZoomUpdata = true;
            }, targetZoom, 0.5f);
        }

        public void MinZoom()
        {
            float targetZoom = minZoom;
            for (int i = 0; i < zoomLevels.Length; i++)
                if (zoomLevels[i] < zoomTotal)
                {
                    targetZoom =  zoomLevels[i];
                    break;
                }


            // Если значение меньше минимума, устанавливаем его на минимальное значение
            if (targetZoom < minZoom)
            {
                targetZoom = minZoom;
            }

            DOTween.To(() => zoomTotal, x =>
            {
                zoomTotal = x;
                AutoZoomForReturn = zoomTotal;
                CommonZoomUpdata = true;
            }, targetZoom, 0.5f);
        }

    }
}
