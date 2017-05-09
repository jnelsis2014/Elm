using UnityEngine;
using System.Collections;


public class AutoTransparency : MonoBehaviour
{
    private Color _oldColor;
    private Shader _oldShader;
    private float _transparency = 0.3f;
    private const float _targetTransparency = 0.3f;
    private const float _fallOff = 0.1f; // returns to 100% in 0.1 sec


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