using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : PoolableObject
{
    public EnemyHealth Health;
    public EnemyLineOfSightCheck LineOfSightChecker;
    public AttackRadius AttackRadius;
    public Animator Animator;
    public NavMeshAgent Agent;
    public EnemyMovement Movement;
    public EnemyScriptableObject EnemyScriptableObject;

    public EnemyState DefaultState;
    private EnemyState _state;
    public EnemyState State
    {
        get { return _state; }
        set
        {
            OnStateChange?.Invoke(_state, value);
            _state = value;
        }
    }

    public float IdleLocationRadius = 4f;

    public float IdleMoveSpeedMultiplier = 0.5f;

    public delegate void StateChangeEvent(EnemyState oldState, EnemyState newState);
    public StateChangeEvent OnStateChange;

    public virtual void Awake()
    {
        OnStateChange += HandleStateChange;

        LineOfSightChecker.OnGainSight += HandleGainSight;
        LineOfSightChecker.OnLoseSight += HandleLoseSight;
    }

    public virtual void Start()
    {
        // TODO Health.OnTakeDamage += PainResponse.HandlePain;
        Health.OnDeath += Die;
    }

    public virtual void Update() { }

    public void Spawn()
    {
        Movement.SampleWaypoints();
        OnStateChange?.Invoke(EnemyState.Spawn, DefaultState);
    }

    private void HandleGainSight(Player player)
    {
        State = EnemyState.Chase;
    }

    private void HandleLoseSight(Player player)
    {
        State = DefaultState;
    }

    private void HandleStateChange(EnemyState oldState, EnemyState newState)
    {
        if (oldState != newState)
        {
            Movement.StopMovement();

            if (oldState == EnemyState.Idle)
            {
                Agent.speed /= IdleMoveSpeedMultiplier;
            }

            switch (newState)
            {
                case EnemyState.Idle:
                    Movement.StartIdleMotion();
                    break;
                case EnemyState.Patrol:
                    Movement.StartPatrolMotion();
                    break;
                case EnemyState.Chase:
                    Movement.StartChasing();
                    break;
            }
        }
    }

    void Die(Vector3 Position)
    {
        Movement.DisableMovement();
        // TODO PainResponse.HandleDeath();
    }

    public virtual void OnEnable() { }

    public override void OnDisable()
    {
        base.OnDisable();
        Agent.enabled = false;
        _state = DefaultState;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
