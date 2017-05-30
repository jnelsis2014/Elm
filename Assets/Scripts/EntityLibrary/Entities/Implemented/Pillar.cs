using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : Obstacle {

    public override string globalName
    {
        get
        {
            return "Pillar";
        }
    }

    public override string instanceName
    {
        get
        {
            return ID + " " + globalName;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
