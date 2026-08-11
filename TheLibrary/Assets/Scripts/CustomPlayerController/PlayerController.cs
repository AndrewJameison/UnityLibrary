using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector3 _moveDirection;
    private float _pitch;
    private float _yaw;

    [SerializeField]
    private float _clampXRotation = 75f;

    private Rigidbody _rb;
    private Camera _cam;

    [SerializeField]
    private float LookSensitivity;

    [SerializeField]
    private float MoveSpeed;

    [Tooltip("The transform that dictates the horizontal orientation of the player, used for moving forward and back")]
    [SerializeField]
    private Transform FlatOrientation;

    [SerializeField]
    private float _fallMultiplier;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _cam = GetComponentInChildren<Camera>();
    }

    void LateUpdate()
    {
        transform.position += MoveSpeed * Time.deltaTime * _moveDirection;
    }

    public void OnPlayerMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();

        if (moveInput != Vector2.zero)
        {
            _moveDirection = FlatOrientation.forward * moveInput.y + FlatOrientation.right * moveInput.x;

            _rb.AddForce(MoveSpeed * _moveDirection.normalized, ForceMode.Force);
        }

        // To make the jump feel less floatly, we add a multiplier to the downward fall of every jump / leap
        if (_rb.linearVelocity.y < -0.1f)
        {
            // Gravity is still being applied by the rb, so we remove a factor of 1x to not double up on gravity
            _rb.AddForce((_fallMultiplier - 1.0f) * Physics2D.gravity.y * Vector3.up);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public void OnPlayerLook(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();

        // NOTE: In order to clamp the pitch, we have to store its value instead of using transform.Rotate. We do the same for yaw for simplicity
        // Rotate Yaw
        _yaw += LookSensitivity * lookInput.x * Time.deltaTime;

        // Rotate Pitch
        _pitch -= LookSensitivity * lookInput.y * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, -_clampXRotation, _clampXRotation);

        _cam.transform.localRotation = Quaternion.Euler(_pitch, _yaw, 0f);
        FlatOrientation.transform.localRotation = Quaternion.Euler(0f, _yaw, 0f);
    }
}
