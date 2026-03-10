using UnityEngine;
using UnityEngine.InputSystem;

public class ColorInteract : MonoBehaviour
{
    public Color[] colors;
    private Material _material;
    private bool _isHolding = false;

    void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
        _material.color = colors[0];
    }

    void Update()
    {
                    _material.color = (_material.color == colors[0]) ? colors[1] : colors[0];
    }
}