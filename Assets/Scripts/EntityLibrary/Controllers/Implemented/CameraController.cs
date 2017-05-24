using UnityEngine;

public class CameraController : MonoBehaviour
{

    private Transform _target;
    public Transform target
    {
        get
        {
            return _target;
        }
    }
    
    private Vector3 _targetCenter;
    public Vector3 targetCenter
    {
        get
        {
            return _targetCenter;
        }
    }

    private float _horizontalOffset = 1f;
    public float horizontallOffset
    {
        get
        {
            return _horizontalOffset;
        }
    }

    private float _verticalOffset = 1f;
    public float verticalOffset
    {
        get
        {
            return _verticalOffset;
        }
    }

    private float _dist = 5.0f;
    public float distance
    {
        get
        {
            return _dist;
        }
    }

    private float _xSpeed = 120.0f;

    public float xSpeed
    {
        get
        {
            return _xSpeed;
        }
    }

    private float _ySpeed = 120.0f;
    public float ySpeed
    {
        get
        {
           return _ySpeed;
        }
    }

    private float _yMinLimit = -20f;
    public float yMinLimit
    {
        get
        {
            return _yMinLimit;
        }
    }

    private float _yMaxLimit = 80f;
    public float yMaxLimit
    {
        get
        {
            return _yMaxLimit;
        }
    }

    private float _distMin = .5f;
    public float distanceMin
    {
        get
        {
            return _distMin;
        }
    }

    private float _distMax = 15f;
    public float distanceMax
    {
        get
        {
            return _distMax;
        }
    }

    private float _yaw = 0.0f;
    public float x
    {
        get
        {
            return _yaw;
        }
    }

    private float _pitch = 0.0f;
    public float y
    {
        get
        {
            return _pitch;
        }
    }

    private void Awake()
    {
        _target = GameObject.FindGameObjectWithTag("player").transform;
    }
    
    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles; //get the rotation of the camera around the global axes
        _yaw = angles.y;
        _pitch = angles.x;

        Cursor.lockState = CursorLockMode.Locked; //lock the cursor to enable mouse movement input without
                                                  //leaving the window
    }

    void LateUpdate()
    {
        if (_target)
        {
            _yaw += Input.GetAxis("Mouse X") * _xSpeed * _dist * 0.02f;   //increment the cameras yaw
                                                                          //based on mouse input

            _pitch -= Input.GetAxis("Mouse Y") * _ySpeed * 0.02f;         //increment the cameras pitch
                                                                          //based on mouse input

            _pitch = ClampAngle(_pitch, _yMinLimit, _yMaxLimit);          //clamp the pitch so that
                                                                          //it cannot fall outside a maximum
                                                                          //and minimum pitch
            
            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);      //create a new rotation using pitch and yaw,
                                                                          //with a rotation of 0 around the z axis. 

            _dist = Mathf.Clamp(_dist - Input.GetAxis("Mouse ScrollWheel") * 5, _distMin, _distMax);
            //clamp the zoom distance so that it cannot exceed a minimum or maximum distance

            Debug.Log(Input.GetAxis("Mouse ScrollWheel"));
            //distance already initialized to a default value. value is subtracted by mouse scroll 
            //wheel input times five, clamped between _distMin and _distMax

            RaycastHit hit;
            if (Physics.Linecast(_target.position, transform.position, out hit)) //check for intersections
                                                                                 //along vector between
                                                                                 //camera and target
            {
                GameObject obstruction = hit.collider.gameObject;
                if (obstruction.GetComponent<Plane>() != null) //if the object hit is a plane, new camera
                                                               //distance is current distance minus the
                                                               //the distance where the plane intersects
                                                               //the camera/target vector
                {
                    _dist -= hit.distance;
                }
                else //otherwise, instantiate an auto transparency script, attach to GO, and call
                     //BeTranparent().
                {
                    AutoTransparency AT = obstruction.GetComponent<AutoTransparency>();
                    if (AT == null)
                    {
                        AT = obstruction.AddComponent<AutoTransparency>();
                    }

                    obstruction.GetComponent<AutoTransparency>().BeTransparent();
                }
            }

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -_dist); //create a new vector representing
                                                                   //the negative value of the distance
                                                                   //to the target

            Vector3 position = rotation * negDistance + _target.position;
            //rotate the negative distance vector (pointing at the target) by the current rotation, 
            //and add the targets position then assign this to the cameras current position to
            //place the target at the center of the forward vector of the new rotation

            transform.rotation = rotation; //assign the rotation to the camera
            transform.position = position + (transform.right * _horizontalOffset) + (transform.up * verticalOffset); //assign the position to the camera
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}