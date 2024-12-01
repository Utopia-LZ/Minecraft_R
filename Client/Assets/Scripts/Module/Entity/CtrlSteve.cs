using UnityEngine;
using UnityEngine.EventSystems;

public class CtrlSteve : BaseSteve, PoolObject
{
    public static float syncInterval; //0.05f; //同步帧率
    public static float actInterval; //攻击间隔
    public static float actDistance;
    public static float jumpForce;
    public static float gravityForce;
    public static float WalkSpeed;
    public static float RunSpeed;
    public static float RotateSpeed;

    private float lastSendSyncTime = 0; //上一次发送同步信息的时间
    private float actTimer = 0.1f;
    public Vector3 viewOffset = Vector3.up * 0.5f;
    public Vector3Int currentChunk;

    private int layerOwn;
    private int layerSteve;
    private int layerTerrain;
    private int layerChest;
    private int layerBomb;
    private int layerLight;
    private int layerZombie;
    private int layerCarrion;

    private void Update()
    {
        MoveUpdate();
        SyncUpdate();
        ActUpdate();

        if(Input.GetKeyDown(KeyCode.Q))
        {
            ItemInfo info = BagManager.Instance.RemoveItem();
            Vector3 pos = transform.position;
            Vector3 dir = Camera.main.transform.forward;
            MsgDropItem msgD = new MsgDropItem();
            msgD.info = info;
            msgD.pos = Vector3Int.V3ToV3Int(pos);
            msgD.dir = Vector3Int.V3ToV3Int(dir);
            msgD.locked = true;
            NetManager.Send(msgD);
        }
#region Test
        if (Input.GetKeyDown(KeyCode.O))
        {
            ItemInfo item = new();
            item.count = 1;
            item.type = BlockType.Grass;
            BagManager.Instance.AddItem(item);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ItemInfo item = new();
            item.count = 1;
            item.type = BlockType.Chest;
            BagManager.Instance.AddItem(item);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            ItemInfo item = new();
            item.count = 1;
            item.type = BlockType.Bomb;
            BagManager.Instance.AddItem(item);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            ItemInfo item = new();
            item.count = 1;
            item.type = BlockType.Carrion;
            BagManager.Instance.AddItem(item);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            ItemInfo item = new();
            item.count = 1;
            item.type = BlockType.Light;
            BagManager.Instance.AddItem(item);
        }

        #endregion
        rb.AddForce(-transform.up * gravityForce, ForceMode.Force);
    }

    public override void Init()
    {
        base.Init();
        currentChunk = GetChunkPos();
        layerOwn = LayerMask.NameToLayer("Own");
        layerSteve = LayerMask.NameToLayer("Steve");
        layerTerrain = LayerMask.NameToLayer("Terrain");
        layerChest = LayerMask.NameToLayer("Chest");
        layerBomb = LayerMask.NameToLayer("Bomb");
        layerLight = LayerMask.NameToLayer("Light");
        layerZombie = LayerMask.NameToLayer("Zombie");
        layerCarrion = LayerMask.NameToLayer("Carrion");
    }

