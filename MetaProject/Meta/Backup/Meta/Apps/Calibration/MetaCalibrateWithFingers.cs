// Decompiled with JetBrains decompiler
// Type: Meta.Apps.Calibration.MetaCalibrateWithFingers
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using Meta;
using UnityEngine;
using UnityEngine.UI;

namespace Meta.Apps.Calibration
{
  internal class MetaCalibrateWithFingers : MonoBehaviour
  {
    [SerializeField]
    private float scalingFactor;
    [SerializeField]
    private float maxCorrectionDistance;
    [SerializeField]
    private TextMesh fingerMessage;
    [SerializeField]
    private Text message;
    [SerializeField]
    private Text message2;
    [SerializeField]
    private GameObject fingerTracker;
    [SerializeField]
    private GameObject confirmButton;
    [SerializeField]
    private GameObject recalibrateButton;
    [SerializeField]
    private GameObject handOutline;
    [SerializeField]
    private GameObject handFilled;
    [SerializeField]
    private GameObject stallIndicator;
    [SerializeField]
    private GameObject leftCursor;
    [SerializeField]
    private GameObject rightCursor;
    [SerializeField]
    private GameObject markerObject;
    [SerializeField]
    private GameObject lensCanvas;
    [SerializeField]
    private GameObject confirmationCanvas;
    [SerializeField]
    private GameObject goodJobCanvas;
    [SerializeField]
    private GameObject MGUIStartLocation;
    [SerializeField]
    private GameObject instructions;
    [SerializeField]
    private GameObject interactionZoneVisualizer;
    [SerializeField]
    private GameObject reach;
    [SerializeField]
    private GameObject basketball;
    [SerializeField]
    private AudioClip countDownBegin;
    [SerializeField]
    private AudioClip countDownFinish;
    [SerializeField]
    private AudioClip calibrationComplete;
    [SerializeField]
    private Vector3 variance;
    [SerializeField]
    private float threshold;
    private MetaCalibrateWithFingers.CalibrationDesign design;
    private Vector3 newPosition;
    private bool stable;
    private Vector3 average;
    private int stallFrames;
    private Vector3[] recentLocations;
    private int index;
    private bool calibrating;
    private bool lensSelected;
    private bool confirmed;
    private float preCalibrationTime;
    private float stableStartTime;
    private float confirmationTime;
    private bool playStableSound;

    public MetaCalibrateWithFingers()
    {
      base.\u002Ector();
    }

    private void Start()
    {
      MetaSingleton<InputIndicators>.Instance.fingertipIndicators = true;
      MetaSingleton<InputIndicators>.Instance.handCloud = false;
      MetaSingleton<RenderingCameraManagerBase>.Instance.m_renderingProfile = (RenderingSettings) ScriptableObject.CreateInstance<RenderingSettings>();
      ((MetaBody) ((Component) this.instructions.get_transform().get_parent()).GetComponent<MetaBody>()).hud = true;
      this.variance = (Vector3) null;
      this.recentLocations = new Vector3[this.stallFrames];
      this.calibrating = true;
    }

