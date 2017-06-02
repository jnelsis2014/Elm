using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviours : MonoBehaviour
{

    public MovingEntity movingEntity;
    public float detectionBoxLength;
    public float minDetectionBoxLength;
    public MovingEntity targetEntity1;
    public MovingEntity targetEntity2;
    public float weightCohesion;
    public float weightAlignment;
    public float weightSeparation;
    public float weightObstacleAvoidance;
    public float weightWander;
    public float weightWallAvoidance;
    public float viewDistance;
    public float wallDetectionFeelerLength;
    public float feelers;
    public enum Deceleration { slow = 3, normal = 2, fast = 1};
    public Deceleration deceleration;
    public float wanderRadius;
    public float wanderDistance;
    public float wanderJitter;
    public float waypointSeekDistanceSqr;
    public float weightSeek;
    public float weightFlee;
    public float weightArrive;
    public float weightPursuit;
    public float weightOffsetPursuit;
    public float weightInterpose;
    public float weightHide;
    public float weightEvade;
    public float weightFollowPath;
    public bool cellSpaceOn;
    public bool wallAvoidanceOn;
    public bool obstacleAvoidanceOn;
    public bool seekOn;
    public bool arriveOn;
    public bool wanderOn;
    public bool pursuitOn;
    public bool offsetPursuitOn;
    public bool interposeOn;
    public bool hideOn;
    public bool followPathOn;
    public bool evadeOn;
    public bool fleeOn;
    public bool spatialPartitioningOn;
    public bool separationOn;
    public bool alignmentOn;
    public bool cohesionOn;
    public enum SummingMethod { weighted_average, prioritized, dithered};
    public SummingMethod summingMethod;
    public string obstacleAvoidanceTag;
    public Vector3 steeringForce;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    float theta = Random.Range(0, 1) * Mathf.PI * 2;

    public Vector3 wanderTarget
    {
        get
        {
           return new Vector3(wanderRadius * Mathf.Cos(theta), 0, wanderRadius * Mathf.Sin(theta));
        }
        private set
        {
            Debug.Log("Cannot set the wander target for " + movingEntity.instanceName + " using a setter.");
        }
    }

    public Vector3 calculate()
    {
        steeringForce = Vector3.zero;

        if (!spatialPartitioningOn)
        {
            if (separationOn || alignmentOn || cohesionOn)
            {
                //calculate neighbors based on view distance
            }
        }

        switch (summingMethod)
        {
            case SummingMethod.weighted_average:
                steeringForce = calculateWeightedSum(); break;

            case SummingMethod.prioritized:
                steeringForce = calculatePrioritized(); break;

            case SummingMethod.dithered:
                steeringForce = calculateDithered(); break;

            default:
                steeringForce = new Vector3(0, 0, 0); break;
        }
        return steeringForce;
    }

    public float forwardComponent()
    {
        return Vector3.Dot(movingEntity.heading, steeringForce);
    }

    public float sideComponent()
    {
        return Vector3.Dot(movingEntity.side, steeringForce);
    }

    bool accumulateForce(Vector3 runningTotal, Vector3 forceToAdd)
    {
        float magnitudeSoFar = runningTotal.magnitude;
        float magnitudeRemaining = movingEntity.maxForce - magnitudeSoFar;

        if (magnitudeRemaining <= 0f)
            return false;

        float magnitudeToAdd = forceToAdd.magnitude;

        if (magnitudeToAdd < magnitudeRemaining)
        {
            runningTotal += forceToAdd;
        }
        else
        {
            runningTotal += (forceToAdd.normalized * magnitudeRemaining);
        }

        return true;
    }

    public Vector3 calculateWeightedSum()
    {
        Vector3 force = Vector3.zero;
        return steeringForce;
    }

    public Vector3 calculatePrioritized()
    {
        Vector3 force = Vector3.zero;

        if (wallAvoidanceOn)
        {
            
        }

        return steeringForce;
    }

    public Vector3 calculateDithered()
    {
        Vector3 force = Vector3.zero;
        return steeringForce;
    }
    

    private Vector3 seek(Vector3 targetPos)
    {
        Vector3 desiredVelocity = (targetPos - movingEntity.position).normalized * movingEntity.maxSpeed;
        return (desiredVelocity - movingEntity.velocity);
    }

    private Vector3 flee(Vector3 targetPos)
    {
        Vector3 desiredVelocity = (movingEntity.position - targetPos).normalized * movingEntity.maxSpeed;
        return (desiredVelocity - movingEntity.velocity);
    }

    private Vector3 flee(Vector3 targetPos, float panicDistance)
    {
        float panicDistSqr = panicDistance * panicDistance;
        float rangeSqr = Vector3.SqrMagnitude(targetPos - movingEntity.position);
        if (rangeSqr > panicDistSqr)
        {
            return new Vector3(0, 0, 0);
        }
        Vector3 desiredVelocity = (movingEntity.position - targetPos).normalized * movingEntity.maxSpeed;
        return (desiredVelocity - movingEntity.velocity);
    }

    private Vector3 arrive(Vector3 targetPos, Deceleration theDeceleration)
    {
        Vector3 toTarget = targetPos - movingEntity.position;
        float distance = toTarget.magnitude;

        if (distance > 0)
        {
            float offset = .3f; //offset for fine turning of the deceleration

            float speed = distance / ((float)theDeceleration * offset);
            speed = Mathf.Min(speed, movingEntity.maxSpeed);
            Vector3 desiredVelocity = toTarget * speed / distance;
            return (desiredVelocity - movingEntity.velocity);
        }

        return new Vector3(0, 0, 0);
    }

    private Vector3 Pursuit(MovingEntity evader)
    {
        Vector3 toEvader = evader.position - movingEntity.position;

        /*relative heading = 0: no growth in the original direction
        relative heading > 0: positive growth in the original direction
        relative heading < 0: negative (reverse) growth in the original direction*/
        float relativeHeading = Vector3.Dot(movingEntity.heading, evader.heading);

        //if movingEntity velocity pushing movingEntity toward evader and movingEntity heading and evader heading are
        //pointing toward one another within a tolerance angle of 18 degrees(acos(.95) = 18 degs)
        if ((Vector3.Dot(toEvader, movingEntity.heading) > 0) && (relativeHeading < -.95))
        {
            return seek(evader.position);
        }

        //look ahead time is proportional to the distance between the evader and the pursuer. it is inversely proportional to the sum of the MovingEntities velocities. in other words, as the distance between the pursuer and the invader increases, must look ahead further into the future. as the speeds of the puruser and the invader increase, look ahead time decreases because more distance is covered faster.
        float lookAheadTime = toEvader.magnitude / (movingEntity.maxSpeed + evader.speed);

        lookAheadTime += turnAroundTime(movingEntity, evader.position); //see turnAroundTime function
                                                               //documentation

        //seek to the predicted future position of the evader
        return seek(evader.position + evader.velocity * lookAheadTime);
    }

    private float turnAroundTime(MovingEntity movingEntity, Vector3 targetPos)
    {
        Vector3 toTarget = (targetPos - movingEntity.position).normalized;
        float dot = Vector3.Dot(movingEntity.heading, toTarget);
        float coefficent = .5f; //the higher the max turn rate, the higher this value should be
                               //default value of .5 means a turn around time of 1 second.

        //dot product gives a value of 1 if target is directly ahead and -1 if target is directly behind
        //subtracting 1 and multiplying by -coefficent gives proportional value scaled with rotational
        //displacement
        return (dot - 1.0f) * -coefficent;
    }

    private Vector3 evade(MovingEntity pursuer)
    {
        Vector3 toPursuer = pursuer.position - movingEntity.position;

        //look ahead time proportional to distance between the pursuer and the evader. Inversely 		//proportional to MovingEntities velocities.
        float lookAheadTime = toPursuer.magnitude / (movingEntity.maxSpeed + pursuer.speed);

        //return a fleeing vector pointing away from the future position of the pursuer.
        return flee(pursuer.position + pursuer.velocity * lookAheadTime);
    }

    private Vector3 wander()
    {
        wanderTarget += new Vector3(
        UnityEngine.Random.Range(-1, 1) * wanderJitter,
        UnityEngine.Random.Range(-1, 1) * wanderJitter,
        UnityEngine.Random.Range(-1, 1) * wanderJitter
        );

        wanderTarget = wanderTarget.normalized;
        wanderTarget *= wanderRadius;

        //move the local target wanderDistance units in front of the movingEntity
        Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
        Vector3 targetWorld = transform.TransformPoint(targetLocal);
        return targetWorld - movingEntity.position;
    }

    public Vector3 obstacleAvoidance(List<BaseEntity> obstacles)
    {
        //detection box length is proportional to movingEntity velocity
        detectionBoxLength = minDetectionBoxLength + (movingEntity.speed / movingEntity.maxSpeed) * detectionBoxLength;
        movingEntity.gameManager.tagObstaclesWithinViewRange(movingEntity, detectionBoxLength);
        BaseEntity closestIntersectingObstacle = null;
        float distToClosestIP = Mathf.Infinity;
        Vector3 localPosOfClosestObstacle = Vector3.zero;

        foreach (BaseEntity obstacle in obstacles)
        {
            if (obstacle.tag.Contains(movingEntity.obstacleAvoidanceTag))
	        {
                Vector3 localPos = transform.InverseTransformPoint(obstacle.position);
                if (localPos.z >= 0) //if the obstacle is not behind the movingEntity
                {
                    float expandedRadius = obstacle.bRadius + movingEntity.bRadius;
                    if (Mathf.Abs(localPos.x) < expandedRadius)
                    {
                        //intersection test with a circle representing the radius of
                        //the obstacle plus the radius of the movingEntity, which is the
                        //amount of space the movingEntity could collide with an obstacle
                        //in
                        float cX = localPos.z;
                        float cY = localPos.x;
                        float sqrtPart = Mathf.Sqrt(expandedRadius * expandedRadius - cX * cX);
                        float ip = cX - sqrtPart;

                        if (ip <= 0)
                            ip = cX + sqrtPart;

                        if (ip < distToClosestIP)
                        {
                            distToClosestIP = ip;
                            closestIntersectingObstacle = obstacle;
                            localPosOfClosestObstacle = localPos;
                        }
                    }
                }
            }
        }

        Vector3 steeringForce = new Vector3(0, 0, 0);

        if (closestIntersectingObstacle != null)
        {
            float multiplier = 1.0f + (detectionBoxLength - localPosOfClosestObstacle.z) /
            detectionBoxLength;

            steeringForce.x = (closestIntersectingObstacle.bRadius - localPosOfClosestObstacle.x) * multiplier;

            float brakingWeight = .2f;

            steeringForce.z = (closestIntersectingObstacle.bRadius - localPosOfClosestObstacle.z) * brakingWeight;
        }

        return transform.TransformPoint(steeringForce);
    }

    public Vector3 wallAvoidance(List<Wall> walls)
    {
        Vector3 result = Vector3.zero;
        return result;
    }
}
