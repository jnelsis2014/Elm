using UnityEngine;

public abstract class MovingEntityPoint : MountPoint
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public abstract void aim();
    public abstract void swing();
    public abstract void toss(Vector3 target);
    public abstract void drop();
}
