using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region References
    [Header("References")]
    [Tooltip("Reference to the main camera used for the player")]
    public Camera PlayerCamera;
    #endregion

    #region Movement
    [Header("Movement")]
    [Tooltip("Max movement speed of the character")]
    [Range(5f, 20f)]
    [SerializeField] private float maxSpeed = 10f;
    [Tooltip("How snappy the character movement is")]
    [Range(5f, 20f)]
    [SerializeField] private float movementResponse = 8f;
    private Vector3 characterVelocity;
    #endregion

    #region Look
    [Header("Look")]
    [Tooltip("Look sensitivity")]
    [Range(1f, 20f)]
    [SerializeField] private float lookSensitivity = 10f;
    [Range(0.1f, 1f)]
    [Tooltip("Vertical axis sensitivity")]
    [SerializeField] private float lookSensitivityVMultiplier = 0.5f;
    private float cameraVAngle = 0f;
    #endregion

    #region Jumping
    [Header("Jumping")]
    [Tooltip("Uncheck to disable jumping")]
    [SerializeField] private bool enableJump = true;
    [Tooltip("Jump height")]
    [Range(5f, 20f)]
    [SerializeField] private float jumpForce = 8f;
    [Tooltip("Max movement speed of the character in the air")]
    [Range(0.1f, 2f)]
    [SerializeField] private float airMaxSpeedMultiplier = 0.5f;
    [Tooltip("How snappy the character movement is in the air")]
    [Range(0.1f, 2f)]
    [SerializeField] private float airMovementResponseMultiplier = 0.5f;
    [SerializeField] private LayerMask jumpLayerMask;
    [Range(10f, 40f)]
    [SerializeField] private float gravityForce = 20f;
    /// Cooldown so we don't double jump
    private readonly float jumpCooldown = 0.1f;
    private float lastJumpedTime = 0f;
    [Tooltip("Whether the player is grounded")]
    public bool IsGrounded;
    #endregion

    #region Components
    private PlayerInputHandler inputHandler;
    private PlayerManager playerManager;
    private CharacterController characterController;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        // Component that are on this gameobject
        inputHandler = GetComponent<PlayerInputHandler>();
        DebugUtility.HandleErrorIfNullGetComponent(inputHandler, this);

        playerManager = GetComponent<PlayerManager>();
        DebugUtility.HandleErrorIfNullGetComponent(playerManager, this);

        characterController = GetComponent<CharacterController>();
        DebugUtility.HandleErrorIfNullGetComponent(characterController, this);

        // Components that are not attached to this gameobject
        DebugUtility.HandleErrorIfNullGetComponent(PlayerCamera, this);
    }

    // Update is called once per frame
    void Update()
    {
        // The one thing we do everytime
        CheckGrounded();

        if (playerManager.CurrentState == PlayerStates.Move)
        {
            HandleCharacterMovement();
        }


    }

    /// <summary>
    /// Handle all the movement logic here when PlayerStates.Move.
    /// </summary>
    private void HandleCharacterMovement()
    {

        // horizontal character rotation
        {
            // rotate the transform with the input speed around its local Y axis
            transform.Rotate(
                new Vector3(0f, (inputHandler.GetLookInputsHorizontal() * lookSensitivity),
                    0f), Space.Self);
        }

        // vertical camera rotation
        {
            // add vertical inputs to the camera's vertical angle
            cameraVAngle -= inputHandler.GetLookInputsVertical() * lookSensitivity * lookSensitivityVMultiplier;

            // limit the camera's vertical angle to min/max
            cameraVAngle = Mathf.Clamp(cameraVAngle, -89f, 89f);

            // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
            PlayerCamera.transform.localEulerAngles = new Vector3(cameraVAngle, 0, 0);
        }



        // character movement handling
        {
            Vector3 worldspaceMoveInput = transform.TransformVector(inputHandler.GetMoveInput());

            if (IsGrounded)
            {
                Vector3 targetVelocity = worldspaceMoveInput * maxSpeed;
                // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
                characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity, movementResponse * Time.deltaTime);

                if (enableJump && inputHandler.GetJumpInputDown() && Time.time - lastJumpedTime > jumpCooldown)
                {

                    // start by canceling out the vertical component of our velocity
                    characterVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);

                    // then, add the jumpSpeed value upwards
                    characterVelocity += Vector3.up * jumpForce;

                    // reset the lastJumpTime
                    lastJumpedTime = Time.time;
                }
            }
            else
            {

                // add air acceleration
                characterVelocity += worldspaceMoveInput * movementResponse * airMovementResponseMultiplier * Time.deltaTime;

                // limit air speed to a maximum, but only horizontally
                float verticalVelocity = characterVelocity.y;
                Vector3 horizontalVelocity = Vector3.ProjectOnPlane(characterVelocity, Vector3.up);
                horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxSpeed * airMaxSpeedMultiplier);
                characterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

                // apply the gravity to the velocity
                characterVelocity += Vector3.down * gravityForce * Time.deltaTime;
            }
            characterController.Move(characterVelocity * Time.deltaTime);
        }
    }

    /// <summary>
    /// Dynamically calculate the ground check ray length based on the height of the player
    /// </summary>
    /// <returns>Length of raycast to the ground</returns>
    private float GetRayLength()
    {
        return characterController.height / 2 + 0.1f;
    }

    /// <summary>
    /// Source of truth for whether the character is grounded
    /// </summary>
    private void CheckGrounded()
    {
        Color debugRayColor;
        float rayLength = GetRayLength();

        if (Physics.Raycast(transform.position, Vector3.down, rayLength, jumpLayerMask))
        {
            IsGrounded = true;
            debugRayColor = Color.green;
        }
        else
        {
            IsGrounded = false;
            debugRayColor = Color.red;
        }

        //if (inputHandler.GetJumpInputDown())
        //{
        Debug.DrawLine(transform.position, transform.position + Vector3.down * rayLength, debugRayColor, 0.1f);
        //}
    }
}
