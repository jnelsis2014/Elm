public class Arrow : Projectile
{

    private const string GLOBAL_NAME = "Arrow";
    public override string globalName
    {
        get
        {
            return GLOBAL_NAME;
        }
    }

    public override string instanceName
    {
        get
        {
            return GLOBAL_NAME + " " + ID;
        }
    }

    public override float integrity
    {
        get
        {
            return 1;
        }
    }

    public override float vDeltaMax
    {
        get
        {
            return 30;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
