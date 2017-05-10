using UnityEngine;

public class AutoTransparency : MonoBehaviour
{

    private Color _oldColor;
    public Color OldColor
    {
        get
        {
            if (_oldColor != null)
                return _oldColor;
            else
                return Color.white;
        }
    }

    private Shader _oldShader;
    public Shader OldShader
    {
        get
        {
            if (_oldShader != null)
                return _oldShader;
            else
            {
                return new Shader();
            }
        }
    }

    private float _transparency = 0.3f;
    public float Transparency
    {
        get
        {
            return _transparency;
        }
    }

    private const float _targetTransparency = 0.3f;
    public static float TargetTransparency
    {
        get
        {
            return _targetTransparency;
        }
    }

    private const float _fallOff = 0.1f; // returns to 100% in 0.1 sec
    public static float FallOff
    {
        get
        {
            return _fallOff;
        }
    }

    public void BeTransparent()
    {
        // reset the transparency;
        _transparency = _targetTransparency;
        if (_oldShader == null)
        {
            _oldShader = GetComponent<Renderer>().material.shader;
            _oldColor = GetComponent<Renderer>().material.color;
            GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
        }
    }

    void Update()
    {
        if (_transparency < 1.0f)
        {
            Color C = GetComponent<Renderer>().material.color;
            C.a = _transparency;
            GetComponent<Renderer>().material.color = C;
        }
        else
        {
            // Reset the shader
            GetComponent<Renderer>().material.shader = _oldShader;
            GetComponent<Renderer>().material.color = _oldColor;
            // And remove this script
            Destroy(this);
        }
        _transparency += ((1.0f - _targetTransparency) * Time.deltaTime) / _fallOff;
    }
}