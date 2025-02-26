using Combat;
using Core;
using Core.Camera;
using Core.Player;
using Healths;
using Movement;
using UnityEngine;
using UnityEngine.Serialization;

namespace AINavigation
{
    public class AIController : MonoBehaviour
    {
        [FormerlySerializedAs("suspicionTimer")] [SerializeField]
        private float _suspicionTimer = 10f;

        [SerializeField] private PatrolPath patrolPath; // TODO bug on village scene
        [SerializeField] private float _chaseDistance = 5f;
        [SerializeField] private float _tolerance = 1f;

        [FormerlySerializedAs("minDwellTime")] [SerializeField]
        private float _minDwellTime;

        [FormerlySerializedAs("maxDwellTime")] [SerializeField]
        private float _maxDwellTime;

        [FormerlySerializedAs("currentDwellTime")] [SerializeField]
        private float _currentDwellTime;

        [FormerlySerializedAs("timeSinceLastSawPlayer")] [SerializeField]
        private float _timeSinceLastSawPlayer = Mathf.Infinity;

        [FormerlySerializedAs("timeSinceLastHit")] [SerializeField]
        private float _timeSinceLastHit = Mathf.Infinity;

        private int _currentWayPointIndex = 0;
        private Vector3 _lastKnownLocation;
        private Vector3 _guardLocation;
        private Quaternion _guardRotation;

        private Fighter _fighter;
        private Mover _mover;
        private Health _health;
        private MainPlayer _player;

        private GameObject _halfCircle; // TODO remove GO
        private MeshRenderer _halfCircleRenderer;
        private MeshFilter _halfCircleFilter;

        private float _lastHealth;

        private float _startDistanceForShowIcon = 300f;
        private float _maxOpacity = 0.2f;

        public void Construct(MainPlayer player, IGame igame)
        {
            _fighter = GetComponent<Fighter>();
            _mover = GetComponent<Mover>();
            _health = GetComponent<Health>();

            _player = player;
            _fighter.Construct(igame);
            _guardLocation = transform.position;
            _guardRotation = transform.rotation;

            CreateHalfCircle();
            _health.RedHalfCircle = _halfCircle;

            FollowCamera.OnCameraDistance += FollowCamera_OnCameraDistance;
        }

        // private void Awake() // TODO Construct
        // {
        //     _fighter = GetComponent<Fighter>();
        //     _mover = GetComponent<Mover>();
        //     _health = GetComponent<Health>();
        //
        //     _guardLocation = transform.position;
        //     _guardRotation = transform.rotation;
        //
        //     CreateHalfCircle();
        //     _health.RedHalfCircle = _halfCircle;
        //
        //     FollowCamera.OnCameraDistance += FollowCamera_OnCameraDistance;
        // }

        private void OnDestroy()
        {
            FollowCamera.OnCameraDistance -= FollowCamera_OnCameraDistance;
        }

        private void CreateHalfCircle() // TODO move to factory AIController -> HalfCircle factory
        {
            _halfCircle = new GameObject("HalfCircle");
            _halfCircle.layer = LayerMask.NameToLayer("Enemy");
            _halfCircle.transform.parent = transform;
            _halfCircle.transform.localPosition = new Vector3(0, 1, 0);
            _halfCircle.transform.localEulerAngles = new Vector3(0, 22, 180);

            _halfCircleRenderer = _halfCircle.AddComponent<MeshRenderer>();
            _halfCircleFilter = _halfCircle.AddComponent<MeshFilter>();
            _halfCircleFilter.mesh = CreateHalfCircleMesh(_chaseDistance, 14);

            _halfCircleRenderer.material = new Material(Shader.Find("Standard"));
            _halfCircleRenderer.material.color = new Color(1, 0, 0, _maxOpacity); // Используем maxOpacity
            _halfCircleRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            _halfCircleRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            _halfCircleRenderer.material.SetInt("_ZWrite", 0);
            _halfCircleRenderer.material.DisableKeyword("_ALPHATEST_ON");
            _halfCircleRenderer.material.DisableKeyword("_ALPHABLEND_ON");
            _halfCircleRenderer.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            _halfCircleRenderer.material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }

        private Mesh CreateHalfCircleMesh
        (
            float radius,
            int segments) // TODO move to factory AIController -> HalfCircle(mesh) factory
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[segments + 2];
            int[] triangles = new int[segments * 3];

            vertices[0] = Vector3.zero;
            float angleStep = Mathf.PI * 0.75f / segments;

            for (int i = 0; i <= segments; i++)
            {
                float angle = i * angleStep;
                vertices[i + 1] = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            }

