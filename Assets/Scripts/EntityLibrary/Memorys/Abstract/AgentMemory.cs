using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentMemory : MonoBehaviour {

    public Vector3 movementTarget; //vector3 reperesenting the location that the agent is currently moving towards
    public bool inFlockRadius; //bool representing if the agent is within the appropriate distance to flock toward a like agent.
}
