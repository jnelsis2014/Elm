using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentController : MonoBehaviour{

    public int controllerID
    {
        get
        {
            return GetInstanceID();
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
                Debug.Log("The controller " + controllerID + " does not have an agent attached.");
                return null;
            }
        }
    }
}
