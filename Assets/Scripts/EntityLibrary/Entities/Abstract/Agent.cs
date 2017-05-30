using System.Collections.Generic;
using UnityEngine;

public abstract class Agent : BaseEntity
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

    public abstract float blindDetectRadius
    {
        get;
    }

    public List<Agent> _blindDetectedAgents;
    public List<Agent> blindDetectedAgents
    {
        get
        {
            if (_blindDetectedAgents != null)
            {
                return _blindDetectedAgents;
            }
            else
            {
                _blindDetectedAgents = new List<Agent>();
                return _blindDetectedAgents;
            }
        }
    }

    public abstract float maxSpeed
    {
        get;
    }

    public float obstacleDetectRadius
    {
        get
        {
            return GetComponent<CapsuleCollider>().bounds.extents.x;
        }
    }

    public float obstacleDetectDistance
    {
        get
        {
            float minDetectDistance = 2f;
            return minDetectDistance + ((GetComponent<Rigidbody>().velocity.z/maxSpeed) * minDetectDistance);
        }
    }

    private float _obstacleDistance;
    public float obstacleDistance
    {
        get
        {
            return _obstacleDistance;
        }
    }

    public Obstacle _closestObstacle;
    public Obstacle closestObstacle
    {
        get
        {
            return _closestObstacle;
        }
    }

    public void updateBlindDetectedAgents()
    {
        foreach (Agent agent in GameManager.getGameManager().agents)
        {
            if (agent != this)
            {
                float distanceSqr = (agent.transform.position - transform.position).sqrMagnitude;
                if (distanceSqr <= blindDetectRadius)
                {
                    if (!(blindDetectedAgents.Contains(agent)))
                    {
                        blindDetectedAgents.Add(agent);
                    }
                }
                else
                    blindDetectedAgents.Remove(agent);
            }
        }
    }
    public abstract void addForce(Vector3 force, ForceMode mode);
    public abstract void interact();

    //steering behaviors
    public Vector3 getSeekVector(Vector3 target)
    {
        Vector3 result = (target - transform.position).normalized * maxSpeed;
        return result;
    }

    public Vector3 getFleeVector(Vector3 target)
    {
        Vector3 result = (transform.position - target).normalized * maxSpeed;
        return result;
    }

    public Vector3 getArriveVector(Vector3 target, float deceleration)
    {
        Vector3 result = transform.position;
        Vector3 toTarget = (target - transform.position).normalized;
        float distance = Vector3.Distance(target, transform.position);

        if (distance > 0)
        {
            float dOffset = .3f;
            float dSpeed = distance / (deceleration * dOffset);
            dSpeed = Mathf.Min(dSpeed, maxSpeed);
            result = toTarget * dSpeed;
            return result;
        }
        return Vector3.zero;
    }

    //flocking behaviors
    public Vector3 getFlockAlignment()
    {
        Vector3 result = this.GetComponent<Rigidbody>().velocity;
        int neighborCount = 0;
        foreach (Agent agent in _blindDetectedAgents)
        {
            result += agent.GetComponent<Rigidbody>().velocity;
            neighborCount++;
        }

        if (neighborCount == 0)
            return this.GetComponent<Rigidbody>().velocity;

        result /= neighborCount;
        result = result.normalized;

        return result;
    }

    public Vector3 getFlockCohesion()
    {
        Vector3 result = this.GetComponent<Rigidbody>().velocity;
        int neighborCount = 0;
        foreach (Agent agent in _blindDetectedAgents)
        {
            result += agent.transform.position;
            neighborCount++;
        }

        if (neighborCount == 0)
            return this.GetComponent<Rigidbody>().velocity;

        result /= neighborCount;
        result = (result - this.transform.position);
        result = result.normalized;

        return result;
    }

    public Vector3 getFlockSeparation()
    {
        Vector3 result = this.GetComponent<Rigidbody>().velocity;
        int neighborCount = 0;
        foreach (Agent agent in _blindDetectedAgents)
        {
            result += agent.transform.position;
            neighborCount++;
        }

        if (neighborCount == 0)
            return this.GetComponent<Rigidbody>().velocity;

        result /= neighborCount;
        result = result - this.transform.position;
        result = result * -1;
        result = result.normalized;

        return result;
    }

    public Vector3 getFlockingVector()
    {
        Vector3 result = this.GetComponent<Rigidbody>().velocity;
        Vector3 alignment = getFlockAlignment();
        Vector3 cohesion = getFlockCohesion();
        Vector3 separation = getFlockSeparation();

        result = alignment + cohesion + separation;
        result = result.normalized * maxSpeed;
        return result;
    }

    public Vector3 getFlockingVector(Vector3 aWeight, Vector3 cWeight, Vector3 sWeight)
    {
        Vector3 result = this.GetComponent<Rigidbody>().velocity;
        Vector3 alignment = getFlockAlignment() + aWeight;
        Vector3 cohesion = getFlockCohesion() + cWeight;
        Vector3 separation = getFlockSeparation() + sWeight;

        result += alignment + cohesion + separation;
        result = result.normalized * maxSpeed;
        return result;
    }

    //obstacle avoidance behaviors
    public void updateObstacleDistance()
    {
        RaycastHit hit;

        if (Physics.SphereCast(transform.position, obstacleDetectRadius, transform.forward, out hit, obstacleDetectDistance))
        {
            if (hit.collider.GetComponent<Obstacle>() != null)
            {
                Debug.Log("Obstacle detected" + hit.collider.GetComponent<Obstacle>().instanceName);
                _obstacleDistance = hit.distance;
                _closestObstacle = hit.collider.GetComponent<Obstacle>();
            }
        }
        else
        {
            _obstacleDistance = 0;
            _closestObstacle = null;
        }
    }

    public Vector3 getObstacleAvoidanceVector()
    {
        float multiplier = 1f + (obstacleDetectDistance/Vector3.Distance(transform.position, _closestObstacle.transform.position));

        float avoidanceX = (_closestObstacle.radius - closestObstacle.transform.position.x) * multiplier;
        float avoidanceZ = (_closestObstacle.radius - closestObstacle.transform.position.z) * multiplier;

        Vector3 result = new Vector3(avoidanceX, transform.position.y, avoidanceZ) * maxSpeed;
        return result;
    }

    public Vector3 getWanderPoint(float wDistance)
    {
        Vector3 wTarget = UnityEngine.Random.insideUnitSphere * wDistance;
        wTarget.y = 0;
        wTarget += transform.position;
        return wTarget;
    }
    
    //behavior methods
    public bool hasObstacle()
    {
        if (_closestObstacle != null)
        {
            return true;
        }
        return false;
    }

    public bool hasNearbyAgents()
    {
        if (_blindDetectedAgents != null)
        {
            return true;
        }
        return false;
    }
}
