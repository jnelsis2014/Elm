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

    public override float speed
    {
        get
        {
            return 10;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
