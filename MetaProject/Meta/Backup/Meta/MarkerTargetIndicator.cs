// Decompiled with JetBrains decompiler
// Type: Meta.MarkerTargetIndicator
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System.Collections.Generic;
using UnityEngine;

namespace Meta
{
  public class MarkerTargetIndicator : MonoBehaviour
  {
    public static Dictionary<int, GameObject> markerIndicators = new Dictionary<int, GameObject>();
    private static bool _indicatorsVisible = true;
    [SerializeField]
    private GameObject _markerIndicatorBase;
    [SerializeField]
    private bool _isController;
    private static GameObject _indicatorParent;

    public static bool indicatorsVisible
    {
      get
      {
        return MarkerTargetIndicator._indicatorsVisible;
      }
      set
      {
        if (MarkerTargetIndicator._indicatorsVisible == value || !Object.op_Inequality((Object) MarkerTargetIndicator._indicatorParent, (Object) null))
          return;
        MarkerTargetIndicator._indicatorParent.SetActive(value);
        MarkerTargetIndicator._indicatorsVisible = value;
      }
    }

    public MarkerTargetIndicator()
    {
      base.\u002Ector();
    }

    public static KeyValuePair<int, GameObject> GetClosestMarkerIndicator(Transform gameObj, ref float distance)
    {
      KeyValuePair<int, GameObject> keyValuePair = new KeyValuePair<int, GameObject>(-1, (GameObject) null);
      float num1 = MetaSingleton<MarkerDetector>.Instance.markerReleaseRange;
      using (Dictionary<int, GameObject>.Enumerator enumerator = MarkerTargetIndicator.markerIndicators.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          KeyValuePair<int, GameObject> current = enumerator.Current;
          float num2 = Vector3.Distance(current.Value.get_transform().get_position(), gameObj.get_position());
          if ((double) num2 < (double) num1)
          {
            num1 = num2;
            keyValuePair = current;
          }
        }
      }
      distance = num1;
      return keyValuePair;
    }

    public static void UpdateClosestMarkerID(Transform source, bool active)
    {
      MetaBody metaBody = (MetaBody) ((Component) source).GetComponent<MetaBody>();
      if (!Object.op_Inequality((Object) metaBody, (Object) null) || !metaBody.markerTarget || !metaBody.markerTargetPlaceable)
        return;
      float distance = -1f;
      KeyValuePair<int, GameObject> closestMarkerIndicator = MarkerTargetIndicator.GetClosestMarkerIndicator(source, ref distance);
      if (MarkerTargetIndicator.indicatorsVisible && metaBody.markerTargetID != -1 && (metaBody.markerTargetID != closestMarkerIndicator.Key || !active))
        MarkerTargetIndicator.UnhighlightMarker(metaBody.markerTargetID);
      metaBody.markerTargetID = closestMarkerIndicator.Key;
      if (!MarkerTargetIndicator.indicatorsVisible || closestMarkerIndicator.Key == -1 || (!metaBody.markerTargetPlaceableHighlight || !active))
        return;
      MarkerTargetIndicator.HighlightMarker(metaBody.markerTargetID, distance);
    }

    public static void HighlightMarker(int markerID, float distance)
    {
      if (!MarkerTargetIndicator.markerIndicators.ContainsKey(markerID))
        return;
      GameObject gameObject = MarkerTargetIndicator.markerIndicators[markerID];
      HighlightableObject highlightableObject = (HighlightableObject) gameObject.GetComponent<HighlightableObject>();
      if (Object.op_Equality((Object) highlightableObject, (Object) null))
        highlightableObject = (HighlightableObject) gameObject.AddComponent<HighlightableObject>();
      ((Behaviour) highlightableObject).set_enabled(true);
      float num1 = 0.05f;
      float num2 = MetaSingleton<MarkerDetector>.Instance.markerReleaseRange;
      float num3 = (float) (1.0 - ((double) distance - (double) num1) / ((double) num2 - (double) num1));
      if ((double) num3 < 0.0)
        num3 = 0.0f;
      if ((double) num3 > 1.0)
        num3 = 1f;
      highlightableObject.ConstantOnImmediate(new Color(1f, 1f, 1f, num3));
    }

