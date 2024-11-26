public class Attack : IState
{
    public Zombie Zombie;
    private int Interval = 40;
    private int counter = 0;
    private Room room;

    public Attack(Zombie zombie)
    {
        Zombie = zombie;
        room = RoomManager.GetRoom(Zombie.roomId);
    }
    public void OnEnter()
    {

    }
    public void OnUpdate()
    {
        if (Zombie.ChasedPlayer == null) return;
        counter++;
        if(counter >= Interval)
        {
            counter = 0;
            MsgZombieAttack msg = new MsgZombieAttack();
            msg.playerId = Zombie.ChasedPlayer.id;
            msg.zombieId = Zombie.id;
            room.Broadcast(msg);
        }
    }
    public void OnExit()
    {

    }
}