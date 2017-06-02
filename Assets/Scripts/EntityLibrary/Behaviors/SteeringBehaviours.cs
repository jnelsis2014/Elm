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
    public bool spacePartitioningOn;
    public bool separationOn;
    public bool alignmentOn;
    public bool cohesionOn;
    public enum SummingMethod { weighted_average, prioritized, dithered};
    public SummingMethod summingMethod;
    public string obstacleAvoidanceTag;
    public Vector3 steeringForce;
    public float theta;

    //CalculateDithered probabilites
    public float prWallAvoidance;
    public float prObstacleAvoidance;
    public float prSeparation;
    public float prFlee;
    public float prEvade;
    public float prAlignment;
    public float prCohesion;
    public float prWander;
    public float prSeek;
    public float prArrive;
    public float prPursuit;

    // Use this for initialization
    void Start()
    {
        theta = Random.Range(0, 1) * Mathf.PI * 2;
    }

    // Update is called once per frame
    void Update()
    {

    }

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
        Debug.Log(movingEntity.instanceName + " is calculating its movement vector.");
        steeringForce = Vector3.zero;

        if (!spacePartitioningOn)
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

    public bool accumulateForce(ref Vector3 runningTotal, Vector3 forceToAdd)
    {
        float magnitudeSoFar = runningTotal.magnitude;
        float magnitudeRemaining = movingEntity.maxForce - magnitudeSoFar;

        if (magnitudeRemaining <= 0f)
        {
            Debug.Log("No remeining magnitude.");
            return false;
        }
           
        float magnitudeToAdd = forceToAdd.magnitude;

        if (magnitudeToAdd < magnitudeRemaining)
        {
            runningTotal += forceToAdd;
        }
        else
        {
            runningTotal += (forceToAdd.normalized * magnitudeRemaining);
        }
        Debug.Log("magnitude so far: " + magnitudeSoFar + "\nMagnitude remaining: " + magnitudeRemaining +
            "\nRunning total(steeringForce): " + runningTotal + "\nForce to add: " + forceToAdd + 
            "\nActual value of steeringForce: " + steeringForce);
        return true;
    }

    public Vector3 calculateWeightedSum()
    {
        Debug.Log(movingEntity.instanceName + " is calculating its movement vector using weighted sum.");
        Vector3 force = Vector3.zero;

        if (wallAvoidanceOn)
        {
            force = wallAvoidance(GameManager.getGameManager().walls) * weightWallAvoidance;
        }

        if (obstacleAvoidanceOn)
        {
            force = obstacleAvoidance(GameManager.getGameManager().baseEntities) * weightObstacleAvoidance;
        }

        if (evadeOn)
        {
            if (targetEntity1 != null)
            {
                force = evade(targetEntity1) * weightEvade;
            }
        }

        if (fleeOn)
        {
            if (targetEntity1 != null)
            {
                force = flee(targetEntity1.position) * weightFlee;
            }
        }

        //flocking behaviors and spatial partitioning considerations go here

        if (seekOn)
        {
            force = seek(targetEntity1.position) * weightSeek;
        }

        if (arriveOn)
        {
            force = arrive(targetEntity1.position, deceleration) * weightArrive;
        }

        if (arriveOn)
        {
            force = wander() * weightWander;
        }

        if (pursuitOn)
        {
            force = pursuit(targetEntity1) * weightPursuit;
        }

        //offset pursuit goes here

        //interpose goes here

        //hide goes here

        //follow path goes here

        Vector3.ClampMagnitude(steeringForce, movingEntity.maxForce);
        return steeringForce;
    }

    public Vector3 calculatePrioritized()
    {
        string velocityString = "";
        velocityString += movingEntity.instanceName + " is calculating its movement vector using prioritized.\n";
        Vector3 force = Vector3.zero;

        if (wallAvoidanceOn)
        {
            force = wallAvoidance(GameManager.getGameManager().walls) * weightWallAvoidance;
            if (!accumulateForce(ref steeringForce, force)) { Debug.Log(velocityString); return steeringForce; }
            velocityString += "'s current steering force with respect to wall avoidance is " + steeringForce + "\n";
        }
        velocityString += "'s current steering force with respect to wall avoidance " + steeringForce + "\n";
        if (obstacleAvoidanceOn)
        {
            force = obstacleAvoidance(GameManager.getGameManager().baseEntities) * weightObstacleAvoidance;
            if (!accumulateForce(ref steeringForce, force)) { Debug.Log(velocityString); return steeringForce; }
            velocityString += "'s current steering force with respect to obstacle avoidance is " + steeringForce + "\n";
        }
        velocityString += "'s current steering force with respect to obstacle avoidance " + steeringForce + "\n";

        if (evadeOn)
        {
            if (targetEntity1 != null)
            {
                force = evade(targetEntity1) * weightEvade;
                if (!accumulateForce(ref steeringForce, force)) { Debug.Log(velocityString); return steeringForce; }
            }
            velocityString += "'s current steering force with respect to evasion is " + steeringForce + "\n";
        }

        velocityString += "'s current steering force with respect to evasion is " + steeringForce + "\n";

        if (fleeOn)
        {
            if (targetEntity1 != null)
            {
                force = flee(targetEntity1.position) * weightFlee;
                if (!accumulateForce(ref steeringForce, force)) { Debug.Log(velocityString); return steeringForce; }
                velocityString += "'s current steering force with respect to fleeing is " + steeringForce + "\n";
            }
            velocityString += "'s current steering force with respect to fleeing is " + steeringForce + "\n";
        }

        //flocking behaviors and spatial partitioning considerations go here

        if (seekOn)
        {
            force = seek(targetEntity1.position) * weightSeek;
            Debug.Log(force);
            if (!accumulateForce(ref steeringForce, force)) { Debug.Log(velocityString); return steeringForce; }
            velocityString += "'s current steering force with respect to seeking is " + steeringForce + "\n";
        }
        velocityString += "'s current steering force with respect to seeking is " + steeringForce + "\n";

        if (arriveOn)
        {
            force = arrive(targetEntity1.position, deceleration) * weightArrive;
            if (!accumulateForce(ref steeringForce, force)) { Debug.Log(velocityString); return steeringForce; }
            velocityString += "'s current steering force with respect to arriving is " + steeringForce + "\n";
        }
        velocityString += "'s current steering force with respect to arriving is " + steeringForce + "\n";

        if (wanderOn)
        {
            force = wander() * weightWander;
            if (!accumulateForce(ref steeringForce, force)) { Debug.Log(velocityString); return steeringForce; }
            velocityString += "'s current steering force with respect to wandering is " + steeringForce + "\n";
        }
        velocityString += "'s current steering force with respect to wandering is " + steeringForce + "\n";

        if (pursuitOn)
        {
            force = pursuit(targetEntity1) * weightPursuit;
            if (!accumulateForce(ref steeringForce, force)) { Debug.Log(velocityString); return steeringForce; }
            Debug.Log(movingEntity.instanceName + "'s current steering force with respect to pursuit is " + steeringForce);
        }
        velocityString += "'s current steering force with respect to pursuit is " + steeringForce + "\n";
        //offset pursuit goes here

        //interpose goes here

        //hide goes here

        //follow path goes here
        Debug.Log(velocityString);
        return steeringForce;
    }

    public Vector3 calculateDithered()
    {
        Debug.Log(movingEntity.instanceName + " is calculating its movement vector using dithered.");
        Vector3 force = Vector3.zero;

        if (wallAvoidanceOn && Random.Range(0, 1) < prWallAvoidance)
        {
            force = wallAvoidance(GameManager.getGameManager().walls) * weightWallAvoidance;
        }

        if (obstacleAvoidanceOn && Random.Range(0, 1) < prObstacleAvoidance)
        {
            force = obstacleAvoidance(GameManager.getGameManager().baseEntities) * weightObstacleAvoidance;
        }

        if (evadeOn && Random.Range(0, 1) < prEvade)
        {
            if (targetEntity1 != null)
            {
                force = evade(targetEntity1) * weightEvade;
            }
        }

        if (fleeOn && Random.Range(0, 1) < prFlee)
        {
            if (targetEntity1 != null)
            {
                force = flee(targetEntity1.position) * weightFlee;
            }
        }

        //flocking behaviors and spatial partitioning considerations go here

        if (seekOn && Random.Range(0, 1) < prSeek)
        {
            force = seek(targetEntity1.position) * weightSeek;
        }

        if (arriveOn && Random.Range(0, 1) < prArrive)
        {
            force = arrive(targetEntity1.position, deceleration) * weightArrive;
        }

        if (wanderOn && Random.Range(0, 1) < prWander)
        {
            force = wander() * weightWander;
        }

        if (pursuitOn && Random.Range(0, 1) < prPursuit)
        {
            force = pursuit(targetEntity1) * weightPursuit;
        }

        //offset pursuit goes here

        //interpose goes here

        //hide goes here

        //follow path goes here

        return steeringForce;
    }
    

    public Vector3 seek(Vector3 targetPos)
    {
        Debug.Log(movingEntity.instanceName + " is seeking " + targetEntity1.instanceName);
        Vector3 desiredVelocity = (targetPos - movingEntity.position).normalized * movingEntity.maxSpeed;
        return (desiredVelocity - movingEntity.velocity);
    }

    public Vector3 flee(Vector3 targetPos)
    {
        Vector3 desiredVelocity = (movingEntity.position - targetPos).normalized * movingEntity.maxSpeed;
        return (desiredVelocity - movingEntity.velocity);
    }

    public Vector3 flee(Vector3 targetPos, float panicDistance)
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

    public Vector3 arrive(Vector3 targetPos, Deceleration theDeceleration)
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

    public Vector3 pursuit(MovingEntity evader)
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

    public float turnAroundTime(MovingEntity movingEntity, Vector3 targetPos)
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

    public Vector3 evade(MovingEntity pursuer)
    {
        Vector3 toPursuer = pursuer.position - movingEntity.position;

        //look ahead time proportional to distance between the pursuer and the evader. Inversely 		//proportional to MovingEntities velocities.
        float lookAheadTime = toPursuer.magnitude / (movingEntity.maxSpeed + pursuer.speed);

        //return a fleeing vector pointing away from the future position of the pursuer.
        return flee(pursuer.position + pursuer.velocity * lookAheadTime);
    }

    public Vector3 wander()
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
