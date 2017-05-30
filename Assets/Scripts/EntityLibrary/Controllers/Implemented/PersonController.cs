using UnityEngine;
using FluentBehaviourTree;
using System.Collections.Generic;

public class PersonController : AgentController
{

    public const float _MIN_JUMP_HORIZONTAL_V = 0;      //minumum value of the amount of velocity that may be applied
                                                        //to the GO's RB while not grounded

    public const float _MAX_JUMP_HORIZONTAL_V = .5f;    //maximum value of the amount of velocity that may be applied
                                                        //to the GO's RB while not grounded

    public const float _MIN_JUMP_CHARGE = 4.2f;         //minimum vertical velocity that is applied when the jump
                                                        //button is pressed

    public const float _MAX_JUMP_CHARGE = 6.2f;         //maximum vertical velocity that is applied when the jump
                                                        //button is pressed

    private CameraController _mainCamera;
    public CameraController mainCamera
    {
        get
        {
            return _mainCamera;
        }
    }

    private float _jumpCharge;
    public float jumpCharge
    {
        get
        {
            return _jumpCharge;
        }
    }

    private Person _person;
    public Person person
    {
        get
        {
            return _person;
        }
    }

    private PersonMemory _personMemory;
    public PersonMemory personMemory
    {
        get
        {
            return _personMemory;
        }
    }

    private AgentPoint _activePoint;
    public AgentPoint activePoint
    {
        get
        {
            return _activePoint;
        }
    }

    private Vector3 _movementTarget;
    public Vector3 movementTarget
    {
        get
        {
            return _movementTarget;
        }
    }

    private IBehaviourTreeNode _behaviourTree;
    public IBehaviourTreeNode behaviourTree
    {
        get
        {
            return _behaviourTree;
        }
    }

    public void Awake()
    {
        if (Camera.main.GetComponent<CameraController>() != null)
        {
            _mainCamera = Camera.main.GetComponent<CameraController>();
        }
        else
        {
            Debug.Log(_person.instanceName + "'s controller attempted to access the main cameras control script, but none was assigned.");
        }

        if ((_person = GetComponent<Person>()) == null)
        {
            Debug.Log("Warning. You have attempted to attach a person controller to an object which does not have a "
                + " person script. Please attach a person script to this object or the controller will not " +
                "function correctly");
        }

        if ((_personMemory = GetComponent<PersonMemory>()) == null)
        {
            Debug.Log("Warning. You have attempted to attach a person controller to an object which does not have a "
                + " person memory script. Please attach a person memory script to this object or the controller will not " +
                "function correctly");
        }

        if (_person.getOccupiedPoint() != null)
        {
            _activePoint = person.getOccupiedPoint();
        }

        _person.GetComponent<Rigidbody>().freezeRotation = true;
        if (tag == "player")
            _person.isPlayerControlled = true;
    }

