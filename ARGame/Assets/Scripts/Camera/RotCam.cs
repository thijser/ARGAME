using UnityEngine;

/// <summary>
/// Rotates the camera around the origin using the arrow keys.
/// </summary>
public class RotCam : MonoBehaviour
{
    /// <summary>
    /// The movement speed of the camera.
    /// </summary>
    public float Speed;

    /// <summary>
    /// Rotates the camera when the arrow keys are pressed.
    /// </summary>
    public void Update()
    {
        this.ArrowRotate();
    }

    /// <summary>
    /// Rotates the camera when the arrow keys are pressed.
    /// </summary>
    public void ArrowRotate()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(transform.forward * -1 * this.Speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(transform.forward * this.Speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Rotate(transform.right * this.Speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Rotate(-1 * transform.right * this.Speed * Time.deltaTime);
        }
    }
}
