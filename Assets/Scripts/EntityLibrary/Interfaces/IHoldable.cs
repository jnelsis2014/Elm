using UnityEngine;

public interface IHoldable : IInteractable
{

    void aim();
    void swing();
    void toss(Vector3 target);
    void pickUp(MovingEntityPoint point);
    void drop();
    void follow();
}
