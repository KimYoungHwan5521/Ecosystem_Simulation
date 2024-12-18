using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimalAction { Resting, FindingPrey, Eating, Sleeping, RunAway, Mating, Hunting, Dead }
public enum Diet { Herbivore, Carnivore, Omnivore }

public class Animal : Organisms
{
    [SerializeField] protected Diet diet;
    [SerializeField] protected bool sex;
    [SerializeField] protected float weightCorrection;

    [SerializeField] protected AnimalAction currentAction;
    [SerializeField] protected GameObject targetPrey;

    protected bool isAdult;
    protected bool isPregnent;
    protected bool isMenopause;
    protected bool isDead;

    [SerializeField] protected float hunger;
    [SerializeField] protected float sleepiness;
    [SerializeField] protected float lifespan;
    [SerializeField] protected float age;
    [SerializeField] protected float corruptionRate;

    [SerializeField] protected float detectionRange;
    [SerializeField] protected float moveSpeed;

    protected float inMouthNutrients;
    [SerializeField] protected float chewTime;
    protected float curChewTime;

    protected Queue<float> digestionQueue;
    protected float digestingNutrients;
    [SerializeField] protected float digestionTime;
    protected float curDigestionTime;
    [SerializeField] protected float digestibility = 0.8f;

    [SerializeField]Vector2 moveDirection = Vector2.right;

    protected override void Start()
    {
        base.Start();
        digestionQueue = new Queue<float>();
    }

    protected override void MyStart()
    {
        transform.localScale = Vector3.one * 0.5f;
    }

    protected override void MyUpdate(float deltaTime)
    {
        if(!isDead)
        {
            Aging(deltaTime);
            AIAction(deltaTime);
            Digest(deltaTime);
            Move(moveDirection, deltaTime);
        }
        else
        {
            corruptionRate += deltaTime;
            if(corruptionRate > 100)
            {
                GameManager.Instance.ObjectDestroy += MyDestroy;
            }
        }
    }

    protected override void MyDestroy()
    {
        base.MyDestroy();
        NutritionalLoss(nutrients);
        Destroy(gameObject);
    }

    protected void NutritionalLoss(float amount)
    {
        RaycastHit2D hit;
        int layerMask = LayerMask.GetMask("Tile");
        hit = Physics2D.Raycast(transform.position, Vector2.right, 1, layerMask);
        if (hit.collider.TryGetComponent(out Tile tile))
        {
            tile.nutrient += amount;
            nutrients -= amount;
            if (nutrients < 0) Debug.LogWarning($"{gameObject}'s nutrients less than 0!");
        }
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
        currentAction = AnimalAction.Dead;
        transform.Rotate(new Vector3(0, 0, 90));
    }

    protected void AIAction(float deltaTime)
    {
        hunger += deltaTime;
        sleepiness += deltaTime;
        if (hunger > 100) { Dead(); return; }
        if(hunger < 10 && targetPrey != null && inMouthNutrients == 0) { Rest(); return; }

        if(targetPrey == null) FindPrey();
        else
        {
            if (Vector2.Distance(targetPrey.transform.position, transform.position) < 0.1f || inMouthNutrients > 0)
            {
                moveDirection = Vector2.zero;
                Eat(targetPrey.GetComponent<Organisms>(), deltaTime);
            }
            else
            {
                currentAction = AnimalAction.FindingPrey;
                moveDirection = targetPrey.transform.position - transform.position;
            }
        }
    }

    protected void Rest()
    {
        currentAction = AnimalAction.Resting;
        moveDirection = Vector2.zero;
    }

    protected void FindPrey()
    {
        currentAction = AnimalAction.FindingPrey;
        RaycastHit2D[] hits;
        hits = Physics2D.CircleCastAll(transform.position, detectionRange, Vector2.right);
        foreach(RaycastHit2D hit in hits)
        {
            if(hit.collider.TryGetComponent(out Organisms detected))
            {
                if((diet == Diet.Herbivore || diet == Diet.Omnivore) && detected.TryGetComponent(out Grass grass) && grass.GrowthLevel > 0)
                {
                    targetPrey = grass.gameObject;
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

    protected void Eat(Organisms prey, float deltaTime)
    {
        currentAction = AnimalAction.Eating;
        if(targetPrey.TryGetComponent(out Grass grass))
        {
            Graze(grass, deltaTime);
        }
        else
        {

        }
    }

    protected void Graze(Grass grass, float deltaTime)
    {
        if(inMouthNutrients == 0)
        {
            inMouthNutrients = grass.GetEaten(this);
            if (grass.GrowthLevel < 1) targetPrey = null;
        }
        else
        {
            curChewTime += deltaTime;
            if(curChewTime > chewTime)
            {
                curChewTime = 0;
                digestionQueue.Enqueue(inMouthNutrients);
                hunger -= inMouthNutrients;
                inMouthNutrients = 0;
            }
        }
    }

    void Digest(float deltaTime)
    {
        if(digestingNutrients == 0)
        {
            if (digestionQueue.Count == 0) return;
            digestingNutrients = digestionQueue.Dequeue();
        }
        else
        {
            curDigestionTime += deltaTime;
            if(curDigestionTime > digestionTime)
            {
                curDigestionTime = 0;
                nutrients += digestingNutrients * digestibility;
                // Poop
                RaycastHit2D hit;
                int layerMask = LayerMask.GetMask("Tile");
                hit = Physics2D.Raycast(transform.position, Vector2.right, 1, layerMask);
                if(hit.collider.TryGetComponent(out Tile tile))
                {
                    tile.nutrient += digestingNutrients * (1 - digestibility);
                }
                digestingNutrients = 0;
            }
        }
    }

    protected void Move(Vector2 preferDirection, float deltaTime)
    {
        preferDirection.Normalize();
        transform.position += (Vector3)preferDirection * deltaTime;
        if (preferDirection == Vector2.zero) NutritionalLoss(deltaTime * 0.01f * weightCorrection);
        else NutritionalLoss(deltaTime * moveSpeed * moveSpeed * 0.01f * weightCorrection);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, moveDirection * detectionRange);
    }
}
