namespace Projection
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using TestUtilities;
    using UnityEngine;
    using Projection;
    using Network;
    [TestFixture]
    public class ProjectionTests : MirrorsUnitTest
    {
        [Test]
        public void createIncestMirrors()
        {




            GameObject marker1 = GameObjectFactory.createMarker();
            marker1.transform.position = new Vector3(0, 0, 0);
            Assert.AreEqual(new Vector3(0, 0, 0), marker1.transform.position);

            GameObject marker2 = GameObjectFactory.createMarker();
            marker2.transform.SetParent(marker1.transform);
            GameObject syncer = new GameObject();
            syncer.AddComponent<RemoteObjectSyncer>();
            RemoteObjectSyncer sync = syncer.GetComponent<RemoteObjectSyncer>();
            sync.LevelMarker = marker1;
            sync.Scale = 1;
            sync.RegisterObjectsOnStartup = new System.Collections.Generic.List<GameObject>();
            sync.RegisterObjectsOnStartup.Add(marker1);
            sync.RegisterObjectsOnStartup.Add(marker2);
            sync.Start();
            //	PositionUpdate(UpdateType type, float x, float y, float rotation, int id)
            PositionUpdate pu1 = new PositionUpdate(UpdateType.Update, 1, 1, 0, 0);

            sync.OnPositionUpdate(pu1);
            Assert.AreEqual(new Vector3(0, 0, 0), marker1.transform.position);
            Assert.AreEqual(new Vector3(0, 0, 0), marker2.transform.position);
            PositionUpdate pu2 = new PositionUpdate(UpdateType.Update, 2, 2, 0, 1);
            sync.OnPositionUpdate(pu1);
            Assert.AreEqual(new Vector3(1, 0, 1), marker2.transform);


        }


    }

}