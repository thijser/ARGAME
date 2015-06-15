using UnityEngine;
using System.Collections;
/// <summary>
/// only for use in the levelLoader
/// </summary>
namespace Level{
	public class InputEntry {
		 public char type{ get; set;}
		 public char dir{ get; set;}
		 public Vector2 pos{ get; set;}
		public int getAngle(){
			return 45*('0'-dir);
		}
	}
}
