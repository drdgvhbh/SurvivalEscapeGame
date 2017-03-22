using UnityEngine;
using System.IO;
using SimpleJSON;

public class ItemDatabase : MonoBehaviour {
    private string Path;
    private string JsonString;
    public static JSONNode JsonNode;

	void Awake () {
        Path = Application.streamingAssetsPath + "/ItemData.json";
        JsonString = File.ReadAllText(Path);
        JsonNode = JSON.Parse(JsonString);
	}
}