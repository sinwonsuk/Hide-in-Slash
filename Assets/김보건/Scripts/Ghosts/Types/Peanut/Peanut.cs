using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Peanut : Ghost
{
    public override GhostState moveState { get; protected set; }
    private GhostState stunnedState;
    [SerializeField] private bool isStunned = false;

    private Vector2 lastDir = Vector2.right;   // �⺻���� ������

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

        //�÷��̾��� �� ������ ������ ���߰�

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void UpdateAnimParam(Vector2 input)
    {
        Debug.Log($"[Peanut] DirX: {input.x}, DirY: {input.y}");
        if (input != Vector2.zero)
            lastDir = input.normalized;

        bool isMoving = input != Vector2.zero;

        anim.SetBool("IsMoving", isMoving);
        anim.SetFloat("DirX", lastDir.x);
        anim.SetFloat("DirY", lastDir.y);

        if (Mathf.Abs(lastDir.x) >= Mathf.Abs(lastDir.y))  
        {
            sr.flipX = lastDir.x < 0;
        }

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

