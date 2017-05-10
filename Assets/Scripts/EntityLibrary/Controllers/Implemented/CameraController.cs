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

    private Vector3 _targetOffset;
    public Vector3 targetOffset
    {
        get
        {
            return _targetOffset;
        }
    }

    private float _offset = 5f;
    public float offset
    {
        get
        {
            return _offset;
        }
    }

    private float _distance = 5.0f;
    public float distance
    {
        get
        {
            return _distance;
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

    private float _distanceMin = .5f;
    public float distanceMin
    {
        get
        {
            return _distanceMin;
        }
    }

    private float _distanceMax = 15f;
    public float distanceMax
    {
        get
        {
            return _distanceMax;
        }
    }

    private float _x = 0.0f;
    public float x
    {
        get
        {
            return _x;
        }
    }

    private float _y = 0.0f;
    public float y
    {
        get
        {
            return _y;
        }
    }

    private void Awake()
    {
        _target = GameObject.FindGameObjectWithTag("player").transform;
    }
    
    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        _x = angles.y;
        _y = angles.x;

        if (_target)
        {
            _targetCenter = _target.transform.position;
            _targetOffset = _target.right * _offset;
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (_target)
        {
            _x += Input.GetAxis("Mouse X") * _xSpeed * _distance * 0.02f;
            _y -= Input.GetAxis("Mouse Y") * _ySpeed * 0.02f;

            _y = ClampAngle(_y, _yMinLimit, _yMaxLimit);
            
            Quaternion rotation = Quaternion.Euler(_y, _x, 0);

            _distance = Mathf.Clamp(_distance - Input.GetAxis("Mouse ScrollWheel") * 5, _distanceMin, _distanceMax);


            RaycastHit hit;
            if (Physics.Linecast(_target.position, transform.position, out hit))
            {
                GameObject obstruction = hit.collider.gameObject;
                if (obstruction.GetComponent<Plane>() != null)
                {
                    _distance -= hit.distance;
                }
                else
                {
                    AutoTransparency AT = obstruction.GetComponent<AutoTransparency>();
                    if (AT == null)
                    {
                        AT = obstruction.AddComponent<AutoTransparency>();
                    }

                    obstruction.GetComponent<AutoTransparency>().BeTransparent();
                }
            }

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -_distance);
            Vector3 position = rotation * negDistance + _target.position;

            transform.rotation = rotation;
            transform.position = position;
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