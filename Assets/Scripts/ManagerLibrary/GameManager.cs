using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public List<Agent> _agents;
    public List<Agent> agents
    {
        get
        {
            return _agents;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void addAgent(Agent agent)
    {
        _agents.Add(agent);
    }

    public void removeAgent(Agent agent)
    {
        _agents.Remove(agent);
    }

    public static GameManager getGameManager()
    {
        return GameObject.FindGameObjectWithTag("game_manager").GetComponent<GameManager>();
    }
}
