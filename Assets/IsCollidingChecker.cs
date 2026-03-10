using UnityEngine;
using UnityEngine.EventSystems;

public class IsCollidingChecker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool isColliding;

    public Color[] colors;  
    private Material _material;

    void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
        _material.color = colors[0];
    }

    //Detect current clicks on the GameObject (the one with the script attached)
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        _material.color = colors[1];
        isColliding = true;
    }

    //Detect if clicks are no longer registering
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        _material.color = colors[0];
        isColliding = false;
    }
}