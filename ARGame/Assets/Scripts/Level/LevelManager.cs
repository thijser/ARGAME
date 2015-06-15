using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Level{
	public class LevelManager : MonoBehaviour {
		private LevelLoader levelLoader= new LevelLoader();
		private GameObject level;
		int currentLevelIndex=0;

		void Start() {
				loadLevel(0);
		}
		public void nextLevel(){
			loadLevel(currentLevelIndex);
		}
		public void restartLevel(){
			loadLevel(currentLevelIndex);	
		}
		public void restartGame(){
			loadLevel (0);
		}

		public void loadLevel(int index){
				Destroy(level);
				Debug.Log ("loading level"+index);
				level=levelLoader.CreateLevel("Assets/resources/Levels/"+index+".txt");
		}
	}
}