using UnityEngine.InputSystem;

// Input actions shared by the hero and the title / game over screen
public static class GameInput {

    static InputAction s_Move;
    static InputAction s_Dash;

    public static InputAction Move {
        get {
            if (s_Move == null) {
                s_Move = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");
                // Digital mode keeps keyboard diagonals at (1, 1)
                s_Move.AddCompositeBinding("2DVector(mode=1)")
                    .With("Up", "<Keyboard>/upArrow")
                    .With("Down", "<Keyboard>/downArrow")
                    .With("Left", "<Keyboard>/leftArrow")
                    .With("Right", "<Keyboard>/rightArrow");
                s_Move.AddCompositeBinding("2DVector(mode=1)")
                    .With("Up", "<Keyboard>/w")
                    .With("Down", "<Keyboard>/s")
                    .With("Left", "<Keyboard>/a")
                    .With("Right", "<Keyboard>/d");
                s_Move.AddBinding("<Gamepad>/leftStick");
                s_Move.AddBinding("<Gamepad>/dpad");
                s_Move.Enable();
            }
            return s_Move;
        }
    }

    public static InputAction Dash {
        get {
            if (s_Dash == null) {
                s_Dash = new InputAction("Dash", InputActionType.Button);
                s_Dash.AddBinding("<Keyboard>/space");
                s_Dash.AddBinding("<Gamepad>/buttonNorth");
                s_Dash.Enable();
            }
            return s_Dash;
        }
    }
}
