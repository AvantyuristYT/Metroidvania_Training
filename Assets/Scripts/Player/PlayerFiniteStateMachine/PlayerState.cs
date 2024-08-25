using UnityEngine;

public class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected PlayerData playerData;

    protected float startTime; // Время жизни текущего Состояния

    private string animBoolName; // Название анимации для Аниматора

    public PlayerState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.playerData = playerData;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        DoChecks();
        player.AnimatorController.SetBool(animBoolName, true);
        startTime = Time.time;

        Debug.Log("Вошёл в " + animBoolName);
    }

    public virtual void Exit()
    {
        player.AnimatorController.SetBool(animBoolName, false);

        Debug.Log("Вышел из " + animBoolName);
    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    public virtual void DoChecks()
    {

    }
}
