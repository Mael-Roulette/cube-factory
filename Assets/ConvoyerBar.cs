using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public Vector3 beltDirection = new Vector3(0f, 0f, 1f);
    public float beltSpeed = 2.0f;

    private void OnTriggerEnter(Collider other)
    {
        Moving moving = other.GetComponent<Moving>();
        if (moving != null)
        {
            moving.moveDirection = beltDirection;
            moving.speed = beltSpeed;
            moving.isMoving = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Moving moving = other.GetComponent<Moving>();
        if (moving != null)
        {
            moving.isMoving = false;
        }
    }
}