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
        private StreamWriter logWriter = null;
        private DateTime levelStartTime;

        private LevelManager levelManager = null;
        private RemoteMarkerHolder markerHolder = null;

        private Dictionary<int, DateTime> timeKeeper = new Dictionary<int, DateTime>();
        private Dictionary<int, Vector2> positionKeeper = new Dictionary<int, Vector2>();

        public void Start()
        {
            Debug.Log("Starting logger.");

            Directory.CreateDirectory("logs");

            levelManager = GetComponent<LevelManager>();
            markerHolder = GetComponent<RemoteMarkerHolder>();
        }

        public void OnPositionUpdate(PositionUpdate update)
        {
            int updateID = update.Id;
            if (timeKeeper.ContainsKey(updateID) && positionKeeper.ContainsKey(updateID))
            {
                TimeSpan span = DateTime.Now - timeKeeper[updateID];
                Vector2 movement = positionKeeper[updateID] - update.Coordinate;
                if (span.Seconds > 1 && Math.Abs(movement.x) > 0.1 && Math.Abs(movement.y) > 0.1)
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
            if (logWriter != null)
            {
                logWriter.WriteLine(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffff") + ": " + message);
            }
        }

        public void NewLevel()
        {
            if (logWriter != null)
            {
                TimeSpan playTime = DateTime.Now - levelStartTime;
                WriteLog("Finished playthrough of level #" + levelManager.CurrentLevelIndex + " after " + playTime.ToString());
            }

            Debug.Log("Logging playthough of a new level...");

            int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            logWriter = new StreamWriter("logs/level_" + levelManager.CurrentLevelIndex + "_playthrough_" + unixTimestamp + ".txt");

            WriteLog("Started new playthrough of level #" + levelManager.CurrentLevelIndex);
            levelStartTime = DateTime.Now;
        }
    }
}