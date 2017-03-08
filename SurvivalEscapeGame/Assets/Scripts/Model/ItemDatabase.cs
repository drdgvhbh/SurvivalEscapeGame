using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ItemDatabase : MonoBehaviour {
    private static string path = Application.streamingAssetsPath + "/data.json";
    private string jsonString;


	// Use this for initialization
	void Start () {
        jsonString = File.ReadAllText(path);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
