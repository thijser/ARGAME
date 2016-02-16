// Decompiled with JetBrains decompiler
// Type: Meta.HelpKeys
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;
using UnityEngine.UI;

namespace Meta
{
  internal class HelpKeys : MonoBehaviour
  {
    [SerializeField]
    private GameObject HelpPanel;
    [SerializeField]
    private GameObject HelpKey;
    [SerializeField]
    private GameObject MonoStereoKey;
    [SerializeField]
    private GameObject WideRectifiedKey;
    [SerializeField]
    private GameObject RecalibrateKey;
    [SerializeField]
    private GameObject ReloadKey;
    [SerializeField]
    private GameObject HandPointCloudKey;
    [SerializeField]
    private GameObject FingertipIndicatorsKey;
    [SerializeField]
    private GameObject RGBFeedKey;
    [SerializeField]
    private GameObject ShadeFOVLensKey;

    public HelpKeys()
    {
      base.\u002Ector();
    }

    private void UpdateKeyText(GameObject key, string newKeyText)
    {
      if (!Object.op_Inequality((Object) key, (Object) null) || !Object.op_Inequality((Object) key.GetComponent<Text>(), (Object) null))
        return;
      ((Text) key.GetComponent<Text>()).set_text(newKeyText.ToUpper());
    }

    public void UpdateKeys()
    {
      this.UpdateKeyText(this.HelpKey, MetaSingleton<KeyboardShortcuts>.Instance.toggleHelpPanel);
      this.UpdateKeyText(this.MonoStereoKey, MetaSingleton<KeyboardShortcuts>.Instance.toggleMonoStereo);
      this.UpdateKeyText(this.WideRectifiedKey, MetaSingleton<KeyboardShortcuts>.Instance.toggleWideRectified);
      this.UpdateKeyText(this.RecalibrateKey, MetaSingleton<KeyboardShortcuts>.Instance.recalibrate);
      this.UpdateKeyText(this.ReloadKey, MetaSingleton<KeyboardShortcuts>.Instance.reload);
      this.UpdateKeyText(this.HandPointCloudKey, MetaSingleton<KeyboardShortcuts>.Instance.toggleHandPointCloud);
      this.UpdateKeyText(this.FingertipIndicatorsKey, MetaSingleton<KeyboardShortcuts>.Instance.toggleFingertipIndicators);
      this.UpdateKeyText(this.RGBFeedKey, MetaSingleton<KeyboardShortcuts>.Instance.toggleRGBFeed);
      this.UpdateKeyText(this.ShadeFOVLensKey, MetaSingleton<KeyboardShortcuts>.Instance.toggleShadeFovLens);
    }

    private void Update()
    {
      if (!Input.GetKeyDown(MetaSingleton<KeyboardShortcuts>.Instance.toggleHelpPanel))
        return;
      this.UpdateKeys();
      if (!MetaCore.Instance.trueScale || MetaSingleton<RenderingCameraManagerBase>.Instance.fovExpanded)
        ((Transform) this.HelpPanel.GetComponent<RectTransform>()).set_localScale(new Vector3(0.00025f, 0.00025f, 0.00025f));
      else
        ((Transform) this.HelpPanel.GetComponent<RectTransform>()).set_localScale(new Vector3(0.00017f, 0.00017f, 0.00017f));
      this.HelpPanel.SetActive(!this.HelpPanel.get_activeSelf());
    }
  }
}
