public class Attack : IState
{
    public Zombie Zombie;
    private static int attackInterval = 40;
    private int counter = 0;
    private Room room;

    public Attack(Zombie zombie)
    {
        Zombie = zombie;
        room = RoomManager.GetRoom(Zombie.roomId);
        attackInterval = DataManager.Config.ZombieAttackInterval;
    }
    public void OnEnter()
    {

    }
    public void OnUpdate()
    {
        if (Zombie.ChasedPlayer == null) return;
        counter++;
        if(counter >= attackInterval)
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