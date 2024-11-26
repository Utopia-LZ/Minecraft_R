using System.Text;

public class Chase : IState
{
    public Zombie Zombie;
    private int frameCnt = 0;
    private List<Vector3Int> path = new List<Vector3Int>();
    private Random rand;
    private Vector3Int lastPos;
    private Vector3Int curPos;
    private bool beSmart = false;

    private Room room;

    public Chase(Zombie zombie)
    {
        rand = new Random();
        frameCnt = 0;
        Zombie = zombie;
        room = RoomManager.GetRoom(zombie.roomId);
    }
    public void OnEnter()
    {
        frameCnt = 0;
        lastPos = Zombie.pos;
        curPos = Zombie.pos;
    }
    public void OnUpdate()
    {
        if (frameCnt < Zombie.frameCount)
        {
            Zombie.pos += (curPos - lastPos) / Zombie.frameCount;
            frameCnt++;
        }
        else
        {
            if(path.Count > 0)
            {
                frameCnt = 0;
                lastPos = curPos;
                curPos = path[path.Count - 1];
                Console.WriteLine("Follow path: " + curPos);
                path.RemoveAt(path.Count - 1);
                Zombie.rot = curPos - lastPos;
                Zombie.rot.y = 0;
            }
            else if(Zombie.ChasedPlayer != null)
            {
                frameCnt = 0;
                Vector3Int delta = Zombie.ChasedPlayer.pos - Zombie.pos;
                Console.WriteLine("Delta: "+delta.ToString());
                int dir = -1;
                if(Math.Abs(delta.x) > Math.Abs(delta.z))
                {
                    if (delta.x > 0) dir = 3;
                    else if (delta.x < 0) dir = 2;
                }
                else
                {
                    if (delta.z > 0) dir = 1;
                    else if (delta.z < 0) dir = 0;
                }
                if (dir == -1 || lastPos == curPos)
                {
                    beSmart = true;
                    Console.WriteLine("Besmart");
                }
                else
                {
                    lastPos = curPos;
                    curPos = room.mapManager.GetEdge(Block.GetCornerPos(curPos), dir);
                    curPos = Block.GetStandPos(curPos);
                    Zombie.rot = curPos - lastPos;
                    Zombie.rot.y = 0;
                    Console.WriteLine("Dir: " + dir + " pos: " +  curPos.ToString());
                }
            }
            else
            {
                int res = rand.Next(0, 10);
                if (res < 2)
                {
                    frameCnt = 0;
                    lastPos = curPos;
                    curPos = room.mapManager.RandomGetEdge(Block.GetCornerPos(curPos));
                    curPos = Block.GetStandPos(curPos);
                    Console.WriteLine("Find a way:" + curPos.ToString());
                    Zombie.rot = curPos - lastPos;
                    Zombie.rot.y = 0;
                }
            }
        }

        //Update path
        if (!beSmart) return;
        beSmart = false;
        if (Zombie.ChasedPlayer == null) return;
        if (path.Count > 0) return; //temp
        //Console.WriteLine("Start: " + Block.GetCornerPos(Zombie.pos));
        //Console.WriteLine("End: " + Block.GetCornerPos(Zombie.ChasedPlayer.pos));
        path.Clear();
        path = FindPath.AStar(
            Block.GetCornerPos(Zombie.pos), 
            Block.GetCornerPos(Zombie.ChasedPlayer.pos),
            room);

        for(int i = 0; i < path.Count; i++)
        {
            path[i] = Block.GetStandPos(path[i]);
            //Console.Write(path[i].ToString()+" ");
        }
        Console.WriteLine(" Count: " + path.Count);
    }
    public void OnExit() { }
}