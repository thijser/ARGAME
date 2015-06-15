using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class LevelManager : MonoBehaviour {
	public LevelLoader levelLoader= new LevelLoader();
	// Use this for initialization
	void Start () {
		levelLoader.loadLetters("C:/Users/thijs/Desktop/ARGAME/Levels/example.txt");
			levelLoader.constructLevel();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
