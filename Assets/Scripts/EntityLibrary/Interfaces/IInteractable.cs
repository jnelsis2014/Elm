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

    MovingEntity holder
    {
        get;
    }

    int IID
    {
        get;
    }

    float getInteractableDistance(Vector3 MovingEntityForward); //Should return the distance from an MovingEntity which is
                                        //interacting with the object

    void interact(MovingEntity MovingEntity);         //Should execute the interaction;
}
