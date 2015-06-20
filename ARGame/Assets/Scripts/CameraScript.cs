using UnityEngine;

/// <summary>
/// Enumeration describing the movement style of the camera.
/// </summary>
public enum Cameratype
{
    /// <summary>
    /// Camera type that allows moving the camera with the arrow keys.
    /// </summary>
    FreeArrowcamMode = 0,

    /// <summary>
    /// Camera type that rotates around the board center.
    /// </summary>
    CentreRotateMode = 1,
    
    /// <summary>
    /// Camera type that follows the view of a local player.
    /// </summary>
    FollowPlayerMode = 2,
    
    /// <summary>
    /// Camera type that maintains a fixed position.
    /// </summary>
    FixedPositionMode = 3
}

/// <summary>
/// Control script for the remote camera.
/// </summary>
public class CameraScript : MonoBehaviour
{
    /// <summary>
    /// The type of camera movement.
    /// </summary>
    public Cameratype CameraType;

    /// <summary>
    /// Whether to allow zooming the camera.
    /// </summary>
    public bool AllowZoom;

    /// <summary>
    /// The ZoomLevel level of the camera.
    /// </summary>
    public float ZoomLevel;

    /// <summary>
    /// The movement Speed of the camera.
    /// </summary>
    public float Speed;

    /// <summary>
    /// The Speed with which the camera moves when not controlled by the user.
    /// </summary>
    public float AutoSpeed = 0.1f;
    
    /// <summary>
    /// The playing board.
    /// </summary>
    private Transform board;

    /// <summary>
    /// Initializes this <see cref="CameraScript"/> instance.
    /// </summary>
    public void Start()
    {
        this.board = GameObject.FindGameObjectsWithTag("PlayingBoard")[0].transform;
    }

    /// <summary>
    /// Modifies the transform of the camera corresponding with the current settings
    /// and the input from the player.
    /// </summary>
    public void Update()
    {
        switch (this.CameraType)
        {
            case Cameratype.FreeArrowcamMode:
                this.FreeArrowCam();
                return;
            case Cameratype.CentreRotateMode:
                this.CentreRotate();
                return;
            case Cameratype.FollowPlayerMode:
                this.FollowPlayer();
                return;
            case Cameratype.FixedPositionMode:
                this.FreeArrowCam();
                return;
        }
    }

    /// <summary>
    /// Make the camera follow the player.
    /// </summary>
    public void FollowPlayer()
    {
    }

    /// <summary>
    /// Move the camera with the arrow keys.
    /// </summary>
    public void ArrowMove()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(transform.right * this.Speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(transform.up * this.Speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(transform.right * -this.Speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(transform.up * -this.Speed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Rotate the camera around the center of the board.
    /// </summary>
    public void CentreRotate()
    {
        this.SmoothLookAt(this.board);
        this.SmoothMoveToTarget(this.board, this.ZoomLevel, this.AutoSpeed);
        this.ArrowMove();
    }

    /// <summary>
    /// Smoothly moves the camera towards an arbitrary Target.
    /// </summary>
    /// <param name="target">The Target transform.</param>
    /// <param name="desiredDistance">The desired distance from the Target.</param>
    /// <param name="speed">The Speed with which to move.</param>
    public void SmoothMoveToTarget(Transform target, float desiredDistance, float speed)
    {
        Vector3 dist = target.position - transform.position;
        float spc = dist.magnitude - desiredDistance;
        transform.Translate(dist * spc * speed);
    }

    /// <summary>
    /// Smoothly rotates the camera to look at an arbitrary Target.
    /// <para>
    /// This method computes the current <see cref="Quaternion"/> to be
    /// used for rotation, but does not affect the state of this 
    /// <see cref="CameraScript"/> instance.
    /// </para>
    /// </summary>
    /// <param name="target">The Target.</param>
    /// <returns>The new rotation for this camera.</returns>
    public Quaternion SmoothLookAt(Transform target)
    {
        Vector3 dir = transform.position - target.position;
        Quaternion qdir = Quaternion.Euler(dir);
        return Quaternion.Slerp(transform.rotation, qdir, Time.deltaTime * this.AutoSpeed);
    }

    /// <summary>
    /// Allows moving the camera using the arrow keys.
    /// </summary>
    public void FreeArrowCam()
    {
        this.ArrowMove();
    }

    /// <summary>
    /// Does nothing.
    /// </summary>
    public void FixedPosition()
    {
    }
}
