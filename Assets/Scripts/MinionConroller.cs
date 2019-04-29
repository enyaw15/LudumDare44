using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MinionConroller : combatUnit
{
    public Unit Necromancer;
    //public Vector3 moveTarget;
    //public bool isMoving;
    //public float moveSpeed;

    // Start is called before the first frame update
    protected new void Start()
    {
        //gets the first item in the play list(should be the only item and should be the player)
        List<Unit> holder = unitsByType["Player"];
        Necromancer = holder[0];
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        /*
          ! in base.Update(); !
            //if you are only moving just move
           if(state == STATE.MOVE)
           {
                move();
           }
           else if(state == STATE.IDLE)
           {
               findNewTarget();
             }
            else if(state == STATE.ATTACK_MOVE)
           {
                move();
                attack();
            }
            else if(state == STATE.HOLD)
            {
                attack();
            }
        */
        base.Update();      
    }

    protected override void onKill()
    {
        Necromancer.applyDamage(-5f);
        base.onKill();
    }


    //sets the move target and makes the state move
    public void setMoveTarget(Vector3 target)
    {
        moveTarget = target;
        moveTarget.z = 0;
        state = STATE.MOVE;
    }
}