    private void Update()
    {
      if (Hands.right == null)
        return;
      this.setVisibility();
      RenderingCameraManagerBase instance = MetaSingleton<RenderingCameraManagerBase>.Instance;
      if (this.design == MetaCalibrateWithFingers.CalibrationDesign.Marker)
      {
        this.newPosition = this.markerObject.get_transform().get_position();
      }
      else
      {
        this.newPosition = Hands.right.pointer.position;
        if (!Hands.right.isValid)
          this.newPosition = Hands.left.pointer.position;
      }
      this.fingerTracker.get_transform().set_position(this.newPosition);
      this.fingerMessage.set_text(string.Empty);
      if (!this.calibrating || !this.lensSelected || (double) this.preCalibrationTime > 0.0)
        return;
      this.handOutline.get_transform().set_localPosition(Vector3.op_Addition(new Vector3(0.0f, 0.0f, 0.399f), Vector3.op_Division(Vector3.op_Multiply(new Vector3(0.0f, 0.0f, -0.05f), 1f + Mathf.Sin(4f * Time.get_time())), 2f)));
      if (this.design != MetaCalibrateWithFingers.CalibrationDesign.Marker && !Hands.left.isValid && !Hands.right.isValid)
      {
        this.stable = false;
      }
      else
      {
        float x = (float) this.fingerTracker.get_transform().get_localPosition().x * this.scalingFactor;
        float y = (float) this.fingerTracker.get_transform().get_localPosition().y * this.scalingFactor;
        float num = Mathf.Sqrt((float) ((double) x * (double) x + (double) y * (double) y));
        if ((double) num > (double) this.maxCorrectionDistance)
        {
          x *= this.maxCorrectionDistance / num;
          y *= this.maxCorrectionDistance / num;
        }
        instance.TranslateDisplayTo(x, y);
        instance.CalibrationValue = (float) ((this.fingerTracker.get_transform().get_localPosition().z - 0.300000011920929) * 2.0);
        this.recentLocations[this.index++ % this.stallFrames] = this.newPosition;
        if (this.index >= this.stallFrames)
        {
          this.average.x = (__Null) (double) (this.average.y = this.average.z = (__Null) 0.0f);
          foreach (Vector3 vector3_1 in this.recentLocations)
          {
            MetaCalibrateWithFingers calibrateWithFingers = this;
            Vector3 vector3_2 = Vector3.op_Addition(calibrateWithFingers.average, vector3_1);
            calibrateWithFingers.average = vector3_2;
          }
          MetaCalibrateWithFingers calibrateWithFingers1 = this;
          Vector3 vector3_3 = Vector3.op_Division(calibrateWithFingers1.average, (float) this.stallFrames);
          calibrateWithFingers1.average = vector3_3;
          Vector3 vector3_4 = (Vector3) null;
          foreach (Vector3 vector3_1 in this.recentLocations)
          {
            // ISSUE: explicit reference operation
            // ISSUE: variable of a reference type
            Vector3& local1 = @vector3_4;
            // ISSUE: explicit reference operation
            // ISSUE: variable of the null type
            __Null local2 = (^local1).x + vector3_1.x * vector3_1.x;
            // ISSUE: explicit reference operation
            (^local1).x = local2;
            // ISSUE: explicit reference operation
            // ISSUE: variable of a reference type
            Vector3& local3 = @vector3_4;
            // ISSUE: explicit reference operation
            // ISSUE: variable of the null type
            __Null local4 = (^local3).y + vector3_1.y * vector3_1.y;
            // ISSUE: explicit reference operation
            (^local3).y = local4;
            // ISSUE: explicit reference operation
            // ISSUE: variable of a reference type
            Vector3& local5 = @vector3_4;
            // ISSUE: explicit reference operation
            // ISSUE: variable of the null type
            __Null local6 = (^local5).z + vector3_1.z * vector3_1.z;
            // ISSUE: explicit reference operation
            (^local5).z = local6;
          }
          vector3_4 = Vector3.op_Division(vector3_4, (float) this.stallFrames);
          this.variance.x = vector3_4.x - this.average.x * this.average.x;
          this.variance.y = vector3_4.y - this.average.y * this.average.y;
          this.variance.z = vector3_4.z - this.average.z * this.average.z;
          this.stable = this.variance.x < (double) this.threshold && this.variance.y < (double) this.threshold && this.variance.z < (double) this.threshold;
        }
      }
      if (Vector3.op_Inequality(MetaCamera.GetCameraProfile().m_sensorScreenOffset, new Vector3(-0.01275f, -0.025f, 0.0f)))
        Debug.Log((object) "Sensor Screen Offset in camera profile should be (-0.01275, -0.025, 0)!");
      if ((double) MetaCamera.GetCameraProfile().m_sensorScreenAngle != -5.0)
        Debug.Log((object) "Problem: Sensor Screen Angle in camera profile should be -5!");
      if (this.stable)
      {
        float num = (Time.get_time() - this.stableStartTime) / this.confirmationTime;
        if (this.playStableSound)
        {
          ((AudioSource) ((Component) this).GetComponent<AudioSource>()).set_clip(this.countDownBegin);
          ((AudioSource) ((Component) this).GetComponent<AudioSource>()).Play();
          this.playStableSound = false;
        }
        this.fingerMessage.set_text(string.Format("Hold still: {0}", (object) Mathf.Ceil((float) ((1.0 - (double) num) * 5.0))));
        this.stallIndicator.get_transform().set_localScale(Vector3.op_Multiply(0.04f, new Vector3(1f - num, 1f - num, 1f - num)));
        if ((double) num <= 1.0)
          return;
        ((AudioSource) ((Component) this).GetComponent<AudioSource>()).set_clip(this.countDownFinish);
        ((AudioSource) ((Component) this).GetComponent<AudioSource>()).Play();
        this.calibrating = false;
        this.confirmationCanvas.get_transform().set_position(this.MGUIStartLocation.get_transform().get_position());
        this.confirmationCanvas.get_transform().set_rotation(this.MGUIStartLocation.get_transform().get_rotation());
      }
      else
      {
        this.stableStartTime = Time.get_time();
        this.stallIndicator.get_transform().set_localScale(new Vector3(0.0f, 0.0f, 0.0f));
        this.playStableSound = true;
      }
    }

