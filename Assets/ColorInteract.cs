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
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Mouse.current.leftButton.isPressed)
        {
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
            {
                if (!_isHolding)
                {
                    _isHolding = true;
                    _material.color = (_material.color == colors[0]) ? colors[1] : colors[0];
                }
            }
        }
        else
        {
            _isHolding = false;
        }
    }
}