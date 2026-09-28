using UnityEngine;
using UnityEngine.InputSystem;

namespace DungeonTrace.Player
{
    /// <summary>Owns device polling and exposes one frame command to gameplay components.</summary>
    public sealed class PlayerInputReader : MonoBehaviour
    {
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction fireAction;
        private InputAction alternateFireAction;
        private InputAction switchWeaponAction;
        private InputAction interactAction;
        private InputAction pauseAction;

        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool FirePressedThisFrame { get; private set; }
        public bool FireReleasedThisFrame { get; private set; }
        public bool AlternateFirePressedThisFrame { get; private set; }
        public bool SwitchWeaponPressedThisFrame { get; private set; }
        public bool InteractPressedThisFrame { get; private set; }
        public bool PausePressedThisFrame { get; private set; }

        private void Awake()
        {
            moveAction = new InputAction("Move", InputActionType.Value, "<Gamepad>/leftStick");
            moveAction.AddCompositeBinding("2DVector").With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s").With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
            lookAction = new InputAction("Look", InputActionType.Value, "<Pointer>/delta");
            lookAction.AddBinding("<Gamepad>/rightStick");
            fireAction = new InputAction("Fire", InputActionType.Button, "<Mouse>/leftButton");
            fireAction.AddBinding("<Gamepad>/rightTrigger");
            alternateFireAction = new InputAction("AlternateFire", InputActionType.Button, "<Mouse>/rightButton");
            alternateFireAction.AddBinding("<Gamepad>/leftTrigger");
            switchWeaponAction = new InputAction("SwitchWeapon", InputActionType.Button, "<Keyboard>/q");
            switchWeaponAction.AddBinding("<Gamepad>/dpad/right");
            interactAction = new InputAction("Interact", InputActionType.Button, "<Keyboard>/e");
            pauseAction = new InputAction("Pause", InputActionType.Button, "<Keyboard>/escape");
            pauseAction.AddBinding("<Gamepad>/start");
        }

        private void OnEnable() { moveAction.Enable(); lookAction.Enable(); fireAction.Enable(); alternateFireAction.Enable(); switchWeaponAction.Enable(); interactAction.Enable(); pauseAction.Enable(); }
        private void OnDisable() { moveAction.Disable(); lookAction.Disable(); fireAction.Disable(); alternateFireAction.Disable(); switchWeaponAction.Disable(); interactAction.Disable(); pauseAction.Disable(); }
        private void OnDestroy() { moveAction.Dispose(); lookAction.Dispose(); fireAction.Dispose(); alternateFireAction.Dispose(); switchWeaponAction.Dispose(); interactAction.Dispose(); pauseAction.Dispose(); }

        private void Update()
        {
            Move = moveAction.ReadValue<Vector2>();
            Look = lookAction.ReadValue<Vector2>();
            FirePressedThisFrame = fireAction.WasPressedThisFrame();
            FireReleasedThisFrame = fireAction.WasReleasedThisFrame();
            AlternateFirePressedThisFrame = alternateFireAction.WasPressedThisFrame();
            SwitchWeaponPressedThisFrame = switchWeaponAction.WasPressedThisFrame();
            InteractPressedThisFrame = interactAction.WasPressedThisFrame();
            PausePressedThisFrame = pauseAction.WasPressedThisFrame();
        }
    }
}
