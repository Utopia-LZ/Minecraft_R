using UnityEngine.Networking;

public class WebTool
{
    public static UnityWebRequest Create(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
#if !UNITY_EDITOR
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        request.certificateHandler = new BypassCertificate();
#endif
        return request;
    }
}