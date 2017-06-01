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

    public List<Obstacle> _localObstacles;
    public List<Obstacle> localObstacles
    {
        get
        {
            return _localObstacles;
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

    public abstract float minObstacleDetectDistance
    {
        get;
        set;
    }

    public abstract double brakingWeight
    {
        get;
    }

    public float obstacleDetectDistance
    {
        get
        {
            return minObstacleDetectDistance + (GetComponent<Rigidbody>().velocity.z / maxSpeed) * minObstacleDetectDistance;
        }
    }

    public abstract float obstacleDetectWidth
    {
        get;
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
    public void updateLocalObstacles()
    {
        List<Obstacle> theLocalObstacles = new List<Obstacle>();
        foreach (Obstacle obstacle in GameManager.getGameManager().obstacles)
        {
            Vector3 localObsPos = transform.InverseTransformPoint(obstacle.transform.position);
            
            //Debug.Log(instanceName + " is checking " + obstacle.instanceName + " at LP " + localObsPos + " and GP " + obstacle.transform.position + " for intersections with its OAA");
            if (((localObsPos.z - obstacle.radius) < obstacleDetectDistance) //if the objects bounds do not lie outside the obstacle detection radius
            && (localObsPos.z >= 0) //if the obstacle is not behind the agent
            && (obstacle.radius + (obstacleDetectWidth / 2) > Mathf.Abs(localObsPos.x))) //if the obstacles avoidance buffer is outside of the detection width
            {
                theLocalObstacles.Add(obstacle);
                Debug.Log(instanceName + " detected " + obstacle.instanceName + " intersecting at LP " + localObsPos + "and GP " + obstacle.transform.position);
            }
        }
        _localObstacles = theLocalObstacles;
    }

    public Vector3 getObstacleAvoidanceVector()
    {
        Obstacle closest = null;
        float distanceToClosest = 0;

        foreach (Obstacle obstacle in _localObstacles)
        {
            if (closest != null && Vector3.Distance(closest.transform.position, obstacle.transform.position) < distanceToClosest)
            {
                closest = obstacle;
            }
            else
            {
                closest = obstacle;
            }
        }

        Vector3 localPosClosest = transform.InverseTransformPoint(closest.transform.position);

        double multiplier = 1.0 + (obstacleDetectDistance - localPosClosest.z) / obstacleDetectDistance;

        double steeringForceX = (closest.radius - localPosClosest.x) * multiplier;
        double steeringForceZ = (closest.radius - localPosClosest.z) * brakingWeight;

        Vector3 result = new Vector3((float)steeringForceX, 0, (float)steeringForceZ);
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
        if (_localObstacles.Count > 0)
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
