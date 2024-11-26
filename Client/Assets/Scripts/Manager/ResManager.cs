using System.Collections.Generic;
using UnityEngine;

public interface PoolObject
{
    public void OnRecycle();
}

public enum ObjType
{
    None,
    Chunk,
    Steve,
    Zombie,
    Item,
    Dropped,
    Chest,
    Bomb,
    Light,
    Carrion,
    ChestPanel,
    IconGrass,
    IconChest,
    IconBomb,
    IconLight,
    IconCarrion,
    MatChest,
    MatBomb,
    MatLight,
    MatChunk,
    MatGrass,
    MatDirt,
    MatAim,
}

public class ResManager : Singleton<ResManager>
{
    private Dictionary<ObjType, string> map = new Dictionary<ObjType, string>();

    private Dictionary<ObjType, List<GameObject>> pool = new();
    private Dictionary<ObjType, GameObject> prefabs = new();

    public void Init()
    {
        map[ObjType.None] = "";
        map[ObjType.Chunk] = "prefab_chunk";
        map[ObjType.Steve] = "prefab_steve";
        map[ObjType.Zombie] = "prefab_zombie";
        map[ObjType.Item] = "prefab_item";
        map[ObjType.Dropped] = "prefab_droppeditem";
        map[ObjType.Chest] = "prefab_chest";
        map[ObjType.Bomb] = "prefab_bomb";
        map[ObjType.Light] = "prefab_light";
        map[ObjType.Carrion] = "prefab_carrion";
        map[ObjType.ChestPanel] = "prefab_chestpanel";
        map[ObjType.IconGrass] = "icon_grass";
        map[ObjType.IconChest] = "icon_chest";
        map[ObjType.IconBomb] = "icon_bomb";
        map[ObjType.IconLight] = "icon_light";
        map[ObjType.IconCarrion] = "icon_carrion";
        map[ObjType.MatChunk] = "mat_chunk";
        map[ObjType.MatChest] = "mat_chest";
        map[ObjType.MatBomb] = "mat_bomb";
        map[ObjType.MatLight] = "mat_light";
        map[ObjType.MatGrass] = "mat_grass";
        map[ObjType.MatDirt] = "mat_dirt";
        map[ObjType.MatAim] = "mat_aim";
    }

    public GameObject GetGameObject(ObjType type, Transform Root = null)
    {
        GameObject result;
        if (!prefabs.ContainsKey(type))
        {
            prefabs[type] = LoadResources<GameObject>(type);
            pool[type] = new List<GameObject>();
        }
        if (pool[type].Count > 0)
        {
            result = pool[type][0];
            result.SetActive(true);
            pool[type].Remove(result);
        }
        else
        {
            result = GameObject.Instantiate(prefabs[type], Root);
            result.name = map[type];
        }
        return result;
    }

    public void RecycleObj(GameObject obj,ObjType type)
    {
        obj.SetActive(false);
        pool[type].Add(obj);
    }

    public T LoadResources<T> (ObjType type) where T : Object
    {
        string resName = map[type];
        return ABManager.Instance.LoadResource<T>(resName.Split('_')[0], resName);
    }
}