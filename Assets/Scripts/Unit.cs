using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public static Dictionary<string, List<Unit>> unitsByType;
    public string type;
    public float health = 100;

    public GameObject healthBarFront;

    //the state of the unit
    public STATE state;
    //not all units will have all states implemented
    [System.Serializable]
    public enum STATE
    {
        MOVE,//the unit is just moving
        ATTACK_MOVE,//the unit is moving until it can attack
        ATTACK,//the unit is attacking(possibly redundant due to HOLD and ATTACK_MOVE)!!!
        HOLD,//the unit is not moving but will try to attack
        IDLE,//Unit will do nothing and try to transition state
        STOP//Unit will do nothing and not try to transition state(state must be changed externally)
    }

    // Start is called before the first frame update
    protected void Start()
    {
        if(unitsByType == null)
        {
            unitsByType = new Dictionary<string, List<Unit>>();
        }
        if(unitsByType.ContainsKey(type) == false)
        {
            unitsByType.Add(type, new List<Unit>());
        }
        unitsByType[type].Add(this);
        state = STATE.IDLE;

        if(gameObject.transform.Find("HealthBar") != null)
        {
            healthBarFront = gameObject.transform.Find("HealthBar").Find("Front").gameObject;
        }
        else
        {
            Debug.Log("healthbar not found in unit.Start()!");
        }
    }

    /// <summary>
    /// applies damage to the target
    /// </summary>
    /// <param name="dmg">amount of damage done(negative values heal)</param>
    /// <returns>returns true if the target looses all health</returns>
    public bool applyDamage(float dmg)
    {
        health -= dmg;
        healthBarFront.transform.localScale = new Vector3( health / 100, healthBarFront.transform.localScale.y, healthBarFront.transform.localScale.z);
        if(health > 500)
        {
            health = 500;
        }
        if (health <= 0)
        {
            onDeath();
            unitsByType[type].Remove(this);//remove this instance from the list
            Destroy(gameObject);//destory the gameobject
            return true;//i am dead
        }
        else
        {
            return false;
        }
    }

    protected virtual void onDeath()
    {
        //dead
    }
 

    // Update is called once per frame
    void Update()
    {
        
    }
}
