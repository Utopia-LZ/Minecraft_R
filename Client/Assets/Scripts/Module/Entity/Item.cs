using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour, PoolObject
{
    public TMP_Text Count;
    public Image Icon;
    public Image HighLight;

    public int id = -1;
    public BlockType type = BlockType.None;
    public int count = 0;

    private void Start()
    {
        HighLight.enabled = false;
        GetComponent<Button>().onClick.AddListener(OnClickSelect);
        Icon = GetComponent<Image>();
    }

    public void Init(BlockType type)
    {
    }

    public void Refresh(BlockType type, int delta = 0)
    {
        ObjType objType = type switch
        {
            BlockType.Grass => ObjType.IconGrass,
            BlockType.Chest => ObjType.IconChest,
            BlockType.Bomb => ObjType.IconBomb,
            BlockType.Light => ObjType.IconLight,
            BlockType.Carrion => ObjType.IconCarrion,
            _ => ObjType.None
        };
        Icon.sprite = ResManager.Instance.LoadResources<Sprite>(objType);
        count += delta;
        Debug.Log("Count: " + count + " Delta: " + delta);
        this.type = type;
        Count.text = count.ToString();
        if (count <= 0)
        {
            //Destroy(gameObject);
            ResManager.Instance.RecycleObj(gameObject, ObjType.Item);
            Debug.Log("Recycle Item");
        }
    }
    
    private void OnClickSelect()
    {
        ItemBasePanel basePanel = transform.parent.GetComponentInParent<ItemBasePanel>();
        int idx = transform.parent.GetSiblingIndex();
        if(basePanel is BagPanel)
        {
            BagManager.Instance.Select(idx);
        }
        else if(basePanel is ChestPanel)
        {
            ChestManager.Instance.Select(idx); 
        }
    }

    public ItemInfo Info() //:temp
    {
        return new ItemInfo
        {
            count = count,
            type = type,
            id = id,
        };
    }

    public void OnRecycle()
    {
        count = 0;
    }
}