    public void Recalibrate()
    {
      this.calibrating = true;
      this.stableStartTime = Time.get_time();
      this.index = 0;
      for (int index = 0; index < this.recentLocations.Length; ++index)
        this.recentLocations[index] = new Vector3((float) (100 * index), (float) (100 * index), (float) (100 * index));
    }

    public void RestartCalibration()
    {
      this.lensCanvas.get_transform().set_position(this.MGUIStartLocation.get_transform().get_position());
      this.lensCanvas.get_transform().set_rotation(this.MGUIStartLocation.get_transform().get_rotation());
      this.lensCanvas.SetActive(true);
      this.goodJobCanvas.SetActive(false);
      this.lensSelected = false;
      this.confirmed = false;
      this.calibrating = true;
    }

    public void ConfirmSettings()
    {
      this.reach.get_transform().set_position(this.MGUIStartLocation.get_transform().get_position());
      this.reach.get_transform().set_rotation(this.MGUIStartLocation.get_transform().get_rotation());
      this.reach.SetActive(true);
      this.confirmationCanvas.SetActive(false);
      this.calibrating = false;
      this.confirmed = true;
      this.SaveValues();
    }

    public void SetReach(float reachDist)
    {
      MetaSingleton<MetaUserSettingsManager>.Instance.SetUserProfileValue("m_reachDistance", (object) reachDist);
      ((AudioSource) ((Component) this).GetComponent<AudioSource>()).set_clip(this.calibrationComplete);
      ((AudioSource) ((Component) this).GetComponent<AudioSource>()).Play();
      this.reach.SetActive(false);
      this.goodJobCanvas.SetActive(true);
      this.goodJobCanvas.get_transform().set_position(this.MGUIStartLocation.get_transform().get_position());
      this.goodJobCanvas.get_transform().set_rotation(this.MGUIStartLocation.get_transform().get_rotation());
    }

    public void Finish()
    {
      Application.Quit();
    }

    public void SelectFOVLens()
    {
      this.lensSelected = true;
      this.lensCanvas.SetActive(false);
      MetaSingleton<MetaUserSettingsManager>.Instance.SetUserProfileValue("m_FOVExpanded", (object) true);
      MetaSingleton<RenderingCameraManagerBase>.Instance.fovExpanded = true;
      this.instructions.SetActive(true);
    }

    public void SelectShadeLens()
    {
      this.lensSelected = true;
      this.lensCanvas.SetActive(false);
      MetaSingleton<MetaUserSettingsManager>.Instance.SetUserProfileValue("m_FOVExpanded", (object) false);
      MetaSingleton<RenderingCameraManagerBase>.Instance.fovExpanded = false;
      this.instructions.SetActive(true);
    }

    public void SetReachDistance(float distance)
    {
      MetaSingleton<MetaUserSettingsManager>.Instance.SetUserProfileValue("m_reachDistance", (object) distance);
    }

