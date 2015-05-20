using System;

namespace Network
{
    public class PositionUpdate 
    {
        public PositionUpdate(float x, float y, int id, long timestamp){
            this.X = x;
            this.Y = y;
            this.ID = id;
            this.TimeStamp = timestamp;
        }

        public float X { get; private set; }

        public float Y { get; private set; }

        public int ID { get; private set; }

        public long TimeStamp { get; private set; }

    	public override int GetHashCode()
    	{
    		return (int)(Math.Pow((float)TimeStamp,(float)ID)*Math.Pow(X,Y));
    	}

    	public override bool Equals(object o)
    	{
            if(o == null || o.GetType() != this.GetType())
            {
    			return false;
    		}
            PositionUpdate that = o as PositionUpdate;
            return this.X == that.X
                && this.Y == that.Y
                && this.ID == that.ID
                && this.TimeStamp == that.TimeStamp;
    	}

    	public override string ToString()
    	{
    		return ("[PositionUpdate: ID<]"+ID+">, X<"+X+">,"+"Y<"+Y+">,TimeStamp<"+TimeStamp+">");
    	}
    }
}