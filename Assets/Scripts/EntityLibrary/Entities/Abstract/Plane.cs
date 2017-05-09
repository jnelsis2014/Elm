﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : Constant {

    private const string GLOBAL_NAME = "Plane";
    public override string globalName
    {
        get
        {
            return GLOBAL_NAME;
        }
    }

    public override string instanceName
    {
        get
        {
            return GLOBAL_NAME + " " + ID;
        }
    }

    public override float vDeltaMax
    {
        get
        {
            return 0;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}