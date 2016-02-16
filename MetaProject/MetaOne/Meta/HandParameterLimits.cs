using System;

namespace Meta
{
	internal static class HandParameterLimits
	{
		public const short minDepthMinVal = 10;

		public const short minDepthMaxVal = 500;

		public const short maxDepthMinVal = 800;

		public const short maxDepthMaxVal = 2000;

		public const short minConfidenceMinVal = 50;

		public const short minConfidenceMaxVal = 250;

		public const float areaLimitMinVal = 500f;

		public const float areaLimitMaxVal = 3000f;

		public const float handVelocityMinVal = 0f;

		public const float handVelocityMaxVal = 20f;

		public const float grabThresholdMinVal = -10f;

		public const float grabThresholdMaxVal = 10f;

		public const float palmRayCastSpreadMinVal = 5f;

		public const float palmRayCastSpreadMaxVal = 45f;

		public const float palmObjectOfInterestMinConfidenceMinVal = 15f;

		public const float palmObjectOfInterestMinConfidenceMaxVal = 90f;

		public const float pointRayCastSpreadMinVal = 5f;

		public const float pointRayCastSpreadMaxVal = 45f;

		public const float pointerObjectOfInterestMinConfidenceMinVal = 15f;

		public const float pointerObjectOfInterestMinConfidenceMaxVal = 90f;

		public const float defaultRayCastSpreadMinVal = 5f;

		public const float defaultRayCastSpreadMaxVal = 45f;

		public const float defaultObjectOfInterestMinConfidenceMinVal = 15f;

		public const float defaultObjectOfInterestMinConfidenceMaxVal = 90f;

		public const int minSwipeFrameMinVal = 1;

		public const int minSwipeFrameMaxVal = 5;

		public const int maxSwipeFrameMinVal = 10;

		public const int maxSwipeFrameMaxVal = 30;
	}
}
