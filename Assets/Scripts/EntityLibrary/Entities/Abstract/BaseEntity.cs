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

    //is the bottom of an object touching a surface?
    public bool isGrounded
    {
        get
        {
            return Physics.Raycast(transform.position, -Vector3.up, transform.localScale.y + .01f);
        }
    }

    public abstract float speed
    {
        get;
    }
}
