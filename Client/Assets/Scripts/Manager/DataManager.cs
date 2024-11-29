using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

[System.Serializable]
public class Config
{
    public int ChunkWidth; //������
    public int ChunkHeight; //����߶�
    public int ChunkBaseHeight; //����߶�
    public float Frequency; //����Ƶ��
    public float Amplitude; //�������
    public int TickTime; //ʱ��(ms)
    public int CycleTime; //һ��ʱ��(ʱ��)
    public int ZombieHp; //��ʬѪ��
    public int ZombieDamage; //��ʬ������
    public int ZombieAttackDis; //��ʬ��������
    public int ZombieChaseDis; //��ʬ׷������
    public int ZombieRefreshMinDis; //��ʬˢ���������
    public int ZombieRefreshMaxDis; //��ʬˢ����Զ����
    public int GenerateInterval; //��ʬ����ˢ�¼��(tick)
    public int ZombieAttackInterval; //��ʬ�������(tick)
    public int FrameCount; //��ʬ�ƶ���λ���뻮��֡��
    public int RecoverHunger; //����ָ�����ֵ
    public int RecoverSaturation; //����ָ���ʳ��
    public int PlayerHp; //���Ѫ��
    public float WalkSpeed; //��������ٶ�
    public float RunSpeed; //����ܲ��ٶ�
    public float RotateSpeed; //���ת���ٶ�
    public int PlayerDamage; //��ҹ�����
    public float JumpForce; //�����Ծ����
    public int Saturation; //��ұ�ʳ��
    public int HungerInterval; //��Ҽ������
    public int Hunger; //��Ҽ���ֵ
    public int StarveDamage; //��ҿո���Ѫֵ
    public float ThirdSpeed; //�����˳��ӽ�ת���ٶ�
    public float FollowSpeed; //��������ٶ�
    public float ZoomSpeed; //������Ұ�ٶ�
    public float CameraRadius; //Ĭ���������
    public float CameraMinRadius; //����������
    public float CameraMaxRadius; //��Զ�������
    public float ActInterval; //����ʱ����(s)
    public int ActDistance; //��Զ��������
    public float SyncInterval; //λ��ͬ��Ƶ��(s)
    public int SlotMaxItemCount; //��Ʒ���ѵ���
    public int SlotCount; //����������(����Ʒ��)
    public int ChestSlotCount; //���Ӹ�����
    public float LockTime; //��������ȴʱ��(s)
    public int LightRadius; //�Ʒ���뾶
    public int BombBurnTime; //TNT����ʱ��
    public int BombDamage; //TNT��ը�˺�(����)
    public int BombRadius; //TNT��ը�뾶
    public int BombFalloff; //TNT�˺�˥��
    public string LocalSrcUrl; //������Դ��ַ
    public string ServerSrcUrl; //��������Դ��ַ
}

public class DataManager : Singleton<DataManager>
{
    public Config Config;

    private string dataPath
    {
        get
        {
#if UNITY_EDITOR
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