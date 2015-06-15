using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StreamReader = System.IO.StreamReader;

public class LevelLoader{
	char[ , ] letters;
	Vector2 size; 
	public Dictionary<char,Object> objectMapper;
	GameObject level;
	public void loadPrefabs(){
		objectMapper = new Dictionary<char,Object>();
		objectMapper.Add('w',Resources.Load("wall"));
		objectMapper.Add('t',Resources.Load("Laser Target"));
		objectMapper.Add('e',Resources.Load("Emitter"));

	}

	public void LoadLettersdif(string path){
		StreamReader reader = new StreamReader(path);
		int width= int.Parse(reader.ReadLine());
		int height=int.Parse (reader.ReadLine());
		string strlevel = reader.ReadToEnd();
		letters = new char[height , 2*width];

		int x=0;
		int y=0;
		for(int i=0;i<strlevel;i++){
			if(!char.IsWhiteSpace(strlevel[i])){
				x++;
				if(x>=xsize){
					x=0;
					y++;
				}
				char type = strlevel[i];
				char dir = strlevel[++i];

			}
		}
	}
	public void loadLetters(string path){
		StreamReader reader = new StreamReader(path);
		int xsize= int.Parse(reader.ReadLine());
		int ysize=int.Parse (reader.ReadLine());
		size=new Vector2(xsize,ysize);
		letters = new char[ysize , 2*xsize];
		for(int y=0;y<size.y;y++){
			for(int x=0;x<size.x;x++){

				letters[y,x]=(char)reader.Read();
				Debug.Log ("type="+letters[y,x]);
				letters[y,x+1]=(char)reader.Read();
				Debug.Log ("dir="+letters[y,x+1]);
				Debug.Log ("skipping , space"+(char)reader.Read());

			}
			Debug.Log ("skipping , n;"+(char)reader.Read());
			Debug.Log ("skipping , n;"+(char)reader.Read());
		}
	}
	public void constructLevel(){
		if((objectMapper)==null){
			loadPrefabs();
		}
		level = new GameObject("level");
		for(int y=0;y<size.y;y++){
			for(int x=0;x<size.x;x++){
				char type = letters[y,x];
				int dirrection = '0'-letters[y,x+2];
				if (type!='.'){
					Debug.Log ("placing " + type + " at" +dirrection );
					GameObject newObject=GameObject.Instantiate((GameObject)objectMapper[type]);
					newObject.transform.position= new Vector3(x,0,y);
					newObject.transform.rotation=Quaternion.Euler(0,45*dirrection,0);
					newObject.transform.SetParent(level.transform);
				}
			}
		}
	}

}
