using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : Destructable {

    public override string globalName
    {
        get
        {
            return "Crate";
        }
    }

    public override string instanceName
    {
        get
        {
            return "Crate " + ID;
        }
    }

    public override float integrity
    {
        get
        {
            return 10f;
        }
    }

    public override float vDeltaMax
    {
        get
        {
            return 10f;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
