using Core.Camera;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(SphereCollider), typeof(SpriteRenderer))]
    public class IconForFarCamera : MonoBehaviour
    {
        [SerializeField] private float scaleThis = 3f;
        [SerializeField] private float startDistanceForShowIcon = 125; // TODO rename
        [SerializeField] private bool isMovable = false; // TODO rename
        [TextArea(3, 10)] public string description; // TODO rename

        private SpriteRenderer _image; // TODO Duplicate
        private Vector3 _eulerAngles;

        private SpriteRenderer _spriteRenderer; // TODO Duplicate
        private bool _isMouseOver = false;

        private SphereCollider _sphereCollider;

        private void Awake() // TODO construct
        {
            FollowCamera.OnCameraDistance += FollowCamera_OnCameraDistance;
            FollowCamera.NewYRotation += FollowCamera_NewYRotation;
            FollowCamera.NewXRotation += FollowCamera_NewXRotation;
            FollowCamera.OnupdateEulerAngles += FollowCamera_OnupdateEulerAngles;

            _image = GetComponent<SpriteRenderer>();
            _image.gameObject.SetActive(false);
            _eulerAngles = _image.transform.eulerAngles;

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _sphereCollider = GetComponent<SphereCollider>();
            _sphereCollider.isTrigger = true;
            _sphereCollider.radius = 0.5f; // TODO magic number
        }

        private void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // TODO can be allocated memory
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    if (_isMouseOver == false)
                    {
                        _OnMouseEnter();
                    }

                    _isMouseOver = true;
                }
                else
                {
                    if (_isMouseOver)
                    {
                        _OnMouseExit();
                    }

                    _isMouseOver = false;
                }
            }
            else
            {
                if (_isMouseOver)
                {
                    _OnMouseExit();
                }

                _isMouseOver = false;
            }

            if (isMovable)
            {
                UpdateData();
            }
        }

        private void _OnMouseEnter()
        {
            IGame.Instance._uiManager.UpdateIconMapPanel(description);
        }

        private void _OnMouseExit()
        {
            IGame.Instance._uiManager.UpdateIconMapPanel("");
        }

        private void OnDestroy()
        {
            FollowCamera.OnCameraDistance -= FollowCamera_OnCameraDistance;
            FollowCamera.NewYRotation -= FollowCamera_NewYRotation;
            FollowCamera.NewXRotation -= FollowCamera_NewXRotation;
            FollowCamera.OnupdateEulerAngles -= FollowCamera_OnupdateEulerAngles;
        }

        private void FollowCamera_OnupdateEulerAngles(Vector3 obj)
        {
            _eulerAngles.x = obj.x;
            _eulerAngles.y = obj.y;
            UpdateData();
        }

        private void UpdateData()
        {
            _image.gameObject.transform.eulerAngles = _eulerAngles;
        }

        private void FollowCamera_NewXRotation(float obj)
        {
            _eulerAngles.x = obj;
            UpdateData();
        }

        private void FollowCamera_NewYRotation(float obj)
        {
            _eulerAngles.y = obj;
            UpdateData();
        }

        private void FollowCamera_OnCameraDistance(float obj)
        {
            if (_image != null)
            {
                if (obj > startDistanceForShowIcon)
                {
                    _image.gameObject.SetActive(true);
                    Color newColor = _image.color; 
                    newColor.a = Mathf.Min(((obj - startDistanceForShowIcon) / 50f), 1); // TODO magic numbers
                    _image.color = newColor;

                    float _scale = ((obj - startDistanceForShowIcon) / 100f + 1) * scaleThis / // TODO magic numbers
                                   _image.sprite.bounds.size.magnitude;
                    _image.transform.localScale = new Vector3(_scale, _scale, _scale);
                }
                else
                {
                    _OnMouseExit();
                    _image.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogWarning("�� ������� ��������, ���� ���������"); // TODO UTF-8 ERROR
            }
        }
    }
}