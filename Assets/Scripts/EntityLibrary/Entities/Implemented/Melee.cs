using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Weapon, IHoldable {

    //BaseEntity implemented properties
    private const string _globalName = "Plank";
    public override string globalName
    {
        get
        {
            return _globalName;
        }
    }

    //Weapon implmented properties
    public override float integrity
    {
        get
        {
            return 10;
        }
    }

    //IInteractable implemented properties

    

    public override string IGlobalName
    {
        get
        {
            return _globalName;
        }
    }

    public override string IInstanceName
    {
        get
        {
            return _globalName + " " + ID;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		

	}

    private void FixedUpdate()
    {
        if (followTarget != null && holder.isPlayerControlled == true)
        {
            GetComponent<Rigidbody>().useGravity = false;
            follow();
        }
    }

    public override void aim()
    {   
        transform.position = Vector3.Lerp(transform.position, followTarget.transform.position, Time.deltaTime * 10);
    }

    public override void swing()
    {
        rb.AddForce((holder.transform.forward - holder.transform.up)* 50, ForceMode.VelocityChange);
    }

    public override void toss(Vector3 target)
    {
        rb.AddForce((target - transform.position) * 20, ForceMode.VelocityChange);
    }
    
    public override void pickUp(AgentPoint point)
    {
        followTarget = point;
    }

    public override void drop()
    {
        foreach (Collider collider in holder.GetComponents<Collider>())
        {
            if (collider.bounds.Contains(this.transform.position))
            {
                holder.inInteractionRange = this;
            }
        }

        holder = null;
        followTarget = null;
        rb.useGravity = true;
    }

    public override void follow()
    {
        if (holder.tag == "player")
        {
            float distance = Vector3.Distance(transform.position, followTarget.transform.position);
            Vector3 newVelocity = Vector3.zero;
            Vector3 newAngularVelocity = Vector3.zero;
            Rigidbody theObjectRB = GetComponent<Rigidbody>();

            if (distance > minFollowDistance)
            {
                newVelocity =
                (
                    ((followTarget.transform.position - transform.position).normalized) * holder.mobility * 6
                );
            }
            else //weapon is in the follow zone
            {
                if (v != Vector3.zero)
                {
                    newVelocity -= v / 2;
                    newAngularVelocity = GetComponent<Rigidbody>().angularVelocity / 2;
                }
            }

            theObjectRB.angularVelocity = Vector3.Lerp(theObjectRB.velocity, newAngularVelocity, Time.deltaTime);
            theObjectRB.velocity = Vector3.Lerp(theObjectRB.velocity, newVelocity, Time.deltaTime);
        }
    }
        
}
