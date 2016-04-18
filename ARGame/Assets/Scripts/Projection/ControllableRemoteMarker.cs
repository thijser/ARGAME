namespace Projection
{
    using System;
    using Network;
    using UnityEngine;

    /// <summary>
    /// Remote player marker that can be controlled using the arrow keys.
    /// <para>
    /// Following this marker allows the remote player to move the view using the keyboard.
    /// </para>
    /// </summary>
    public class ControllableRemoteMarker : RemotePlayerMarker
    {
        /// <summary>
        /// The fake marker id used for the virtual marker.
        /// </summary>
        public const int FakePlayerId = 1234;

        /// <summary>
        /// The horizontal rotation speed.
        /// </summary>
        public const float HorizontalSpeed = 1f;

        /// <summary>
        /// The vertical rotation speed.
        /// </summary>
        public const float VerticalSpeed = 1f;

        /// <summary>
        /// The center point looked at by the camera.
        /// </summary>
        private Vector3 center;

        /// <summary>
        /// The current rotation.
        /// </summary>
        private Vector2 rotation;

        /// <summary>
        /// Gets the fake marker transform.
        /// <para>
        /// The <see cref="Transform"/> object is only used for its <see cref="Transform.RotateAround"/> 
        /// method. The final marker position can be accessed through the <see cref="RemotePosition"/> property
        /// of this <see cref="RemoteMarker"/>.
        /// </para>
        /// </summary>
        public Transform FakeMarker { get; private set; }

        /// <summary>
        /// Gets the distance from the board center.
        /// </summary>
        public float ViewDistance { get; private set; }

        /// <summary>
        /// Initializes this <see cref="RemotePlayerMarker"/> and initializes 
        /// the <see cref="FakeMarker"/> <see cref="Transform"/> to be used as
        /// the virtual marker.
        /// </summary>
        public override void Start()
        {
            base.Start();
            this.ViewDistance = 10;
            this.FakeMarker = new GameObject("RemotePlayer").transform;
            this.FakeMarker.Translate(-ViewDistance, 0, 0);
            this.RemotePosition = new MarkerPosition(Vector3.zero, Quaternion.identity, DateTime.Now, Vector3.one, FakePlayerId);

            // Switch to this marker on start.
            this.transform.parent.GetComponent<RemoteMarkerHolder>().PlayerToFollow = this;
        }

        /// <summary>
        /// Updates the rotation based on the provided rotation.
        /// </summary>
        /// <param name="newRotation">A <see cref="Vector2"/> containing the horizontal
        /// and vertical rotation, in degrees.</param>
        public void UpdateRotation(Vector2 newRotation)
        {
            // Reset the fake marker back to initial state.
            this.FakeMarker.rotation = Quaternion.identity;
            this.FakeMarker.position = new Vector3(-ViewDistance, 0, 0);
            this.FakeMarker.Translate(this.center);
            this.FakeMarker.LookAt(this.center, Vector3.up);

            // Horizontal rotation.
            Vector3 horizontalAxis = Vector3.up;
            this.FakeMarker.RotateAround(this.center, horizontalAxis, newRotation.x);

            // Vertical rotation.
            newRotation.y = Mathf.Clamp(newRotation.y, 10, 85);
            Vector3 verticalAxis = Vector3.Cross(Vector3.up, (this.center - this.FakeMarker.position));
            this.FakeMarker.RotateAround(this.center, verticalAxis, newRotation.y);

            this.rotation = newRotation;

            this.RemotePosition.Position = this.FakeMarker.position;
            this.RemotePosition.Rotation = this.FakeMarker.rotation;
            this.RemotePosition.Scale = this.FakeMarker.lossyScale;
        }

        /// <summary>
        /// Updates the <see cref="FakeMarker"/> <see cref="Transform"/> based on the 
        /// state of the arrow keys.
        /// </summary>
        public virtual void Update()
        {
            Vector2 movement = this.GetMovement();
            Vector2 newRotation = this.rotation + movement;
            this.UpdateRotation(newRotation);
        }

        /// <summary>
        /// Adjusts the center point of the remote camera to point to the updated board center.
        /// </summary>
        /// <param name="level">The <see cref="LevelUpdate"/>.</param>
        public void OnLevelUpdate(LevelUpdate level)
        {
            Vector3 newCenter = new Vector3(level.Size.x / 2, 0, -level.Size.y / 2);
            float maxDimension = Math.Max(level.Size.x, level.Size.y);
            this.ViewDistance = maxDimension;

            Debug.Log("Remote center changed to: " + newCenter);
            this.center = newCenter;
        }

        /// <summary>
        /// Updates the position of this marker based on the <see cref="FakeMarker"/> <see cref="Transform"/>.
        /// </summary>
        /// <param name="transformMatrix">The transformation matrix.</param>
        public override void UpdatePosition(Matrix4x4 transformMatrix)
        {
            Matrix4x4 playerMatrix = this.FakeMarker.worldToLocalMatrix;
            this.transform.SetFromMatrix(playerMatrix * transformMatrix);
        }

        /// <summary>
        /// Gets the <see cref="Vector2"/> that represents the movement as indicated by the 
        /// W, A, S and D keys.
        /// </summary>
        /// <returns></returns>
        private Vector2 GetMovement()
        {
            float horizontal = 0;
            float vertical = 0;
            if (Input.GetKey(KeyCode.W))
            {
                vertical += VerticalSpeed;
            }

            if (Input.GetKey(KeyCode.S))
            {
                vertical -= VerticalSpeed;
            }

            if (Input.GetKey(KeyCode.D))
            {
                horizontal -= HorizontalSpeed;
            }

            if (Input.GetKey(KeyCode.A))
            {
                horizontal += HorizontalSpeed;
            }

            return new Vector2(horizontal, vertical);
        }
    }
}