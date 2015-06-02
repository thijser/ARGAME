using UnityEngine;
using System.Collections;
namespace Projection{
public interface MetaMarkerInterface  {
	 void RegisterMeta();
	 bool MoveTransformToMarker(int loc ,  Transform trans);
}

}