    // Use this for initialization
    void Start() {
        _movementTarget = transform.position;
        BehaviourTreeBuilder builder = new BehaviourTreeBuilder();
        _behaviourTree = builder
        .Sequence("NotifyPackBehaviour")
            .Do("Notify", t =>
            {
                return BehaviourTreeStatus.Success;
            })
        .End()
        .Selector("PackBehaviour")
            .Sequence("SearchForLikeEntities")
                .Condition("IsAlone", t =>
                {
                    if (_person.blindDetectedAgents.Count > 0)
                    {
                        return false;
                    }
                    return true;
                })
                .Do("RandomWander", t =>
                {
                    if (Vector3.Distance(movementTarget, transform.position) < 1)
                    {
                        _movementTarget = _person.getWanderPoint(10);
                        return BehaviourTreeStatus.Success;
                    }
                    else
                    {
                        applyVelocity(getArriveVector(_movementTarget, 2));
                        return BehaviourTreeStatus.Running;
                    }
                })
            .End()
            .Sequence("Flock")
                .Condition("IsNotAlone", t =>
                {
                    if (_person.blindDetectedAgents.Count > 0)
                    {
                        return true;
                    }
                    return false;
                })
                .Do("FlockWithLikeEntity", t =>
                {
                    applyVelocity(getFlockingVector());
                    return BehaviourTreeStatus.Success;
                })
            .End()
            .Sequence("Attack")
            .End()
            .Sequence("Nest")
            .End()
            .Sequence("Socialize")
            .End()
        .End()
        .Build();
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void FixedUpdate()
    {
        if (_person.isPlayerControlled == true)
            getInputs();
        else
        {
            _behaviourTree.Tick(new TimeData(Time.deltaTime));   
        }

        if (_person.GetComponent<Rigidbody>().velocity != Vector3.zero && _person.isGrounded)
        {
            transform.rotation = Quaternion.RotateTowards
            (
                transform.rotation, Quaternion.LookRotation(new Vector3(_person.GetComponent<Rigidbody>().velocity.x, 0, _person.GetComponent<Rigidbody>().velocity.z)),
                Time.deltaTime * _person.speed * _person.rotationOffset
            );
        }
    }

    private void getInputs()
    {
        Vector3 movementVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Transform cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        bool isJumpDown = Input.GetKeyDown("space");
        bool isJumpHeld = Input.GetKey("space");
        bool isJumpUp = Input.GetKeyUp("space");
        bool isInteractUp = Input.GetKeyUp("tab");
        bool isInteractHeld = Input.GetKey("tab");
        bool isInteractDown = Input.GetKeyDown("tab");
        bool isAttackDown = Input.GetKeyDown("mouse 0");
        bool isAttackHeld = Input.GetKey("mouse 0");
        bool isAttackUp = Input.GetKeyUp("mouse 0");
        bool isSwitchDown = Input.GetKeyDown("mouse 1");
        bool isSwitchHeld = Input.GetKey("mouse 1");
        bool isSwitchUp = Input.GetKeyUp("mouse 1");
        bool isReleaseDown = Input.GetKeyDown(KeyCode.Escape);
        bool isReleaseHeld = Input.GetKey(KeyCode.Escape);
        bool isReleaseUp = Input.GetKeyUp(KeyCode.Escape);
        bool isTossDown = Input.GetKeyDown(KeyCode.E);
        bool isTossHeld = Input.GetKey(KeyCode.Q);
        bool isTossUp = Input.GetKeyUp(KeyCode.Q);
        bool isLockDown = Input.GetKeyUp(KeyCode.Q);
        bool isLockHeld = Input.GetKeyUp(KeyCode.Q);
        bool isLockUp = Input.GetKeyUp(KeyCode.Q);

        if (_person.isPlayerControlled == true)
        {
            //movement
            if (_person.isGrounded)
            {
                applyVelocity
                (
                    movementVelocity * _person.speed,
                    cameraTransform
                );
            }

            if (isJumpHeld)
            {
                chargeJump(.1f);
            }
            else if (isJumpUp && _person.isGrounded)
            {
                applyJumpVelocity(_jumpCharge);
            }

            //actions
            if (_person.getOccupiedPoint() != null) //if there is an occupied point
            {
                if (_activePoint != null && _activePoint.occupant != null) //if the current point selected is occupied
                {
                    if (isReleaseDown) //if attack and switch are held simulaneously
                    {
                        _person.removeIHoldable(_activePoint.occupant);
                        _activePoint.drop();
                    }
                    else if (isSwitchDown)
                    {
                        _activePoint = _person.getNextOccupiedPoint(_activePoint);
                    }
                    else if (isAttackHeld || isTossHeld) //if attach is held down
                    {
                        //set camera to over the shoulder mode
                        _activePoint.aim();
                    }
                    else if (isAttackUp)
                    {
                        // Bit shift the index of the layer (8) to get a bit mask
                        int layerMask = 1 << 8;

                        // This would cast rays only against colliders in layer 8.
                        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
                        layerMask = ~layerMask;
                        Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0));
                        RaycastHit hit;
                        // Does the ray intersect any objects excluding the player layer
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                        {
                            Debug.DrawLine(ray.origin, hit.point, Color.red, 3f);
                            Debug.Log("Hit the object " + hit.collider.GetComponent<BaseEntity>().instanceName);
                            _activePoint.toss(hit.point);
                        }
                        else
                        {
                            Debug.DrawLine(ray.origin, hit.point, Color.yellow, 3f);
                            Debug.Log("Did not Hit");
                        }
                    }
                }
                else if (_person.getNextOccupiedPoint(_activePoint) != null) //if there is another point with an occupant
                                                                             //and the current point selected is NOT occupied
                {
                    _activePoint = person.getNextOccupiedPoint(_activePoint); //set the active point to the next occupied point
                }

                //lock on
                if (isLockDown)
                {
                    //toggle lockon
                    //if lockon = true, lock on to agent closest to the center of the screen
                    //else, return camera to free movement
                }
            }

            //interaction
            if (isInteractDown)
            {
                IInteractable interactable = _person.inInteractionRange;
                if (interactable != null)
                    interactable.interact(_person);
                else
                    Debug.Log("There were no interactables in the " + _person.instanceName + "'s interaction range.");
            }
        }
    }

    private void applyVelocity(Vector3 myV)
    {
        Vector3 targetV = myV;
        Vector3 vDelta = targetV - _person.GetComponent<Rigidbody>().velocity;
        vDelta.x = Mathf.Clamp(vDelta.x, -10, 10);
        vDelta.z = Mathf.Clamp(vDelta.z, -10, 10);
        vDelta.y = 0;
        _person.addForce(vDelta, ForceMode.VelocityChange);
    }

    private void applyVelocity(Vector3 myV, Transform relativeTo)
    {
        Vector3 targetV = relativeTo.TransformDirection(myV);
        Vector3 vDelta = targetV - _person.GetComponent<Rigidbody>().velocity;
        vDelta.x = Mathf.Clamp(vDelta.x, -(_person.speed), _person.speed);
        vDelta.z = Mathf.Clamp(vDelta.z, -(_person.speed), _person.speed);
        vDelta.y = 0;
        _person.addForce(vDelta, ForceMode.VelocityChange);
    }

    private void chargeJump(float speed)
    {
        if (_jumpCharge < _MIN_JUMP_CHARGE)
        {
            _jumpCharge = _MIN_JUMP_CHARGE;
        }
        _jumpCharge += .01f + speed;
        _jumpCharge = Mathf.Clamp(_jumpCharge, _MIN_JUMP_CHARGE, _MAX_JUMP_CHARGE);
        Debug.Log("Jump multiplier is " + _jumpCharge);
    }

    private void applyJumpVelocity(float velocity)
    {
        Vector3 jumpVector = new Vector3
        (
            Mathf.Clamp(Input.GetAxis("Horizontal"), _MIN_JUMP_HORIZONTAL_V, _MAX_JUMP_HORIZONTAL_V),
            _person.transform.up.y * velocity,
            Mathf.Clamp(Input.GetAxis("Vertical"), _MIN_JUMP_HORIZONTAL_V, _MAX_JUMP_HORIZONTAL_V)
        );

        _person.addForce(jumpVector, ForceMode.VelocityChange);
        _jumpCharge = _MIN_JUMP_CHARGE;
    }

    private Vector3 getFlockingVector()
    {
        return _person.getFlockingVector();
    }

    private Vector3 getFlockingVector(Vector3 aWeight, Vector3 cWeight, Vector3 sWeight)
    {
        return _person.getFlockingVector(aWeight, cWeight, sWeight);
    }

    private Vector3 getSeekVector(Vector3 target, float speed)
    {
        return _person.getSeekVector(target);
    }

    private Vector3 getFleeVector(Vector3 target, float speed)
    {
        return _person.getFleeVector(target);
    }

    private Vector3 getArriveVector(Vector3 target, float deceleration)
    {
        return _person.getArriveVector(target, deceleration);
    }
}
