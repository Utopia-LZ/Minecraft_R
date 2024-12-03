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
    IconDirt,
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
    MatCarrion,

    MusicClick,
    MusicAttack,
    MusicHurt,
    MusicPut,
    MusicBroke,
    MusicEat,
    MusicMove,
    MusicChest,
    MusicBurn,
    MusicExplode,
    MusicTaunt,
    MusicZombieHurt,
    MusicZombieDeath,
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
        map[ObjType.IconDirt] = "icon_dirt";
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
        map[ObjType.MatCarrion] = "mat_carrion";
        map[ObjType.MusicClick] = "music_click";
        map[ObjType.MusicAttack] = "music_attack";
        map[ObjType.MusicHurt] = "music_hurt";
        map[ObjType.MusicPut] = "music_put";
        map[ObjType.MusicBroke] = "music_broke";
        map[ObjType.MusicEat] = "music_eat";
        map[ObjType.MusicMove] = "music_move";
        map[ObjType.MusicChest] = "music_chest";
        map[ObjType.MusicBurn] = "music_burn";
        map[ObjType.MusicExplode] = "music_explode";
        map[ObjType.MusicTaunt] = "music_taunt";
        map[ObjType.MusicZombieHurt] = "music_zombiehurt";
        map[ObjType.MusicZombieDeath] = "music_zombiedeath";
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
            result = pool[type][pool[type].Count-1];
            result.SetActive(true);
            pool[type].Remove(result);
        }
        else
        {
            result = GameObject.Instantiate(prefabs[type], Root);
            result.name = map[type];
        }

        switch (type)
        {
            case ObjType.Chunk: result.AddComponent<Chunk>(); break;
            case ObjType.Zombie: result.AddComponent<Zombie>(); break;
            case ObjType.Item: result.AddComponent<Item>(); break;
            case ObjType.Dropped: result.AddComponent<DroppedItem>(); break;
            case ObjType.Chest: result.AddComponent<Chest>(); break;
            case ObjType.Bomb: result.AddComponent<Bomb>(); break;
            case ObjType.Light: result.AddComponent<Light>(); break;
            case ObjType.ChestPanel: result.AddComponent<ChestPanel>(); break;
            default: break;
        }
        result.transform.parent = Root;
        if(Root != null) result.transform.position = Root.position;

        return result;
    }

    public void RecycleObj(GameObject obj,ObjType type, PoolObject pobj)
    {
        pobj.OnRecycle();
        obj.SetActive(false);
        pool[type].Add(obj);
    }

    public T LoadResources<T> (ObjType type) where T : Object
    {
        Debug.Log("LoadResources: " + type.ToString());
        string resName = map[type];
        return ABManager.Instance.LoadResource<T>(resName.Split('_')[0], resName);
    }
}