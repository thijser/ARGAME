using System;
using UnityEngine;

namespace Meta
{
	public class MetaTrigger : MonoBehaviour
	{
		public enum Mode
		{
			Trigger,
			Replace,
			Activate,
			Enable,
			Animate,
			Deactivate,
			Message,
			URL
		}

		public MetaTrigger.Mode action = MetaTrigger.Mode.Activate;

		public string triggerName = string.Empty;

		public string messageName = string.Empty;

		public string messageValue = string.Empty;

		public Object target;

		public GameObject source;

		public int triggerCount = 1;

		public bool repeatTrigger;

		public bool activateOnClick;

		public bool activateOnFinger;

		public bool deactivateToggle;

		public bool delayingTimer;

		public bool autoStartAfterDelay;

		public float delayTime;

		public float delayTimeStart;

		public bool activateOnReticle;

		public float activateOnReticleDelay;

		public bool activateOnReticleFeedback;

		public bool delayedTrigger;

		public bool delaying;

		public string url = string.Empty;

		public void DoTrigger()
		{
			if (this.delayedTrigger)
			{
				this.delaying = true;
			}
			else
			{
				this.HandleTrigger();
			}
		}

		private void HandleTrigger()
		{
			this.triggerCount--;
			if (this.triggerCount == 0 || this.repeatTrigger)
			{
				Object @object = (!(this.target != null)) ? base.get_gameObject() : this.target;
				Behaviour behaviour = @object as Behaviour;
				GameObject gameObject = @object as GameObject;
				if (behaviour != null)
				{
					gameObject = behaviour.get_gameObject();
				}
				switch (this.action)
				{
				case MetaTrigger.Mode.Trigger:
					gameObject.BroadcastMessage("DoTrigger");
					break;
				case MetaTrigger.Mode.Replace:
					if (this.source != null)
					{
						Object.Instantiate(this.source, gameObject.get_transform().get_position(), gameObject.get_transform().get_rotation());
						Object.DestroyObject(gameObject);
					}
					break;
				case MetaTrigger.Mode.Activate:
					gameObject.SetActive(true);
					break;
				case MetaTrigger.Mode.Enable:
					if (behaviour != null)
					{
						behaviour.set_enabled(true);
					}
					break;
				case MetaTrigger.Mode.Animate:
					gameObject.GetComponent<Animation>().Play();
					break;
				case MetaTrigger.Mode.Deactivate:
					gameObject.SetActive(false);
					break;
				case MetaTrigger.Mode.Message:
					if (this.messageValue == string.Empty)
					{
						gameObject.SendMessage(this.messageName, 1);
					}
					else
					{
						gameObject.SendMessage(this.messageName, this.messageValue, 1);
					}
					break;
				case MetaTrigger.Mode.URL:
					if (this.url != string.Empty)
					{
						this.url = this.url.Replace("%META_SDK%", Environment.GetEnvironmentVariable("META_SDK").Replace("\\", "/"));
						Application.OpenURL(this.url);
					}
					break;
				}
				if (this.deactivateToggle)
				{
					if (this.action == MetaTrigger.Mode.Activate)
					{
						this.action = MetaTrigger.Mode.Deactivate;
					}
					else if (this.action == MetaTrigger.Mode.Deactivate)
					{
						this.action = MetaTrigger.Mode.Activate;
					}
				}
			}
		}

		public void StartDelay()
		{
			this.delayTimeStart = Time.get_time();
			this.delayingTimer = true;
		}

		public void OnReticleHit()
		{
			if (this.activateOnReticle)
			{
				this.DoTrigger();
			}
		}

		private void Start()
		{
			if (this.autoStartAfterDelay)
			{
				this.StartDelay();
			}
		}

		private void Update()
		{
			if (this.activateOnFinger && ((Hands.left.isValid && base.GetComponent<Collider>().get_bounds().Contains(Hands.left.pointer.position)) || (Hands.right.isValid && base.GetComponent<Collider>().get_bounds().Contains(Hands.right.pointer.position))))
			{
				this.DoTrigger();
			}
			if (this.delayingTimer && Time.get_time() - this.delayTimeStart > this.delayTime)
			{
				this.DoTrigger();
			}
		}

		private void LateUpdate()
		{
			if (this.delayedTrigger && this.delaying)
			{
				this.HandleTrigger();
				this.delaying = false;
			}
		}

		private void OnMouseUp()
		{
			if (this.activateOnClick)
			{
				this.DoTrigger();
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (this.activateOnFinger)
			{
				if (other.get_name().Contains("Pointer"))
				{
					this.DoTrigger();
				}
			}
			else
			{
				this.DoTrigger();
			}
		}
	}
}