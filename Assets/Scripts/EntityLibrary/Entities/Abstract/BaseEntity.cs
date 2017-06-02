using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{

    public abstract string globalName
    {
        get;
    }

    public abstract string instanceName
    {
        get;
    }

    //unique identifier of the entity
    public int ID
    {
        get
        {
            return GetInstanceID();
        }
    }

    public Vector3 position
    {
        get
        {
            return transform.position;
        }

        private set
        {
            transform.position = value;
        }
    }

    public abstract float scale
    {
        get;
        set;
    }

    public abstract float bRadius
    {
        get;
        set;
    }

    //is the bottom of an object touching a surface?
    public bool isGrounded
    {
        get
        {
            return Physics.Raycast(transform.position, -Vector3.up, transform.localScale.y + .01f);
        }
    }
}
