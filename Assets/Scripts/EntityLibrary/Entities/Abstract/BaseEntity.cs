using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
    public List<string> obstacleAvoiderTags;

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

    public Vector3 position
    {
        get
        {
            return transform.position;
        }

        private set
        {
            transform.position = value;
        }
    }

    public abstract float scale
    {
        get;
        set;
    }

    public float bRadius;

    //is the bottom of an object touching a surface?
    public bool isGrounded
    {
        get
        {
            return Physics.Raycast(transform.position, -Vector3.up, transform.localScale.y + .01f);
        }
    }

    public void tagAsObstacle(MovingEntity avoider, float detectionLength)
    {
        string debugString = "";
        List<string> types = avoider.obstacleTypes.Split(',').ToList<string>();
        debugString += avoider.instanceName + "'s obstacle avoidance list: \n";
        foreach (string type in types)
            debugString += type + "\n";
     
        //in list
        if (types.Contains(this.GetType().ToString()))
        {
            //inform found
            debugString += instanceName + "'s type, " + this.GetType() + ", was found in list.\n";
            //in range
            if (Vector3.Distance(avoider.position, position) <= detectionLength)
            {
                //Debug.Log(detectionLength);
                //already tagged
                if (obstacleAvoiderTags.Contains(avoider.obstacleAvoidanceTag))
                {
                    //inform already tagged
                    debugString += instanceName + " was within obstacle avoidance distance, but already tagged.";
                }
                else
                {
                    //not in range. add to list and inform.
                    debugString += instanceName + " was within obstacle avoidance distance. Adding tag.\n";
                    obstacleAvoiderTags.Add(avoider.obstacleAvoidanceTag);
                }
            }
            //in list, not in range. inform not in range and remove tag
            else
            {
                //debugString += instanceName + " was not within obstacle avoidance distance. Removing tag.";
                obstacleAvoiderTags.Remove(avoider.obstacleAvoidanceTag);
            }
        }
        else
        {
            //inform not in list
            debugString += instanceName + "'s type, " + this.GetType() + ", was not found in list";
        }
        //Debug.Log(debugString);
    }

}
