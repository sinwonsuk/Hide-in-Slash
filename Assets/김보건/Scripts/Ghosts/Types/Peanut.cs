using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Peanut : Ghost
{
    public override GhostState moveState { get; protected set; }

    protected override void Awake()
    {

        base.Awake();
        moveState = new PeanutMove(this, ghostStateMachine, "Move");
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        //�÷��̾��� �� ������ ������ ���߰�

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
