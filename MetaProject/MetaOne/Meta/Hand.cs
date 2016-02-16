using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta
{
	public class Hand
	{
		private bool _correctedCoordinateSystem;

		private bool _isValid;

		private List<Finger> _fingers = new List<Finger>();

		private Palm _palm;

		private Pointer _pointer;

		private Pointer _rightMostPoint;

		private Pointer _leftMostPoint;

		private float _angle;

		private Gesture _gesture;

		private float _handOpenness;

		private CppGestureData[] cachedGestures = new CppGestureData[10];

		private MetaGesture prevGesture = MetaGesture.NONE;

		public bool correctedCoordinateSystem
		{
			get
			{
				return this._correctedCoordinateSystem;
			}
			internal set
			{
				this._correctedCoordinateSystem = value;
			}
		}

		public bool isValid
		{
			get
			{
				return this._isValid;
			}
			internal set
			{
				this._isValid = value;
			}
		}

		internal List<Finger> fingers
		{
			get
			{
				return this._fingers;
			}
			set
			{
				this._fingers = value;
			}
		}

		public Palm palm
		{
			get
			{
				return this._palm;
			}
			internal set
			{
				this._palm = value;
			}
		}

		public Pointer pointer
		{
			get
			{
				return this._pointer;
			}
			internal set
			{
				this._pointer = value;
			}
		}

		internal Pointer rightMostPoint
		{
			get
			{
				return this._rightMostPoint;
			}
			set
			{
				this._rightMostPoint = value;
			}
		}

		internal Pointer leftMostPoint
		{
			get
			{
				return this._leftMostPoint;
			}
			set
			{
				this._leftMostPoint = value;
			}
		}

		internal float angle
		{
			get
			{
				return this._angle;
			}
			set
			{
				this._angle = value;
			}
		}

		public Gesture gesture
		{
			get
			{
				return this._gesture;
			}
			internal set
			{
				this._gesture = value;
			}
		}

		public float handOpenness
		{
			get
			{
				return this._handOpenness;
			}
			internal set
			{
				this._handOpenness = value;
			}
		}

		public Hand()
		{
			for (int i = 0; i < 5; i++)
			{
				this._fingers.Add(new Finger());
			}
			this._palm = new Palm();
			this._gesture = new Gesture();
			this._pointer = new Pointer();
			this._rightMostPoint = new Pointer();
			this._leftMostPoint = new Pointer();
		}

		internal void LocalToWorldCoordinate(Transform parentTransform)
		{
			if (!this._correctedCoordinateSystem)
			{
				this._palm.CorrectCoordinates(parentTransform);
				this._pointer.CorrectCoordinates(parentTransform);
				this._rightMostPoint.CorrectCoordinates(parentTransform);
				this._leftMostPoint.CorrectCoordinates(parentTransform);
				this._gesture.LocalToWorldCoordinate(parentTransform);
				for (int i = 0; i < 5; i++)
				{
					this._fingers[i].CorrectCoordinates(parentTransform);
				}
				this._correctedCoordinateSystem = true;
			}
		}

		private CppGestureData SmoothGesture(CppGestureData newGesture)
		{
			for (int i = 0; i < this.cachedGestures.Length - 1; i++)
			{
				this.cachedGestures[i] = this.cachedGestures[i + 1];
			}
			this.cachedGestures[this.cachedGestures.Length - 1] = newGesture;
			Dictionary<MetaGesture, int> dictionary = new Dictionary<MetaGesture, int>();
			for (int j = 0; j < this.cachedGestures.Length; j++)
			{
				if (dictionary.ContainsKey(this.cachedGestures[j].manipulationGesture))
				{
					Dictionary<MetaGesture, int> dictionary2;
					Dictionary<MetaGesture, int> expr_84 = dictionary2 = dictionary;
					MetaGesture manipulationGesture;
					MetaGesture expr_98 = manipulationGesture = this.cachedGestures[j].manipulationGesture;
					int num = dictionary2[manipulationGesture];
					expr_84[expr_98] = num + 1;
				}
				else
				{
					dictionary.Add(this.cachedGestures[j].manipulationGesture, 1);
				}
			}
			int num2 = 0;
			MetaGesture manipulationGesture2 = newGesture.manipulationGesture;
			foreach (KeyValuePair<MetaGesture, int> current in dictionary)
			{
				if (current.Value > num2)
				{
					num2 = current.Value;
					manipulationGesture2 = current.Key;
				}
			}
			if (num2 > 5)
			{
				newGesture.manipulationGesture = manipulationGesture2;
				this.prevGesture = manipulationGesture2;
			}
			else
			{
				newGesture.manipulationGesture = this.prevGesture;
			}
			return newGesture;
		}

		internal void Update(CppHandData cppHand)
		{
			this._correctedCoordinateSystem = false;
			this._isValid = cppHand.valid;
			this._angle = (float)cppHand.angle;
			this._handOpenness = cppHand.handOpenness;
			this._pointer.localPosition = MetaUtils.FloatToVector3(cppHand.top);
			this._pointer.isValid = cppHand.valid;
			this._rightMostPoint.localPosition = MetaUtils.FloatToVector3(cppHand.right);
			this._rightMostPoint.isValid = cppHand.valid;
			this._leftMostPoint.localPosition = MetaUtils.FloatToVector3(cppHand.left);
			this._leftMostPoint.isValid = cppHand.valid;
			this._palm.localPosition = MetaUtils.FloatToVector3(cppHand.center);
			this._palm.isValid = cppHand.valid;
			this._palm.radius = (float)cppHand.palm.radius;
			this._palm.normal = MetaUtils.FloatToVector3(cppHand.palm.normalVector);
			this._palm.orientations = MetaUtils.FloatToVector2(cppHand.palm.orientationAngles);
			this._palm.SetOrientation(this._angle);
			CppGestureData cppGesture = this.SmoothGesture(cppHand.gesture);
			if (this.gesture.type == cppGesture.manipulationGesture)
			{
				this._gesture.Update(cppGesture);
			}
			else
			{
				switch (cppGesture.manipulationGesture)
				{
				case MetaGesture.GRAB:
					this._gesture = new GrabGesture(cppGesture);
					goto IL_1BE;
				case MetaGesture.PINCH:
					this._gesture = new PinchGesture(cppGesture);
					goto IL_1BE;
				}
				this._gesture = new Gesture(cppGesture);
			}
			IL_1BE:
			for (int i = 0; i < cppHand.fingers.Length; i++)
			{
				this._fingers[i].localPosition = MetaUtils.FloatToVector3(cppHand.fingers[i].location);
				this._fingers[i].isValid = cppHand.fingers[i].found;
			}
		}
	}
}
