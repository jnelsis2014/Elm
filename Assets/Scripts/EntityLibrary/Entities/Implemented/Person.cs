using System;
using System.Collections.Generic;
using UnityEngine;

public class Person : Agent
{

    public List<AgentPoint> agentPoints; //public convenience field for unity editor
    private List<AgentPoint> _agentPoints = new List<AgentPoint>(); //private field stores actual references to weapon positions

    private const string GLOBAL_NAME = "Person";
    public override string globalName
    {
        get
        {
            return GLOBAL_NAME;
        }
    }

    public override string instanceName
    {
        get
        {
            return GLOBAL_NAME + " " + ID;
        }
    }

    public override bool isMutable
    {
        get
        {
            return true;
        }
    }

    public override float speed
    {
        get
        {
            return 5;
        }
    }

    public override float rotationOffset
    {
        get
        {
            return 40;
        }
    }

    public override float blindDetectRadius
    {
        get
        {
            return 40f;
        }
    }

    public override float obstacleDetectRadius
    {
        get
        {
            return GetComponent<SphereCollider>().radius;
        }
    }

    public override float obstacleDetectDistance
    {
        get
        {
            float detectDistanceOffset = 5f;
            return GetComponent<Rigidbody>().velocity.z/10 * detectDistanceOffset;
        }
    }

    private float _obstacleDistance;
    public override float obstacleDistance
    {
        get
        {
            return _obstacleDistance;
        }
    }

    public Obstacle _closestObstacle;
    public override Obstacle closestObstacle
    {
        get
        {
            return _closestObstacle;
        }
    }

    public List<Agent> _blindDetectedAgents = new List<Agent>();
    public List<Agent> blindDetectedAgents
    {
        get
        {
            return _blindDetectedAgents;
        }
    }

    private List<IInteractable> _inInteractionRange = new List<IInteractable>(); //return to private
    public override IInteractable inInteractionRange
    {
        get
        {
            IInteractable result;
            if (_inInteractionRange.Count <= 0)
            {
                Debug.Log("There were no objects in " + instanceName + "'s interaction range.");
                return null;
            }
            else
            {
                result = _inInteractionRange[0];
            }

            foreach (IInteractable interactable in _inInteractionRange)
            {
                if (interactable.getInteractableDistance(transform.forward) <= result.getInteractableDistance(transform.forward))
                {
                    result = interactable;
                    Debug.Log("The closest interactable to " + instanceName + " is " + interactable.IInstanceName);
                }
            }

            return result;
        }
        set
        {
            _inInteractionRange.Add(value);
            Debug.Log(value.IID + " entered " + ID + "'s interaction range and was added to its interaction range");
        }
    }

    private List<IHoldable> _holdables = new List<IHoldable>();
    public IHoldable holdables
    {
        set
        {
            _holdables.Add(value);

            _inInteractionRange.Remove(value);

            Debug.Log(value.IInstanceName + " was added to the weapons list of " + instanceName);
            foreach (AgentPoint point in agentPoints)
            {
                if (point.occupant == null)
                {
                    point.occupant = value;
                    value.pickUp(point);
                    break;
                }
            }
        }
    }
            

    private int _activePoint;
    public int activePoint
    {
        set
        {
            if (value < agentPoints.Count)
            {
                _activePoint = value;
                Debug.Log(instanceName + "'s active point was set to the index " + value + ".");
            }
            else
            {
                _activePoint = 0;
                Debug.Log(instanceName + "'s active point exceeded the length of the points assigned to it. Setting the" +
                    " active point back to 0");
            }
        }
        
        get
        {
            return _activePoint;
        }
    }