    private void SaveValues()
    {
      MetaSingleton<RenderingCameraManagerBase>.Instance.SaveRenderingProfile();
    }

    private void setVisibility()
    {
      float num1 = 0.175f;
      float num2 = 0.31875f;
      this.interactionZoneVisualizer.get_transform().set_localScale(new Vector3(this.maxCorrectionDistance / this.scalingFactor, (float) (((double) num2 - (double) num1) / 2.0), this.maxCorrectionDistance / this.scalingFactor));
      this.interactionZoneVisualizer.get_transform().set_localPosition(new Vector3(0.0f, 0.0f, (float) (((double) num2 + (double) num1) / 2.0)));
      if (!this.lensSelected)
        this.lensCanvas.SetActive(true);
      else if ((double) this.preCalibrationTime > 0.0)
      {
        this.preCalibrationTime -= Time.get_deltaTime();
        MetaSingleton<RenderingCameraManagerBase>.Instance.m_renderingProfile = (RenderingSettings) ScriptableObject.CreateInstance<RenderingSettings>();
      }
      else
      {
        this.fingerTracker.SetActive(true);
        bool flag = this.calibrating && !Hands.right.isValid && !Hands.left.isValid;
        if (this.design == MetaCalibrateWithFingers.CalibrationDesign.Cloud)
        {
          this.instructions.SetActive(flag);
          MetaSingleton<InputIndicators>.Instance.handCloud = true;
          MetaSingleton<InputIndicators>.Instance.fingertipIndicators = true;
          this.handOutline.SetActive(flag);
          this.handFilled.SetActive(flag);
          this.leftCursor.SetActive(false);
          this.rightCursor.SetActive(false);
          this.markerObject.SetActive(false);
        }
        else if (this.design == MetaCalibrateWithFingers.CalibrationDesign.Fingertip)
        {
          this.instructions.SetActive(flag);
          MetaSingleton<InputIndicators>.Instance.handCloud = false;
          MetaSingleton<InputIndicators>.Instance.fingertipIndicators = !this.calibrating;
          this.handOutline.SetActive(false);
          this.handFilled.SetActive(false);
          this.leftCursor.SetActive(false);
          this.rightCursor.SetActive(false);
          this.markerObject.SetActive(false);
          this.basketball.SetActive(this.calibrating && !flag);
          if (this.basketball.get_activeSelf())
            this.basketball.get_transform().Rotate(0.0f, Time.get_deltaTime() * 250f, 0.0f);
        }
        else if (this.design == MetaCalibrateWithFingers.CalibrationDesign.CircleDot)
        {
          this.instructions.SetActive(flag);
          MetaSingleton<InputIndicators>.Instance.handCloud = false;
          MetaSingleton<InputIndicators>.Instance.fingertipIndicators = !this.calibrating;
          this.handOutline.SetActive(false);
          this.handFilled.SetActive(false);
          this.leftCursor.SetActive(this.calibrating);
          this.rightCursor.SetActive(this.calibrating);
          this.markerObject.SetActive(false);
        }
        else if (this.design == MetaCalibrateWithFingers.CalibrationDesign.Marker)
        {
          this.instructions.SetActive(false);
          MetaSingleton<InputIndicators>.Instance.handCloud = false;
          MetaSingleton<InputIndicators>.Instance.fingertipIndicators = true;
          this.handOutline.SetActive(false);
          this.handFilled.SetActive(false);
          this.leftCursor.SetActive(false);
          this.rightCursor.SetActive(false);
          this.markerObject.SetActive(true);
        }
        if (flag)
          MetaSingleton<RenderingCameraManagerBase>.Instance.m_renderingProfile = (RenderingSettings) ScriptableObject.CreateInstance<RenderingSettings>();
        this.stallIndicator.SetActive(this.calibrating);
        this.confirmationCanvas.SetActive(!this.calibrating && !this.confirmed);
      }
    }

    private enum CalibrationDesign
    {
      Cloud,
      Fingertip,
      CircleDot,
      Marker,
    }
  }
}