            for (int i = 0; i < segments; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        private void Update()
        {
            if (PauseClass.GetPauseState())
            {
                return;
            }

            if (_health.IsDead()) // TODO rename "IsDead"
            {
                return;
            }

            if (_player.PlayerController.GetPlayerInvisibility() == false &&
                DistanceToPlayer() < 40) // TODO magic number
            {
                InteractWithCombat();
            }

            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceLastHit += Time.deltaTime;

            // Check if health has decreased, indicating a ranged attack
            if (_health.GetCurrentHealth() < _lastHealth)
            {
                _timeSinceLastHit = 0;
                _lastKnownLocation = _player.transform.position;
                AttackBehavior();
            }

            // Update last health
            _lastHealth = _health.GetCurrentHealth();
        }

        private float DistanceToPlayer() // TODO Vector3 Extensions
        {
            return Vector3.Distance(_player.transform.position, transform.position);
        }

        private void InteractWithCombat()
        {
            if (IsPlayerInSight() && DistanceToPlayer() <= _chaseDistance &&
                (_fighter.CanAttack(_player.gameObject) || IsAttacked()))
            {
                if (IsPlayerBehind())
                {
                    // Игрок атакует со спины
                    _fighter.Hit();
                }
                else
                {
                    AttackBehavior();
                }
            }
            else if (_suspicionTimer > _timeSinceLastSawPlayer || _timeSinceLastHit < 5f)
            {
                SuspicionBehavior();
            }
            else
            {
                PatrolBehavior();
            }
        }

        private bool IsAttacked()
        {
            var player = _player;
            bool isAttacked =
                player.GetComponent<Fighter>().Target ==
                gameObject.GetComponent<Health>(); // TODO Remove GetComponent in live fight

            return isAttacked;
        }

        private bool IsPlayerInSight()
        {
            Vector3 directionToPlayer = (_player.transform.position - transform.position).normalized;
            float angleBetween = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleBetween <= 120f) // TODO magic number 
            {
                return true;
            }

            return false;
        }

        private bool IsPlayerBehind()
        {
            Vector3 directionToPlayer = (_player.transform.position - transform.position).normalized;
            float angleBetween = Vector3.Angle(transform.forward, directionToPlayer);

            return
                angleBetween >
                120; // Угол, определяющий, что игрок позади (например, > 135 градусов) // TODO magic number 
        }

        private void PatrolBehavior()
        {
            Vector3 nextPos = _guardLocation;

            if (patrolPath)
            {
                if (AtWayPoint())
                {
                    if (_currentDwellTime > 0)
                    {
                        _currentDwellTime -= Time.deltaTime;
                    }
                    else
                    {
                        GoToNextWayPoint();
                    }
                }
                else
                {
                    // Randomly decide to change direction while moving
                    if (Random.Range(0f, 1f) < 0.01f) // Adjust the probability as needed // TODO magic number 
                    {
                        GoToNextWayPoint();
                    }
                }

                nextPos = GetCurrentWayPoint();
            }

            _mover.StartMoveAction(nextPos);

            if (patrolPath == false && _mover.IsAtLocation(_tolerance))
            {
                transform.rotation = _guardRotation;
            }
        }

        private void GoToNextWayPoint()
        {
            // Randomly choose the next waypoint
            _currentWayPointIndex = Random.Range(0, patrolPath.transform.childCount);

            _currentDwellTime = Random.Range(_minDwellTime, _maxDwellTime);
        }

        private bool AtWayPoint() // TODO Vector3 Extensions
        {
            return Vector3.Distance(transform.position, patrolPath.transform.GetChild(_currentWayPointIndex).position) <
                   _tolerance;
        }

        private Vector3 GetCurrentWayPoint()
        {
            return patrolPath.transform.GetChild(_currentWayPointIndex).position;
        }

        private void SuspicionBehavior()
        {
            _mover.StartMoveAction(_lastKnownLocation);
        }

        private void AttackBehavior()
        {
            _timeSinceLastSawPlayer = 0;
            _fighter.Attack(_player.gameObject);
            _lastKnownLocation = _player.transform.position;
        }

        private void FollowCamera_OnCameraDistance(float zoomTotal)
        {
            if (_halfCircleRenderer != null)
            {
                if (zoomTotal < _startDistanceForShowIcon)
                {
                    _halfCircle.SetActive(true);
                    Color newColor = _halfCircleRenderer.material.color;
                    newColor.a =
                        Mathf.Min(((_startDistanceForShowIcon - zoomTotal) / 100f), _maxOpacity); // TODO magic number
                    _halfCircleRenderer.material.color = newColor;

                    //float _scale = (startDistanceForShowIcon - obj) / 100f + 1;
                    //halfCircle.transform.localScale = new Vector3(_scale, _scale, _scale); // TODO not used
                }
                else
                {
                    _halfCircle.SetActive(false);
                }
            }
            else
            {
                Debug.LogWarning("Полусфера не найдена, пожалуйста проверьте инициализацию.");
            }
        }
    }
}