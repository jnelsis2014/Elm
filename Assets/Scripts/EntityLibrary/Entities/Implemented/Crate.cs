using System;

public class Crate : Destructable
{
    public override string globalName
    {
        get
        {
            return "Crate";
        }
    }

    public override string instanceName
    {
        get
        {
            return "Crate " + ID;
        }
    }

    public override float integrity
    {
        get
        {
            return 10f;
        }
    }

    public override float scale
    {
        get
        {
            throw new NotImplementedException();
        }

        set
        {
            throw new NotImplementedException();
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
