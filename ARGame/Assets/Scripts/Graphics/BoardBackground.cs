//----------------------------------------------------------------------------
// <copyright file="BoardBackground.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Graphics
{
    using System;
    using System.Net;
    using Network;
    using UnityEngine;
    using UnityEngine.Assertions;

    /// <summary>
    /// Loads a dynamic board texture from a HTTP webserver.
    /// </summary>
    public class BoardBackground : MonoBehaviour, IDisposable
    {
        /// <summary>
        /// Indicates whether to load a board texture remotely.
        /// </summary>
        public bool UseRemote;

        /// <summary>
        /// The <see cref="WWW"/> instance used for connecting to the server.
        /// </summary>
        private WWW webpage;

        /// <summary>
        /// Gets or sets the address to connect to.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the port to connect to.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Indicates if the image has been loaded.
        /// </summary>
        private bool imageLoaded = false;

        /// <summary>
        /// Disposes the web connection if it is still active.
        /// </summary>
        public virtual void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Disposes the web connection if it is still active.
        /// </summary>
        /// <param name="alsoManaged">True to dispose managed resources, 
        /// false to only dispose native resources.</param>
        public virtual void Dispose(bool alsoManaged)
        {
            if (alsoManaged && this.webpage != null)
            {
                this.webpage.Dispose();
            }
        }

        /// <summary>
        /// If we are using the remote check if image is done and use.
        /// <para>
        /// If the image has loaded, this <see cref="BoardBackground"/> 
        /// component is destroyed.
        /// </para>
        /// </summary>
        public void Update()
        {
            if (this.UseRemote && !this.imageLoaded)
            {
                bool complete = this.TryImage();

                if (complete) {
                    this.imageLoaded = true;
                }
            }
        }

        public void OnLevelUpdate(LevelUpdate update) {
            ClientSocket clientSocket = GetComponent<ClientSocket>();

            if (clientSocket != null) {
                this.IPAddress = clientSocket.ServerAddress;
                this.Port = clientSocket.ServerPort + 1;

                this.imageLoaded = false;
                this.GrabImage();
            }
        }

        /// <summary>
        /// Attempts to use the image if it is done loading yet if not we can always try again later 
        /// </summary>
        public bool TryImage()
        {
            if (this.webpage != null && this.webpage.isDone && this.GetComponentInChildren<Board>() != null)
            {
                Renderer renderer = this.GetComponentInChildren<Board>().gameObject.GetComponent<Renderer>();
                renderer.material.mainTexture = this.webpage.texture;
                Debug.Log("texture has loaded");
                this.webpage.Dispose();
                this.webpage = null;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Start loading the image, note that if the result is to be used then tryImage has to be called later
        /// </summary>
        public void GrabImage()
        {
            Debug.Log("loading image from: " + this.IPAddress + ":" + this.Port);
            this.webpage = new WWW(this.IPAddress + ":" + this.Port);
        }
    }
}