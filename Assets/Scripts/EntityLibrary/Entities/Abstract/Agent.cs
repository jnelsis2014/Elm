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

    public abstract float speed
    {
        get;
    }

    public abstract float obstacleDetectRadius
    {
        get;
    }

    public abstract float obstacleDetectDistance
    {
        get;
    }

    public abstract float obstacleDistance
    {
        get;
    }

    public abstract Obstacle closestObstacle
    {
        get;
    }

    public abstract void updateBlindDetectedAgents();
    public abstract void updateObstacleDistance();

    public abstract void addForce(Vector3 force, ForceMode mode);
    public abstract void interact();
}
