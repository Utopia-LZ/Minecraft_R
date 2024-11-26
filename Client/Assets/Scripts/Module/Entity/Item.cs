using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
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
        string resName = "icon_" + type.ToString().ToLower();
        Icon.sprite = ResManager.LoadResources<Sprite>(resName);
        count += delta;
        this.type = type;
        Count.text = count.ToString();
        if (count <= 0)
        {
            Destroy(gameObject);
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
}