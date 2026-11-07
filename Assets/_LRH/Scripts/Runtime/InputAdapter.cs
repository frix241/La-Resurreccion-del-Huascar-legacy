using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace LRH.Runtime
{
    public readonly struct ShipCommand
    {
        public readonly float Forward;
        public readonly float Turn;
        public readonly bool Fire;
        public readonly Vector2 Aim;
        public ShipCommand(float forward, float turn, bool fire, Vector2 aim)
        { Forward = forward; Turn = turn; Fire = fire; Aim = aim; }
    }

    public interface IInputSource { ShipCommand Read(Camera camera); }

    // Adapter: only this class knows the Input System's concrete keyboard/mouse API.
    public sealed class UnityInputAdapter : IInputSource
    {
        public ShipCommand Read(Camera camera)
        {
#if ENABLE_INPUT_SYSTEM
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            float forward = keyboard == null ? 0 : (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1 : 0)
                - (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? 1 : 0);
            float turn = keyboard == null ? 0 : (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1 : 0)
                - (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1 : 0);
            Vector2 aim = mouse == null ? Vector2.up : camera.ScreenToWorldPoint(mouse.position.ReadValue());
            return new ShipCommand(forward, turn, mouse != null && mouse.leftButton.isPressed, aim);
#else
            // Unity may open a fresh project with the legacy backend until Player Settings are saved.
            float forward = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ? 1 : 0)
                - (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? 1 : 0);
            float turn = (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? 1 : 0)
                - (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1 : 0);
            Vector2 aim = camera.ScreenToWorldPoint(Input.mousePosition);
            return new ShipCommand(forward, turn, Input.GetMouseButton(0), aim);
#endif
        }
    }
}
