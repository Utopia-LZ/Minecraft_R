﻿
public class ZombieManager
{
    public Dictionary<string, Zombie> Zombies = new Dictionary<string, Zombie>();
    public int index = 0;

    public int generateCount = 60;
    public int counter = 0;
    public Random rand;
    public Room room;

    public ZombieManager(Room room)
    {
        this.room = room;
        rand = new Random();
    }

    public void Update()
    {
        if (room.Time >= Room.DAY_TIME)
        {
            counter++;
            if (counter > generateCount)
            {
                counter = 0;
                int rdm = rand.Next(0, 5);
                if (rdm == 1)
                {
                    RandmGenerate(room);
                }
            }
        }

        foreach(Zombie zombie in Zombies.Values)
        {
            zombie.Update(room);
        }
    }

    public Zombie GetZombie(string id)
    {
        if(Zombies.ContainsKey(id))
            return Zombies[id];
        return null;
    }

    public void Remove(string id)
    {
        if (Zombies.ContainsKey(id))
        {
            Zombies.Remove(id);
        }
    }

    public void RandmGenerate(Room room)
    {
        Random rand = new Random();
        Vector3Int pos = new Vector3Int();
        Vector3Int center = PlayerManager.GetCenterPosition(room.id);
        center = Block.GetCornerPos(center);
        int count = 0;
        while (count < 200)
        {
            count++;
            pos = Vector3Int.Random(center-Player.ignoreDistance,center+Player.ignoreDistance);
            Block block = room.mapManager.GetBlock(pos);
            if (block == null || !block.canStand) continue;
            if (LightManager.LightValue(pos) > 0) continue;
            Generate(room, Block.GetStandPos(pos));
            break;
        }
        Console.WriteLine("Tried Count: " + count + " Success Pos: " + pos.ToString());
    }

    public void Generate(Room room, Vector3Int pos)
    {
        Console.WriteLine("Generate Zombie");
        Zombie zombie = new(pos,room.id)
        {
            id = index.ToString(),
            hp = 10,
            Kind = Kind.Zombie,
        };
        index++;
        Zombies[zombie.id] = zombie;

        MsgUpdateZombie msg = new MsgUpdateZombie();
        msg.info = new()
        {
            hp = zombie.hp,
            id = zombie.id,
            Kind = Kind.Zombie,
            pos = pos,
        };
        room.Broadcast(msg);
    }

    public void BombExplode(Vector3Int pos, Room room)
    {
        foreach (var zombie in Zombies.Values)
        {
            if (zombie.roomId != room.id) continue;
            if ((zombie.pos - pos).Magnitude > Bomb.radius * Bomb.radius) continue;
            //:explode hurts
        }
    }

    public void LoadZombies(Player player)
    {
        List<CharacterInfo> list = new List<CharacterInfo>();
        foreach(var zombie in Zombies.Values)
        {
            if(zombie.roomId != player.roomId) continue;
            CharacterInfo info = new CharacterInfo();
            info.id = zombie.id;
            info.hp = zombie.hp;
            info.Kind = zombie.Kind;
            info.pos = zombie.pos;
            info.rot = zombie.rot;
            list.Add(info);
        }
        if (list.Count == 0) return;
        MsgLoadZombie msg = new();
        msg.zombies = new CharacterInfo[list.Count];
        int i = 0;
        foreach(var zombie in list)
        {
            msg.zombies[i++] = zombie;
        }
        player.Send(msg);
    }
}