﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StreamReader = System.IO.StreamReader;
using System.Text.RegularExpressions;
namespace Level{
	public class LevelLoader{

		Dictionary<char,GameObject> indexes;
		List<InputEntry> entries;
		GameObject level;
		
		public void loadObjects(){
			indexes=new Dictionary<char, GameObject>();
			indexes.Add('w',(GameObject)Resources.Load("prefabs/BOXWALL"));
			indexes.Add('e',(GameObject)Resources.Load("prefabs/Emitter"));
			indexes.Add('t',(GameObject)Resources.Load("prefabs/Laser Target"));
			indexes.Add('m',(GameObject)Resources.Load("prefabs/Mirror"));

		}

		public GameObject CreateLevel(string path){

			LoadLetters(path);
			if(indexes==null){
				loadObjects();
			}
			return ConstructLevel();
		}
		public void LoadLetters(string path){
			StreamReader reader = new StreamReader(path);
			int y=0;
			entries=new List<InputEntry>();
				while(!reader.EndOfStream){
					string line = reader.ReadLine();
					line=Regex.Replace(line, @"\s+", "");
					for(int x=0;x<line.Length;x++){
						InputEntry ie=new InputEntry();

						Debug.Log (ie.type=line[x]);
						x++;
					Debug.Log (ie.dir=line[x]);
						ie.pos=new Vector2(x/2,y);
						if(ie.type!='.')
							entries.Add(ie);
					}
				}
			}

			public GameObject constructEntry(InputEntry ie){
				GameObject pref=indexes[ie.type];
				GameObject go = GameObject.Instantiate(pref);
				go.transform.rotation=Quaternion.Euler(0f,(float)ie.getAngle(),0f);
			return go;
		}
			public GameObject ConstructLevel(){
				
			level = new GameObject("level");
				foreach(InputEntry ie in entries){
					GameObject go=constructEntry(ie);
				go.transform.SetParent(level.transform);
				go.transform.localPosition=new Vector3(ie.pos.x,0,ie.pos.y);
			}
			return level;
		}
	}
}