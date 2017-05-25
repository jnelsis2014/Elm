using FluentBehaviourTree;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentController : MonoBehaviour
{
    public const string GLOBAL_NAME = "Agent Controller";

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

    public Agent agent
    {
        get
        {
            if (GetComponent<Agent>() != null)
            {
                return GetComponent<Agent>();
            }
            else
            {
                Debug.Log(instanceName + "attempted to access the Agent script on its assigned GO," +
                    " but no Agent was attached.");
                return null;
            }
        }
    }
}
