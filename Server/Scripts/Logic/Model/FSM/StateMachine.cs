public interface IState
{
    public void OnEnter();
    public void OnUpdate();
    public void OnExit();
}

public enum State
{
    None,Chase,Attack
}

public class StateMachine
{
    public Dictionary<State, IState> States = new();
    public State curStateType;
    public IState CurrentState;
    public Zombie Zombie;

    public StateMachine(Zombie zombie)
    {
        this.Zombie = zombie;
        States[State.Chase] = new Chase(Zombie);
        States[State.Attack] = new Attack(Zombie);
        SwitchState(State.Chase);
    }

    public void Update()
    {
        CurrentState.OnUpdate();
        int distance = 1000000;
        Player cplayer = null;
        foreach (var player in PlayerManager.players.Values)
        {
            if(player.roomId == -1) continue; //离线
            int curDis = (player.pos - Zombie.pos).Magnitude;
            if(curDis < distance)
            {
                distance = curDis;
                cplayer = player;
            }
        }

        if (distance < Zombie.attackDisSquared)
        {
            if (curStateType != State.Attack)
                SwitchState(State.Attack);
        }
        else if (curStateType != State.Chase)
        {
            SwitchState(State.Chase);
        }

        if (distance < Zombie.chaseDisSquared)
        {
            if (Zombie.ChasedPlayer == null || Zombie.ChasedPlayer.roomId==-1)
                Zombie.ChasedPlayer = cplayer;
        }
        else
        {
            Zombie.ChasedPlayer = null;
        }
    }

    public void SwitchState(State state)
    {
        Console.WriteLine("SwitchState: " + state.ToString());
        CurrentState?.OnExit();
        CurrentState = States[state];
        CurrentState.OnEnter();
        curStateType = state;
    }
}