using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Peanut : Ghost
{
    public override GhostState moveState { get; protected set; }
    private GhostState stunnedState;
    [SerializeField] private bool isStunned = false;

    protected override void Awake()
    {

        base.Awake();
        idleState = new PeanutIdle(this, ghostStateMachine, "Idle");
        moveState = new PeanutMove(this, ghostStateMachine, "Move");
        stunnedState = new PeanutStunned(this, ghostStateMachine, "sturnned");

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerSight") && !isStunned)
        {
            StartCoroutine(StunnedTime(5f));
        }
    }

    private IEnumerator StunnedTime(float time)
    {
        isStunned = true;
        ghostStateMachine.ChangeState(stunnedState);
        yield return new WaitForSeconds(time);
        isStunned = false;
        ghostStateMachine.ChangeState(idleState);
    }
}

