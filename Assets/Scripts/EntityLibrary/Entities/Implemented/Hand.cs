using UnityEngine;

public class Hand : AgentPoint
{

    public override string globalName
    {
        get
        {
            return "Hand";
        }
    }

    public override string instanceName
    {
        get
        {
            return globalName + " " + ID;
        }
    }

    public override float integrity
    {
        get
        {
            return 10;
        }
    }

    private IHoldable _occupant;
    public override IHoldable occupant
    {
        get
        {
            return _occupant;
        }
        set
        {
            if (_occupant == null)
                _occupant = value;
            else
                Debug.Log(value.IInstanceName + " could not be placed in " + instanceName + " because " + instanceName + " is already occupied by "
                    + _occupant.IInstanceName + ".");
        }
    }

    public override float vDeltaMax
    {
        get
        {
            return 10;
        }
    }

    private void Awake()
    {

    } 

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = transform.parent.rotation;
    }

    private void FixedUpdate()
    {

    }

    public override void aim()
    {
        _occupant.aim();
    }

    public override void swing()
    {
        _occupant.swing();
    }

    public override void toss(Vector3 target)
    {
        _occupant.toss(target);
    }

    public override void drop()
    {
        _occupant.drop();
        _occupant = null;
    }
}
