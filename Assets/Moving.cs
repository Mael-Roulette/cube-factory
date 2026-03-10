using UnityEngine;
public class Moving : MonoBehaviour
{
    public float speed = 2.0f;
    public Vector3 moveDirection = Vector3.zero;
    public bool isMoving = false;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (isMoving)
        {
            rb.linearVelocity = new Vector3(
                moveDirection.x * speed,
                rb.linearVelocity.y,
                moveDirection.z * speed
            );
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("conveyor"))
        {
            moveDirection = collision.gameObject.transform.right;
            isMoving = true;
            speed = -2f;
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("conveyor"))
        {
            isMoving = false;
        }
    }
}