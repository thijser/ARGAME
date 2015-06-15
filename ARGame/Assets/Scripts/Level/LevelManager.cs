using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Level{
public class LevelManager : MonoBehaviour {
	public LevelLoader levelLoader= new LevelLoader();
	// Use this for initialization
	void Start () {
			levelLoader.CreateLevel("C:/Users/thijs/Desktop/ARGAME/Levels/example.txt");
	}

}

}