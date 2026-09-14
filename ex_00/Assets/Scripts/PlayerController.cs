using UnityEngine;
using UnityEngine.InputSystem; // The new Input System package
using System.Collections;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using System;


public class PlayerController : MonoBehaviour
{
    private PlayerControls input;   // generated clas from inputSystem_actions from file
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float inputSpeed;
    [SerializeField] private float jumpForce;

    [SerializeField] private GameObject EndScreen;

    private InputAction moveAction;
    private AudioSource audioSource;
    private bool isGrounded = true;



    // Start instead is called before the first frame update and only if object is active
    void Awake() // created once when the object is initialized
    {
        input = new PlayerControls();    // create instance of the generated class
        moveAction = input.Player.Move;
    }

    private Vector2 inputDirection;
    void OnEnable() => input.Player.Enable();
    void OnDisable() => input.Player.Disable();
    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // moght be useful in awake?
    }
    void Update() // called at frame therefore expensive to handle 
    {
        inputDirection = moveAction.ReadValue<Vector2>(); // reads x and y input from bi-dimensional controller
        HandleJump();
    }

    void FixedUpdate() // using FixedUpdate uses fixes intervals which is better to handle costly calculations that are heavy on the CPU (like physics calculations)
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 movement = new Vector3(inputDirection.x, 0f, inputDirection.y) * inputSpeed; // vec3 is x y z but vec2 is x y, in a 3d space our z axis is the "vertical 2d" direction
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z); // applies speed to each considered axis (y is kept at its current value since jumping axis)
    }
    private void HandleJump() // jump i sthe y axis
    {
        if (input.Player.Jump.triggered && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            Debug.Log("Player is jumping:" + isGrounded);
        }
        // add landing physics
    }


    private void OnCollisionEnter(Collision collision)
    {
        bool touchedLava = collision.gameObject.CompareTag("Lava");
        bool touchedGround = collision.gameObject.CompareTag("Ground");

        Debug.Log("PlayerController on collision is active!");
        if (touchedGround)
        {
            audioSource.Play(); // bounce sound
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
