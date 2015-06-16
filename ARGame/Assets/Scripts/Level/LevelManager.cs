using UnityEngine;
using System.Collections;
using Vision;
using System.Collections.Generic;
using Network;

namespace Level{
	public class LevelManager : MonoBehaviour {
		private LevelLoader levelLoader= new LevelLoader();
		private GameObject level;
		int currentLevelIndex=0;
		Vector2 boardsize{get;set;}
		float IARscale{get;set;}
		void Start() {
			IARscale=1;
			restartGame();
		}

		public void nextLevel(){
			loadLevel(++currentLevelIndex);
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
				currentLevelIndex=index;
		}
		public void OnLevelUpdate(LevelUpdate levelup){
			boardsize=levelup.Size;
			if (currentLevelIndex!=levelup.NextLevelIndex){
				loadLevel(levelup.NextLevelIndex);
			}
		}

		public void scaleLevel(){
			Levelcomp levelcomp=level.GetComponent<Levelcomp>();

			float xproportions=boardsize.x/levelcomp.size.x;
			float yproportions=boardsize.y/levelcomp.size.y;
			if(xproportions<yproportions)
				level.transform.localScale=new Vector3(xproportions,xproportions,xproportions)*IARscale;
			else
				level.transform.localScale=new Vector3(yproportions,yproportions,yproportions)*IARscale;
		}	

	}
}