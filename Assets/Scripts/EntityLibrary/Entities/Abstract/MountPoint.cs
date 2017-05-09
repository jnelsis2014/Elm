using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MountPoint : Destructable {

    public abstract IHoldable occupant
    {
        get;
        set;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
