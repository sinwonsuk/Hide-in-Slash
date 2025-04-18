using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Peanut : Ghost
{
    public override GhostState moveState { get; protected set; }

    protected override void Awake()
    {

        base.Awake();
        idleState = new PeanutIdle(this, ghostStateMachine, "Idle");
        moveState = new PeanutMove(this, ghostStateMachine, "Move");

        ghostStateMachine.Initialize(idleState);
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        //플레이어의 빛 구간에 들어오면 멈추게

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
