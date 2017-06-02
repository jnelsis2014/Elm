using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviours : MonoBehaviour
{

    public MovingEntity movingEntity;
    public string obstacleAvoidanceTag;
    public enum deceleration { slow = 3, normal = 2, fast = 1};
    public float wanderRadius;
    public float wanderDistance;
    public float wanderJitter;
    public float minDetectionBoxLength;
    public float detectionBoxLength;
    public Vector3 wanderTarget;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

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

    private Vector3 arrive(Vector3 targetPos, deceleration theDeceleration)
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

}
