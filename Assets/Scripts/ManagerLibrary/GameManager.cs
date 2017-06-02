using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public List<MovingEntity> _MovingEntitys;
    public List<MovingEntity> MovingEntitys
    {
        get
        {
            return _MovingEntitys;
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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    public void addMovingEntity(MovingEntity MovingEntity)
    {
        _MovingEntitys.Add(MovingEntity);
    }

    public void removeMovingEntity(MovingEntity MovingEntity)
    {
        _MovingEntitys.Remove(MovingEntity);
    }

    public static GameManager getGameManager()
    {
        return GameObject.FindGameObjectWithTag("game_manager").GetComponent<GameManager>();
    }
}