    private void Awake()
    {
        foreach (AgentPoint agentPoint in agentPoints)
        {
            if (agentPoint != null)
            {
                if (agentPoint.transform.parent != transform)
                {
                    Debug.Log("Position for object with ID number " + agentPoint.GetInstanceID() + " was not added to " + ID + "'s " +
                        " agentPoint list. The agent point fields for that object are only intended for its child objects." +
                        "Please check that the objects in the agent point fields are children of " + ID);
                }
                else
                {
                    _agentPoints.Add(agentPoint);
                    _holdables.Add(null);
                }
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        GameManager.getGameManager().addAgent(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        GameManager.getGameManager().removeAgent(this);
    }

    public override void addForce(Vector3 force, ForceMode mode)
    {
        if (GetComponent<Rigidbody>() != null)
        {
            GetComponent<Rigidbody>().AddForce(force, mode);
        }
        else
        {
            Debug.Log("Force could not be applied to the entity " + ID + 
                ". There was no Rigidbody attached.");
        }
    }

    public void exceededRange(IInteractable outOfRange)
    {
        if (outOfRange != null)
        {
            _inInteractionRange.Remove(outOfRange);
            Debug.Log(outOfRange.IID + " exceeded " + ID + "'s interaction range and was removed to its interaction range");
        }
        else
        {
            Debug.Log("Attempted to remove an IInteractable from " + ID + 
                "interactable range list, but the IInteractable no longer exists.");
        }
    }

    public override void interact()
    {

    }

    //Accessor methods
    public void removeIHoldable(IHoldable theHoldable)
    {
        if (_holdables.Contains(theHoldable))
        {
            int removeIndex = _holdables.IndexOf(theHoldable);
            _holdables[removeIndex] = null;
        }
    }

    public AgentPoint getOccupiedPoint()
    {
        AgentPoint result = null;

        foreach(AgentPoint point in _agentPoints)
        {
            if (point.occupant != null)
            {
                result = point;
                break;
            }
        }

        if (result == null)
            Debug.Log("Attempted to find an occupied point in " + instanceName + "'s agent points, but there were none.");

        return result;
    }

    public AgentPoint getNextOccupiedPoint(AgentPoint currentPoint)
    {
        AgentPoint result = null;

        foreach(AgentPoint point in _agentPoints)
        {
            if (point != currentPoint && point.occupant != null)
            {
                result = point;
                break;
            }
            else
            {
                result = null;
                Debug.Log("Tried to access the next occupied point in agent points for " + instanceName + " but there were none.");
            }
        }
        return result;
    }

    public override void updateBlindDetectedAgents()
    { 
        foreach (Agent agent in GameManager.getGameManager().agents)
        {
            if (agent != this)
            {
                float distanceSqr = (agent.transform.position - transform.position).sqrMagnitude;
                if (distanceSqr <= blindDetectRadius)
                {
                    if (!(_blindDetectedAgents.Contains(agent)))
                    {
                        _blindDetectedAgents.Add(agent);
                    }
                }
                else
                    _blindDetectedAgents.Remove(agent);
            }
        }
    }

    public override void updateObstacleDistance()
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
        result = result.normalized * speed;
        return result;
    }

    public Vector3 getFlockingVector(Vector3 aWeight, Vector3 cWeight, Vector3 sWeight)
    {
        Vector3 result = this.GetComponent<Rigidbody>().velocity;
        Vector3 alignment = getFlockAlignment() + aWeight;
        Vector3 cohesion = getFlockCohesion() + cWeight;
        Vector3 separation = getFlockSeparation() + sWeight;

        result += alignment + cohesion + separation;
        result = result.normalized * speed;
        return result;
    }

    public Vector3 getSeekVector(Vector3 target)
    {
        Vector3 result = (target - transform.position).normalized * speed;
        return result;
    }

    public Vector3 getFleeVector(Vector3 target)
    {
        Vector3 result = (transform.position - target).normalized * speed;
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
            dSpeed = Mathf.Min(dSpeed, speed);
            result = toTarget * dSpeed;
            return result;        
        }
        return Vector3.zero;
    }

    public Vector3 getObstacleAvoidanceVector()
    {
        float multiplier = 1f + (obstacleDetectDistance - transform.InverseTransformPoint(_closestObstacle.transform.position).z) / obstacleDetectDistance;

        float avoidanceX = (_closestObstacle.radius - closestObstacle.transform.position.x) * multiplier;
        float avoidanceZ = (_closestObstacle.radius - closestObstacle.transform.position.z) * multiplier; ;

        Vector3 result = new Vector3(avoidanceX, 0, avoidanceZ) * speed;
        return result;
    }

    public Vector3 getWanderPoint(float wDistance)
    {
        Vector3 wTarget = UnityEngine.Random.insideUnitSphere * wDistance;
        wTarget.y = 0;
        wTarget += transform.position;
        return wTarget;
    }
}
