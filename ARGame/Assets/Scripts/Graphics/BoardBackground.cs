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
    using System.Globalization;
    using System.Net;
    using UnityEngine;
    using UnityEngine.Assertions;

    /// <summary>
    /// Loads a dynamic board texture from a HTTP webserver.
    /// </summary>
    public class BoardBackground : MonoBehaviour
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
        /// If we are using the remote check if image is done and use. 
        /// </summary>
        public void Update()
        {
            if (this.UseRemote)
            {
                this.TryImage();
            }
        }

        /// <summary>
        /// Sets the IPAddress and Port to match the IPEndPoint and creates
        /// the <see cref="WWW"/> instance to retrieve the texture from.
        /// <para>
        /// Called from ClientSocket when the server is ready.
        /// </para>
        /// </summary>
        /// <param name="endPoint">The IP EndPoint of the Socket.</param>
        public void OnSocketStart(IPEndPoint endPoint)
        {
            Assert.IsNotNull(endPoint);
            this.IPAddress = endPoint.Address.ToString();
            this.Port = endPoint.Port + 1;
            if (this.webpage == null)
            {
                this.GrabImage();
            }
        }

        /// <summary>
        /// Attempts to use the image if it is done loading yet if not we can always try again later 
        /// </summary>
        public void TryImage()
        {
            if (this.webpage != null && this.webpage.isDone)
            {
                Renderer renderer = GameObject.Find("RemoteController/Board/Tabletop").GetComponent<Renderer>();
                renderer.material.mainTexture = this.webpage.texture;
                Debug.Log("texture has loaded");
                this.webpage.Dispose();
                this.webpage = null;
            }
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