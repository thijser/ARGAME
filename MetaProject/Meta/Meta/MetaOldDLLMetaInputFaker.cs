// Decompiled with JetBrains decompiler
// Type: Meta.MetaOldDLLMetaInputFaker
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Meta
{
  internal static class MetaOldDLLMetaInputFaker
  {
    private static HandData oldLeftCppHand = new HandData();
    private static HandData oldRightCppHand = new HandData();
    private static MetaOldDLLMetaInputFaker.DynamicGestureData dynamicGesture = new MetaOldDLLMetaInputFaker.DynamicGestureData();
    private static GCHandle m_leftCppHandVerticesDataHandle;
    private static GCHandle m_rightCppHandVerticesDataHandle;
    private static float[] tempDataAllocationArraym_leftMetaHandVertices;
    private static float[] tempDataAllocationArraym_rightMetaHandVertices;
    private static GCHandle m_leftCppHandMeshVerticesDataHandle;
    private static GCHandle m_rightCppHandMeshVerticesDataHandle;
    private static GCHandle m_leftCppHandMeshTrianglesDataHandle;
    private static GCHandle m_rightCppHandMeshTrianglesDataHandle;
    private static GCHandle m_leftCppHandMeshSpatialConfDataHandle;
    private static GCHandle m_rightCppHandMeshSpatialConfDataHandle;
    private static GCHandle m_leftCppHandMeshTemporalConfDataHandle;
    private static GCHandle m_rightCppHandMeshTemporalConfDataHandle;
    private static GCHandle m_leftCppHandMeshNormalsDataHandle;
    private static GCHandle m_rightCppHandMeshNormalsDataHandle;
    private static float[] tempDataAllocationArraym_leftMetaHandMeshVertices;
    private static float[] tempDataAllocationArraym_rightMetaHandMeshVertices;
    private static float[] tempDataAllocationArraym_leftMetaHandMeshTriangles;
    private static float[] tempDataAllocationArraym_rightMetaHandMeshTriangles;
    private static float[] tempDataAllocationArraym_leftMetaHandMeshSpatialConf;
    private static float[] tempDataAllocationArraym_rightMetaHandMeshSpatialConf;
    private static float[] tempDataAllocationArraym_leftMetaHandMeshTemporalConf;
    private static float[] tempDataAllocationArraym_rightMetaHandMeshTemporalConf;
    private static float[] tempDataAllocationArraym_leftMetaHandMeshNormals;
    private static float[] tempDataAllocationArraym_rightMetaHandMeshNormals;

    [DllImport("MetaVisionDLL", EntryPoint = "getHandAndGestureData")]
    internal static extern void GetHandAndGestureData(ref HandData leftCppHand, ref HandData rightCppHand, ref MetaOldDLLMetaInputFaker.DynamicGestureData m_dynamicGestureCppLabel);

    internal static void UpdateData()
    {
      MetaOldDLLMetaInputFaker.GetHandAndGestureData(ref MetaOldDLLMetaInputFaker.oldLeftCppHand, ref MetaOldDLLMetaInputFaker.oldRightCppHand, ref MetaOldDLLMetaInputFaker.dynamicGesture);
    }

    internal static void GetHandData(ref CppHandData leftHandData, ref CppHandData rightHandData)
    {
      leftHandData.top = MetaOldDLLMetaInputFaker.oldLeftCppHand.top;
      leftHandData.left = MetaOldDLLMetaInputFaker.oldLeftCppHand.left;
      leftHandData.right = MetaOldDLLMetaInputFaker.oldLeftCppHand.right;
      leftHandData.center = MetaOldDLLMetaInputFaker.oldLeftCppHand.center;
      leftHandData.valid = MetaOldDLLMetaInputFaker.oldLeftCppHand.valid;
      leftHandData.angle = MetaOldDLLMetaInputFaker.oldLeftCppHand.angle;
      leftHandData.palm.radius = MetaOldDLLMetaInputFaker.oldLeftCppHand.palm.radius;
      leftHandData.palm.orientationAngles = MetaOldDLLMetaInputFaker.oldLeftCppHand.palm.orientation_angles;
      leftHandData.palm.normalVector = MetaOldDLLMetaInputFaker.oldLeftCppHand.palm.norm_vec;
      for (int index = 0; index < MetaOldDLLMetaInputFaker.oldLeftCppHand.fingers.Length; ++index)
      {
        leftHandData.fingers[index].location = MetaOldDLLMetaInputFaker.oldLeftCppHand.fingers[index].loc;
        leftHandData.fingers[index].direction = MetaOldDLLMetaInputFaker.oldLeftCppHand.fingers[index].dir;
        leftHandData.fingers[index].found = MetaOldDLLMetaInputFaker.oldLeftCppHand.fingers[index].found;
      }
      rightHandData.top = MetaOldDLLMetaInputFaker.oldRightCppHand.top;
      rightHandData.left = MetaOldDLLMetaInputFaker.oldRightCppHand.left;
      rightHandData.right = MetaOldDLLMetaInputFaker.oldRightCppHand.right;
      rightHandData.center = MetaOldDLLMetaInputFaker.oldRightCppHand.center;
      rightHandData.valid = MetaOldDLLMetaInputFaker.oldRightCppHand.valid;
      rightHandData.angle = MetaOldDLLMetaInputFaker.oldRightCppHand.angle;
      rightHandData.palm.radius = MetaOldDLLMetaInputFaker.oldRightCppHand.palm.radius;
      rightHandData.palm.orientationAngles = MetaOldDLLMetaInputFaker.oldRightCppHand.palm.orientation_angles;
      rightHandData.palm.normalVector = MetaOldDLLMetaInputFaker.oldRightCppHand.palm.norm_vec;
      for (int index = 0; index < Enumerable.Count<CppFinger>((IEnumerable<CppFinger>) rightHandData.fingers); ++index)
      {
        rightHandData.fingers[index].location = MetaOldDLLMetaInputFaker.oldRightCppHand.fingers[index].loc;
        rightHandData.fingers[index].direction = MetaOldDLLMetaInputFaker.oldRightCppHand.fingers[index].dir;
        rightHandData.fingers[index].found = MetaOldDLLMetaInputFaker.oldRightCppHand.fingers[index].found;
      }
      leftHandData.gesture.manipulationGesture = (MetaGesture) MetaOldDLLMetaInputFaker.oldLeftCppHand.state_data.state;
      leftHandData.handOpenness = MetaOldDLLMetaInputFaker.oldLeftCppHand.state_data.grab_value;
      leftHandData.gesture.valid = MetaOldDLLMetaInputFaker.oldLeftCppHand.state_data.state != HandData.StateData.State.NONE;
      switch (leftHandData.gesture.manipulationGesture + 1)
      {
        case MetaGesture.OPEN:
          leftHandData.gesture.gesturePoint = MetaOldDLLMetaInputFaker.oldLeftCppHand.center;
          break;
        case MetaGesture.POINT:
          leftHandData.gesture.gesturePoint = MetaOldDLLMetaInputFaker.oldLeftCppHand.center;
          break;
        case MetaGesture.PINCH:
          leftHandData.gesture.gesturePoint = MetaOldDLLMetaInputFaker.oldLeftCppHand.top;
          break;
        case (MetaGesture) 4:
          leftHandData.gesture.gesturePoint = MetaOldDLLMetaInputFaker.oldLeftCppHand.state_data.pinch_pt;
          break;
      }
      rightHandData.gesture.manipulationGesture = (MetaGesture) MetaOldDLLMetaInputFaker.oldRightCppHand.state_data.state;
      rightHandData.handOpenness = MetaOldDLLMetaInputFaker.oldRightCppHand.state_data.grab_value;
      rightHandData.gesture.valid = MetaOldDLLMetaInputFaker.oldRightCppHand.state_data.state != HandData.StateData.State.NONE;
      switch (rightHandData.gesture.manipulationGesture + 1)
      {
        case MetaGesture.OPEN:
          rightHandData.gesture.gesturePoint = MetaOldDLLMetaInputFaker.oldRightCppHand.center;
          break;
        case MetaGesture.POINT:
          rightHandData.gesture.gesturePoint = MetaOldDLLMetaInputFaker.oldRightCppHand.center;
          break;
        case MetaGesture.PINCH:
          rightHandData.gesture.gesturePoint = MetaOldDLLMetaInputFaker.oldRightCppHand.top;
          break;
        case (MetaGesture) 4:
          rightHandData.gesture.gesturePoint = MetaOldDLLMetaInputFaker.oldRightCppHand.state_data.pinch_pt;
          break;
      }
    }

    internal static void GetPointCloudDisplayData(ref PointCloudData leftHandDisplay, ref PointCloudData rightHandDisplay)
    {
      leftHandDisplay.size = MetaOldDLLMetaInputFaker.oldLeftCppHand.hand_point_cloud.size;
      leftHandDisplay.vertices = MetaOldDLLMetaInputFaker.oldLeftCppHand.hand_point_cloud.vertices;
      rightHandDisplay.size = MetaOldDLLMetaInputFaker.oldRightCppHand.hand_point_cloud.size;
      rightHandDisplay.vertices = MetaOldDLLMetaInputFaker.oldRightCppHand.hand_point_cloud.vertices;
    }

    internal static void GetHandMeshDisplayData(ref MeshData leftHandDisplay, ref MeshData rightHandDisplay)
    {
      leftHandDisplay.valid = MetaOldDLLMetaInputFaker.oldLeftCppHand.hand_mesh.valid;
      leftHandDisplay.vertices_length = MetaOldDLLMetaInputFaker.oldLeftCppHand.hand_mesh.vertices_length;
      leftHandDisplay.triangles_length = MetaOldDLLMetaInputFaker.oldLeftCppHand.hand_mesh.triangles_length;
      leftHandDisplay.vertices = MetaOldDLLMetaInputFaker.oldLeftCppHand.hand_mesh.vertices;
      leftHandDisplay.triangles = MetaOldDLLMetaInputFaker.oldLeftCppHand.hand_mesh.triangles;
      if (MetaSingleton<Hands>.Instance._handConfig._enableMeshRandD)
      {
        leftHandDisplay.normals_length = MetaOldDLLMetaInputFaker.oldLeftCppHand.hand_mesh.normals_length;
        leftHandDisplay.normals = MetaOldDLLMetaInputFaker.oldLeftCppHand.hand_mesh.normals;
        leftHandDisplay.spatial_conf = MetaOldDLLMetaInputFaker.oldLeftCppHand.hand_mesh.spatial_conf;
        leftHandDisplay.temporal_conf = MetaOldDLLMetaInputFaker.oldLeftCppHand.hand_mesh.temporal_conf;
        rightHandDisplay.normals_length = MetaOldDLLMetaInputFaker.oldRightCppHand.hand_mesh.normals_length;
        rightHandDisplay.normals = MetaOldDLLMetaInputFaker.oldRightCppHand.hand_mesh.normals;
        rightHandDisplay.spatial_conf = MetaOldDLLMetaInputFaker.oldRightCppHand.hand_mesh.spatial_conf;
        rightHandDisplay.temporal_conf = MetaOldDLLMetaInputFaker.oldRightCppHand.hand_mesh.temporal_conf;
      }
      rightHandDisplay.valid = MetaOldDLLMetaInputFaker.oldRightCppHand.hand_mesh.valid;
      rightHandDisplay.vertices_length = MetaOldDLLMetaInputFaker.oldRightCppHand.hand_mesh.vertices_length;
      rightHandDisplay.triangles_length = MetaOldDLLMetaInputFaker.oldRightCppHand.hand_mesh.triangles_length;
      rightHandDisplay.vertices = MetaOldDLLMetaInputFaker.oldRightCppHand.hand_mesh.vertices;
      rightHandDisplay.triangles = MetaOldDLLMetaInputFaker.oldRightCppHand.hand_mesh.triangles;
    }

    internal static void InitializeHandData()
    {
      MetaOldDLLMetaInputFaker.oldLeftCppHand.top = new float[3];
      MetaOldDLLMetaInputFaker.oldLeftCppHand.left = new float[3];
      MetaOldDLLMetaInputFaker.oldLeftCppHand.right = new float[3];
      MetaOldDLLMetaInputFaker.oldLeftCppHand.center = new float[3];
      MetaOldDLLMetaInputFaker.oldLeftCppHand.type = ~HandData.Type.NONE;
      MetaOldDLLMetaInputFaker.oldLeftCppHand.state_data = new HandData.StateData();
      MetaOldDLLMetaInputFaker.oldLeftCppHand.state_data.state = HandData.StateData.State.OPEN;
      MetaOldDLLMetaInputFaker.oldLeftCppHand.state_data.pinch_pt = new float[3];
      MetaOldDLLMetaInputFaker.oldLeftCppHand.valid = false;
      MetaOldDLLMetaInputFaker.oldLeftCppHand.valid = false;
      MetaOldDLLMetaInputFaker.oldLeftCppHand.angle = 0;
      MetaOldDLLMetaInputFaker.oldLeftCppHand.palm = new HandData.Palm();
      MetaOldDLLMetaInputFaker.oldLeftCppHand.palm.radius = 0;
      MetaOldDLLMetaInputFaker.oldLeftCppHand.palm.orientation_angles = new float[2];
      MetaOldDLLMetaInputFaker.oldLeftCppHand.palm.norm_vec = new float[3];
      MetaOldDLLMetaInputFaker.oldLeftCppHand.fingers = new HandData.Finger[5];
      for (int index = 0; index <= 4; ++index)
      {
        MetaOldDLLMetaInputFaker.oldLeftCppHand.fingers[index] = new HandData.Finger();
        MetaOldDLLMetaInputFaker.oldLeftCppHand.fingers[index].loc = new float[3];
        MetaOldDLLMetaInputFaker.oldLeftCppHand.fingers[index].dir = new float[3];
        MetaOldDLLMetaInputFaker.oldLeftCppHand.fingers[index].found = false;
        MetaOldDLLMetaInputFaker.oldLeftCppHand.fingers[index].found = false;
      }
      MetaOldDLLMetaInputFaker.oldRightCppHand.top = new float[3];
      MetaOldDLLMetaInputFaker.oldRightCppHand.left = new float[3];
      MetaOldDLLMetaInputFaker.oldRightCppHand.right = new float[3];
      MetaOldDLLMetaInputFaker.oldRightCppHand.center = new float[3];
      MetaOldDLLMetaInputFaker.oldRightCppHand.type = ~HandData.Type.NONE;
      MetaOldDLLMetaInputFaker.oldRightCppHand.state_data = new HandData.StateData();
      MetaOldDLLMetaInputFaker.oldRightCppHand.state_data.state = HandData.StateData.State.OPEN;
      MetaOldDLLMetaInputFaker.oldRightCppHand.state_data.pinch_pt = new float[3];
      MetaOldDLLMetaInputFaker.oldRightCppHand.valid = false;
      MetaOldDLLMetaInputFaker.oldRightCppHand.valid = false;
      MetaOldDLLMetaInputFaker.oldRightCppHand.angle = 0;
      MetaOldDLLMetaInputFaker.oldRightCppHand.palm = new HandData.Palm();
      MetaOldDLLMetaInputFaker.oldRightCppHand.palm.radius = 0;
      MetaOldDLLMetaInputFaker.oldRightCppHand.palm.orientation_angles = new float[2];
      MetaOldDLLMetaInputFaker.oldRightCppHand.palm.norm_vec = new float[3];
      MetaOldDLLMetaInputFaker.oldRightCppHand.fingers = new HandData.Finger[5];
      for (int index = 0; index <= 4; ++index)
      {
        MetaOldDLLMetaInputFaker.oldRightCppHand.fingers[index] = new HandData.Finger();
        MetaOldDLLMetaInputFaker.oldRightCppHand.fingers[index].loc = new float[3];
        MetaOldDLLMetaInputFaker.oldRightCppHand.fingers[index].dir = new float[3];
        MetaOldDLLMetaInputFaker.oldRightCppHand.fingers[index].found = false;
        MetaOldDLLMetaInputFaker.oldRightCppHand.fingers[index].found = false;
      }
      MetaOldDLLMetaInputFaker.InitializePointCloud();
      MetaOldDLLMetaInputFaker.InitializeMeshData();
    }

    private static void InitializePointCloud()
    {
      MetaCore.Instance.Log("Initialize point cloud");
      MetaOldDLLMetaInputFaker.tempDataAllocationArraym_leftMetaHandVertices = new float[MetaSingleton<Hands>.Instance._handConfig._maxHandVertices * 3];
      MetaOldDLLMetaInputFaker.tempDataAllocationArraym_rightMetaHandVertices = new float[MetaSingleton<Hands>.Instance._handConfig._maxHandVertices * 3];
      MetaOldDLLMetaInputFaker.m_leftCppHandVerticesDataHandle = GCHandle.Alloc((object) MetaOldDLLMetaInputFaker.tempDataAllocationArraym_leftMetaHandVertices, GCHandleType.Pinned);
      MetaOldDLLMetaInputFaker.oldLeftCppHand.hand_point_cloud.vertices = MetaOldDLLMetaInputFaker.m_leftCppHandVerticesDataHandle.AddrOfPinnedObject();
      MetaOldDLLMetaInputFaker.m_leftCppHandVerticesDataHandle.Free();
      MetaOldDLLMetaInputFaker.m_rightCppHandVerticesDataHandle = GCHandle.Alloc((object) MetaOldDLLMetaInputFaker.tempDataAllocationArraym_rightMetaHandVertices, GCHandleType.Pinned);
      MetaOldDLLMetaInputFaker.oldRightCppHand.hand_point_cloud.vertices = MetaOldDLLMetaInputFaker.m_rightCppHandVerticesDataHandle.AddrOfPinnedObject();
      MetaOldDLLMetaInputFaker.m_rightCppHandVerticesDataHandle.Free();
    }

    private static void InitializeMeshData()
    {
      MetaCore.Instance.Log("Initialize Mesh Data");
      MetaOldDLLMetaInputFaker.tempDataAllocationArraym_leftMetaHandMeshVertices = new float[MetaSingleton<Hands>.Instance._handConfig._maxHandVertices * 3];
      MetaOldDLLMetaInputFaker.tempDataAllocationArraym_rightMetaHandMeshVertices = new float[MetaSingleton<Hands>.Instance._handConfig._maxHandVertices * 3];
      MetaOldDLLMetaInputFaker.tempDataAllocationArraym_leftMetaHandMeshTriangles = new float[MetaSingleton<Hands>.Instance._handConfig._maxHandVertices * 2 * 3];
      MetaOldDLLMetaInputFaker.tempDataAllocationArraym_rightMetaHandMeshTriangles = new float[MetaSingleton<Hands>.Instance._handConfig._maxHandVertices * 2 * 3];
      if (MetaSingleton<Hands>.Instance._handConfig._enableMeshRandD)
      {
        MetaOldDLLMetaInputFaker.tempDataAllocationArraym_leftMetaHandMeshSpatialConf = new float[MetaSingleton<Hands>.Instance._handConfig._maxHandVertices];
        MetaOldDLLMetaInputFaker.tempDataAllocationArraym_rightMetaHandMeshSpatialConf = new float[MetaSingleton<Hands>.Instance._handConfig._maxHandVertices];
        MetaOldDLLMetaInputFaker.tempDataAllocationArraym_leftMetaHandMeshTemporalConf = new float[MetaSingleton<Hands>.Instance._handConfig._maxHandVertices];
        MetaOldDLLMetaInputFaker.tempDataAllocationArraym_rightMetaHandMeshTemporalConf = new float[MetaSingleton<Hands>.Instance._handConfig._maxHandVertices];
        MetaOldDLLMetaInputFaker.tempDataAllocationArraym_leftMetaHandMeshNormals = new float[MetaSingleton<Hands>.Instance._handConfig._maxHandVertices * 3];
        MetaOldDLLMetaInputFaker.tempDataAllocationArraym_rightMetaHandMeshNormals = new float[MetaSingleton<Hands>.Instance._handConfig._maxHandVertices * 3];
      }
      MetaOldDLLMetaInputFaker.m_leftCppHandMeshVerticesDataHandle = GCHandle.Alloc((object) MetaOldDLLMetaInputFaker.tempDataAllocationArraym_leftMetaHandMeshVertices, GCHandleType.Pinned);
      MetaOldDLLMetaInputFaker.oldLeftCppHand.hand_mesh.vertices = MetaOldDLLMetaInputFaker.m_leftCppHandMeshVerticesDataHandle.AddrOfPinnedObject();
      MetaOldDLLMetaInputFaker.m_leftCppHandMeshVerticesDataHandle.Free();
      MetaOldDLLMetaInputFaker.m_rightCppHandMeshVerticesDataHandle = GCHandle.Alloc((object) MetaOldDLLMetaInputFaker.tempDataAllocationArraym_rightMetaHandMeshVertices, GCHandleType.Pinned);
      MetaOldDLLMetaInputFaker.oldRightCppHand.hand_mesh.vertices = MetaOldDLLMetaInputFaker.m_rightCppHandMeshVerticesDataHandle.AddrOfPinnedObject();
      MetaOldDLLMetaInputFaker.m_rightCppHandMeshVerticesDataHandle.Free();
      MetaOldDLLMetaInputFaker.m_leftCppHandMeshTrianglesDataHandle = GCHandle.Alloc((object) MetaOldDLLMetaInputFaker.tempDataAllocationArraym_leftMetaHandMeshTriangles, GCHandleType.Pinned);
      MetaOldDLLMetaInputFaker.oldLeftCppHand.hand_mesh.triangles = MetaOldDLLMetaInputFaker.m_leftCppHandMeshTrianglesDataHandle.AddrOfPinnedObject();
      MetaOldDLLMetaInputFaker.m_leftCppHandMeshTrianglesDataHandle.Free();
      MetaOldDLLMetaInputFaker.m_rightCppHandMeshTrianglesDataHandle = GCHandle.Alloc((object) MetaOldDLLMetaInputFaker.tempDataAllocationArraym_rightMetaHandMeshTriangles, GCHandleType.Pinned);
      MetaOldDLLMetaInputFaker.oldRightCppHand.hand_mesh.triangles = MetaOldDLLMetaInputFaker.m_rightCppHandMeshTrianglesDataHandle.AddrOfPinnedObject();
      MetaOldDLLMetaInputFaker.m_rightCppHandMeshTrianglesDataHandle.Free();
      if (!MetaSingleton<Hands>.Instance._handConfig._enableMeshRandD)
        return;
      MetaOldDLLMetaInputFaker.m_leftCppHandMeshSpatialConfDataHandle = GCHandle.Alloc((object) MetaOldDLLMetaInputFaker.tempDataAllocationArraym_leftMetaHandMeshSpatialConf, GCHandleType.Pinned);
      MetaOldDLLMetaInputFaker.oldLeftCppHand.hand_mesh.spatial_conf = MetaOldDLLMetaInputFaker.m_leftCppHandMeshSpatialConfDataHandle.AddrOfPinnedObject();
      MetaOldDLLMetaInputFaker.m_leftCppHandMeshSpatialConfDataHandle.Free();
      MetaOldDLLMetaInputFaker.m_rightCppHandMeshSpatialConfDataHandle = GCHandle.Alloc((object) MetaOldDLLMetaInputFaker.tempDataAllocationArraym_rightMetaHandMeshSpatialConf, GCHandleType.Pinned);
      MetaOldDLLMetaInputFaker.oldRightCppHand.hand_mesh.spatial_conf = MetaOldDLLMetaInputFaker.m_rightCppHandMeshSpatialConfDataHandle.AddrOfPinnedObject();
      MetaOldDLLMetaInputFaker.m_rightCppHandMeshSpatialConfDataHandle.Free();
      MetaOldDLLMetaInputFaker.m_leftCppHandMeshTemporalConfDataHandle = GCHandle.Alloc((object) MetaOldDLLMetaInputFaker.tempDataAllocationArraym_leftMetaHandMeshTemporalConf, GCHandleType.Pinned);
      MetaOldDLLMetaInputFaker.oldLeftCppHand.hand_mesh.temporal_conf = MetaOldDLLMetaInputFaker.m_leftCppHandMeshTemporalConfDataHandle.AddrOfPinnedObject();
      MetaOldDLLMetaInputFaker.m_leftCppHandMeshTemporalConfDataHandle.Free();
      MetaOldDLLMetaInputFaker.m_rightCppHandMeshTemporalConfDataHandle = GCHandle.Alloc((object) MetaOldDLLMetaInputFaker.tempDataAllocationArraym_rightMetaHandMeshTemporalConf, GCHandleType.Pinned);
      MetaOldDLLMetaInputFaker.oldRightCppHand.hand_mesh.temporal_conf = MetaOldDLLMetaInputFaker.m_rightCppHandMeshTemporalConfDataHandle.AddrOfPinnedObject();
      MetaOldDLLMetaInputFaker.m_rightCppHandMeshTemporalConfDataHandle.Free();
      MetaOldDLLMetaInputFaker.m_leftCppHandMeshNormalsDataHandle = GCHandle.Alloc((object) MetaOldDLLMetaInputFaker.tempDataAllocationArraym_leftMetaHandMeshNormals, GCHandleType.Pinned);
      MetaOldDLLMetaInputFaker.oldLeftCppHand.hand_mesh.normals = MetaOldDLLMetaInputFaker.m_leftCppHandMeshNormalsDataHandle.AddrOfPinnedObject();
      MetaOldDLLMetaInputFaker.m_leftCppHandMeshNormalsDataHandle.Free();
      MetaOldDLLMetaInputFaker.m_rightCppHandMeshNormalsDataHandle = GCHandle.Alloc((object) MetaOldDLLMetaInputFaker.tempDataAllocationArraym_rightMetaHandMeshNormals, GCHandleType.Pinned);
      MetaOldDLLMetaInputFaker.oldRightCppHand.hand_mesh.normals = MetaOldDLLMetaInputFaker.m_rightCppHandMeshNormalsDataHandle.AddrOfPinnedObject();
      MetaOldDLLMetaInputFaker.m_rightCppHandMeshNormalsDataHandle.Free();
    }

    internal static void GetHandGestureData(ref CppGestureData leftHand, ref CppGestureData rightHand)
    {
    }

    internal static void GetDynamicHandGestureData(ref DynamicGesture dynamicHandGesture)
    {
      dynamicHandGesture = (DynamicGesture) MetaOldDLLMetaInputFaker.dynamicGesture.dynamic_gesture;
    }

    internal struct DynamicGestureData
    {
      internal MetaOldDLLMetaInputFaker.DynamicGestureData.DynamicGesture dynamic_gesture;

      internal enum DynamicGesture
      {
        UNDETERMINED,
        NONE,
        PULL,
        PUSH,
        SWIPE_LEFT,
        SWIPE_RIGHT,
        THROW,
      }
    }
  }
}
