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

    public override float maxSpeed
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
            return 80;
        }
    }

    public override float blindDetectRadius
    {
        get
        {
            return 40f;
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

    public override float minObstacleDetectDistance
    {
        get
        {
            return 2.5f;
        }
    }

    public override float obstacleDetectWidth
    {
        get
        {
            return GetComponent<CapsuleCollider>().bounds.extents.x;
        }
    }

    public override double brakingWeight
    {
        get
        {
            return .2d;
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
}
