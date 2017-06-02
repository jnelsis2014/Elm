using FluentBehaviourTree;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingEntityController : MonoBehaviour
{
    public const string GLOBAL_NAME = "MovingEntity Controller";

    public int id
    {
        get
        {
            return GetInstanceID();
        }
    }

    public string instanceName
    {
        get
        {
            return GLOBAL_NAME + " " + id;
        }
    }

    public MovingEntity MovingEntity
    {
        get
        {
            if (GetComponent<MovingEntity>() != null)
            {
                return GetComponent<MovingEntity>();
            }
            else
            {
                Debug.Log(instanceName + "attempted to access the MovingEntity script on its assigned GO," +
                    " but no MovingEntity was attached.");
                return null;
            }
        }
    }
}
