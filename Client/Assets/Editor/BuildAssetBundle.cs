using UnityEditor;
using System.IO;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// AssetBundle 打包工具
/// </summary>
public class BuildAssetBundle
{
    private static string mainABName
    {
        get
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return "/StandaloneWindows/";
#elif UNITY_IPHONE
            return "/IOS/";
#elif UNITY_ANDROID
            return "/Android/";
#endif
        }
    }
    private static string assetPath = Application.streamingAssetsPath + mainABName;

    /// <summary>
    /// 打包生成所有的AssetBundles（包）
    /// </summary>
    [MenuItem("Tools/BuildAllAssetBundles")]
    public static void BuildAllAB()
    {
        // 判断文件夹是否存在，不存在则新建
        if (Directory.Exists(assetPath) == false)
        {
            Directory.CreateDirectory(assetPath);
        }

        // 打包生成AB包 (目标平台根据需要设置即可)
        BuildPipeline.BuildAssetBundles(assetPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

        Caching.ClearCache();

        CreateUpdateTXT();
        AssetDatabase.Refresh();
    }

    // 创建更新文本
    private static void CreateUpdateTXT()
    {
        string[] files = Directory.GetFiles(assetPath);
        StringBuilder sb = new();
        //sb.Append("1.0\n"); //版本号
        foreach (string filePath in files)
        {
            if (filePath.EndsWith(".meta") || filePath.EndsWith(".txt")) continue;
            string name = Path.GetFileName(filePath);
            string md5 = BuildFileMd5(filePath);
            sb.Append(name + ":" + md5 + "\n");
        }
        string updatePath = Path.Combine(assetPath, "update.txt");
        WriteTXT(updatePath, sb.ToString());
    }

    private static string BuildFileMd5(string filePath)
    {
        string fileMd5 = string.Empty;
        try
        {
            using (FileStream fs = File.OpenRead(filePath))
            {
                MD5 md5 = MD5.Create();
                byte[] fileMd5Bytes = md5.ComputeHash(fs);  // 计算FileStream 对象的哈希值
                fileMd5 = System.BitConverter.ToString(fileMd5Bytes).Replace("-", "").ToLower();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }

        return fileMd5;
    }

    private static void WriteTXT(string path, string content)
    {
        string directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        using FileStream fs = File.Create(path);
        using StreamWriter sw = new StreamWriter(fs, Encoding.ASCII);
        try
        {
            sw.Write(content);
        }
        catch (IOException e)
        {
            Debug.Log(e.Message);
        }
    }
}