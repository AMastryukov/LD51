using System;
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

    [Tooltip("How snappy the character movement is")]
    [Range(5f, 20f)]
    [SerializeField] private float movementResponse = 8f;
    [Range(5f, 20f)]
    [SerializeField] private float movementResponseWithBuff = 16f;

    [Tooltip("Max movement speed of the character")]
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float maxSpeedWithBuff = 20f;

    [SerializeField] private float sprintMultiplier = 1.25f;

    [Min(1)]
    [SerializeField] private int sprintStaminaDrainPerSecond = 10;
    [Min(1)]
    [SerializeField] private int sprintStaminaDrainPerSecondWithBuff = 10;

    private bool isSprinting = false;
    private DateTime nextStaminaDrain = DateTime.Now;

    public Vector3 CharacterVelocity { get; private set; }
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
    [Tooltip("Radius of Spehere used to check IsGrounded")]
    [Range(0.1f, 1f)]
    [SerializeField] private float groundCheckRadius = 0.5f;

    [Tooltip("Jump force")]
    [Range(5f, 20f)]
    [SerializeField] private float jumpForce = 8f;
    [Range(5f, 20f)]
    [SerializeField] private float jumpForceWithBuff = 16f;

    [Tooltip("How snappy the character movement is in the air")]
    [Range(0.1f, 2f)]
    [SerializeField] private float airMovementResponseMultiplier = 0.5f;
    [Range(0.1f, 2f)]
    [SerializeField] private float airMovementResponseMultiplierWithBuff = 1f;

    [SerializeField] private LayerMask jumpLayerMask;

    [Range(10f, 40f)]
    [SerializeField] private float gravityForce = 20f;
    [Range(10f, 40f)]
    [SerializeField] private float gravityForceWithBuff = 30f;

    /// Cooldown so we don't double jump
    private readonly float jumpCooldown = 0.1f;
    private float lastJumpedTime = 0f;
    [Tooltip("Whether the player is grounded")]
    public bool IsGrounded;
    #endregion

    #region Components
    private PlayerInputHandler inputHandler;
    private PlayerManager playerManager;
    public CharacterController characterController { get; private set; }
    private PlayerItemManager itemManager;
    private Player player;
    #endregion

    public float MaxSpeed {
        get {
            float valueToReturn = PlayerBuffsManager.Instance.IsBuffActive(Buffs.FasterMoveSpeed) ? maxSpeedWithBuff : maxSpeed;

            if (isSprinting)
            {
                valueToReturn *= sprintMultiplier;
                AttemptToDrainStamina();
            }

            return valueToReturn;
        }
    }
    private float MovementResponse {
        get {
            float valueToReturn = PlayerBuffsManager.Instance.IsBuffActive(Buffs.FasterMoveSpeed) ? movementResponseWithBuff : movementResponse;

            if (isSprinting)
            {
                valueToReturn *= sprintMultiplier;
                AttemptToDrainStamina();
            }

            return valueToReturn;
        }
    }

    private float JumpForce => PlayerBuffsManager.Instance.IsBuffActive(Buffs.HigherJumpHeight) ? jumpForceWithBuff : jumpForce;
    private float AirMovementResponseMultiplier => PlayerBuffsManager.Instance.IsBuffActive(Buffs.HigherJumpHeight) ? airMovementResponseMultiplierWithBuff : airMovementResponseMultiplier;
    private float GravityForce => PlayerBuffsManager.Instance.IsBuffActive(Buffs.HigherJumpHeight) ? gravityForceWithBuff : gravityForce;

    private int SprintStaminaDrainPerSecond => PlayerBuffsManager.Instance.IsBuffActive(Buffs.SlowerStaminaDrain) ? sprintStaminaDrainPerSecond : sprintStaminaDrainPerSecondWithBuff;

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

        itemManager = GetComponent<PlayerItemManager>();
        DebugUtility.HandleErrorIfNullGetComponent(itemManager, this);

        // Components that are not attached to this gameobject
        DebugUtility.HandleErrorIfNullGetComponent(PlayerCamera, this);

        player = GetComponent<Player>();
        DebugUtility.HandleErrorIfNullGetComponent(player, this);
    }

    // Physics updated
    private void FixedUpdate()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ProcessSprintingInput();

        // The one thing we do everytime
        CheckGrounded();

        if (playerManager.CurrentState == PlayerStates.Move)
        {
            HandleCharacterMovement();
            UseItems();
        }
    }

    private void ProcessSprintingInput()
    {
        bool wasSprinting = isSprinting;
        bool isSprintingInput = inputHandler.GetSprintInput() &&
            player.StaminaRegenerativeValue.CurrentValue > 0 &&
            inputHandler.GetMoveInput().magnitude > 0;

        if (isSprintingInput && player.StaminaRegenerativeValue.RegenerationIsActive)
        {
            player.StaminaRegenerativeValue.StopRegeneration();
        }
        else if (!isSprintingInput && wasSprinting && player.StaminaRegenerativeValue.CanStartRegeneration)
        {
            player.StaminaRegenerativeValue.StartRegeneration();
        }

        isSprinting = isSprintingInput;
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
                Vector3 targetVelocity = worldspaceMoveInput * MaxSpeed;
                // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
                CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity, MovementResponse * Time.deltaTime);

                if (enableJump && inputHandler.GetJumpInputDown() && Time.time - lastJumpedTime > jumpCooldown)
                {

                    // start by canceling out the vertical component of our velocity
                    CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);

                    // then, add the jumpSpeed value upwards
                    CharacterVelocity += Vector3.up * JumpForce;

                    // reset the lastJumpTime
                    lastJumpedTime = Time.time;
                }
            }
            else
            {

                // add air acceleration
                CharacterVelocity += worldspaceMoveInput * MovementResponse * AirMovementResponseMultiplier * Time.deltaTime;

                // limit air speed to a maximum, but only horizontally
                float verticalVelocity = CharacterVelocity.y;
                Vector3 horizontalVelocity = Vector3.ProjectOnPlane(CharacterVelocity, Vector3.up);
                horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, MaxSpeed);
                CharacterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

                // apply the gravity to the velocity
                CharacterVelocity += Vector3.down * GravityForce * Time.deltaTime;
            }
            characterController.Move(CharacterVelocity * Time.deltaTime);
        }
    }

    /// <summary>
    /// Handle clicks and item usage when the player is moving
    /// </summary>
    private void UseItems()
    {
        if (inputHandler.GetUseInputDown())
        {
            itemManager.Use(false);
        }
        else if (inputHandler.GetUseInputHeld())
        {
            itemManager.Use(true);
        }
    }

    /// <summary>
    /// Return the bottom of the player capsule
    /// </summary>
    /// <returns>Position of ground relative to player</returns>
    private Vector3 GetCharacterButtcrack()
    {
        return transform.position + Vector3.down * (characterController.height / 2 + 0.1f) + Vector3.up * characterController.radius * groundCheckRadius;
    }

    /// <summary>
    /// Source of truth for whether the character is grounded
    /// </summary>
    private void CheckGrounded()
    {
        Color debugRayColor;

        if (Physics.CheckSphere(GetCharacterButtcrack(), groundCheckRadius, jumpLayerMask))
        {
            IsGrounded = true;
            debugRayColor = Color.green;
        }
        else
        {
            IsGrounded = false;
            debugRayColor = Color.red;
        }

        Debug.DrawLine(transform.position, GetCharacterButtcrack(), debugRayColor, Time.deltaTime);
    }

    private void AttemptToDrainStamina()
    {
        if (DateTime.Now < nextStaminaDrain)
        {
            return;
        }

        int staminaDrain = PlayerBuffsManager.Instance.IsBuffActive(Buffs.SlowerStaminaDrain) ? sprintStaminaDrainPerSecondWithBuff : sprintStaminaDrainPerSecond;
        player.DecrementStamina(staminaDrain);

        nextStaminaDrain = DateTime.Now.AddSeconds(1);
    }
}
