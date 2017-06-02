using System.Collections.Generic;
using UnityEngine;

public abstract class MovingEntity : BaseEntity
{

    private bool _isPlayerControlled;
    public bool isPlayerControlled
    {
        get
        {
            return _isPlayerControlled;
        }
        set
        {
            _isPlayerControlled = value;
        }
    }

    public abstract IInteractable inInteractionRange
    {
        get;
        set;
    }

    public abstract float rotationOffset
    {
        get;
    }

    public abstract bool isMutable
    {
        get;
    }

    public Vector3 velocity
    {
        get
        {
            return GetComponent<Rigidbody>().velocity;
        }
    }

    public Vector3 heading
    {
        get
        {
            return transform.forward;
        }

        set
        {
            transform.forward = value;
        }
    }

    public Vector3 _side
    {
        get
        {
            return transform.right;
        }

        private set
        {
            Debug.Log("Cannot set _side for " + instanceName);
        }
    }

    public abstract float mass
    {
        get;
        set;
    }

    public float speed
    {
        get
        {
            return velocity.magnitude;
        }

        set
        {
            Debug.Log("Cannot directly set the speed of agent " + instanceName);
        }
    }

    public abstract float maxSpeed
    {
        get;
        set;
    }

    public abstract float maxForce
    {
        get;
        set;
    }

    public abstract float maxTurnRate
    {
        get;
        set;
    }

    public abstract void addForce(Vector3 force, ForceMode mode);
    public abstract void interact();

}