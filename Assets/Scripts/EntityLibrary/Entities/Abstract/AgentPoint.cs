using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentPoint : MountPoint {

    public enum swingTypes
    {
        left,
        right,
        up,
        down,
        thrust
    }

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
