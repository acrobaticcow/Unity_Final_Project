using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(
    fileName = "EnemyScriptableObject",
    menuName = "Scriptable Objects/EnemyScriptableObject"
)]
public class EnemyScriptableObject : ScriptableObject
{
    public Enemy Prefab;

    [Header("Stats")]
    public int MaxHealth = 100;

    [Header("Attack stats")]
    public float AttackDelay = 1f;
    public int Damage = 5;
    public float AttackRadius = 1.5f;
    public LayerMask LineOfSightLayers;

    [Header("Movement stats")]
    public EnemyState DefaultState;
    public float IdleLocationRadius = 4f;
    public float IdleMoveSpeedMultiplier = 0.5f;

    [Range(2, 10)]
    public int Waypoints = 4;
    public float LineOfSightRange = 6f;
    public float FieldOfView = 90f;

    [Header("NavMeshAgent Configs")]
    public float AIUpdateInterval = 0.1f;

    public float Acceleration = 8;
    public float AngularSpeed = 120;
    public int AreaMask = -1; // -1 means everything
    public int AvoidancePriority = 50;
    public float BaseOffset = 0;
    public float Height = 2f;
    public ObstacleAvoidanceType ObstacleAvoidanceType =
        ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    public float Radius = 0.5f;
    public float Speed = 3f;
    public float StoppingDistance = 0.5f;

    public void SetupEnemy(Enemy enemy)
    {
        enemy.Agent.acceleration = Acceleration;
        enemy.Agent.angularSpeed = AngularSpeed;
        enemy.Agent.areaMask = AreaMask;
        enemy.Agent.avoidancePriority = AvoidancePriority;
        enemy.Agent.baseOffset = BaseOffset;
        enemy.Agent.height = Height;
        enemy.Agent.obstacleAvoidanceType = ObstacleAvoidanceType;
        enemy.Agent.radius = Radius;
        enemy.Agent.speed = Speed;
        enemy.Agent.stoppingDistance = StoppingDistance;

        enemy.Movement.UpdateRate = AIUpdateInterval;
        enemy.DefaultState = DefaultState;
        enemy.IdleMoveSpeedMultiplier = IdleMoveSpeedMultiplier;
        enemy.IdleLocationRadius = IdleLocationRadius;
        enemy.Movement.Waypoints = new Vector3[Waypoints];
        enemy.LineOfSightChecker.FieldOfView = FieldOfView;
        enemy.LineOfSightChecker.Collider.radius = LineOfSightRange;
        enemy.LineOfSightChecker.LineOfSightLayers = LineOfSightLayers;

        enemy.Health.SetUp(MaxHealth);

        (
            enemy.AttackRadius.Collider == null
                ? enemy.AttackRadius.GetComponent<SphereCollider>()
                : enemy.AttackRadius.Collider
        ).radius = AttackRadius;
        enemy.AttackRadius.AttackDelay = AttackDelay;
        enemy.AttackRadius.Damage = Damage;
    }
}
