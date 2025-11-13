using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    private PlayerControls input;   // generated clas from inputSystem_actions from file
    private Rigidbody rb;

    public float moveSpeed = 10f;
    public float jumpForce = 7f;
    public int maxJumps = 1;
    public GameObject EndScreen;
    // private int jumpCount = 0;
    private bool isGrounded = true;



    // Start instead is called before the first frame update and only if object is active
    void Awake() // created once when the object is initialized
    {
        input = new PlayerControls();    // create instance of the generated class
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable() => input.Player.Enable(); // enable the action map when object is active
    void OnDisable() => input.Player.Disable(); // disable the action map when object is destroyed
    void Start()
    {
        // Debug.Log("PlayerController is active!");
        EndScreen.SetActive(false);
    }

    private void HandleMovement()
    {
        Vector2 key = input.Player.Move.ReadValue<Vector2>(); // reads value as 2dvector
        float z = 0f;
        Vector3 movement = new Vector3(key.x, z, key.y) * moveSpeed;
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);


    }
    private void HandleJump()
    {
        if (input.Player.Jump.triggered && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            Debug.Log("Player is jumping:" + isGrounded);
        }
    }

    void Update() // at every rendered frame
    {
        HandleMovement();
        HandleJump();
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool touchedLava = collision.gameObject.CompareTag("Lava");
        bool touchedGround = collision.gameObject.CompareTag("Ground");

        Debug.Log("PlayerController on collision is active!");
        if (touchedGround)
        {
            Debug.Log("Player touched ground");
            isGrounded = true;
        }

        if (touchedLava)
        {
            Debug.Log("You are dead!");
            EndScreen.SetActive(true);
            Destroy(gameObject);
            OnDisable();
        }
    }
}
