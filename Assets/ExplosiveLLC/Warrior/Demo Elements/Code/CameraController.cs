using UnityEngine;
using UnityEngine.SceneManagement;

namespace WarriorAnims
{
    public class CameraController : MonoBehaviour
    {
        public GameObject cameraTarget;
        public float cameraTargetOffsetY = 1.0f;
        private Vector3 cameraTargetOffset;
        public float rotateSpeed = 1.0f;
        private float rotate;
        public float height = 2.75f;
        public float distance = 1.25f;
        public float zoomAmount = 0.2f;
        public float smoothing = 2.0f;
        private Vector3 offset;
        private bool following = true;
        private Vector3 lastPosition;

        private void Awake()
        {
            if (!cameraTarget) { cameraTarget = GameObject.FindWithTag("Player"); }
        }

        private void Start()
        {
            offset = new Vector3(cameraTarget.transform.position.x, cameraTarget.transform.position.y + height, cameraTarget.transform.position.z - distance);
            lastPosition = new Vector3(cameraTarget.transform.position.x, cameraTarget.transform.position.y + height, cameraTarget.transform.position.z - distance);
        }

        private void Update()
        {
            // Follow cam.
            if (Input.GetKeyDown(KeyCode.F))
            {
                following = !following;
            }
            if (following) { CameraFollow(); }
            else { transform.position = lastPosition; }

            // Rotate cam.
            if (Input.GetKey(KeyCode.Q)) { rotate = -1; }
            else if (Input.GetKey(KeyCode.E)) { rotate = 1; }
            else { rotate = 0; }

            // Mouse zoom.
            if (Input.mouseScrollDelta.y == 1) { distance += zoomAmount; height += zoomAmount; }
            else if (Input.mouseScrollDelta.y == -1) { distance -= zoomAmount; height -= zoomAmount; }

            // Set cameraTargetOffset as cameraTarget + cameraTargetOffsetY.
            cameraTargetOffset = cameraTarget.transform.position + new Vector3(0, cameraTargetOffsetY, 0);

            // Smoothly look at cameraTargetOffset.
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(cameraTargetOffset - transform.position), Time.deltaTime * smoothing);

            // Check for zoom input
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                MaxZoom();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                MinZoom();
            }
        }

        private void CameraFollow()
        {
            offset = Quaternion.AngleAxis(rotate * rotateSpeed, Vector3.up) * offset;
            transform.position = new Vector3(Mathf.Lerp(lastPosition.x, cameraTarget.transform.position.x + offset.x, smoothing * Time.deltaTime),
                Mathf.Lerp(lastPosition.y, cameraTarget.transform.position.y + offset.y * height, smoothing * Time.deltaTime),
                Mathf.Lerp(lastPosition.z, cameraTarget.transform.position.z + offset.z * distance, smoothing * Time.deltaTime));
        }

        private void LateUpdate()
        {
            lastPosition = transform.position;
        }

        private void SetZoom(float zoomLevel)
        {
            distance = zoomLevel;
            height = zoomLevel;
        }

        public void MaxZoom()
        {
            SetZoom(-20);
        }

        public void MinZoom()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName == "BattleScene" || currentSceneName == "BattleScene 1" || currentSceneName == "BS_3")
            {
                SetZoom(-500);
            }
            else if (currentSceneName == "Holl" || currentSceneName == "Library" || currentSceneName == "SceneFive")
            {
                SetZoom(-50);
            }
            else
            {
                SetZoom(-20); // Default fallback
            }
        }
    }
}
