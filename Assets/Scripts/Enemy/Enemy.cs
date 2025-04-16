using UnityEngine.AI;
using UnityEngine;
public abstract class Enemy : PoolableObject, IDamageable
{
	public EnemyLineOfSightCheck LineOfSightChecker;
	public AttackRadius AttackRadius;
	public Animator Animator;
	public NavMeshAgent Agent;
	public EnemyMovement Movement;
	public EnemyScriptableObject EnemyScriptableObject;
	public int Health = 100;

	public EnemyState DefaultState;
	private EnemyState _state;
	public EnemyState State
	{
		get
		{
			return _state;
		}
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

	public virtual void Update() { }
	public virtual void Start() { }

	public void Spawn()
	{
		Movement.SampleWaypoints();
		OnStateChange?.Invoke(EnemyState.Spawn, DefaultState);
	}
	private void HandleGainSight(Controller player)
	{
		State = EnemyState.Chase;
	}

	private void HandleLoseSight(Controller player)
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

	public virtual void OnEnable()
	{
	}
	public override void OnDisable()
	{
		base.OnDisable();
		Agent.enabled = false;
		_state = DefaultState;
	}
	public void TakeDamage(int Damage)
	{
		Health -= Damage;

		if (Health <= 0)
		{
			gameObject.SetActive(false);
		}
	}

	public Transform GetTransform()
	{
		return transform;
	}
}
