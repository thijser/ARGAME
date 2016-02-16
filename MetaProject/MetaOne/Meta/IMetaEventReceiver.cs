using System;

namespace Meta
{
	internal interface IMetaEventReceiver
	{
		void MetaInit();

		void MetaUpdate();

		void MetaLateUpdate();

		void MetaOnDestroy();
	}
}
