using System;
using System.Collections.Generic;
using UnityEngine;

public class Person : MovingEntity
{

    public List<MovingEntityPoint> MovingEntityPoints; //public convenience field for unity editor
    private List<MovingEntityPoint> _MovingEntityPoints = new List<MovingEntityPoint>(); //private field stores actual references to weapon positions

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
                    //Debug.Log("The closest interactable to " + instanceName + " is " + interactable.IInstanceName);
                }
            }

            return result;
        }
        set
        {
            _inInteractionRange.Add(value);
            //Debug.Log(value.IID + " entered " + ID + "'s interaction range and was added to its interaction range");
        }
    }

    private List<IHoldable> _holdables = new List<IHoldable>();
    public IHoldable holdables
    {
        set
        {
            _holdables.Add(value);

            _inInteractionRange.Remove(value);

            //Debug.Log(value.IInstanceName + " was added to the weapons list of " + instanceName);
            foreach (MovingEntityPoint point in MovingEntityPoints)
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
            if (value < MovingEntityPoints.Count)
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

    public override float maxForce
    {
        get
        {
            return Mathf.Infinity;
        }

        set
        {
            Debug.Log("Cannot set " + instanceName + "'s maxForce with a setter");
        }
    }

    public override float maxTurnRate
    {
        get
        {
            return Mathf.Infinity;
        }

        set
        {
            Debug.Log("Cannot set " + instanceName + "'s maxTurnRate with a setter");
        }
    }

    public override float scale
    {
        get
        {
            return 1;
        }

        set
        {
            Debug.Log("Cannot set " + instanceName + "'s scale with a setter");
        }
    }

    private float _minObstacleDetectDistance;

    private void Awake()
    {
        foreach (MovingEntityPoint MovingEntityPoint in MovingEntityPoints)
        {
            if (MovingEntityPoint != null)
            {
                if (MovingEntityPoint.transform.parent != transform)
                {
                    Debug.Log("Position for object with ID number " + MovingEntityPoint.GetInstanceID() + " was not added to " + ID + "'s " +
                        " MovingEntityPoint list. The MovingEntity point fields for that object are only intended for its child objects." +
                        "Please check that the objects in the MovingEntity point fields are children of " + ID);
                }
                else
                {
                    _MovingEntityPoints.Add(MovingEntityPoint);
                    _holdables.Add(null);
                }
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        GameManager.getGameManager().addMovingEntity(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        GameManager.getGameManager().removeMovingEntity(this);
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

    public MovingEntityPoint getOccupiedPoint()
    {
        MovingEntityPoint result = null;

        foreach(MovingEntityPoint point in _MovingEntityPoints)
        {
            if (point.occupant != null)
            {
                result = point;
                break;
            }
        }

        return result;
    }

    public MovingEntityPoint getNextOccupiedPoint(MovingEntityPoint currentPoint)
    {
        MovingEntityPoint result = null;

        foreach(MovingEntityPoint point in _MovingEntityPoints)
        {
            if (point != currentPoint && point.occupant != null)
            {
                result = point;
                break;
            }
            else
            {
                result = null;
                Debug.Log("Tried to access the next occupied point in MovingEntity points for " + instanceName + " but there were none.");
            }
        }
        return result;
    }
}
