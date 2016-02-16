using System;

namespace Meta
{
	public class GrabGesture : Gesture
	{
		internal GrabGesture(CppGestureData cppGesture)
		{
			this._position = MetaUtils.FloatToVector3(cppGesture.gesturePoint);
			this._isValid = cppGesture.valid;
			this._type = cppGesture.manipulationGesture;
		}
	}
}
