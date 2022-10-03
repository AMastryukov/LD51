using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInputHandler : MonoBehaviour
{
    public const string AXIS_VERTICAL = "Vertical";
    public const string AXIS_HORIZONTAL = "Horizontal";
    public const string AXIS_MOUSE_Y = "Mouse Y";
    public const string AXIS_MOUSE_X = "Mouse X";
    public const string BUTTON_JUMP = "Jump";
    public const string BUTTON_FIRE1 = "Fire1";
    public const string BUTTON_FIRE2 = "Fire2";
    public const string BUTTON_FIRE3 = "Fire3";
    public const string BUTTON_INTERACT = "Interact";
    public const string BUTTON_SPRINT = "Sprint";

    [SerializeField] private bool lockCursor = true;

    public void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public Vector3 GetMoveInput()
    {


        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0f,
            Input.GetAxisRaw("Vertical"));

        // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
        move = Vector3.ClampMagnitude(move, 1);

        return move;

    }

    public bool GetJumpInputDown()
    {
        return Input.GetButtonDown(BUTTON_JUMP);
    }

    public float GetLookInputsHorizontal()
    {
        return Input.GetAxisRaw(AXIS_MOUSE_X);
    }

    public float GetLookInputsVertical()
    {
        return Input.GetAxisRaw(AXIS_MOUSE_Y);
    }

    public bool GetUseInputDown()
    {
        return Input.GetButtonDown(BUTTON_FIRE1);
    }

    public bool GetUseInputHeld()
    {
        return Input.GetButton(BUTTON_FIRE1);
    }

    public bool GetInteractInput()
    {
        return Input.GetButton(BUTTON_INTERACT);
    }

    public bool GetSprintInput()
    {
        return Input.GetButton(BUTTON_SPRINT);
    }
}
