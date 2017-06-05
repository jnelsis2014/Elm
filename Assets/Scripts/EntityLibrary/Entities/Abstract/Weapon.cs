using UnityEngine;

public abstract class Weapon : Destructable , IHoldable
{

    public override string instanceName
    {
        get
        {
            return globalName + " " + ID;
        }
    }

    public int IID
    {
        get
        {
            return ID;
        }
    }

    public abstract string IGlobalName
    {
        get;
    }

    public abstract string IInstanceName
    {
        get;
    }

    private MovingEntity _holder;
    public MovingEntity holder
    {
        get
        {
            return _holder;
        }
        set
        {
            _holder = value;
        }
    }

    private MountPoint _followTarget;
    public MountPoint followTarget
    {
        get
        {
            return _followTarget;
        }
        set
        {
            _followTarget = value;
        }
    }

    private float _minFollowDistance = 1; //default
    public float minFollowDistance
    {
        get
        {
            return _minFollowDistance;
        }
        set
        {
            _minFollowDistance = value;
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

    }

    void FixedUpdate()
    {

    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider != null && collider.gameObject.GetComponent<Person>() != null && holder == null)
        {
            Person interactor = collider.gameObject.GetComponent<Person>();
            interactor.inInteractionRange = this;
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider != null && collider.gameObject.GetComponent<Person>() != null && holder == null)
        {
            Person interactor = collider.gameObject.GetComponent<Person>();
            interactor.exceededRange(this);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        BaseEntity theEntity = collision.gameObject.GetComponent<BaseEntity>();
        Rigidbody entityRB = theEntity.GetComponent<Rigidbody>();
        if (entityRB != null && theEntity.tag != "player")
            entityRB.AddForceAtPosition(transform.forward + (entityRB.mass * entityRB.velocity), collision.transform.position, ForceMode.Force);
    }

    public float getInteractableDistance(Vector3 MovingEntityForward)
    {
        float result = Vector3.Distance(MovingEntityForward, transform.position);
        return result;
    }

    public void interact(MovingEntity MovingEntity)
    {
        string IString = "A(n) ";
        if (MovingEntity.GetType() == typeof(Person))
        {
            IString += "person with the ID " + MovingEntity.instanceName + " interacted with " + instanceName;
            ((Person)MovingEntity).holdables = this;
            holder = MovingEntity;
        }
        Debug.Log(IString);
    }

    public abstract void aim();
    public abstract void swing();
    public abstract void toss(Vector3 target);
    public abstract void drop();
    public abstract void follow();
    public abstract void pickUp(MovingEntityPoint point);
}
