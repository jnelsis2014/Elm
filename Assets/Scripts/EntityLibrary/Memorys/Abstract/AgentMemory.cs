using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingEntityMemory : MonoBehaviour {

    public Vector3 movementTarget; //vector3 reperesenting the location that the MovingEntity is currently moving towards
    public bool inFlockRadius; //bool representing if the MovingEntity is within the appropriate distance to flock toward a like MovingEntity.
}
