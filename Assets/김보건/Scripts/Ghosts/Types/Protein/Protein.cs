using Unity.VisualScripting;
using UnityEngine;

public class Protein : Ghost
{
    public override GhostState moveState { get; protected set; }

    [Header("단백질섭취(벌크업)")]
    [SerializeField] private float ProteinDuration = 10f; //벌크업 지속시간
    [SerializeField] private float ProteincooldownDuration = 5f;    //쿨타임
    [SerializeField] private float buffedSpeed = 2f;
    private bool isProtein = false; //단백질 섭취 여부
    private bool isProteinCooldown = false; //단백질 쿨타임 여부
    [SerializeField] private float proteinTimer; //지속시간 타이머
    [SerializeField] private float proteinCooldownTimer; //쿨타임 타이머

    private float originalSpeed;
    private Vector3 originalScale;


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
        originalSpeed = moveSpeed;
        originalScale = transform.localScale;
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Alpha1) && !isProtein && !isProteinCooldown)
        {
            activateProtein();
        }

        if (isProtein)
        {
            proteinTimer -= Time.deltaTime;
            if (proteinTimer <= 0)
            {
                EndProtein();
            }
        }

        if(isProteinCooldown)
        {
            proteinCooldownTimer -= Time.deltaTime;
            if (proteinCooldownTimer <= 0)
            {
                isProteinCooldown = false;
                proteinCooldownTimer = ProteincooldownDuration;
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private void activateProtein()
    {
        isProtein = true;
        proteinTimer = ProteinDuration;
        moveSpeed = buffedSpeed;
        transform.localScale = originalScale * 1.5f;

        Debug.Log("단백질 섭취");
    }

    private void EndProtein()
    {
        isProtein = false;
        isProteinCooldown = true;
        proteinCooldownTimer = ProteincooldownDuration;

        moveSpeed = originalSpeed;
        transform.localScale = originalScale;
        Debug.Log("단백질 섭취 종료");
    }
}