    public static void UnhighlightMarker(int markerID)
    {
      if (!MarkerTargetIndicator.markerIndicators.ContainsKey(markerID))
        return;
      HighlightableObject highlightableObject = (HighlightableObject) MarkerTargetIndicator.markerIndicators[markerID].GetComponent<HighlightableObject>();
      if (!Object.op_Inequality((Object) highlightableObject, (Object) null))
        return;
      highlightableObject.ConstantOff();
    }

    private void UpdateMarkerIndicators()
    {
      using (List<int>.Enumerator enumerator = MetaSingleton<MarkerDetector>.Instance.updatedMarkerTransforms.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          int current = enumerator.Current;
          if (!MarkerTargetIndicator.markerIndicators.ContainsKey(current))
          {
            GameObject gameObject = (GameObject) Object.Instantiate((Object) this._markerIndicatorBase);
            if (Object.op_Equality((Object) MarkerTargetIndicator._indicatorParent, (Object) null))
            {
              MarkerTargetIndicator._indicatorParent = new GameObject();
              ((Object) MarkerTargetIndicator._indicatorParent).set_name("MarkerIndicators");
              MarkerTargetIndicator._indicatorParent.get_transform().set_position(Vector3.get_zero());
              MarkerTargetIndicator._indicatorParent.get_transform().set_rotation(Quaternion.get_identity());
            }
            ((Object) gameObject).set_name("MarkerIndicator" + (object) current);
            gameObject.get_transform().set_parent(MarkerTargetIndicator._indicatorParent.get_transform());
            Texture2D texture2D = Resources.Load("Markers/AprilTags/tag36_11_" + current.ToString("D5")) as Texture2D;
            if (gameObject.GetComponentsInChildren<Renderer>(true).Length > 0)
              ((Renderer) gameObject.GetComponentsInChildren<Renderer>(true)[0]).get_material().set_mainTexture((Texture) texture2D);
            MarkerTarget markerTarget = (MarkerTarget) gameObject.GetComponent<MarkerTarget>();
            if (Object.op_Inequality((Object) markerTarget, (Object) null))
              markerTarget.id = current;
            MarkerTargetIndicator.markerIndicators.Add(current, gameObject);
          }
          Transform transform = MarkerTargetIndicator.markerIndicators[current].get_transform();
          MetaSingleton<MarkerDetector>.Instance.GetMarkerTransform(current, ref transform);
        }
      }
    }

    private void Awake()
    {
      if (!Object.op_Equality((Object) MarkerTargetIndicator._indicatorParent, (Object) null))
        return;
      MarkerTargetIndicator._indicatorParent = new GameObject();
      ((Object) MarkerTargetIndicator._indicatorParent).set_name("MarkerIndicators");
      MarkerTargetIndicator._indicatorParent.get_transform().set_position(Vector3.get_zero());
      MarkerTargetIndicator._indicatorParent.get_transform().set_rotation(Quaternion.get_identity());
    }

    private void Start()
    {
      if (!this._isController)
        return;
      MarkerTargetIndicator.markerIndicators = new Dictionary<int, GameObject>();
    }

    private void Update()
    {
      if (!this._isController || !Object.op_Inequality((Object) MetaSingleton<MarkerDetector>.Instance, (Object) null) || !((Behaviour) MetaSingleton<MarkerDetector>.Instance).get_enabled())
        return;
      this.UpdateMarkerIndicators();
    }

    private void OnEnable()
    {
      if (!Object.op_Inequality((Object) MarkerTargetIndicator._indicatorParent, (Object) null))
        return;
      MarkerTargetIndicator._indicatorParent.SetActive(true);
    }

    private void OnDisable()
    {
      if (!Object.op_Inequality((Object) MarkerTargetIndicator._indicatorParent, (Object) null))
        return;
      MarkerTargetIndicator._indicatorParent.SetActive(false);
    }
  }
}
