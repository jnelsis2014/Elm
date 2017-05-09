using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Destructable , IHoldable {

    public override string instanceName
    {
        get
        {
            return globalName + " " + ID;
        }
    }

    public int IID
    {
        get
        {
            return ID;
        }
    }

    public abstract string IGlobalName
    {
        get;
    }

    public abstract string IInstanceName
    {
        get;
    }

    private Agent _holder;
    public Agent holder
    {
        get
        {
            return _holder;
        }
        set
        {
            _holder = value;
        }
    }

    private MountPoint _followTarget;
    public MountPoint followTarget
    {
        get
        {
            return _followTarget;
        }
        set
        {
            _followTarget = value;
        }
    }

    public override float vDeltaMax
    {
        get
        {
            return 10;
        }
    }

    private float _minFollowDistance = 1; //default
    public float minFollowDistance
    {
        get
        {
            return _minFollowDistance;
        }
        set
        {
            _minFollowDistance = value;
        }
    }

    private void Awake()
    {

    }

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {

    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider != null && collider.gameObject.GetComponent<Person>() != null && holder == null)
        {
            Person interactor = collider.gameObject.GetComponent<Person>();
            interactor.inInteractionRange = this;
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider != null && collider.gameObject.GetComponent<Person>() != null && holder == null)
        {
            Person interactor = collider.gameObject.GetComponent<Person>();
            interactor.exceededRange(this);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        BaseEntity theEntity = collision.gameObject.GetComponent<BaseEntity>();
        if (theEntity.rb != null && theEntity.tag != "player")
            theEntity.rb.AddForceAtPosition(transform.forward + (theEntity.rb.mass * theEntity.rb.velocity), collision.transform.position, ForceMode.Force);
        Debug.Log(instanceName + " collided with " + theEntity.instanceName);
    }

    public float getInteractableDistance(Vector3 agentForward)
    {
        float result = Vector3.Distance(agentForward, transform.position);
        return result;
    }

    public void interact(Agent agent)
    {
        string IString = "A(n) ";
        if (agent.GetType() == typeof(Person))
        {
            IString += "person with the ID " + agent.instanceName + " interacted with " + instanceName;
            ((Person)agent).holdables = this;
            holder = agent;
        }
        Debug.Log(IString);
    }

    public abstract void aim();
    public abstract void swing();
    public abstract void toss(Vector3 target);
    public abstract void drop();
    public abstract void follow();
    public abstract void pickUp(AgentPoint point);
}
