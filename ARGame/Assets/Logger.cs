namespace Core
{
    using UnityEngine;
    using System.Collections;
    using System.IO;
    using System;
    using Level;
    using Projection;
    using Network;
    using System.Collections.Generic;

    public class Logger : MonoBehaviour
    {
        private const float PositionLogThreshold = 0.5f;

        private string filePath = null;
        private DateTime levelStartTime;

        private Dictionary<int, DateTime> timeKeeper = new Dictionary<int, DateTime>();
        private Dictionary<int, Vector2> positionKeeper = new Dictionary<int, Vector2>();

        public void Start()
        {
            Debug.Log("Starting logger.");

            Directory.CreateDirectory("logs");
        }

        public void OnPositionUpdate(PositionUpdate update)
        {
            if (update.Type != UpdateType.UpdatePosition && update.Type != UpdateType.UpdateRotation) return;

            int updateID = update.Id;
            if (timeKeeper.ContainsKey(updateID) && positionKeeper.ContainsKey(updateID))
            {
                TimeSpan span = DateTime.Now - timeKeeper[updateID];
                Vector2 movement = positionKeeper[updateID] - update.Coordinate;

                float dist = Mathf.Sqrt(movement.x * movement.x + movement.y * movement.y);

                if (span.Seconds > 1 && dist > PositionLogThreshold)
                {
                    WriteLog(string.Format("marker #{0} moved to position = ({1}, {2}), rotation = {3}", update.Id, update.Coordinate.x, update.Coordinate.y, update.Rotation));

                    positionKeeper[updateID] = update.Coordinate;
                    timeKeeper[updateID] = DateTime.Now;
                }
            }
            else
            {
                positionKeeper[updateID] = update.Coordinate;
                timeKeeper[updateID] = DateTime.Now;

                WriteLog(string.Format("marker #{0} registered, position = ({1}, {2}), rotation = {3}", update.Id, update.Coordinate.x, update.Coordinate.y, update.Rotation));
            }
        }

        public void OnRotationUpdate(RotationUpdate update)
        {
            WriteLog(string.Format("marker #{0} rotated to {1}", update.Id, update.Rotation));
        }

        private void WriteLog(string message)
        {
            if (filePath != null)
            {
                File.AppendAllText(filePath, DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffff") + ": " + message + "\n");
            }
        }

        public void NewLevel(int oldLevel, int newLevel)
        {
            // Ignore level finish of first level change (initialization)
            if (filePath != null)
            {
                TimeSpan playTime = DateTime.Now - levelStartTime;
                WriteLog("Finished playthrough of level #" + oldLevel + " after " + playTime.ToString());
            }

            Debug.Log("Logging playthough of a new level...");

            int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            filePath = "logs/level_" + newLevel + "_playthrough_" + unixTimestamp + ".log";

            WriteLog("Started new playthrough of level #" + newLevel);
            levelStartTime = DateTime.Now;
        }
    }
}