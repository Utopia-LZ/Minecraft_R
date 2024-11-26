using System.IO;

public class Patrol : IState
{
    public Zombie Zombie;

    private Random rand;
    private Vector3Int lastPos;
    private Vector3Int curPos;
    private int counter = 0;
    private Room room;

    public Patrol(Zombie zombie)
    {
        Zombie = zombie;
        rand = new Random();
        room = RoomManager.GetRoom(zombie.roomId);
    }

    public void OnEnter() 
    {
        counter = 0;
        lastPos = Zombie.pos;
        curPos = Zombie.pos;
    }
    public void OnUpdate()
    {
        if (counter < Zombie.frameCount)
        {
            Zombie.pos += (curPos - lastPos) / Zombie.frameCount;
            counter++;
        }
        else
        {
            int res = rand.Next(0,10);
            if(res < 3)
            {
                counter = 0;
                lastPos = curPos;
                curPos = room.mapManager.RandomGetEdge(Block.GetCornerPos(curPos));
                curPos = Block.GetStandPos(curPos);
                Zombie.rot = curPos - lastPos;
                Zombie.rot.y = 0;
            }
        }
    }
    public void OnExit()
    {

    }
}