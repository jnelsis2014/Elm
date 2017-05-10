using UnityEngine;

public abstract class AgentController : MonoBehaviour
{

    public int entityID
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
                Debug.Log("The controller " + entityID + " does not have an agent attached.");
                return null;
            }
        }
    }
}
