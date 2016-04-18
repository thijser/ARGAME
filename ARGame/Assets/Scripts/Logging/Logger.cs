//----------------------------------------------------------------------------
// <copyright file="Logger.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Logging
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Network;
    using UnityEngine;

    /// <summary>
    /// Logging facility for the application.
    /// </summary>
    public class Logger : MonoBehaviour
    {
        /// <summary>
        /// The directory all log files are placed in.
        /// </summary>
        public const string LogDirectory = "logs";

        /// <summary>
        /// The minimum squared distance a marker must be moved for it to be logged as a movement.
        /// </summary>
        private const float PositionLogThreshold = 0.25f;

        /// <summary>
        /// The minimum time interval between consecutive log entries on the same marker, in seconds.
        /// </summary>
        private const float TimeInterval = 1f;

        /// <summary>
        /// The path to the log file.
        /// </summary>
        private string filePath = null;

        /// <summary>
        /// The time the current level was started.
        /// </summary>
        private DateTime levelStartTime;
        
        /// <summary>
        /// The dictionary keeping track of the currently logged information of markers.
        /// </summary>
        private Dictionary<int, PositionEntry> markerPositions = new Dictionary<int, PositionEntry>();

        /// <summary>
        /// Initializes the logger and creates the logging directory.
        /// </summary>
        public void Start()
        {
            Debug.Log("Starting logger.");
            Directory.CreateDirectory("logs");
        }

        /// <summary>
        /// Logs the movement of a marker.
        /// </summary>
        /// <param name="update">The <see cref="PositionUpdate"/>.</param>
        public void OnPositionUpdate(PositionUpdate update)
        {
            if (update.Type != UpdateType.UpdatePosition && update.Type != UpdateType.UpdateRotation)
            {
                return;
            }
            
            PositionEntry entry = new PositionEntry(update);

            if (this.markerPositions.ContainsKey(entry.MarkerId))
            {
                PositionEntry current = this.markerPositions[entry.MarkerId];
                TimeSpan span = DateTime.Now - current.LastMoved;
                Vector2 movement = current.Position - update.Coordinate;

                float dist = movement.SqrMagnitude();

                if (span.Seconds >= TimeInterval && dist >= PositionLogThreshold)
                {
                    this.markerPositions[entry.MarkerId] = entry;
                    this.WriteLog(entry.ToUpdateString());
                }
            }
            else
            {
                this.markerPositions[entry.MarkerId] = entry;
                this.WriteLog(entry.ToCreateString());
            }
        }

        /// <summary>
        /// Logs a message that the rotation of a marker has been updated.
        /// </summary>
        /// <param name="update">The <see cref="RotationUpdate"/> instance.</param>
        public void OnRotationUpdate(RotationUpdate update)
        {
            this.WriteLog(string.Format("marker #{0} rotated to {1}", update.Id, update.Rotation));
        }

        /// <summary>
        /// Writes the provided log message to the log file.
        /// <para>
        /// A timestamp is added by this method, and therefore does not need to be included in the message.
        /// </para>
        /// </summary>
        /// <param name="message">The message to write.</param>
        private void WriteLog(string message)
        {
            if (this.filePath != null)
            {
                File.AppendAllText(filePath, DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss") + " - " + message + Environment.NewLine);
            }
            else
            {
                Debug.LogWarning("Discarding message because log file is not set: " + message);
            }
        }

        /// <summary>
        /// Logs the change between levels.
        /// </summary>
        /// <param name="oldLevel">The old level index.</param>
        /// <param name="newLevel">The new level index.</param>
        public void NewLevel(int oldLevel, int newLevel)
        {
            // Ignore level finish of first level change (initialization)
            if (this.filePath != null)
            {
                TimeSpan playTime = DateTime.Now - this.levelStartTime;
                this.WriteLog("Finished playthrough of level #" + oldLevel + " after " + playTime.ToString());
            }
            
            string isoDate = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");
            this.filePath = LogDirectory + Path.DirectorySeparatorChar + isoDate + " - Level " + newLevel + ".log";

            this.WriteLog("Started new playthrough of level #" + newLevel);
            this.levelStartTime = DateTime.Now;
        }
    }
}