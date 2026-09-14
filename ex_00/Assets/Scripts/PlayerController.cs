using UnityEngine;
using UnityEngine.InputSystem; 


public class PlayerController : MonoBehaviour
{


    // [Header("Input System")]
    private PlayerControls input;

    // References to ease access to spec actions
    private InputAction moveAction;
    private InputAction jumpAction;
    
    [Header("Physics")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float inputSpeed;
    [SerializeField] private float jumpForce;
    private bool isGrounded = true;

    [Header ("Other")] // to be moved to their own manager when prototyping is extended
    [SerializeField] private GameObject EndScreen; // with retry button to restart 
    private AudioSource audioSource; // the ball jump/land sound 

    void Awake()
    {
        input = new PlayerControls();
        moveAction = input.Player.Move;
        jumpAction = input.Player.Jump;
    }

    private Vector2 inputDirection;
    void OnEnable() => input.Player.Enable();
    void OnDisable() => input.Player.Disable();
    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // 
    }
    void Update()
    {
        inputDirection = moveAction.ReadValue<Vector2>();
        HandleJump();
    }

    void FixedUpdate() // good to handle logic that can be heavy on the CPU (like physics calculations)
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 movement = new Vector3(inputDirection.x, 0f, inputDirection.y) * inputSpeed; //conversion (vec 3 z is equivalent to vec2 y)
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z); // y kept at its current value since not applies
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


    private void OnCollisionEnter(Collision collision)
    {
        bool touchedLava = collision.gameObject.CompareTag("Lava");
        bool touchedGround = collision.gameObject.CompareTag("Ground");

        Debug.Log("PlayerController on collision is active!");
        if (touchedGround)
        {
            audioSource.Play(); 
            isGrounded = true;
        }

        if (touchedLava)
        {
            EndScreen.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }


    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
