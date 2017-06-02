using UnityEngine;
using FluentBehaviourTree;
using System.Collections.Generic;

public class PersonController : MovingEntityController
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

    private Person _movingEnity;
    public Person movingEntity
    {
        get
        {
            return _movingEnity;
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

    private MovingEntityPoint _movingEntityPoint;
    public MovingEntityPoint movingEntityPoint
    {
        get
        {
            return _movingEntityPoint;
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
            Debug.Log(movingEntity.instanceName + "'s controller attempted to access the main cameras control script, but none was assigned.");
        }

        if ((_movingEnity = GetComponent<Person>()) == null)
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

        if (_movingEnity.getOccupiedPoint() != null)
        {
            _movingEntityPoint = movingEntity.getOccupiedPoint();
        }

        movingEntity.GetComponent<Rigidbody>().freezeRotation = true;
        if (tag == "player")
            movingEntity.isPlayerControlled = true;
    }

    // Use this for initialization
    void Start() {
        _movementTarget = transform.position;
        BehaviourTreeBuilder builder = new BehaviourTreeBuilder();
        _behaviourTree = builder
        .Selector("PersonBehavior")
        .End()
        .Build();
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void FixedUpdate()
    {
        if (movingEntity.isPlayerControlled == true)
            getInputs();
        else
        {
            _behaviourTree.Tick(new TimeData(Time.deltaTime));   
        }

        if (movingEntity.GetComponent<Rigidbody>().velocity != Vector3.zero && movingEntity.isGrounded)
        {
            movingEntity.heading = new Vector3(movingEntity.velocity.x, 0, movingEntity.velocity.z);
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

        if (movingEntity.isPlayerControlled == true)
        {
            //movement
            if (movingEntity.isGrounded)
            {
                applyVelocity
                (
                    movementVelocity * movingEntity.maxSpeed,
                    cameraTransform
                );
            }

            if (isJumpHeld)
            {
                chargeJump(.1f);
            }
            else if (isJumpUp && movingEntity.isGrounded)
            {
                applyJumpVelocity(_jumpCharge);
            }

            //actions
            if (_movingEnity.getOccupiedPoint() != null) //if there is an occupied point
            {
                if (_movingEntityPoint != null && _movingEntityPoint.occupant != null) //if the current point selected is occupied
                {
                    if (isReleaseDown) //if attack and switch are held simulaneously
                    {
                        _movingEnity.removeIHoldable(_movingEntityPoint.occupant);
                        _movingEntityPoint.drop();
                    }
                    else if (isSwitchDown)
                    {
                        _movingEntityPoint = _movingEnity.getNextOccupiedPoint(_movingEntityPoint);
                    }
                    else if (isAttackHeld || isTossHeld) //if attach is held down
                    {
                        //set camera to over the shoulder mode
                        _movingEntityPoint.aim();
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
                            _movingEntityPoint.toss(hit.point);
                        }
                        else
                        {
                            Debug.DrawLine(ray.origin, hit.point, Color.yellow, 3f);
                            Debug.Log("Did not Hit");
                        }
                    }
                }
                else if (_movingEnity.getNextOccupiedPoint(_movingEntityPoint) != null) //if there is another point with an occupant
                                                                             //and the current point selected is NOT occupied
                {
                    _movingEntityPoint = movingEntity.getNextOccupiedPoint(_movingEntityPoint); //set the active point to the next occupied point
                }

                //lock on
                if (isLockDown)
                {
                    //toggle lockon
                    //if lockon = true, lock on to movingEntity closest to the center of the screen
                    //else, return camera to free movement
                }
            }

            //interaction
            if (isInteractDown)
            {
                IInteractable interactable = _movingEnity.inInteractionRange;
                if (interactable != null)
                    interactable.interact(_movingEnity);
                else
                    Debug.Log("There were no interactables in the " + _movingEnity.instanceName + "'s interaction range.");
            }
        }
    }

    private void applyVelocity(Vector3 myV)
    {
        Vector3 targetV = myV;
        Vector3 vDelta = targetV - movingEntity.GetComponent<Rigidbody>().velocity;
        vDelta.x = Mathf.Clamp(vDelta.x, -movingEntity.maxSpeed, movingEntity.maxSpeed);
        vDelta.z = Mathf.Clamp(vDelta.z, -movingEntity.maxSpeed, movingEntity.maxSpeed);
        vDelta.y = 0;
        movingEntity.addForce(vDelta, ForceMode.VelocityChange);
    }

    private void applyVelocity(Vector3 myV, Transform relativeTo)
    {
        Vector3 targetV = relativeTo.TransformDirection(myV);
        Vector3 vDelta = targetV - movingEntity.GetComponent<Rigidbody>().velocity;
        vDelta.x = Mathf.Clamp(vDelta.x, -(movingEntity.maxSpeed), movingEntity.maxSpeed);
        vDelta.z = Mathf.Clamp(vDelta.z, -(movingEntity.maxSpeed), movingEntity.maxSpeed);
        vDelta.y = 0;
        movingEntity.addForce(vDelta, ForceMode.VelocityChange);
    }

    private void chargeJump(float chargeSpeed)
    {
        if (_jumpCharge < _MIN_JUMP_CHARGE)
        {
            _jumpCharge = _MIN_JUMP_CHARGE;
        }
        _jumpCharge += .01f + chargeSpeed;
        _jumpCharge = Mathf.Clamp(_jumpCharge, _MIN_JUMP_CHARGE, _MAX_JUMP_CHARGE);
        Debug.Log("Jump multiplier is " + _jumpCharge);
    }

    private void applyJumpVelocity(float velocity)
    {
        Vector3 jumpVector = new Vector3
        (
            Mathf.Clamp(Input.GetAxis("Horizontal"), _MIN_JUMP_HORIZONTAL_V, _MAX_JUMP_HORIZONTAL_V),
            movingEntity.transform.up.y * velocity,
            Mathf.Clamp(Input.GetAxis("Vertical"), _MIN_JUMP_HORIZONTAL_V, _MAX_JUMP_HORIZONTAL_V)
        );

        movingEntity.addForce(jumpVector, ForceMode.VelocityChange);
        _jumpCharge = _MIN_JUMP_CHARGE;
    }

}
