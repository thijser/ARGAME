using System;
using UnityEngine;
using UnityEngine.UI;

namespace Meta.Apps.Calibration
{
	internal class MetaCalibrateWithFingers : MonoBehaviour
	{
		private enum CalibrationDesign
		{
			Cloud,
			Fingertip,
			CircleDot,
			Marker
		}

		[SerializeField]
		private float scalingFactor = -4000f;

		[SerializeField]
		private float maxCorrectionDistance = 300f;

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
		private float threshold = 1E-05f;

		private MetaCalibrateWithFingers.CalibrationDesign design = MetaCalibrateWithFingers.CalibrationDesign.Fingertip;

		private Vector3 newPosition;

		private bool stable;

		private Vector3 average;

		private int stallFrames = 50;

		private Vector3[] recentLocations;

		private int index;

		private bool calibrating;

		private bool lensSelected;

		private bool confirmed;

		private float preCalibrationTime = 3f;

		private float stableStartTime;

		private float confirmationTime = 5f;

		private bool playStableSound;

		private void Start()
		{
			MetaSingleton<InputIndicators>.Instance.fingertipIndicators = true;
			MetaSingleton<InputIndicators>.Instance.handCloud = false;
			MetaSingleton<RenderingCameraManagerBase>.Instance.m_renderingProfile = ScriptableObject.CreateInstance<RenderingSettings>();
			this.instructions.transform.parent.GetComponent<MetaBody>().hud = true;
			this.variance = default(Vector3);
			this.recentLocations = new Vector3[this.stallFrames];
			this.calibrating = true;
		}

		private void Update()
		{
			if (Hands.right != null)
			{
				this.setVisibility();
				RenderingCameraManagerBase instance = MetaSingleton<RenderingCameraManagerBase>.Instance;
				if (this.design == MetaCalibrateWithFingers.CalibrationDesign.Marker)
				{
					this.newPosition = this.markerObject.transform.position;
				}
				else
				{
					this.newPosition = Hands.right.pointer.position;
					if (!Hands.right.isValid)
					{
						this.newPosition = Hands.left.pointer.position;
					}
				}
				this.fingerTracker.transform.position = this.newPosition;
				this.fingerMessage.text = string.Empty;
				if (this.calibrating && this.lensSelected && this.preCalibrationTime <= 0f)
				{
					this.handOutline.transform.localPosition = new Vector3(0f, 0f, 0.399f) + new Vector3(0f, 0f, -0.05f) * (1f + Mathf.Sin(4f * Time.time)) / 2f;
					if (this.design != MetaCalibrateWithFingers.CalibrationDesign.Marker && !Hands.left.isValid && !Hands.right.isValid)
					{
						this.stable = false;
					}
					else
					{
						float num = this.fingerTracker.transform.localPosition.x * this.scalingFactor;
						float num2 = this.fingerTracker.transform.localPosition.y * this.scalingFactor;
						float num3 = Mathf.Sqrt(num * num + num2 * num2);
						if (num3 > this.maxCorrectionDistance)
						{
							num *= this.maxCorrectionDistance / num3;
							num2 *= this.maxCorrectionDistance / num3;
						}
						instance.TranslateDisplayTo(num, num2);
						instance.CalibrationValue = (this.fingerTracker.transform.localPosition.z - 0.3f) * 2f;
						this.recentLocations[this.index++ % this.stallFrames] = this.newPosition;
						if (this.index >= this.stallFrames)
						{
							this.average.x = (this.average.y = (this.average.z = 0f));
							Vector3[] array = this.recentLocations;
							for (int i = 0; i < array.Length; i++)
							{
								Vector3 vector = array[i];
								this.average += vector;
							}
							this.average /= (float)this.stallFrames;
							Vector3 vector2 = default(Vector3);
							Vector3[] array2 = this.recentLocations;
							for (int j = 0; j < array2.Length; j++)
							{
								Vector3 vector3 = array2[j];
								vector2.x += vector3.x * vector3.x;
								vector2.y += vector3.y * vector3.y;
								vector2.z += vector3.z * vector3.z;
							}
							vector2 /= (float)this.stallFrames;
							this.variance.x = vector2.x - this.average.x * this.average.x;
							this.variance.y = vector2.y - this.average.y * this.average.y;
							this.variance.z = vector2.z - this.average.z * this.average.z;
							this.stable = (this.variance.x < this.threshold && this.variance.y < this.threshold && this.variance.z < this.threshold);
						}
					}
					if (MetaCamera.GetCameraProfile().m_sensorScreenOffset != new Vector3(-0.01275f, -0.025f, 0f))
					{
						Debug.Log("Sensor Screen Offset in UnityEngine.Camera profile should be (-0.01275, -0.025, 0)!");
					}
					if (MetaCamera.GetCameraProfile().m_sensorScreenAngle != -5f)
					{
						Debug.Log("Problem: Sensor Screen Angle in UnityEngine.Camera profile should be -5!");
					}
					if (this.stable)
					{
						float num4 = (Time.time - this.stableStartTime) / this.confirmationTime;
						if (this.playStableSound)
						{
							base.GetComponent<AudioSource>().clip = this.countDownBegin;
							base.GetComponent<AudioSource>().Play();
							this.playStableSound = false;
						}
						this.fingerMessage.text = string.Format("Hold still: {0}", Mathf.Ceil((1f - num4) * 5f));
						this.stallIndicator.transform.localScale = 0.04f * new Vector3(1f - num4, 1f - num4, 1f - num4);
						if (num4 > 1f)
						{
							base.GetComponent<AudioSource>().clip = this.countDownFinish;
							base.GetComponent<AudioSource>().Play();
							this.calibrating = false;
							this.confirmationCanvas.transform.position = this.MGUIStartLocation.transform.position;
							this.confirmationCanvas.transform.rotation = this.MGUIStartLocation.transform.rotation;
						}
					}
					else
					{
						this.stableStartTime = Time.time;
						this.stallIndicator.transform.localScale = new Vector3(0f, 0f, 0f);
						this.playStableSound = true;
					}
				}
			}
		}

