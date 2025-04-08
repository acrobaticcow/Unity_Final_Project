using UnityEngine.AI;

public class Enemy : PoolableObject
{
	public NavMeshAgent Agent;
	public EnemyMovement Movement;
	public EnemyScriptableObject EnemyScriptableObject;
	int health = 100;
	public virtual void OnEnable()
	{
		SetupAgentConfiguration();
	}
	public override void OnDisable()
	{
		base.OnDisable();
		Agent.enabled = false;
	}
	public virtual void SetupAgentConfiguration()
	{
		Agent.acceleration = EnemyScriptableObject.Acceleration;
		Agent.angularSpeed = EnemyScriptableObject.AngularSpeed;
		Agent.areaMask = EnemyScriptableObject.AreaMask;
		Agent.avoidancePriority = EnemyScriptableObject.AvoidancePriority;
		Agent.baseOffset = EnemyScriptableObject.BaseOffset;
		Agent.height = EnemyScriptableObject.Height;
		Agent.obstacleAvoidanceType = EnemyScriptableObject.ObstacleAvoidanceType;
		Agent.radius = EnemyScriptableObject.Radius;
		Agent.speed = EnemyScriptableObject.Speed;
		Agent.stoppingDistance = EnemyScriptableObject.StoppingDistance;
		Movement.UpdateRate = EnemyScriptableObject.AIUpdateInterval;
		health = EnemyScriptableObject.Health;
	}
}
