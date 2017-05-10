using UnityEngine;

public interface IInteractable
{

    string IGlobalName
    {
        get;
    }

    string IInstanceName
    {
        get;
    }

    Agent holder
    {
        get;
    }

    int IID
    {
        get;
    }

    float getInteractableDistance(Vector3 agentForward); //Should return the distance from an agent which is
                                        //interacting with the object

    void interact(Agent agent);         //Should execute the interaction;
}
