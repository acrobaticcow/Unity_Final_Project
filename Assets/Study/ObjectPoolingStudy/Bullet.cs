using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : AutoDestroyPoolableObject
{
	[HideInInspector]
	public Rigidbody2D Body;
	public Vector2 Speed = new(200, 0);
	void Awake()
	{
		Body = GetComponent<Rigidbody2D>();
	}
	public override void OnEnable()
	{
		base.OnEnable();
		Body.linearVelocity = Speed;
	}
	public override void OnDisable()
	{
		base.OnDisable();
		Body.linearVelocity = Vector2.zero;
	}
}