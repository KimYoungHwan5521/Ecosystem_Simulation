using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimalAction { Rest, FindPrey, Eat, Sleep, RunAway, Mating, Defecate }

public class Animal : Organisms
{
    [SerializeField]protected AnimalAction currentAction;
    protected List<string> edibles;
    protected List<string> threats;
    [SerializeField]protected GameObject targetPrey;

    protected float hunger;
    protected float sleepiness;
    protected bool isAdult;
    protected bool isPregnent;
    protected bool isMenopause;
    protected bool isDead;

    [SerializeField] protected float lifespan;
    protected float age;
    protected float corruptionRate;
    [SerializeField] protected float detectionRange;
    [SerializeField] protected float moveSpeed;
    protected float intake;

    [SerializeField]Vector2 moveDirection = Vector2.right;

    protected override void MyStart()
    {
        edibles = new List<string>();
        threats = new List<string>();

        transform.localScale = Vector3.one * 0.5f;
    }

    protected override void MyUpdate(float deltaTime)
    {
        if(!isDead)
        {
            Aging(deltaTime);
            AIAction(deltaTime);
            Move(moveDirection, deltaTime);
        }
        else
        {
            corruptionRate += deltaTime;
            if(corruptionRate > 100)
            {
                MyDestroy();
            }
        }
    }

    protected override void MyDestroy()
    {
        
    }

    void Aging(float deltaTime)
    {
        age += deltaTime;
        if (!isAdult)
        {
            if (age > lifespan * 0.2f) Grow();
        }
        else if (!isMenopause)
        {
            if (age > lifespan * 0.5f) isMenopause = true;
        }
        else if (age > lifespan) Dead();

    }

    void Grow()
    {
        isAdult = true;
        transform.localScale = Vector3.one;
    }

    void Dead()
    {
        isDead = true;
        transform.Rotate(new Vector3(0, 0, 90));
    }

    protected void AIAction(float deltaTime)
    {
        hunger += deltaTime;
        sleepiness += deltaTime;

        if(targetPrey == null) FindPrey();
        else
        {
            if (Vector2.Distance(targetPrey.transform.position, transform.position) < 0.1f) moveDirection = Vector2.zero;
            else moveDirection = targetPrey.transform.position - transform.position;
        }
    }

    protected void FindPrey()
    {
        RaycastHit2D[] hits;
        hits = Physics2D.CircleCastAll(transform.position, detectionRange, Vector2.right);
        foreach(RaycastHit2D hit in hits)
        {
            if(hit.collider.TryGetComponent(out Organisms detected))
            {
                if(edibles.Contains(detected.bioTag))
                {
                    targetPrey = detected.gameObject;
                    return;
                }
            }
        }

        RaycastHit2D[] endHits;
        endHits = Physics2D.RaycastAll(transform.position, moveDirection.normalized, detectionRange);
        foreach(RaycastHit2D hit in endHits)
        {
            if(hit.collider.gameObject.CompareTag("Ground"))
            {
                if (Random.Range(0, 3) < 2)
                {
                    moveDirection = moveDirection.Rotate(90);
                }
                else
                    moveDirection = moveDirection.Rotate(135);
            }
        }
    }

    protected void Move(Vector2 preferDirection, float deltaTime)
    {
        preferDirection.Normalize();
        transform.position += (Vector3)preferDirection * deltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, moveDirection * detectionRange);
    }
}
