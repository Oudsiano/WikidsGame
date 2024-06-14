using RPG.Combat;
using UnityEngine;
using RPG.Core;
using RPG.Movement;

namespace RPG.Controller
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float suspicionTimer = 10f;
        [SerializeField] private PatrolPath patrolPath;
        private int currentWayPointIndex = 0;
        [SerializeField] private float tolerance = 1f;

        [SerializeField] private float minDwellTime;
        [SerializeField] private float maxDwellTime;

        [SerializeField] private float currentDwellTime;

        private Vector3 lastKnownLocation;
        private Vector3 guardLocation;
        private Quaternion guardRotation;
        [SerializeField] private float timeSinceLastSawPlayer = Mathf.Infinity;
        [SerializeField] private float timeSinceLastHit = Mathf.Infinity;

        private Fighter fighter;
        private Mover mover;
        private Health health;

        private GameObject halfCircle;
        private MeshRenderer halfCircleRenderer;
        private MeshFilter halfCircleFilter;

        private float lastHealth;

        [SerializeField] private float startDistanceForShowIcon = 125f;
        [SerializeField] private float maxOpacity = 0.3f;  // Добавлено поле для максимальной непрозрачности

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();

            guardLocation = transform.position;
            guardRotation = transform.rotation;

            CreateHalfCircle();
            health.redHalfCircle = halfCircle;

            FollowCamera.OnCameraDistance += FollowCamera_OnCameraDistance;
        }

        private void OnDestroy()
        {
            FollowCamera.OnCameraDistance -= FollowCamera_OnCameraDistance;
        }

        private void CreateHalfCircle()
        {
            halfCircle = new GameObject("HalfCircle");
            halfCircle.layer = LayerMask.NameToLayer("Enemy");
            halfCircle.transform.parent = transform;
            halfCircle.transform.localPosition = new Vector3(0, 1, 0);
            halfCircle.transform.localEulerAngles = new Vector3(0, 0, 180);

            halfCircleRenderer = halfCircle.AddComponent<MeshRenderer>();
            halfCircleFilter = halfCircle.AddComponent<MeshFilter>();
            halfCircleFilter.mesh = CreateHalfCircleMesh(chaseDistance, 20);

            halfCircleRenderer.material = new Material(Shader.Find("Standard"));
            halfCircleRenderer.material.color = new Color(1, 0, 0, maxOpacity); // Используем maxOpacity
            halfCircleRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            halfCircleRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            halfCircleRenderer.material.SetInt("_ZWrite", 0);
            halfCircleRenderer.material.DisableKeyword("_ALPHATEST_ON");
            halfCircleRenderer.material.DisableKeyword("_ALPHABLEND_ON");
            halfCircleRenderer.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            halfCircleRenderer.material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }

        private Mesh CreateHalfCircleMesh(float radius, int segments)
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[segments + 2];
            int[] triangles = new int[segments * 3];

            vertices[0] = Vector3.zero;
            float angleStep = Mathf.PI / segments;
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
            if (IGame.Instance.IsPause) return;

            if (health.IsDead())
                return;

            if (!IGame.Instance.playerController.GetPlayerInvis() && DistanceToPlayer() < 40)
            {
                InteractWithCombat();
            }

            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceLastHit += Time.deltaTime;

            // Check if health has decreased, indicating a ranged attack
            if (health.GetCurrentHealth() < lastHealth)
            {
                timeSinceLastHit = 0;
                lastKnownLocation = MainPlayer.Instance.transform.position;
                AttackBehavior();
            }

            // Update last health
            lastHealth = health.GetCurrentHealth();
        }

        private float DistanceToPlayer()
        {
            return Vector3.Distance(MainPlayer.Instance.transform.position, transform.position);
        }

        private void InteractWithCombat()
        {
            if (IsPlayerInSight() && DistanceToPlayer() <= chaseDistance && (fighter.CanAttack(MainPlayer.Instance.gameObject) || IsAttacked()))
            {
                if (IsPlayerBehind())
                {
                    // Игрок атакует со спины
                    fighter.Hit();
                }
                else
                {
                    AttackBehavior();
                }
            }
            else if (suspicionTimer > timeSinceLastSawPlayer || timeSinceLastHit < 5f)
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
            var player = MainPlayer.Instance;
            bool isAttacked = player.GetComponent<Fighter>().target == this.gameObject.GetComponent<Health>();
            return isAttacked;
        }

        private bool IsPlayerInSight()
        {
            Vector3 directionToPlayer = (MainPlayer.Instance.transform.position - transform.position).normalized;
            float angleBetween = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleBetween <= 90f)
            {
                return true;
            }
            return false;
        }

        private bool IsPlayerBehind()
        {
            Vector3 directionToPlayer = (MainPlayer.Instance.transform.position - transform.position).normalized;
            float angleBetween = Vector3.Angle(transform.forward, directionToPlayer);

            return angleBetween > 135f; // Угол, определяющий, что игрок позади (например, > 135 градусов)
        }

        private void PatrolBehavior()
        {
            Vector3 nextPos = guardLocation;

            if (patrolPath)
            {
                if (AtWayPoint())
                {
                    if (currentDwellTime > 0)
                        currentDwellTime -= Time.deltaTime;
                    else
                        GoToNextWayPoint();
                }
                else
                {
                    // Randomly decide to change direction while moving
                    if (Random.Range(0f, 1f) < 0.01f) // Adjust the probability as needed
                    {
                        GoToNextWayPoint();
                    }
                }

                nextPos = GetCurrentWayPoint();
            }

            mover.StartMoveAction(nextPos);

            if (!patrolPath && mover.IsAtLocation(tolerance))
            {
                transform.rotation = guardRotation;
            }
        }

        private void GoToNextWayPoint()
        {
            // Randomly choose the next waypoint
            currentWayPointIndex = Random.Range(0, patrolPath.transform.childCount);

            currentDwellTime = Random.Range(minDwellTime, maxDwellTime);
        }

        private bool AtWayPoint()
        {
            return Vector3.Distance(transform.position, patrolPath.transform.GetChild(currentWayPointIndex).position) < tolerance;
        }

        private Vector3 GetCurrentWayPoint()
        {
            return patrolPath.transform.GetChild(currentWayPointIndex).position;
        }

        private void SuspicionBehavior()
        {
            mover.StartMoveAction(lastKnownLocation);
        }

        private void AttackBehavior()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(MainPlayer.Instance.gameObject);
            lastKnownLocation = MainPlayer.Instance.transform.position;
        }

        private void FollowCamera_OnCameraDistance(float obj)
        {
            if (halfCircleRenderer != null)
            {
                if (obj < startDistanceForShowIcon)
                {
                    halfCircle.SetActive(true);
                    Color newColor = halfCircleRenderer.material.color;
                    newColor.a = Mathf.Min(((startDistanceForShowIcon - obj) / 50f), maxOpacity);
                    halfCircleRenderer.material.color = newColor;

                    float _scale = (startDistanceForShowIcon - obj) / 100f + 1;
                    halfCircle.transform.localScale = new Vector3(_scale, _scale, _scale);
                }
                else
                {
                    halfCircle.SetActive(false);
                }
            }
            else
            {
                Debug.LogWarning("Полусфера не найдена, пожалуйста проверьте инициализацию.");
            }
        }
    }
}
