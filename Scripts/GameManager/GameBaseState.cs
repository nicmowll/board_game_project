using UnityEngine;

public abstract class GameBaseState
{
    public abstract void EnterState(GameStateManager sm);
    public abstract void UpdateState(GameStateManager sm);
    public abstract void ExitState(GameStateManager sm);
}
