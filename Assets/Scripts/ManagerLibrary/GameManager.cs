using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public List<BaseEntity> _baseEntities;
    public List<BaseEntity> BaseEntities
    {
        get
        {
            return _baseEntities;
        }
    }

    public List<MovingEntity> _movingEntities;
    public List<MovingEntity> MovingEntities
    {
        get
        {
            return _movingEntities;
        }
    }

    public List<Obstacle> _obstacles;
    public List<Obstacle> obstacles
    {
        get
        {
            return _obstacles;
        }
    }

    public List<Wall> _walls;
    public List<Walls> walls
    {
        get
        {
            return _walls;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    public void addBaseEntity(MovingEntity baseEntity)
    {
        _baseEntities.Add(baseEntity);
    }


    public void removeBaseEntity(MovingEntity baseEntity)
    {
        _baseEntities.Remove(baseEntity);
    }

    public void addMovingEntity(MovingEntity MovingEntity)
    {
        _movingEntities.Add(MovingEntity);
    }

    public void removeMovingEntity(MovingEntity MovingEntity)
    {
        _movingEntities.Remove(MovingEntity);
    }

    public static GameManager getGameManager()
    {
        return GameObject.FindGameObjectWithTag("game_manager").GetComponent<GameManager>();
    }

    public void tagObstaclesWithinViewRange(MovingEntity movingEntity, float obstacleDetectionLength)
    {
        foreach (BaseEntity baseEntity in _baseEntities)
        {
            baseEntity.tagAsObstacle(movingEntity, obstacleDetectionLength);
        }
    }
}
