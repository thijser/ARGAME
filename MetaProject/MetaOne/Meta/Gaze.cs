using System;
using UnityEngine;

namespace Meta
{
	public class Gaze : MetaSingleton<Gaze>
	{
		private int m_numRows = 5;

		private int m_raysPerRow = 5;

		private float m_angle = 5f;

		public LayerMask m_layerMask;

		public bool m_showRays;

		private bool m_descend;

		public GameObject objectOfInterest;

		private bool m_useWeights;

		public float[] m_rowWeights = new float[5];

		public GameObject gazeTarget;

		public GameObject lastGazeTarget;

		public bool isGazeOver;

		public float gazeTargetTime;

		public float lastGazeTargetTime;

		public float gazeTimeElapsed;

		public float dwellTimeThreshold = 2f;

		public bool hasGazeDwelled;

		public int numHits;

		private void UpdateMultiraycast()
		{
			RaycastHit[] array = MultiRaycast.MultiRayCast(base.get_transform().get_position(), base.get_transform().get_forward(), this.m_numRows, this.m_raysPerRow, this.m_angle, this.m_layerMask, this.m_descend);
			GameObject gameObject = (!this.m_useWeights) ? MultiRaycast.MostHit(array) : MultiRaycast.MostHitWithWeights(array, this.m_rowWeights);
			this.objectOfInterest = gameObject;
			if (this.m_showRays)
			{
				RaycastHit[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					RaycastHit raycastHit = array2[i];
					Debug.DrawLine(base.get_transform().get_position(), raycastHit.get_point());
				}
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
			this.UpdateMultiraycast();
			this.GazeCast();
		}

		public void CheckGazeAssertions()
		{
		}

		public void GazeCast()
		{
			this.CheckGazeAssertions();
			this.isGazeOver = false;
			Ray ray = Camera.get_main().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			RaycastHit[] array = Physics.RaycastAll(ray, 100000f, -1);
			if (array == null)
			{
				this.numHits = 0;
			}
			else
			{
				this.numHits = array.Length;
			}
			if (array != null && array.Length > 0)
			{
				RaycastHit[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					RaycastHit raycastHit = array2[i];
					if (raycastHit.get_collider() != null && raycastHit.get_collider().get_gameObject().GetComponent<MetaBody>() != null)
					{
						MetaBody component = raycastHit.get_collider().get_gameObject().GetComponent<MetaBody>();
						if (this.gazeTarget != null && component.get_gameObject() != this.gazeTarget && this.gazeTarget.GetComponent<MetaBody>().gazeableDwellable && this.hasGazeDwelled)
						{
							this.gazeTarget.get_gameObject().SendMessage("OnGazeDwellExit", 1);
							this.hasGazeDwelled = false;
							return;
						}
						if (component.gazeable)
						{
							this.isGazeOver = true;
							if (component.get_gameObject() != this.gazeTarget)
							{
								if (this.gazeTarget != null)
								{
									this.gazeTarget.SendMessage("OnGazeExit", 1);
								}
								this.gazeTargetTime = Time.get_time();
								this.gazeTimeElapsed = 0f;
								this.lastGazeTargetTime = this.gazeTargetTime;
								component.get_gameObject().SendMessage("OnGazeEnter", 1);
								this.lastGazeTarget = this.gazeTarget;
								this.gazeTarget = component.get_gameObject();
							}
							else
							{
								component.get_gameObject().SendMessage("OnGazeHold", 1);
								this.gazeTimeElapsed = Time.get_time() - this.gazeTargetTime;
								if (component.gazeableDwellable && this.gazeTimeElapsed > this.dwellTimeThreshold)
								{
									if (!this.hasGazeDwelled)
									{
										component.get_gameObject().SendMessage("OnGazeDwell", 1);
										this.hasGazeDwelled = true;
									}
									else
									{
										component.get_gameObject().SendMessage("OnGazeDwellHold", 1);
									}
								}
							}
							this.isGazeOver = true;
							break;
						}
					}
				}
			}
			if (!this.isGazeOver)
			{
				if (this.gazeTarget != null && this.gazeTarget.GetComponent<MetaBody>().gazeable)
				{
					if (this.gazeTarget.GetComponent<MetaBody>().gazeableDwellable && this.hasGazeDwelled)
					{
						this.gazeTarget.SendMessage("OnGazeDwellExit", 1);
						this.hasGazeDwelled = false;
						return;
					}
					this.gazeTarget.SendMessage("OnGazeExit", 1);
					this.lastGazeTarget = this.gazeTarget;
					this.gazeTarget = null;
					this.hasGazeDwelled = false;
				}
				this.gazeTimeElapsed = 0f;
				this.gazeTargetTime = 0f;
				this.isGazeOver = false;
				this.lastGazeTarget = this.gazeTarget;
				this.gazeTarget = null;
			}
		}
	}
}
