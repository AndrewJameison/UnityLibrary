using UnityEngine;
using UnityEngine.InputSystem;

// Q: when should movement be restricted and how?
// Q: How can I tie a better animation system into my movement system?

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private float _pitch;
    private float _yaw;
    private Vector3 _moveInput;

    private Rigidbody _rb;
    private Camera _cam;

    [SerializeField]
    private float _clampXRotation = 75f;

    [SerializeField]
    private float _lookSensitivity;

    [SerializeField]
    private float _moveSpeed;

    [SerializeField]
    private float _jumpForce;

    [SerializeField]
    private float _fallMultiplier;

    [Tooltip("The transform that dictates the horizontal orientation of the player, used for moving forward and back")]
    [SerializeField]
    private Transform _flatOrientation;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _cam = GetComponentInChildren<Camera>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        // We add a continuous force each frame to the player if they are moving, but the direction is determined by a Unity Input Event
        if (_moveInput != Vector3.zero)
        {
            Vector3 moveDirection = _flatOrientation.forward * _moveInput.y + _flatOrientation.right * _moveInput.x;

            _rb.AddForce(_moveSpeed * moveDirection.normalized, ForceMode.Force);
        }

        // To make jumping and falling feel less floatly, we add a multiplier to the downward fall of every jump / leap
        if (_rb.linearVelocity.y < -0.01f)
        {
            // Gravity is still being applied by the rb, so we remove a factor of 1x to not double up on gravity
            _rb.AddForce((_fallMultiplier - 1.0f) * Physics2D.gravity.y * Vector3.up);
        }
    }

    /// <summary>
    /// A Unity Input Event to allow the player to jump
    /// </summary>
    /// <param name="context"></param>
    public void OnPlayerJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _rb.AddForce(_jumpForce * Vector3.up, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// A Unity Input Event to handle changing the player's move direction. The actual movement is handled in LateUpdate 
    /// </summary>
    /// <param name="context"></param>
    public void OnPlayerMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();        
    }

    /// <summary>
    /// A Unity Input Event to move the player's camera, and clamp the pitch so we don't break our necks in-game
    /// </summary>
    /// <param name="context"></param>
    public void OnPlayerLook(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();

        // Rotate Yaw
        _yaw += _lookSensitivity * lookInput.x * Time.deltaTime;

        // Rotate Pitch
        _pitch -= _lookSensitivity * lookInput.y * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, -_clampXRotation, _clampXRotation);

        // In order to clamp the pitch, we have to store its value instead of using transform.Rotate. We do the same for yaw for simplicity
        _cam.transform.localRotation = Quaternion.Euler(_pitch, _yaw, 0f);
        _flatOrientation.transform.localRotation = Quaternion.Euler(0f, _yaw, 0f);
    }
}
