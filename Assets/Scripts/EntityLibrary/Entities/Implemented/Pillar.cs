﻿using System;
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

    public override float scale
    {
        get
        {
            throw new NotImplementedException();
        }

        set
        {
            throw new NotImplementedException();
        }
    }

    // Use this for initialization
    void Start () {
        GameManager.getGameManager().baseEntities.Add(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDestroy()
    {
        GameManager.getGameManager().baseEntities.Remove(this);
    }
}
