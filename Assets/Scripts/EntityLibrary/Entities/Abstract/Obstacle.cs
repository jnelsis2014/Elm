using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Obstacle : Constant {

    private float _radius;

    public float radius
    {
        get
        {
            return Mathf.Min(GetComponent<CapsuleCollider>().bounds.extents.x, GetComponent<CapsuleCollider>().bounds.extents.z);
        }
    }
	// Use this for initialization
	void Start () {
        GameManager.getGameManager().obstacles.Add(this);
	}
	
	// Update is called once per frame
	void Update () {
       
	}

    private void OnDestroy()
    {
       GameManager.getGameManager().obstacles.Remove(this);
    }
}
