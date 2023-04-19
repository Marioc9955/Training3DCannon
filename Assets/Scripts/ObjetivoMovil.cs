using UnityEngine;

public class ObjetivoMovil : MonoBehaviour
{
    public Vector3 velocity;
    public float speed;
    [SerializeField] private CannonAgent agent;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        NewVelocity();
    }

    void NewVelocity()
    {
        // Generate a random velocity between 1 and 4.44
        speed = Random.Range(1f, 4.44f);

        // Generate a random direction in the XZ plane
        float angle = Random.Range(0f, Mathf.PI * 2f);
        velocity = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * speed;

        // Rotate the object to face the velocity direction
        transform.rotation = Quaternion.LookRotation(velocity);
    }

    private void FixedUpdate()
    {
        rb.velocity = velocity;
    }

    private void OnEnable()
    {
        Start();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Proyectil") && !collision.gameObject.CompareTag("Suelo"))
        {
            rb.velocity = Vector3.zero;
            agent.ChangeTarget();
            this.enabled = false;
            gameObject.SetActive(false);
        }
    }
}
