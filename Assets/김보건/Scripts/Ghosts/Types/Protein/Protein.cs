using Unity.VisualScripting;
using UnityEngine;

public class Protein : Ghost
{
    public override GhostState moveState { get; protected set; }

    [Header("�ܹ�������(��ũ��)")]
    [SerializeField] private float ProteinDuration = 10f; //��ũ�� ���ӽð�
    [SerializeField] private float ProteincooldownDuration = 5f;    //��Ÿ��
    [SerializeField] private float buffedSpeed = 2f;
    private bool isProtein = false; //�ܹ��� ���� ����
    private bool isProteinCooldown = false; //�ܹ��� ��Ÿ�� ����
    [SerializeField] private float proteinTimer; //���ӽð� Ÿ�̸�
    [SerializeField] private float proteinCooldownTimer; //��Ÿ�� Ÿ�̸�

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

        Debug.Log("�ܹ��� ����");
    }

    private void EndProtein()
    {
        isProtein = false;
        isProteinCooldown = true;
        proteinCooldownTimer = ProteincooldownDuration;

        moveSpeed = originalSpeed;
        transform.localScale = originalScale;
        Debug.Log("�ܹ��� ���� ����");
    }
}
