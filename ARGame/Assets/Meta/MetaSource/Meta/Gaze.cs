

namespace Meta
{
    using UnityEngine;

    public class Gaze : MetaSingleton<Gaze>
	{
		private int m_numRows = 5;

		private int m_raysPerRow = 5;

		private float m_angle = 5f;

		public LayerMask m_layerMask;

		public bool m_showRays;

		private bool m_descend;

		public GameObject ObjectOfInterest;

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
			RaycastHit[] array = MultiRaycast.MultiRayCast(base.transform.position, base.transform.forward, this.m_numRows, this.m_raysPerRow, this.m_angle, this.m_layerMask, this.m_descend);
			GameObject gameObject = (!this.m_useWeights) ? MultiRaycast.MostHit(array) : MultiRaycast.MostHitWithWeights(array, this.m_rowWeights);
			this.ObjectOfInterest = gameObject;
			if (this.m_showRays)
			{
				RaycastHit[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					RaycastHit raycastHit = array2[i];
					Debug.DrawLine(base.transform.position, raycastHit.point);
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
			Ray ray = UnityEngine.Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
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
					if (raycastHit.collider != null && raycastHit.collider.gameObject.GetComponent<MetaBody>() != null)
					{
						MetaBody component = raycastHit.collider.gameObject.GetComponent<MetaBody>();
						if (this.gazeTarget != null && component.gameObject != this.gazeTarget && this.gazeTarget.GetComponent<MetaBody>().gazeableDwellable && this.hasGazeDwelled)
						{
							this.gazeTarget.gameObject.SendMessage("OnGazeDwellExit", SendMessageOptions.DontRequireReceiver);
							this.hasGazeDwelled = false;
							return;
						}
						if (component.gazeable)
						{
							this.isGazeOver = true;
							if (component.gameObject != this.gazeTarget)
							{
								if (this.gazeTarget != null)
								{
									this.gazeTarget.SendMessage("OnGazeExit", SendMessageOptions.DontRequireReceiver);
								}
								this.gazeTargetTime = Time.time;
								this.gazeTimeElapsed = 0f;
								this.lastGazeTargetTime = this.gazeTargetTime;
								component.gameObject.SendMessage("OnGazeEnter", SendMessageOptions.DontRequireReceiver);
								this.lastGazeTarget = this.gazeTarget;
								this.gazeTarget = component.gameObject;
							}
							else
							{
								component.gameObject.SendMessage("OnGazeHold", SendMessageOptions.DontRequireReceiver);
								this.gazeTimeElapsed = Time.time - this.gazeTargetTime;
								if (component.gazeableDwellable && this.gazeTimeElapsed > this.dwellTimeThreshold)
								{
									if (!this.hasGazeDwelled)
									{
										component.gameObject.SendMessage("OnGazeDwell", SendMessageOptions.DontRequireReceiver);
										this.hasGazeDwelled = true;
									}
									else
									{
										component.gameObject.SendMessage("OnGazeDwellHold", SendMessageOptions.DontRequireReceiver);
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
						this.gazeTarget.SendMessage("OnGazeDwellExit", SendMessageOptions.DontRequireReceiver);
						this.hasGazeDwelled = false;
						return;
					}
					this.gazeTarget.SendMessage("OnGazeExit", SendMessageOptions.DontRequireReceiver);
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
