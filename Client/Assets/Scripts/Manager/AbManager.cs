using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// AB�������� ȫ��Ψһ ʹ�õ���ģʽ
/// </summary>
public class ABManager : MonoSingleton<ABManager>
{
    public struct FileMD5
    {
        public string name;
        public string md5;
    }

    //AB������---���AB���޷��ظ����ص����� Ҳ���������Ч�ʡ�
    private Dictionary<string, AssetBundle> abCache = new();
    private AssetBundle mainAB = null; //����
    private AssetBundleManifest mainManifest = null; //�����������ļ�---���Ի�ȡ������
    private Dictionary<string, string> clientDic = new();

    public GameObject waitPanel;

    private string serverUrl
    {
        get
        {
#if UNITY_EDITOR
            return DataManager.Instance.Config.LocalSrcUrl;
#else
            return DataManager.Instance.Config.ServerSrcUrl;
#endif
        }
    }

    //����ƽ̨�µĻ���·�� --- ���ú��жϵ�ǰƽ̨�µ�streamingAssets·��
    private string basePath
    {
        get
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return Application.streamingAssetsPath + '/' + mainABName + '/';
#elif UNITY_IPHONE
            return Application.dataPath + "/Raw/";
#elif UNITY_ANDROID
            return Application.dataPath + "!/assets/";
#endif
        }
    }

    //persistentDataPath �־û�·��
    private string persistentPath
    {
        get
        {
#if UNITY_EDITOR
            return Application.dataPath + "/persistentDataPath/";
#else
            return Application.persistentDataPath + '/';
#endif
        }
    }
        

    //����ƽ̨�µ��������� --- ���Լ���������ȡ������Ϣ
    private string mainABName
    {
        get
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return "StandaloneWindows";
#elif UNITY_IPHONE
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#endif
        }
    }

    public IEnumerator Init()
    {
        CopyDirectory(basePath, persistentPath);
        yield return CheckAndUpdateResources();
    }

    IEnumerator CheckAndUpdateResources()
    {
        using (FileStream fs = File.OpenRead(persistentPath + "update.txt"))
        {
            using (StreamReader sr = new StreamReader(fs))
            {
                string line;
                while (true)
                {
                    line = sr.ReadLine();
                    if (line == null || line == "") break;
                    string[] parts = line.Split(':');
                    clientDic[parts[0]] = parts[1];
                }               
            }
        }

        UnityWebRequest request = UnityWebRequest.Get(serverUrl + "update.txt");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load file: " + request.error);
            yield break;
        }
        string fileContents = request.downloadHandler.text;
        string[] lines = fileContents.Split('\n',System.StringSplitOptions.RemoveEmptyEntries);
        List<FileMD5> updateList = new List<FileMD5>();

        foreach (string line in lines)
        {
            string[] parts = line.Split(':');
            FileMD5 fm = new FileMD5();
            fm.name = parts[0];
            fm.md5 = parts[1];
            if (!clientDic.ContainsKey(parts[0]) || clientDic[parts[0]] != parts[1])
            {
                updateList.Add(fm);
            }
        }

        foreach (FileMD5 fm in updateList)
        {
            yield return UpdateResources(fm.name);
            clientDic[fm.name] = fm.md5;
        }

        waitPanel.SetActive(false);
    }

    IEnumerator UpdateResources(string name)
    {
        // ��һ�����ӷ����������µ� AssetBundle
        Debug.Log("Server url: " + serverUrl + name);
        UnityWebRequest request = UnityWebRequest.Get(serverUrl + name);
        yield return request.SendWebRequest();

        // ������������Ƿ�ɹ�
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error downloading AssetBundle: " + request.error);
            yield break;
        }

        // ���سɹ�����ȡ AssetBundle ����
        byte[] data = request.downloadHandler.data;
        string localPath = persistentPath + name;
        try
        {
            // ����ļ��Ѵ��ڣ���ɾ���ɵ� AssetBundle
            if (File.Exists(localPath))
            {
                File.Delete(localPath);
            }

            // �����ص����ݱ��浽����
            File.WriteAllBytes(localPath, data);
            Debug.Log("AssetBundle saved to: " + localPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save AssetBundle to local storage: " + e.Message);
        }
    }

    //�����ļ�
    private void CopyDirectory(string srcPath, string desPath)
    {
        // ���Ŀ��Ŀ¼�����ڣ��򴴽���
        if (!Directory.Exists(desPath))
        {
            Directory.CreateDirectory(desPath);
        }

        // ��ȡԴĿ¼�е������ļ�
        string[] files = Directory.GetFiles(srcPath);
        foreach (string file in files)
        {
            // ��ȡ�ļ���������Ŀ��·��
            if (file.EndsWith(".meta")) continue;
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(desPath, fileName);

            // �����ļ���Ŀ��Ŀ¼
            File.Copy(file, destFile, true);  // ����true��ʾ����ļ��Ѵ����򸲸�
        }
    }

    //����AB��
    private AssetBundle LoadABPackage(string abName)
    {
        AssetBundle ab;
        //����ab������һ����������������
        if (mainAB == null)
        {
            //���ݸ���ƽ̨�µĻ���·������������������
            mainAB = AssetBundle.LoadFromFile(persistentPath + mainABName);
            //��ȡ�����µ�AssetBundleManifest��Դ�ļ�������������Ϣ��
            mainManifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        //����manifest��ȡ���������������� �̶�API
        string[] dependencies = mainManifest.GetAllDependencies(abName);
        //ѭ����������������
        for (int i = 0; i < dependencies.Length; i++)
        {
            //������ڻ��������
            if (!abCache.ContainsKey(dependencies[i]))
            {
                //�������������ƽ��м���
                ab = AssetBundle.LoadFromFile(persistentPath + dependencies[i]);
                //ע����ӽ����� ��ֹ�ظ�����AB��
                abCache.Add(dependencies[i], ab);
            }
        }
        //����Ŀ��� -- ͬ��ע�⻺������
        if (abCache.ContainsKey(abName)) return abCache[abName];
        else
        {
            ab = AssetBundle.LoadFromFile(persistentPath + abName);
            abCache.Add(abName, ab);
            return ab;
        }


    }


    //==================������Դͬ�����ط�ʽ==================
    //�ṩ���ֵ��÷�ʽ �����������Եĵ��ã�Lua�Է���֧�ֲ��ã�
    #region ͬ�����ص���������

    /// <summary>
    /// ͬ��������Դ---���ͼ��� ��ֱ�� ������ʾת��
    /// </summary>
    /// <param name="abName">ab��������</param>
    /// <param name="resName">��Դ����</param>
    public T LoadResource<T>(string abName, string resName) where T : Object
    {
        //����Ŀ���
        AssetBundle ab = LoadABPackage(abName);

        //������Դ
        return ab.LoadAsset<T>(resName);
    }


    //��ָ������ ����������²�����ʹ�� ʹ��ʱ����ʾת������
    public Object LoadResource(string abName, string resName)
    {
        //����Ŀ���
        AssetBundle ab = LoadABPackage(abName);

        //������Դ
        return ab.LoadAsset(resName);
    }


    //���ò����������ͣ��ʺ϶Է��Ͳ�֧�ֵ����Ե��ã�ʹ��ʱ��ǿת����
    public Object LoadResource(string abName, string resName, System.Type type)
    {
        //����Ŀ���
        AssetBundle ab = LoadABPackage(abName);

        //������Դ
        return ab.LoadAsset(resName, type);
    }

    #endregion


    //================������Դ�첽���ط�ʽ======================

    /// <summary>
    /// �ṩ�첽����----ע�� �������AB����ͬ�����أ�ֻ�Ǽ�����Դ���첽
    /// </summary>
    /// <param name="abName">ab������</param>
    /// <param name="resName">��Դ����</param>
    public void LoadResourceAsync(string abName, string resName, System.Action<Object> finishLoadObjectHandler)
    {
        AssetBundle ab = LoadABPackage(abName);
        //����Э�� �ṩ��Դ���سɹ����ί��
        StartCoroutine(LoadRes(ab, resName, finishLoadObjectHandler));
    }


    private IEnumerator LoadRes(AssetBundle ab, string resName, System.Action<Object> finishLoadObjectHandler)
    {
        if (ab == null) yield break;
        //�첽������ԴAPI
        AssetBundleRequest abr = ab.LoadAssetAsync(resName);
        yield return abr;
        //ί�е��ô����߼�
        finishLoadObjectHandler(abr.asset);
    }


    //����Type�첽������Դ
    public void LoadResourceAsync(string abName, string resName, System.Type type, System.Action<Object> finishLoadObjectHandler)
    {
        AssetBundle ab = LoadABPackage(abName);
        StartCoroutine(LoadRes(ab, resName, type, finishLoadObjectHandler));
    }


    private IEnumerator LoadRes(AssetBundle ab, string resName, System.Type type, System.Action<Object> finishLoadObjectHandler)
    {
        if (ab == null) yield break;
        AssetBundleRequest abr = ab.LoadAssetAsync(resName, type);
        yield return abr;
        //ί�е��ô����߼�
        finishLoadObjectHandler(abr.asset);
    }


    //���ͼ���
    public void LoadResourceAsync<T>(string abName, string resName, System.Action<Object> finishLoadObjectHandler) where T : Object
    {
        AssetBundle ab = LoadABPackage(abName);
        StartCoroutine(LoadRes<T>(ab, resName, finishLoadObjectHandler));
    }

    private IEnumerator LoadRes<T>(AssetBundle ab, string resName, System.Action<Object> finishLoadObjectHandler) where T : Object
    {
        if (ab == null) yield break;
        AssetBundleRequest abr = ab.LoadAssetAsync<T>(resName);
        yield return abr;
        //ί�е��ô����߼�
        finishLoadObjectHandler(abr.asset as T);
    }


    //====================AB��������ж�ط�ʽ=================
    //������ж��
    public void UnLoad(string abName)
    {
        if (abCache.ContainsKey(abName))
        {
            abCache[abName].Unload(false);
            //ע�⻺����һ���Ƴ�
            abCache.Remove(abName);
        }
    }

    //���а�ж��
    public void UnLoadAll()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        //ע����ջ���
        abCache.Clear();
        mainAB = null;
        mainManifest = null;
    }
}
