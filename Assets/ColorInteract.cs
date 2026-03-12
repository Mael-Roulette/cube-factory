using UnityEngine;
using UnityEngine.EventSystems;

public class ColorInteract : MonoBehaviour, IPointerDownHandler
{
    private static ColorInteract _activeButton = null;
    public static Color SelectedColor { get; private set; } = Color.white;

    public Color[] colors; // [0] = inactif, [1] = actif
    private Material _material;

    void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
        _material.color = colors[0];
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (_activeButton != null && _activeButton != this)
            _activeButton._material.color = _activeButton.colors[0];

        _activeButton = this;
        _material.color = colors[1];
        SelectedColor = colors[0]; // Mémorise la couleur choisie
    }

    public bool IsActive => _activeButton == this;
}