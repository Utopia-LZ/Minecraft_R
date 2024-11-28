using UnityEditor;
using System.IO;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// AssetBundle �������
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
    /// ����������е�AssetBundles������
    /// </summary>
    [MenuItem("Tools/BuildAllAssetBundles")]
    public static void BuildAllAB()
    {
        // �ж��ļ����Ƿ���ڣ����������½�
        if (Directory.Exists(assetPath) == false)
        {
            Directory.CreateDirectory(assetPath);
        }

        // �������AB�� (Ŀ��ƽ̨������Ҫ���ü���)
        BuildPipeline.BuildAssetBundles(assetPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

        Caching.ClearCache();

        CreateUpdateTXT();
        AssetDatabase.Refresh();
    }

    // ���������ı�
    private static void CreateUpdateTXT()
    {
        string[] files = Directory.GetFiles(assetPath);
        StringBuilder sb = new();
        //sb.Append("1.0\n"); //�汾��
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
                byte[] fileMd5Bytes = md5.ComputeHash(fs);  // ����FileStream ����Ĺ�ϣֵ
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