using System.Collections.Generic;
using UnityEngine;

public abstract class MovingEntity : BaseEntity
{
    public string obstacleTypes;
    
    public string obstacleAvoidanceTag
    {
        get
        {
            return ID.ToString();
        }

        private set
        {
            Debug.Log(instanceName + " obstacle avoidance tag cannot be modified.");
        }
    }

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

    public Vector3 side
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

    public float mass
    {
        get
        {
            return GetComponent<Rigidbody>().mass; 
        }

        private set
        {
            GetComponent<Rigidbody>().mass = value;
        }
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

    public GameManager gameManager
    {
        get
        {
            return GameManager.getGameManager();
        }

        private set
        {
            Debug.Log("Cannot set " + instanceName + "'s gameManager with a setter.");
        }
    }

    public abstract void addForce(Vector3 force, ForceMode mode);
    public abstract void interact();

}