    private void MoveUpdate()
    {
        if (IsDie()) return;

        //前进后退平移
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float speed = walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = runSpeed;
        }
        Vector3 s = Time.deltaTime * speed * (y * transform.forward + x * transform.right);
        transform.position += s;
        bool onGround = (MapManager.GetType(transform.position-Vector3.up*1.5f) != BlockType.None);
        if (onGround && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(transform.up*jumpForce,ForceMode.Impulse);
        }
    }

    public void SyncUpdate()
    {
        //时间间隔判断
        if (Time.time - lastSendSyncTime < syncInterval)
        {
            return;
        }
        lastSendSyncTime = Time.time;
        //发送同步协议
        MsgSyncSteve msg = new MsgSyncSteve();
        msg.pos = Vector3Int.V3ToV3Int(transform.position);
        msg.rot = Vector3Int.V3ToV3Int(transform.eulerAngles);
        NetManager.Send(msg);

        if(GetChunkPos() != currentChunk)
        {
            currentChunk = GetChunkPos();
            MapManager.IntantiateChunk(currentChunk);
        }
    }

    private void ActUpdate()
    {
        actTimer += Time.deltaTime;
        if (actTimer > 1e9) actTimer = actInterval;
        
        else if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.N))
        {
            if (actTimer < actInterval) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;

            actTimer = 0;
            Vector3 direction = Camera.main.transform.forward;
            Ray ray = new Ray(viewOffset+transform.position, direction);
            RaycastHit hit;
            // 如果射线碰撞到物体
            if (Physics.Raycast(ray, out hit, actDistance))
            {
                // 输出射线与物体碰撞的点
                int layer = hit.collider.gameObject.layer;
                if (layer == layerSteve)
                {
                    MsgHit msg = new MsgHit();
                    msg.id = hit.collider.GetComponent<SyncSteve>().id;
                    msg.damage = damage;
                    NetManager.Send(msg);
                }
                else if (layer == layerTerrain)
                {
                    ChangeBlock(direction, hit.point, BlockType.None);
                }
                else if(layer == layerChest || layer == layerBomb || layer == layerLight)
                {
                    BlockType type = BlockType.None;
                    if (layer == layerChest) type = BlockType.Chest;
                    else if(layer == layerBomb) type = BlockType.Bomb;
                    Vector3Int corner = CalChangeCorner(direction, hit.point, BlockType.None);
                    GameObject go = hit.collider.gameObject;
                    EntityManager.Instance.SendDestroy(corner, type, go);
                }
                else if(layer == layerZombie)
                {
                    MsgZombieHit msg = new MsgZombieHit();
                    msg.zombieId = hit.collider.GetComponent<Zombie>().id;
                    msg.damage = damage;
                    NetManager.Send(msg);
                }
            }
        }
        else if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.M))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector3 direction = Camera.main.transform.forward;
            Ray ray = new Ray(viewOffset + transform.position, direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, actDistance))
            {
                int layer = hit.collider.gameObject.layer;
                if (layer == layerTerrain)
                {
                    Item useItem = BagManager.Instance.SelectSlot;
                    if (useItem == null) return;

                    if(useItem.type == BlockType.Grass) //:可改成判断枚举区间
                    {
                        ChangeBlock(direction, hit.point, BlockType.Grass);
                        BagManager.Instance.RemoveItem(1);
                    }
                    else if(useItem.type == BlockType.Chest 
                         || useItem.type == BlockType.Bomb
                         || useItem.type == BlockType.Light)
                    {
                        Vector3Int corner = CalChangeCorner(direction, hit.point, useItem.type);
                        EntityManager.Instance.SendGenerate(corner, useItem.type);
                        BagManager.Instance.RemoveItem(1);
                    }
                    else if(useItem.type == BlockType.Carrion)
                    {
                        Vector3Int corner = CalChangeCorner(direction, hit.point, useItem.type);
                        MsgTest msg = new MsgTest();
                        msg.corner = corner - Vector3Int.Up;
                        NetManager.Send(msg);
                        BagManager.Instance.RemoveItem(1);
                    }
                }
                else if (layer == layerChest)
                {
                    Chest chest = hit.collider.gameObject.GetComponent<Chest>();
                    chest.OpenChest();
                }
                else if(layer == layerBomb)
                {
                    hit.collider.gameObject.GetComponent<Bomb>().SendIgnite();
                }
            }
            else
            {
                Item useItem = BagManager.Instance.SelectSlot;
                if(useItem != null && useItem.type == BlockType.Carrion)
                {
                    Debug.Log("Eat Carrion");
                    MsgHungry msg = new MsgHungry();
                    msg.id = id;
                    msg.hunger = Zombie.RecoverHunger;
                    msg.saturation = Zombie.RecoverSaturation;
                    NetManager.Send(msg);
                    BagManager.Instance.RemoveItem(1);
                    BattleManager.RefreshHunger(Zombie.RecoverHunger);
                }
            }
        }
    }

    private void ChangeBlock(Vector3 direction, Vector3 pos, BlockType type)
    {
        Vector3Int corner = CalChangeCorner(direction, pos,type);
        MsgMapChange msg = new MsgMapChange();
        msg.blockPos = (corner % Chunk.width + Chunk.width) % Chunk.width;
        msg.chunkPos = corner - msg.blockPos;
        if (!MapManager.chunks.ContainsKey(msg.chunkPos))
        {
            Debug.Log("未加载该区块！" + msg.chunkPos.ToString());
            return;
        }
        msg.chunkId = MapManager.chunks[msg.chunkPos].Id;
        msg.type = type;
        Debug.Log("ChangeBlock: " + msg.blockPos.ToString());
        NetManager.Send(msg);
    }

    private Vector3Int CalChangeCorner(Vector3 direction, Vector3 pos, BlockType type)
    {
        if (type == BlockType.None) direction = -direction;
        Vector3Int corner = new Vector3Int((int)Mathf.Floor(pos.x), (int)Mathf.Floor(pos.y), (int)Mathf.Floor(pos.z));
        if (Mathf.Abs(pos.x - Mathf.Floor(pos.x)) < 0.0001 && direction.x > 0) corner.x--;
        if (Mathf.Abs(pos.y - Mathf.Floor(pos.y)) < 0.0001 && direction.y > 0) corner.y--;
        if (Mathf.Abs(pos.z - Mathf.Floor(pos.z)) < 0.0001 && direction.z > 0) corner.z--;
        return corner;
    }

    public Vector3Int GetChunkPos()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Floor(pos.x / (float)Chunk.width) * Chunk.width;
        pos.z = Mathf.Floor(pos.z / (float)Chunk.width) * Chunk.width;
        return new Vector3Int((int)pos.x, 0, (int)pos.z);
    }

    public void OnRecycle()
    {
        Destroy(GetComponent<CameraFollow>());
        Destroy(this);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 from = viewOffset + transform.position;
        Vector3 to = from + Camera.main.transform.forward * actDistance;
        Gizmos.DrawLine(from, to);
    }
}