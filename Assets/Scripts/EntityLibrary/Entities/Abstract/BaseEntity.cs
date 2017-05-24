﻿using BehaviorLibrary;
using BehaviorLibrary.Components;
using BehaviorLibrary.Components.Composites;
using BehaviorLibrary.Components.Actions;
using BehaviorLibrary.Components.Conditionals;
using BehaviorLibrary.Components.Decorators;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{

    public abstract string globalName
    {
        get;
    }

    public abstract string instanceName
    {
        get;
    }

    //unique identifier of the entity
    public int ID
    {
        get
        {
            return GetInstanceID();
        }
    }

    public float h
    {
        get
        {
            return (transform.localScale.y);
        }
    }

    private bool _isTransparent;
    public bool isTransparent
    {
        get
        {
            return _isTransparent;
        }
        set
        {
            _isTransparent = value;
        }
    }

    //is the bottom of an object touching a surface?
    public bool isGrounded
    {
        get
        {
            Debug.DrawRay(transform.position, -Vector3.up * h, Color.red, 1);
            return Physics.Raycast(transform.position, -Vector3.up, h + .01f);
        }
    }

    //mass of the entity
    public double m
    {
        get
        {
            if (GetComponent<Rigidbody>() != null)
            {
                return GetComponent<Rigidbody>().mass;
            }
            else
            {
                Debug.Log("Cannot return mass for " + ID + ". No Rigidbody attached");
                return 0;
            }
        }
    }

    //Velocity of the entity
    public Vector3 v
    {
        get
        {
            if (GetComponent<Rigidbody>() != null)
            {
                return GetComponent<Rigidbody>().velocity;
            }
            else
            {
                Debug.Log("Cannot return velocity for " + ID + ". No Rigidbody attached");
                return new Vector3(0, 0, 0);
            }
        }
        set
        {
            if (GetComponent<Rigidbody>() != null)
            {
                GetComponent<Rigidbody>().velocity = value;
            }
            else
            {
                Debug.Log("Cannot set velocity for " + ID + ". no Rigidbody attached.");
            }
        }
    }

    //Magnitude of the entity's velocity vector
    public float vMagnitude
    {
        get
        {
            if (GetComponent<Rigidbody>() != null)
            {
                return GetComponent<Rigidbody>().velocity.magnitude;
            }
            else
            {
                Debug.Log("Cannot get vMagnitude for " + ID + ", no Rigidbody attached");
                return 0;
            }
        }
        set
        {
            if (GetComponent<Rigidbody>() != null)
            {
                vMagnitude = GetComponent<Rigidbody>().velocity.magnitude;
            }
            else
            {
                Debug.Log("Cannot set vMagnitude for " + ID + ", no Rigidbody attached");
            }
        }
    }

    public abstract float vDeltaMax
    {
        get;
    }

    EntityBehavior _behavior;
    public EntityBehavior behavior
    {
        get
        {
            if (behavior != null)
            {
                return _behavior;
            }
            else
            {
                return null;
                Debug.Log(instanceName + " has no assigned behavior.");
            }
        }
        set
        {
            _behavior = value;
        }
    }

    public Rigidbody rb
    {
        get
        {
            if (GetComponent<Rigidbody>() != null)
            {
                return GetComponent<Rigidbody>();
            }
            else
            {
                Debug.Log("Attempted to access the rigidbody of " + instanceName + " but it does not have a rigidbody attached.");
                return null;
            }
        }
    }

    private void Awake()
    {
        
    }

    private void Start()
    {

    }

    // FixedUpdate is called right before the Physics engine updates.
    private void Update()
    {

    }
    // Use Fixed Update for anything which adds a force to a BaseEntity.
    private void FixedUpdate()
    {

    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void OnDestroy()
    {

    }

    private int empty()
    {
        return 0;
    }
}
