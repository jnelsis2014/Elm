using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {


    void Awake()
    {
        //keep go alive
        DontDestroyOnLoad(gameObject);
        Debug.Log("DontDestroyOnLoad: " + gameObject.name);
    }
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
