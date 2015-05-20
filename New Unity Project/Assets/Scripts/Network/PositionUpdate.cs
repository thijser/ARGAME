using UnityEngine;
using System.Collections;

public class PositionUpdate : MonoBehaviour {
	public float X{get;private set;}
	public float Y{get;private set;}
	public int ID{get; private set;}
	public int TimeStamp{get; private set;}

	public override int GetHashCode ()
	{
		return (int)(Mathf.Pow((float)TimeStamp,(float)ID)*Mathf.Pow(X,Y));
	}
	public override bool Equals (object o)
	{
		if(o.GetType()==this.GetType()){
		PositionUpdate pu = (PositionUpdate)o;
			if(this.X==pu.X&&this.Y==pu.Y&&this.ID==pu.ID&&this.TimeStamp==pu.TimeStamp){
				return true;
			}
		}
		return false;
	}

	public PositionUpdate(float x,float y , int id,int timestamp){
		X=x;
		Y=y;
		ID=id;
		TimeStamp=timestamp;
	}

	public override string ToString ()
	{
		return ("[PositionUpdate: ID<]"+ID+">, X<"+X+">,"+"Y<"+Y+">,TimeStamp<"+TimeStamp+">");
	}

}
