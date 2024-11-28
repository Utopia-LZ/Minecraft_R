using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// AB包管理器 全局唯一 使用单例模式
/// </summary>
public class ABManager : MonoSingleton<ABManager>
{
    public struct FileMD5
    {
        public string name;
        public string md5;
    }

    //AB包缓存---解决AB包无法重复加载的问题 也有利于提高效率。
    private Dictionary<string, AssetBundle> abCache = new();
    private AssetBundle mainAB = null; //主包
    private AssetBundleManifest mainManifest = null; //主包中配置文件---用以获取依赖包
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

    //各个平台下的基础路径 --- 利用宏判断当前平台下的streamingAssets路径
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

    //persistentDataPath 持久化路径
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
        

    //各个平台下的主包名称 --- 用以加载主包获取依赖信息
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
        // 第一步：从服务器下载新的 AssetBundle
        Debug.Log("Server url: " + serverUrl + name);
        UnityWebRequest request = UnityWebRequest.Get(serverUrl + name);
        yield return request.SendWebRequest();

        // 检查下载请求是否成功
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error downloading AssetBundle: " + request.error);
            yield break;
        }

        // 下载成功，获取 AssetBundle 数据
        byte[] data = request.downloadHandler.data;
        string localPath = persistentPath + name;
        try
        {
            // 如果文件已存在，先删除旧的 AssetBundle
            if (File.Exists(localPath))
            {
                File.Delete(localPath);
            }

            // 将下载的数据保存到本地
            File.WriteAllBytes(localPath, data);
            Debug.Log("AssetBundle saved to: " + localPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save AssetBundle to local storage: " + e.Message);
        }
    }

    //拷贝文件
    private void CopyDirectory(string srcPath, string desPath)
    {
        // 如果目标目录不存在，则创建它
        if (!Directory.Exists(desPath))
        {
            Directory.CreateDirectory(desPath);
        }

        // 获取源目录中的所有文件
        string[] files = Directory.GetFiles(srcPath);
        foreach (string file in files)
        {
            // 获取文件名并构建目标路径
            if (file.EndsWith(".meta")) continue;
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(desPath, fileName);

            // 拷贝文件到目标目录
            File.Copy(file, destFile, true);  // 设置true表示如果文件已存在则覆盖
        }
    }

    //加载AB包
    private AssetBundle LoadABPackage(string abName)
    {
        AssetBundle ab;
        //加载ab包，需一并加载其依赖包。
        if (mainAB == null)
        {
            //根据各个平台下的基础路径和主包名加载主包
            mainAB = AssetBundle.LoadFromFile(persistentPath + mainABName);
            //获取主包下的AssetBundleManifest资源文件（存有依赖信息）
            mainManifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        //根据manifest获取所有依赖包的名称 固定API
        string[] dependencies = mainManifest.GetAllDependencies(abName);
        //循环加载所有依赖包
        for (int i = 0; i < dependencies.Length; i++)
        {
            //如果不在缓存则加入
            if (!abCache.ContainsKey(dependencies[i]))
            {
                //根据依赖包名称进行加载
                ab = AssetBundle.LoadFromFile(persistentPath + dependencies[i]);
                //注意添加进缓存 防止重复加载AB包
                abCache.Add(dependencies[i], ab);
            }
        }
        //加载目标包 -- 同理注意缓存问题
        if (abCache.ContainsKey(abName)) return abCache[abName];
        else
        {
            ab = AssetBundle.LoadFromFile(persistentPath + abName);
            abCache.Add(abName, ab);
            return ab;
        }


    }


    //==================三种资源同步加载方式==================
    //提供多种调用方式 便于其它语言的调用（Lua对泛型支持不好）
    #region 同步加载的三个重载

    /// <summary>
    /// 同步加载资源---泛型加载 简单直观 无需显示转换
    /// </summary>
    /// <param name="abName">ab包的名称</param>
    /// <param name="resName">资源名称</param>
    public T LoadResource<T>(string abName, string resName) where T : Object
    {
        //加载目标包
        AssetBundle ab = LoadABPackage(abName);

        //返回资源
        return ab.LoadAsset<T>(resName);
    }


    //不指定类型 有重名情况下不建议使用 使用时需显示转换类型
    public Object LoadResource(string abName, string resName)
    {
        //加载目标包
        AssetBundle ab = LoadABPackage(abName);

        //返回资源
        return ab.LoadAsset(resName);
    }


    //利用参数传递类型，适合对泛型不支持的语言调用，使用时需强转类型
    public Object LoadResource(string abName, string resName, System.Type type)
    {
        //加载目标包
        AssetBundle ab = LoadABPackage(abName);

        //返回资源
        return ab.LoadAsset(resName, type);
    }

    #endregion


    //================三种资源异步加载方式======================

    /// <summary>
    /// 提供异步加载----注意 这里加载AB包是同步加载，只是加载资源是异步
    /// </summary>
    /// <param name="abName">ab包名称</param>
    /// <param name="resName">资源名称</param>
    public void LoadResourceAsync(string abName, string resName, System.Action<Object> finishLoadObjectHandler)
    {
        AssetBundle ab = LoadABPackage(abName);
        //开启协程 提供资源加载成功后的委托
        StartCoroutine(LoadRes(ab, resName, finishLoadObjectHandler));
    }


    private IEnumerator LoadRes(AssetBundle ab, string resName, System.Action<Object> finishLoadObjectHandler)
    {
        if (ab == null) yield break;
        //异步加载资源API
        AssetBundleRequest abr = ab.LoadAssetAsync(resName);
        yield return abr;
        //委托调用处理逻辑
        finishLoadObjectHandler(abr.asset);
    }


    //根据Type异步加载资源
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
        //委托调用处理逻辑
        finishLoadObjectHandler(abr.asset);
    }


    //泛型加载
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
        //委托调用处理逻辑
        finishLoadObjectHandler(abr.asset as T);
    }


    //====================AB包的两种卸载方式=================
    //单个包卸载
    public void UnLoad(string abName)
    {
        if (abCache.ContainsKey(abName))
        {
            abCache[abName].Unload(false);
            //注意缓存需一并移除
            abCache.Remove(abName);
        }
    }

    //所有包卸载
    public void UnLoadAll()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        //注意清空缓存
        abCache.Clear();
        mainAB = null;
        mainManifest = null;
    }
}
