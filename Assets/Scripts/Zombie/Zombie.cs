using System.Collections;
using UnityEngine;

public class Zombie : Enemy
{
    #region Animation param name
    const string ATTACK_TRIGGER = "Attack";
    #endregion
    private Coroutine LookCoroutine;

    public override void Awake()
    {
        base.Awake();
        AttackRadius.OnAttack += HandleAttackAnimation;
    }

    public override void Update()
    {
        base.Update();
    }

    private void HandleAttackAnimation(IDamageable Target)
    {
        Animator.SetTrigger(ATTACK_TRIGGER);
        Debug.Log("Attack trigger");

        if (LookCoroutine != null)
        {
            StopCoroutine(LookCoroutine);
        }

        LookCoroutine = StartCoroutine(LookAt(Target.GetTransform()));
    }

    private IEnumerator LookAt(Transform Target)
    {
        Quaternion lookRotation = Quaternion.LookRotation(Target.position - transform.position);
        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);

            time += Time.deltaTime * 2;
            yield return null;
        }
        transform.rotation = lookRotation;
    }
}
