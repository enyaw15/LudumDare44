using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class combatUnit : Unit
{
    //combat units move
    public Vector3 moveTarget;
    public float moveSpeed;

    //combat units attack
    public float attackDamage;//the damage applied to the target every second
    public float attackRange;//the distance from which the target can be hit
    public Unit target;//the target
    public string[] targetTags;// the tags that can be targets
    public float targetRange;//how close the target needs to be to start moving towards

    //used to spawn a grave on death
    public GameObject grave;

    // Start is called before the first frame update
    protected new void Start()
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        gameObject.GetComponent<Rigidbody2D>().drag = 5;
        base.Start();
    }

    protected void Update()
    {
        //if you are only moving just move
        if (state == STATE.MOVE)
        {
            //move();
        }
        else if (state == STATE.IDLE)//idleing
        {
            findNewTarget();
        }
        else if (state == STATE.ATTACK_MOVE)//attcking and moving
        {
            //move();
            attack();
            findNewTarget();//if another target is closer attack it
        }
        else if (state == STATE.HOLD)//just attacking
        {
            attack();
        }
        //always move since it checks if movement should happen.
        //it makes sure that in other states the velocity is zero
        move();
    }

    //moves the unit towards the target
    protected void move()
    {
        if (state == STATE.MOVE || state == STATE.ATTACK_MOVE)
        {
            if(state == STATE.ATTACK_MOVE)//if we are moving to a moving unit we need to update the target
            {
                moveTarget = target.transform.position;
            }
            //Debug.Log(Vector3.Normalize(moveTarget - gameObject.GetComponent<Transform>().position)* moveSpeed);
            //move
            //set the velocity to move to the target at speed
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.Normalize(moveTarget - gameObject.GetComponent<Transform>().position) * moveSpeed;
            //Debug.Log(gameObject.GetComponent<Rigidbody2D>().velocity.magnitude);
            //test if unit should stop moving
            if ((moveTarget - gameObject.GetComponent<Transform>().position).magnitude < .1f)// gameObject.GetComponent<Rigidbody2D>().velocity.magnitude < moveSpeed * .1f ||
            {
                //Debug.Log("StopMoving");
                state = STATE.IDLE;
                gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                //Debug.Log(gameObject.GetComponent<Rigidbody2D>().velocity.magnitude);
            }
        }
        else
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }
    }

    //finds the nearest acceptable target
    public void findNewTarget()
    {
        Unit t = null;
        for (int i = 0; i < targetTags.Length; i++)
        {
            foreach (Unit u in unitsByType[targetTags[i]])
            {
                if (t == null || (gameObject.transform.position - t.gameObject.transform.position).magnitude > (gameObject.transform.position - u.gameObject.transform.position).magnitude)
                {
                    t = u;
                }
            }
        }
        if(t == null)
        {
            return;
        }

        //move to the enemy
        if ((gameObject.transform.position - t.gameObject.transform.position).magnitude < targetRange){

            target = t;
            setMoveAttackTarget(target.transform.position);
        }
    }

    public void attack()
    {
        //if there is no target to attack idle and return
        if(target == null)
        {
            state = STATE.IDLE;
            return;
        }

        //if the target is farther than the attack range
        if((gameObject.transform.position - target.gameObject.transform.position).magnitude > attackRange)
        {
            //if we are not moving and attacking we should be moving and attacking
            if(state != STATE.ATTACK_MOVE)
            {
                state = STATE.ATTACK_MOVE;
            }
        }
        //we are close enough to attack
        else
        {
            state = STATE.HOLD;
            //applies damage if the unit is killed the statement is true
            if(target.GetComponent<Unit>().applyDamage(attackDamage*Time.deltaTime))
            {
                state = STATE.IDLE;
                onKill();
                target = null;
            }
        }
    }

    //sets the move target and makes the state attack move
    public void setMoveAttackTarget(Vector3 target)
    {
        moveTarget = target;
        moveTarget.z = 0;
        state = STATE.ATTACK_MOVE;
    }

    protected override void onDeath()
    {
        Debug.Log("combat onDeath");
        //drop a grave
        if(grave != null)
        {
            GameObject go = Instantiate(grave);
            go.transform.position = gameObject.transform.position;
        }
    }

    protected virtual void onKill()
    {

    }
}