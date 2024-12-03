using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public Config Config;

    private string dataPath
    {
        get
        {
#if true //UNITY_EDITOR
            return "file://C:/Users/33572/Desktop/Minecraft_R/Src/Config/config.txt";
#else
            return "https://gitee.com/Utopia-lz/mc_src/raw/master/Config/config.txt";
#endif
        }
    }

    public IEnumerator Init()
    {
        Debug.Log("DataManager Init url: " + dataPath);
        UnityWebRequest request = WebTool.Create(dataPath);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Failed to load file:"  + request.error);
            yield break;
        }
        string json = request.downloadHandler.text;
        Config = JsonUtility.FromJson<Config>(json);
    }
}