using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public float MoveSpeedScalar = 1.0f;

    /// <summary>
    /// 
    /// </summary>
    public float RotationScalar = 0.1f;

    /// <summary>
    /// 
    /// </summary>
    public float LookUpDownScalar = 1.0f;

    /// <summary>
    /// 
    /// </summary>
    private Rigidbody m_body;

    /// <summary>
    /// 
    /// </summary>
    private Camera m_camera;

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        m_body = GetComponent<Rigidbody>();
        if (m_body == null)
        {
            Debug.LogError($"Failed to find rigid body component on the player controller '{name}'.");
            return;
        }

        m_camera = GetComponentInChildren<Camera>();
        if (m_camera == null)
        {
            Debug.LogError($"Failed to find a camera in any of the children of the player controller '{name}'.");
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// 
    /// </summary>
    public void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked 
                ? CursorLockMode.None 
                : CursorLockMode.Locked;
        }

        m_body.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * MoveSpeedScalar, ForceMode.Impulse);
        m_body.AddRelativeForce(Vector3.right * Input.GetAxis("Horizontal") * MoveSpeedScalar, ForceMode.Impulse);

        var mouseX = Input.GetAxis("Mouse X");
        m_body.AddRelativeTorque(Vector3.up * mouseX * RotationScalar, ForceMode.Impulse);

        var mouseY = Input.GetAxis("Mouse Y");
        var oldCameraRotation = m_camera.transform.rotation;
        m_camera.transform.Rotate(Vector3.left * mouseY * LookUpDownScalar);

        var delta = Vector3.Dot(transform.forward, m_camera.transform.forward);
        if (delta <= 0.5f)
        {
            m_camera.transform.rotation = oldCameraRotation;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unlock"></param>
    public void UnlockMouse(bool unlock)
    {
        Cursor.lockState = unlock
            ? CursorLockMode.None
            : CursorLockMode.Locked;
    }
}