		public void Recalibrate()
		{
			this.calibrating = true;
			this.stableStartTime = Time.time;
			this.index = 0;
			for (int i = 0; i < this.recentLocations.Length; i++)
			{
				this.recentLocations[i] = new Vector3((float)(100 * i), (float)(100 * i), (float)(100 * i));
			}
		}

		public void RestartCalibration()
		{
			this.lensCanvas.transform.position = this.MGUIStartLocation.transform.position;
			this.lensCanvas.transform.rotation = this.MGUIStartLocation.transform.rotation;
			this.lensCanvas.SetActive(true);
			this.goodJobCanvas.SetActive(false);
			this.lensSelected = false;
			this.confirmed = false;
			this.calibrating = true;
		}

		public void ConfirmSettings()
		{
			this.reach.transform.position = this.MGUIStartLocation.transform.position;
			this.reach.transform.rotation = this.MGUIStartLocation.transform.rotation;
			this.reach.SetActive(true);
			this.confirmationCanvas.SetActive(false);
			this.calibrating = false;
			this.confirmed = true;
			this.SaveValues();
		}

		public void SetReach(float reachDist)
		{
			MetaSingleton<MetaUserSettingsManager>.Instance.SetUserProfileValue("m_reachDistance", reachDist);
			base.GetComponent<AudioSource>().clip = this.calibrationComplete;
			base.GetComponent<AudioSource>().Play();
			this.reach.SetActive(false);
			this.goodJobCanvas.SetActive(true);
			this.goodJobCanvas.transform.position = this.MGUIStartLocation.transform.position;
			this.goodJobCanvas.transform.rotation = this.MGUIStartLocation.transform.rotation;
		}

		public void Finish()
		{
			Application.Quit();
		}

		public void SelectFOVLens()
		{
			this.lensSelected = true;
			this.lensCanvas.SetActive(false);
			MetaSingleton<MetaUserSettingsManager>.Instance.SetUserProfileValue("m_FOVExpanded", true);
			MetaSingleton<RenderingCameraManagerBase>.Instance.fovExpanded = true;
			this.instructions.SetActive(true);
		}

		public void SelectShadeLens()
		{
			this.lensSelected = true;
			this.lensCanvas.SetActive(false);
			MetaSingleton<MetaUserSettingsManager>.Instance.SetUserProfileValue("m_FOVExpanded", false);
			MetaSingleton<RenderingCameraManagerBase>.Instance.fovExpanded = false;
			this.instructions.SetActive(true);
		}

		public void SetReachDistance(float distance)
		{
			MetaSingleton<MetaUserSettingsManager>.Instance.SetUserProfileValue("m_reachDistance", distance);
		}

		private void SaveValues()
		{
			MetaSingleton<RenderingCameraManagerBase>.Instance.SaveRenderingProfile();
		}

		private void setVisibility()
		{
			float num = 0.175000012f;
			float num2 = 0.318750024f;
			this.interactionZoneVisualizer.transform.localScale = new Vector3(this.maxCorrectionDistance / this.scalingFactor, (num2 - num) / 2f, this.maxCorrectionDistance / this.scalingFactor);
			this.interactionZoneVisualizer.transform.localPosition = new Vector3(0f, 0f, (num2 + num) / 2f);
			if (!this.lensSelected)
			{
				this.lensCanvas.SetActive(true);
			}
			else if (this.preCalibrationTime > 0f)
			{
				this.preCalibrationTime -= Time.deltaTime;
				MetaSingleton<RenderingCameraManagerBase>.Instance.m_renderingProfile = ScriptableObject.CreateInstance<RenderingSettings>();
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
					if (this.basketball.activeSelf)
					{
						this.basketball.transform.Rotate(0f, Time.deltaTime * 250f, 0f);
					}
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
				{
					MetaSingleton<RenderingCameraManagerBase>.Instance.m_renderingProfile = ScriptableObject.CreateInstance<RenderingSettings>();
				}
				this.stallIndicator.SetActive(this.calibrating);
				this.confirmationCanvas.SetActive(!this.calibrating && !this.confirmed);
			}
		}
	}
}
