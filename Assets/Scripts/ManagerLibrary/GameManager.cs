using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public List<Agent> agents;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        agents = updateAgents();
	}

    private List<Agent> updateAgents()
    {
        List<Agent> result = new List<Agent>();

        foreach (Agent agent in FindObjectsOfType<Agent>())
        {
            result.Add(agent);
        }

        return result;
    }

    public static GameManager getGameManager()
    {
        return GameObject.FindGameObjectWithTag("game_manager").GetComponent<GameManager>();
    }
}
