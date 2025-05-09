using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using Photon.Pun;
using Photon.Realtime;
using Unity.Cinemachine;
using ExitGames.Client.Photon;
using UnityEngine.Splines.ExtrusionShapes;
using System.Collections.Generic;
using System.Linq;

public enum EscapeType
{
    Dead = 0,         // 죽은상태
    ExitDoor = 1,     // 탈출구
    Hatch = 2         // 개구멍
}

public enum RunnerStatus
{
    Alive,
    Prison,
    Dead,
    Escaped
}

public class Player : MonoBehaviourPun, IPunObservable
{

    private void Awake()
    {
        if (photonView.IsMine)
            PhotonNetwork.LocalPlayer.TagObject = this;
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        // 콜리전 감지 강화
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        canvas = GameObject.FindGameObjectWithTag("Dark").GetComponent<Canvas>();
        canvas.enabled = false;
        originalSpeed = moveSpeed;

        PlayerStateMachine = new PlayerStateMachine();

        idleState = new PlayerIdle(this, PlayerStateMachine, "Idle");
        moveState = new PlayerMove(this, PlayerStateMachine, "Move");
        deadState = new PlayerDead(this, PlayerStateMachine, "Dead");
        escapeState = new PlayerEscapeState(this, PlayerStateMachine, "Escape");

        flashLight.pointLightOuterRadius = defaultRadius;
        defaultColliderPoints = lightCollider.points;

        // 인스펙터창에 무조건 있어야함 긴급 땜빵
        GameObject gameObject = GameObject.Find("SpawnPlayerProfile");
        profileSlotManager = gameObject.GetComponent<ProfileSlotManager>();


    }

    private void OnEnable()
    {
        EventManager.RegisterEvent(EventType.UseEnergyDrink, BecomeBoost);
        EventManager.RegisterEvent(EventType.UseInvisiblePotion, BecomeInvisible);
        EventManager.RegisterEvent(EventType.UseUpgradedLight, UseUpgradedLightHandler);
        EventManager.RegisterEvent(EventType.UsePrisonKey, usePrisonKeyItem);
        EventManager.RegisterEvent(EventType.UseHatch, HasHatchItem);
        EventManager.RegisterEvent(EventType.LightOn, TurnOnLight);
        EventManager.RegisterEvent(EventType.LightOff, TurnOffLight);
        EventManager.RegisterEvent(EventType.UseMap, HasTriggerMap);
        EventManager.RegisterEvent(EventType.InEventPlayer, SetZeroVelocity);
        EventManager.RegisterEvent(EventType.OutEventPlayer, ResumeMovement);
        EventManager.RegisterEvent(EventType.HasPrisonKey, HasPrisonKey);
        EventManager.RegisterEvent(EventType.EntireLightOn, TurnOnEntireLight);
        EventManager.RegisterEvent(EventType.EntireLightOff, TurnOffEntireLight);
    }

    private void OnDisable()
    {
        EventManager.UnRegisterEvent(EventType.UseEnergyDrink, BecomeBoost);
        EventManager.UnRegisterEvent(EventType.UseInvisiblePotion, BecomeInvisible);
        EventManager.UnRegisterEvent(EventType.UseUpgradedLight, UseUpgradedLightHandler);
        EventManager.UnRegisterEvent(EventType.UsePrisonKey, usePrisonKeyItem);
        EventManager.UnRegisterEvent(EventType.UseHatch, HasHatchItem);
        EventManager.UnRegisterEvent(EventType.LightOn, TurnOnLight);
        EventManager.UnRegisterEvent(EventType.LightOff, TurnOffLight);
        EventManager.UnRegisterEvent(EventType.UseMap, HasTriggerMap);
        EventManager.UnRegisterEvent(EventType.InEventPlayer, SetZeroVelocity);
        EventManager.UnRegisterEvent(EventType.OutEventPlayer, ResumeMovement);
        EventManager.UnRegisterEvent(EventType.EntireLightOn, TurnOnEntireLight);
        EventManager.UnRegisterEvent(EventType.EntireLightOff, TurnOffEntireLight);
    }

    private void Start()
    {

        PlayerStateMachine.Initialize(idleState);

        //flashLight.enabled = true;              
        //lightCollider.enabled = true;
        //isLightOn = true;
        countLife = 2;
        Player.runnerStatuses.Clear();

        if (photonView.IsMine)
        {
            CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
            if (cam != null)
                cam.Follow = transform;

            photonView.RPC("SetFlashlight", RpcTarget.Others, true);

            GameObject playerCanvas = GameObject.Find("PlayerCanvas");
            if (playerCanvas != null)
            {
                Transform minimapObj = playerCanvas.transform.Find("MiniMapRender");
                if (minimapObj != null)
                {
                    minimap = minimapObj.gameObject;
                    minimap.SetActive(false);
                }

                Transform escapeUIObj = playerCanvas.transform.Find("UseItemAndEscape");
                if (escapeUIObj != null)
                {
                    useItemAndEscapeUI = escapeUIObj.gameObject;
                    useItemAndEscapeUI.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("UseItemAndEscape 오브젝트없음");
                }

                //Transform allEscapeObj = playerCanvas.transform.Find("AllEscape");
                //if (allEscapeObj != null)
                //{
                //    exitDoorEscapeUI = allEscapeObj.gameObject;
                //    exitDoorEscapeUI.SetActive(false);
                //}
                //else
                //{
                //    Debug.LogWarning("AllEscape 오브젝트 없음");
                //}

                Transform deathProtein = playerCanvas.transform.Find("ProteinDeathAnim");
                if (deathProtein != null)
                {
                    deathProteinUI = deathProtein.gameObject;
                    deathProteinUI.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("ProteinDeathAnim 못 찾음");
                }

                Transform deathPeanut = playerCanvas.transform.Find("PeanutDeathAnim");
                if (deathPeanut != null)
                {
                    deathPeanutUI = deathPeanut.gameObject;
                    deathPeanutUI.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("deathPeanutUI 못 찾음");
                }

                Transform deathPukeGirl = playerCanvas.transform.Find("PukeGirlDeathAnim");
                if (deathPukeGirl != null)
                {
                    deathPuKeGirlUI = deathPukeGirl.gameObject;
                    deathPuKeGirlUI.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("deathPuKeGirlUI 못 찾음");
                }

                Transform allDead = playerCanvas.transform.Find("AllDeath");
                if (allDead != null)
                { 
                allDeadUI = allDead.gameObject;
                allDeadUI.SetActive(false);
                }
                else 
                    Debug.LogWarning("AllDeath UI 못 찾음");

                Transform someDead = playerCanvas.transform.Find("PlayerDeath");
                if (someDead != null)
                {
                    someDeadUI = someDead.gameObject;
                    someDeadUI.SetActive(false);
                }
                else Debug.LogWarning("PlayerDeath UI 못 찾음");

                Transform allEscape = playerCanvas.transform.Find("AllEscape");
                if (allEscape != null)
                {
                    allEscapeUI = allEscape.gameObject;
                    allEscapeUI.SetActive(false);
                }
                else Debug.LogWarning("AllEscape UI 못 찾음");

                Transform someEscape = playerCanvas.transform.Find("EscapeAlone");
                if (someEscape != null)
                {
                    someEscapeUI = someEscape.gameObject;
                    someEscapeUI.SetActive(false);
                }
                else Debug.LogWarning("EscapeAlone UI 못 찾음");

                Transform prisonEnding = playerCanvas.transform.Find("PrisonEnding");
                if (prisonEnding != null)
                {
                    prisonEndingUI = prisonEnding.gameObject;
                    prisonEndingUI.SetActive(false);
                }
                else Debug.LogWarning("PrisonEnding UI 못 찾음");
            }
        }

        // 초기화 되기전에 아무것도 되지 마라 
        isInitialized = true;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            PlayerStateMachine.currentState.Update();

            if (isInMiniGameTrigger && Input.GetKeyDown(KeyCode.E))
            {
                currentTrigger.TryOpenMiniGame(photonView);
            }


            if (Input.GetKeyDown(KeyCode.M))
            {
                if (!hasMap)
                {
                    Debug.Log("지도 없음");
                }
                else
                {
                    if (!isInMap)
                        OpenMap();
                    else
                        CloseMap();
                }
            }

            if (Input.GetKeyUp(KeyCode.R))
            {
                if (hasHatch && isInHatch)
                {
                    useHatchItem();
                    Debug.Log("개구멍사용");
                }
                else
                {
                    Debug.Log("사용못함");
                }
            }

            // 투명물약지속시간

            if (isInvisible)
            {
                invisibleTimer -= Time.deltaTime;
                if (invisibleTimer <= 0f)
                {
                    ResetTransparency();
                }
            }

            // 이동물약지속시간
            if (isBoosted)
            {
                boostTimer -= Time.deltaTime;
                if (boostTimer <= 0f)
                {
                    ResetMoveSpeed();
                }
            }

            // 손전등 지속시간
            if (isUpgradedLight)
            {
                upgradedLightTimer -= Time.deltaTime;

                // 5초지나면 깜빡
                if (upgradedLightTimer <= 5f && !isBlinking)
                {
                    isBlinking = true;
                    blinkTimer = blinkInterval;
                }

                if (isBlinking)
                {
                    float timeRatio = upgradedLightTimer / 5f;
                    blinkInterval = Mathf.Lerp(0.05f, 0.3f, timeRatio);

                    blinkTimer -= Time.deltaTime;
                    if (blinkTimer <= 0f)
                    {
                        flashLight.enabled = !flashLight.enabled;

                        photonView.RPC("SetFlashlightBlink", RpcTarget.Others, flashLight.enabled);

                        blinkTimer = blinkInterval;
                    }
                }


                if (upgradedLightTimer <= 0f)
                {
                    photonView.RPC("ResetFlashlight", RpcTarget.All);
                    flashLight.enabled = true;
                }
            }
        }
        else
        {
            //이동보간
            transform.position = Vector3.Lerp(transform.position, networkedPosition, Time.deltaTime * lerpSpeed);
            rb.linearVelocity = networkedVelocity;

            anim.SetBool("IsMoving", networkedIsMoving);
            anim.SetFloat("DirX", networkedDirX);
            anim.SetFloat("DirY", networkedDirY);

            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * facingDir;
            scale.y = Mathf.Abs(scale.y) * facingUpDir;
            transform.localScale = scale;

            lightObject.localRotation = Quaternion.Euler(0f, 0f, lightAngle);
        }

        // 테스트용
        if (photonView.IsMine && PlayerStateMachine.currentState != deadState && Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("플레이어죽음");
            EventManager.TriggerEvent(EventType.PlayerHpZero);
            PlayerStateMachine.ChangeState(deadState);
            isDead = true;
            profileSlotManager.photonView.RPC("SyncProfileState", RpcTarget.All, PhotonNetwork.LocalPlayer, ProfileState.deadSprite);
        }
    }


    private void FixedUpdate()
    {
        PlayerStateMachine.currentState.FixedUpdate();
        Vector3 pos = transform.position;
        //transform.position = new Vector3(pos.x, pos.y, pos.y);
    }

    public void UpdateAnimParam(Vector2 input)
    {
        if (input != Vector2.zero)
            lastDir = input.normalized;

        bool isMoving = input != Vector2.zero;

        anim.SetBool("IsMoving", isMoving);
        anim.SetFloat("DirX", lastDir.x);
        anim.SetFloat("DirY", lastDir.y);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine) return;

        if (collision.CompareTag("MiniGameTrigger"))
        {
            currentTrigger = collision.GetComponent<MiniGameTrigger>();
            isInMiniGameTrigger = true;
        }

        if (collision.CompareTag("Peanut") && !isHit && PlayerStateMachine.currentState != escapeState)
        {
            countLife--;
            Debug.Log("땅콩 충돌");
            StartCoroutine(HitCooldown());

            if (countLife <= 0 && !isDead)
            {
                photonView.RPC("PlayScream", RpcTarget.All);
                Debug.Log("너죽음");
                EventManager.TriggerEvent(EventType.PlayerHpZero);
                EventManager.TriggerEvent(EventType.InevntoryOff);
                profileSlotManager.photonView.RPC("SyncProfileState", RpcTarget.All, PhotonNetwork.LocalPlayer, ProfileState.deadSprite);

                if (deathPeanutUI != null)
                {
                    deathPeanutUI.SetActive(true);
                    StartCoroutine(DeathUIDeleteDelay(deathPeanutUI, 3f));
                }
                PlayerStateMachine.ChangeState(deadState);
                //StartCoroutine(GhostDeathSequence(2f));
            }
            else if (countLife == 1)
            {
                photonView.RPC("CaughtByGhost", RpcTarget.AllBuffered);
                if (photonView.IsMine)
                    StartCoroutine(UpdateCameraConfinerDelayed());
                EventManager.TriggerEvent(EventType.PlayerHpOne);
                profileSlotManager.photonView.RPC("SyncProfileState", RpcTarget.All, PhotonNetwork.LocalPlayer, ProfileState.prisonSprite);
                Debug.Log("너한번잡힘 한 번 더 잡히면 너 죽음");

            }
        }


        if (collision.CompareTag("PukeGirl") && !isHit && PlayerStateMachine.currentState != escapeState)
        {

            countLife--;
            Debug.Log("토하는애 충돌");
            StartCoroutine(HitCooldown());

            if (countLife <= 0 && !isDead)
            {
                photonView.RPC("PlayScream", RpcTarget.All);
                Debug.Log("너죽음");

                EventManager.TriggerEvent(EventType.PlayerHpZero);
                EventManager.TriggerEvent(EventType.InevntoryOff);
                profileSlotManager.photonView.RPC("SyncProfileState", RpcTarget.All, PhotonNetwork.LocalPlayer, ProfileState.deadSprite);

                if (deathPuKeGirlUI != null)
                {
                    deathPuKeGirlUI.SetActive(true);
                    StartCoroutine(DeathUIDeleteDelay(deathPuKeGirlUI, 3f));
                }
                PlayerStateMachine.ChangeState(deadState);

            }
            else if (countLife == 1)
            {
                photonView.RPC("CaughtByGhost", RpcTarget.AllBuffered);
                if (photonView.IsMine)
                    StartCoroutine(UpdateCameraConfinerDelayed());
                EventManager.TriggerEvent(EventType.PlayerHpOne);
                profileSlotManager.photonView.RPC("SyncProfileState", RpcTarget.All, PhotonNetwork.LocalPlayer, ProfileState.prisonSprite);
                Debug.Log("너한번잡힘 한 번 더 잡히면 너 죽음");

            }
        }

        if (collision.CompareTag("Protein") && !isHit && PlayerStateMachine.currentState != escapeState
            )
        {

            countLife--;
            Debug.Log("프로틴 충돌");
            StartCoroutine(HitCooldown());

            if (countLife <= 0 && !isDead)
            {
                photonView.RPC("PlayScream", RpcTarget.All);
                Debug.Log("너죽음");
                EventManager.TriggerEvent(EventType.PlayerHpZero);
                EventManager.TriggerEvent(EventType.InevntoryOff);
                profileSlotManager.photonView.RPC("SyncProfileState", RpcTarget.All, PhotonNetwork.LocalPlayer, ProfileState.deadSprite);

                if (deathProteinUI != null)
                {
                    deathProteinUI.SetActive(true);
                    StartCoroutine(DeathUIDeleteDelay(deathProteinUI, 3f));
                }
                //StartCoroutine(GhostDeathSequence(2f));
                PlayerStateMachine.ChangeState(deadState);
            }
            else if (countLife == 1)
            {
                photonView.RPC("CaughtByGhost", RpcTarget.AllBuffered);
                if (photonView.IsMine)
                    StartCoroutine(UpdateCameraConfinerDelayed());
                EventManager.TriggerEvent(EventType.PlayerHpOne);
                profileSlotManager.photonView.RPC("SyncProfileState", RpcTarget.All, PhotonNetwork.LocalPlayer, ProfileState.prisonSprite);
                Debug.Log("너한번잡힘 한 번 더 잡히면 너 죽음");

            }
        }

        if (collision.CompareTag("PlayerSight"))
            return;

        if (collision.CompareTag("ExitDoor"))
        {
            if (CompareTag("DeadPlayer"))
                return;
            Debug.Log("탈출구가능");
            escapeState.SetEscapeType(EscapeType.ExitDoor);
            PlayerStateMachine.ChangeState(escapeState);

            // 나 말고는 상태 바꾸지 않도록
            photonView.RPC("SetEscapeTypeForOthers", RpcTarget.OthersBuffered, (int)EscapeType.Hatch);
        }


        if (collision.CompareTag("PrisonDoor"))
        {
            isInPrisonDoor = true;
            if (hasPrisonKey)
            {
                Debug.Log("감옥해방가능");
            }
            // 이벤트로 isInPrisonDoor = true
            EventManager.TriggerEvent(EventType.InPrisonDoor, true);
        }

        if (collision.CompareTag("Hatch"))
        {
            isInHatch = true;

        }

        if (collision.CompareTag("Prison"))
        {
            isInsidePrison = true;
            isLightOn = false;
            flashLight.enabled = false;
            lightCollider.enabled = false;
            photonView.RPC("SetFlashlight", RpcTarget.Others, false);

            // 서버에 내 상태 알리기
            BroadcastStatus(RunnerStatus.Prison);
            //ExitGames.Client.Photon.Hashtable props = new();
            //props["IsInPrison"] = true;
            //PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }


    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!photonView.IsMine) return;

        if (collision.CompareTag("MiniGameTrigger"))
        {
            isInMiniGameTrigger = false;
            currentTrigger = null;
        }

        if (collision.CompareTag("PrisonDoor"))
        {
            isInPrisonDoor = false;
            // 이벤트로 isInPrisonDoor = true
            EventManager.TriggerEvent(EventType.InPrisonDoor, false);
        }

        if (collision.CompareTag("Hatch"))
        {
            isInHatch = false;
        }

        if (collision.CompareTag("Prison"))
        {
            isLightOn = true;
            flashLight.enabled = true;
            lightCollider.enabled = true;
            isInsidePrison = false;
            photonView.RPC("SetFlashLight", RpcTarget.Others, true);
            profileSlotManager.photonView.RPC("SyncProfileState", RpcTarget.All, PhotonNetwork.LocalPlayer, ProfileState.AliveSprite);
            BroadcastStatus(RunnerStatus.Alive);
            //ExitGames.Client.Photon.Hashtable props = new();
            //props["IsInPrison"] = false;
            //PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

    }

    private IEnumerator HitCooldown()
    {
        isHit = true;
        yield return new WaitForSeconds(hitCooldown);
        isHit = false;
    }

    [PunRPC]
    public void PlayScream()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Scream, false);
    }

    public bool InputE()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    [PunRPC]
    public void CaughtByGhost()
    {
        Transform portal = moveMap.transform.Find(portalName);
        if (portal != null)
            transform.position = portal.position;

        if (photonView.IsMine)
        {
            ExitGames.Client.Photon.Hashtable props = new();
            props["IsInPrison"] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }

    private IEnumerator UpdateCameraConfinerDelayed()
    {
        CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
        CinemachineConfiner2D confiner = cam.GetComponent<CinemachineConfiner2D>();
        confiner.BoundingShape2D = moveMap.GetComponent<Collider2D>();
        canvas.enabled = true;
        yield return new WaitForSeconds(0.05f);
        confiner.InvalidateBoundingShapeCache();
        yield return new WaitForSeconds(0.25f);
        canvas.enabled = false;

    }
    #region 이동관련

    public void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    private void FlipVertical()
    {
        facingUpDir *= -1;
        facingUp = !facingUp;
        transform.Rotate(180, 0, 0);
    }

    public void FlipController(float x, float y)
    {
        if (x > 0 && !facingRight) Flip();
        else if (x < 0 && facingRight) Flip();

        if (y > 0 && !facingUp) FlipVertical();
        else if (y < 0 && facingUp) FlipVertical();
    }

    public void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
    }

    public void SetZeroVelocity()
    {
        rb.linearVelocity = Vector2.zero;
    }

    public void ResumeMovement()
    {
        if (PlayerStateMachine != null)
        {
            PlayerStateMachine.ChangeState(idleState);
        }
    }
    public void ApplySlow(float factor)
    {
        moveSpeed = originalSpeed * factor;
    }

    public void ResetSpeed()
    {
        moveSpeed = originalSpeed;
    }
    [PunRPC]
    void TeleportPlayer(int viewID, Vector3 pos)
    {
        PhotonView view = PhotonView.Find(viewID);
        if (view != null)
        {
            view.transform.position = pos;
            // 보간용 위치도 순간이동 위치로 맞춰줌
            networkedPosition = pos;
            rb.linearVelocity = Vector2.zero;
            isTeleporting = true;
            StartCoroutine(ResetTeleportFlag());

        }
    }
    public IEnumerator ResetTeleportFlag()
    {
        yield return new WaitForSeconds(0.1f);
        isTeleporting = false;
    }

    #endregion

    #region 손전등 관련
    public void RotateLight(Vector2 moveInput)
    {
        if (moveInput == Vector2.zero)
            return;

        float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;

        lightObject.localPosition = Vector3.zero;

        lightObject.localRotation = Quaternion.Euler(0f, 0f, angle);

        if (photonView.IsMine)
        {
            lightAngle = angle;
        }
    }
    //손전등업글
    private void UseUpgradedLightHandler()
    {
        if (!photonView.IsMine)
            return;

        photonView.RPC("UpGradeLight", RpcTarget.All);
    }

    [PunRPC]
    private void UpGradeLight()
    {
        flashLight.pointLightOuterRadius = upgradedRadius;
        flashLight.enabled = true;

        //hasUpgradedFlashlight = false;
        isUpgradedLight = true;
        upgradedLightTimer = upgradeLightDuration;

        float scaleRatio = upgradedRadius / defaultRadius;
        ScalePolygonCollider(scaleRatio);

        Debug.Log("손전등 업글");
    }


    [PunRPC]
    private void ResetFlashlight()
    {
        flashLight.pointLightOuterRadius = defaultRadius;

        isUpgradedLight = false;
        isBlinking = false;
        blinkTimer = 0f;

        ScalePolygonCollider(1.0f); // 손전등원래대로
        Debug.Log("손전등업그레이드 종료");
    }

    [PunRPC]
    public void SetFlashlightBlink(bool turnOn)
    {
        flashLight.enabled = turnOn;
    }

    private void TurnOnLight()
    {
        if (!photonView.IsMine)
            return;

        isLightOn = true;
        flashLight.enabled = true;
        lightCollider.enabled = true;
        photonView.RPC("SetFlashlight", RpcTarget.Others, true);

        Debug.Log("손전등켜짐");
    }

    private void TurnOffLight()
    {
        if (!photonView.IsMine)
            return;

        isLightOn = false;
        flashLight.enabled = false;
        lightCollider.enabled = false;
        photonView.RPC("SetFlashlight", RpcTarget.Others, false);

        Debug.Log("손전등꺼짐");
    }

    private void TurnOnEntireLight()
    {
        if (!photonView.IsMine)
            return;

        circleLight.enabled = true;

        if (isInsidePrison)
            return;

        if (isDead)
            return;
        isLightOn = true;
        flashLight.enabled = true;
        lightCollider.enabled = true;
        if (isInvisible)
        {
            photonView.RPC("SetFlashEntireLight", RpcTarget.Others, false);
            return;
        }
        photonView.RPC("SetFlashEntireLight", RpcTarget.Others, true);
    }

    private void TurnOffEntireLight()
    {
        if (!photonView.IsMine)
            return;
        if (isDead)
            return;
        isLightOn = false;
        flashLight.enabled = false;
        circleLight.enabled = false;
        lightCollider.enabled = false;
        photonView.RPC("SetFlashEntireLight", RpcTarget.Others, false);
    }

    [PunRPC]
    public void SetFlashlight(bool turnOn)
    {
        isLightOn = turnOn;
        flashLight.enabled = turnOn;
        lightCollider.enabled = turnOn;
    }

    [PunRPC]
    public void SetFlashEntireLight(bool turnOn)
    {
        // 투명 상태인 경우, 라이트 켜지지 않도록 예외 처리
        if (isInvisible)
        {
            flashLight.enabled = false;
            circleLight.enabled = false;
            lightCollider.enabled = false;
            return;
        }
        if (isInsidePrison)
        {
            circleLight.enabled = turnOn;
            return;
        }
        isLightOn = turnOn;
        isCircleLightOn = turnOn;
        flashLight.enabled = turnOn;
        circleLight.enabled = turnOn;
        lightCollider.enabled = turnOn;
    }
    private void ScalePolygonCollider(float scale)
    {
        Vector2[] scaled = new Vector2[defaultColliderPoints.Length];
        for (int i = 0; i < scaled.Length; i++)
        {
            scaled[i] = defaultColliderPoints[i] * scale;
        }
        lightCollider.points = scaled;
    }

    //public void OnEvent(EventData data)
    //{
    //    if (data.Code != EVENT_BLACKOUT) return;

    //    float duration = (float)data.CustomData;
    //    if (photonView.IsMine)
    //    {
    //        flashLight.enabled = false;
    //        circleLight.enabled = false;
    //        StartCoroutine(DelayedTurnOn(duration));
    //    }
    //}

    //private IEnumerator DelayedTurnOn(float duration)
    //{
    //    yield return new WaitForSeconds(duration);
    //    flashLight.enabled = true;
    //    circleLight.enabled = true;
    //}

    #endregion

    #region 플레이어 죽음
    public void BecomeGhost()
    {
        if (!photonView.IsMine)
            return;

        Color c = sr.color;
        c.a = 0.5f;
        sr.color = c;

        gameObject.tag = "DeadPlayer";
        gameObject.layer = LayerMask.NameToLayer("DeadPlayer");

        photonView.RPC("SetGhostVisual", RpcTarget.Others);
        photonView.RPC("SetFlashEntireLight", RpcTarget.Others, false);

    }

    [PunRPC]
    public void SetGhostVisual()
    {
        Color c = sr.color;
        c.a = 0f;
        sr.color = c;
        playerNickName.SetActive(false);
        gameObject.tag = "DeadPlayer";
        gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
    }
    private IEnumerator DeathUIDeleteDelay(GameObject ui, float delay)
    {
        yield return new WaitForSeconds(delay);
        ui.SetActive(false);
    }

    private IEnumerator GhostDeathSequence(float delay)
    {
        yield return new WaitForSeconds(delay);

        PlayerStateMachine.ChangeState(deadState); // 죽음 상태로 전환
    }


    #endregion

    #region 플레이어 탈출
    public void SetEscapeType(EscapeType type)
    {
        escapeType = type;
    }

    [PunRPC]
    private void SetEscapeTypeForOthers(int escapeCode)
    {
        escapeType = (EscapeType)escapeCode;
        // 상태 바꾸지 않음
    }

    [PunRPC]
    public void EscapePlayerObject()
    {
        if (!photonView.IsMine)
        {
            // 타 클라에서 탈출플레이어(자신) 렌더링,콜라이더 끄기
            sr.enabled = false; // SpriteRenderer
            flashLight.enabled = false;
            circleLight.enabled = false;

            if (playerNickName != null)
                playerNickName.SetActive(false);

            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;
        }
    }
    #endregion

    #region 아이템
    //투명물약
    public void BecomeInvisible()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Color c = sr.color;
        c.a = 0.5f;
        sr.color = c;

        hasInvisiblePotion = false;
        isInvisible = true;
        invisibleTimer = invisibleDuration;

        photonView.RPC("SetTransparencyVisual", RpcTarget.Others, true);

        Debug.Log("투명버프");

    }

    private void ResetTransparency()
    {
        Color c = sr.color;
        c.a = 1f;
        sr.color = c;

        flashLight.enabled = true;
        isInvisible = false;
        photonView.RPC("SetTransparencyVisual", RpcTarget.Others, false);
        photonView.RPC("SetFlashEntireLight", RpcTarget.Others, true);
        Debug.Log("투명버프끝");
    }

    [PunRPC]
    public void SetTransparencyVisual(bool isInvisible)
    {
        if (!isInvisible)
        {
            // 투명 해제
            Color visible = sr.color;
            visible.a = 1f;
            sr.color = visible;
            flashLight.enabled = isLightOn;
            circleLight.enabled = true;

            if (playerNickName != null)
                playerNickName.SetActive(true);

            return;
        }

        if (!PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out object roleObj) || roleObj == null)
        {
            Debug.LogWarning("Role 없음");
            return;
        }

        string roleName = roleObj.ToString();

        // 투명물약마시면 몬스터한테는 안보임
        bool isReceiverMonster = NetworkProperties.instance.GetMonsterStates(roleName);

        if (isReceiverMonster)
        {

            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f); // 완전 투명
            flashLight.enabled = false;
            circleLight.enabled = false;
            if (playerNickName != null)
                playerNickName.SetActive(false);
        }
        else
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.5f); // 반투명
            flashLight.enabled = isLightOn;
            circleLight.enabled = true;
            if (playerNickName != null)
                playerNickName.SetActive(true);
        }
    }

    // 이동속도업글
    public void BecomeBoost()
    {
        moveSpeed = boostedSpeed;
        hasEnergyDrink = false;
        isBoosted = true;
        boostTimer = boostDuration;
        Debug.Log("이속버프");
    }
    private void ResetMoveSpeed()
    {
        moveSpeed = baseSpeed;
        isBoosted = false;
        Debug.Log("이속버프끝");
    }

    private void HasTriggerMap()
    {
        hasMap = true;
    }

    private void OpenMap()
    {

        isInMap = true;
        Debug.Log(" 맵열림");
        if (hasMap && minimap != null)
        {

            minimap.SetActive(true);
        }
        else
        {
            Debug.Log("맵 없음");
        }
    }
    private void CloseMap()
    {
        isInMap = false;
        if (minimap != null)
        {
            minimap.SetActive(false);
        }
        Debug.Log("맵 닫음");
    }

    private void HasPrisonKey()
    {
        hasPrisonKey = true;
        Debug.Log("감옥키획득");
    }

    private void usePrisonKeyItem()
    {
        Debug.Log("감옥 키");
        EventManager.TriggerEvent(EventType.OpenPrisonDoor);
        hasPrisonKey = false;
    }

    private void HasHatchItem()
    {
        hasHatch = true;
    }

    private void useHatchItem()
    {
        if (!photonView.IsMine) return;

        // EscapeHatch는 자신에게만 적용
        escapeState.SetEscapeType(EscapeType.Hatch);
        PlayerStateMachine.ChangeState(escapeState);

        // 나 말고는 상태 바꾸지 않도록
        photonView.RPC("SetEscapeTypeForOthers", RpcTarget.OthersBuffered, (int)EscapeType.Hatch);
    }

    #endregion

    #region 시리얼라이즈뷰 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (isInitialized == false)
        {
            return;
        }

        if (stream.IsWriting)
        {
            //신호보냄
            stream.SendNext(transform.position);
            stream.SendNext(transform.localScale);
            stream.SendNext(rb.linearVelocity);

            stream.SendNext((int)PlayerStateMachine.currentState.StateType);
            stream.SendNext((int)escapeType);
            stream.SendNext(facingDir);
            stream.SendNext(facingUpDir);
            stream.SendNext(anim.GetBool("IsMoving"));
            stream.SendNext(anim.GetFloat("DirX"));
            stream.SendNext(anim.GetFloat("DirY"));
            stream.SendNext(lightAngle);
        }
        else
        {
            if (!isTeleporting)
            {
                networkedPosition = (Vector3)stream.ReceiveNext();
            }
            else
            {
                stream.ReceiveNext(); // 데이터는 소모해야 순서 안 꼬임
            }


            //networkedPosition = (Vector3)stream.ReceiveNext();

            // 로컬 스케일 처리
            transform.localScale = (Vector3)stream.ReceiveNext();

            // 속도 처리
            networkedVelocity = (Vector2)stream.ReceiveNext();


            // PlayerStateType 형변환 안전 체크
            object receivedStateObject = stream.ReceiveNext();
            if (receivedStateObject is int)
            {
                PlayerStateType receivedState = (PlayerStateType)(int)receivedStateObject;
            }
            else
            {
                Debug.LogError("Received state is not of type int");
            }

            // EscapeType 형변환 안전 체크
            object receivedEscapeObject = stream.ReceiveNext();
            if (receivedEscapeObject is int)
            {
                EscapeType receivedEscape = (EscapeType)(int)receivedEscapeObject;
            }
            else
            {
                Debug.LogError("Received escape type is not of type int");
            }

            // facingDir, facingUpDir 형변환 안전 체크
            object receivedFacingDir = stream.ReceiveNext();
            if (receivedFacingDir is int)
            {
                facingDir = (int)receivedFacingDir;
            }
            else
            {
                Debug.LogError("Received facingDir is not of type int");
            }

            object receivedFacingUpDir = stream.ReceiveNext();
            if (receivedFacingUpDir is int)
            {
                facingUpDir = (int)receivedFacingUpDir;
            }
            else
            {
                Debug.LogError("Received facingUpDir is not of type int");
            }

            // networkedIsMoving 형변환 안전 체크
            object receivedIsMoving = stream.ReceiveNext();
            if (receivedIsMoving is bool)
            {
                networkedIsMoving = (bool)receivedIsMoving;
            }
            else
            {
                Debug.LogError("Received isMoving is not of type bool");
            }

            // networkedDirX, networkedDirY 형변환 안전 체크
            object receivedDirX = stream.ReceiveNext();
            if (receivedDirX is float)
            {
                networkedDirX = (float)receivedDirX;
            }
            else
            {
                Debug.LogError("Received DirX is not of type float");
            }

            object receivedDirY = stream.ReceiveNext();
            if (receivedDirY is float)
            {
                networkedDirY = (float)receivedDirY;
            }
            else
            {
                Debug.LogError("Received DirY is not of type float");
            }

            // lightAngle 형변환 안전 체크
            object receivedLightAngle = stream.ReceiveNext();
            if (receivedLightAngle is float)
            {
                lightAngle = (float)receivedLightAngle;
            }
            else
            {
                Debug.LogError("Received lightAngle is not of type float");
            }
        }
    }

    #endregion

    [PunRPC]
    public void RPC_NotifyStatus(int actorNumber, int statusCode)
    {
        runnerStatuses[actorNumber] = (RunnerStatus)statusCode;

        // 항상 내 로컬 Player 컴포넌트로 검사 돌리기
        if (PhotonNetwork.LocalPlayer.TagObject is Player local)
            local.TryCheckEndingLocally();

        // 술래도 여기서 직접 연출 검사
        foreach (var ghost in FindObjectsByType<Ghost>(FindObjectsSortMode.None))
        {
            if (ghost.photonView.IsMine)
            {
                Debug.Log("[GhostCheck] TryCheckGhostEnding 호출!");
                ghost.TryCheckGhostEnding(); // 💥 이 한 줄이 술래의 연출을 담당
            }
        }
    }
    private bool IsRunner(Photon.Realtime.Player player)
    {
        if (!player.CustomProperties.TryGetValue("Role", out object roleObj) || roleObj == null)
        {
            Debug.LogWarning($"[IsRunner] {player.NickName} 의 Role 없음");
            return false;
        }

        string roleName = roleObj.ToString();
        bool isMonster = NetworkProperties.instance.GetMonsterStates(roleName);
        return !isMonster;
    }

    public void BroadcastStatus(RunnerStatus status)
    {
        int myActor = PhotonNetwork.LocalPlayer.ActorNumber;
        runnerStatuses[myActor] = status;

        TryCheckEndingLocally();   //내 로컬에서도 즉시 검사

        photonView.RPC("RPC_NotifyStatus", RpcTarget.All, myActor, (int)status);
    }

    private void TryCheckEndingLocally()
    {
        Debug.Log($"[TryCheck] {PhotonNetwork.LocalPlayer.NickName} - 상태 검사 시작");

        // 도망자 플레이어 목록 (술래는 제외)
        List<int> runnerList = new List<int>();
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (IsRunner(player))
                runnerList.Add(player.ActorNumber);
        }

        // 모든 도망자의 상태 정보가 모이지 않았다면 기다림
        foreach (int actor in runnerList)
        {
            if (!runnerStatuses.ContainsKey(actor))
                return; // 아직 모른 도망자 있으면 연출x
        }

        //도망자 상태 리스트
        List<RunnerStatus> runnerStatusList = new List<RunnerStatus>();
        foreach (int actor in runnerList)
        {
            runnerStatusList.Add(runnerStatuses[actor]);
        }

        // 아직 살아있는 도망자가 있다면 연출x
        foreach (RunnerStatus status in runnerStatusList)
        {
            if (status == RunnerStatus.Alive)
                return;
        }

        //연출은 딱 한 번
        if (hasTriggeredEnding)
            return;

        hasTriggeredEnding = true;

        // 내 상태 확인
        int myActor = PhotonNetwork.LocalPlayer.ActorNumber;
        RunnerStatus myStatus = runnerStatuses[myActor];

        // 상태별 인원 세기
        int totalDead = 0;
        int totalEscaped = 0;
        foreach (RunnerStatus status in runnerStatusList)
        {
            if (status == RunnerStatus.Dead)
                totalDead++;
            else if (status == RunnerStatus.Escaped)
                totalEscaped++;
        }

        // 전체 연출 조건 판단
        if (totalEscaped == runnerList.Count)
        {
            if (escapeType == EscapeType.Hatch)
                return;
            ShowLocalUI(allEscapeUI);
        }
        else if (totalDead == runnerList.Count)
        {
            ShowLocalUI(allDeadUI);
        }
        else
        {
            // 내 상태 기준 연출
            switch (myStatus)
            {
                case RunnerStatus.Escaped:
                    if (escapeType == EscapeType.Hatch)
                        return;
                    ShowLocalUI(someEscapeUI);
                    break;
                case RunnerStatus.Dead:
                    ShowLocalUI(someDeadUI);
                    break;
                case RunnerStatus.Prison:
                    ShowLocalUI(prisonEndingUI);
                    break;
            }
        }
    }

    private GameObject GetPartialUI(RunnerStatus status)
    {
        return status switch
        {
            RunnerStatus.Escaped => someEscapeUI,
            RunnerStatus.Dead => someDeadUI,
            RunnerStatus.Prison => prisonEndingUI,
            _ => null
        };
    }

    private void ShowLocalUI(GameObject ui)
    {
        Debug.Log($"[연출] ");

        if (ui == null) return;

        ui.transform.SetParent(null);                // 부모 해제 (캔버스 등)
        DontDestroyOnLoad(ui);                       // 씬 전환 후에도 유지
        ui.SetActive(true);                          // 보이기

        Transform black = ui.transform.Find("Black");
        if (black)
        {
            var fade = black.GetComponent<playerDeath>();
            if (fade) fade.TriggerFade();
        }

        // 로비건너가는 핸들러
        GameObject handlerGO = new GameObject("EndingHandler");
        EndingHandler handler = handlerGO.AddComponent<EndingHandler>();
        handler.StartEndSequence(ui, uiDuration);
    }

    private IEnumerator ReturnToLobbyAfter(float sec, GameObject ui)
    {
        yield return new WaitForSeconds(sec);
        if (ui != null)
            Destroy(ui);

        PhotonNetwork.LoadLevel("RobbyScene");

        yield return new WaitForSeconds(0.1f);
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
    }


    public void AnimationTrigger() => PlayerStateMachine.currentState.AnimationFinishTrigger();

    [SerializeField] private GameObject spriteObject;
    [SerializeField] private Transform lightObject;
    [SerializeField] private GameObject playerNickName;

    private Vector3 networkedPosition;
    private Vector3 networkedVelocity;
    private float lerpSpeed = 10f;
    private bool networkedIsMoving;
    private float networkedDirX;
    private float networkedDirY;
    private float lightAngle;

    private Vector2 lastDir = Vector2.right;   // 기본방향

    public ProfileSlotManager profileSlotManager;

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public SpriteRenderer sr { get; private set; }
    private Canvas canvas;

    public int facingDir { get; private set; } = 1;
    public int facingUpDir { get; private set; } = 1;   // 1 = ��, -1 = �Ʒ�
    protected bool facingRight = true;
    protected bool facingUp = true;

    [Header("�̵�")]
    public float moveSpeed = 5f;
    private float originalSpeed;
    public PlayerStateMachine PlayerStateMachine { get; private set; }

    public PlayerIdle idleState { get; private set; }
    public PlayerMove moveState { get; private set; }
    public PlayerDead deadState { get; private set; }
    public PlayerEscapeState escapeState { get; private set; }
    public EscapeType escapeType { get; private set; } = EscapeType.Dead;
    public int EscapeCode => (int)escapeType;

    private int countLife = 2;  //플레이어 수명

    [Header("이동물약")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float boostedSpeed = 10f;
    [SerializeField] private float boostDuration = 10f;
    private bool hasEnergyDrink = false;
    private bool isBoosted = false;
    private float boostTimer;

    [Header("투명물약")]
    [SerializeField] private float invisibleDuration = 5f;
    [SerializeField] private bool hasInvisiblePotion = false;
    private bool isInvisible = false;
    [SerializeField] private float invisibleTimer;   //디버그용 시리얼라이즈필드

    [Header("손전등")]
    [SerializeField] private Light2D flashLight;
    [SerializeField] private Light2D circleLight;
    [SerializeField] private float upgradedRadius = 8f;
    [SerializeField] private float defaultRadius = 3.5f;
    [SerializeField] private float upgradeLightDuration = 10f;
    [SerializeField] private PolygonCollider2D lightCollider;
    [SerializeField] private float upgradedLightTimer; // 업글손전등 시간
    private bool isUpgradedLight = false;
    private bool isLightOn = false; // 켜짐
    private bool isCircleLightOn = false; // 켜짐
    private bool isBlinking = false;
    private float blinkTimer = 0f;
    private float blinkInterval = 0.3f; // 깜빡속도
    private Vector2[] defaultColliderPoints;
    //private bool hasUpgradedFlashlight = false;
    private const byte EVENT_BLACKOUT = 1;

    [Header("감옥키")]
    [SerializeField] private bool hasPrisonKey = false;
    private bool isInPrisonDoor = false;
    private bool isInsidePrison = false;

    [Header("잡혀서 감옥감")]
    [SerializeField] private GameObject moveMap;
    [SerializeField] private string portalName = "PrisonSpawnPoint";

    [Header("개구멍")]
    [SerializeField] private bool hasHatch = false;
    private bool isInHatch = false;

    [Header("지도")]
    private GameObject minimap;
    [SerializeField] private bool hasMap = false;
    private bool isInMap = false;

    [Header("탈출")]
    [SerializeField] private GameObject useItemAndEscapeUI;
    [SerializeField] private GameObject exitDoorEscapeUI;
    public GameObject UseItemAndEscapeUI => useItemAndEscapeUI;
    public GameObject ExitDoorEscapeUI => exitDoorEscapeUI;

    [Header("잡힘")]
    [SerializeField] private GameObject deathProteinUI;
    [SerializeField] private GameObject deathPeanutUI;
    [SerializeField] private GameObject deathPuKeGirlUI;
    private bool isHit = false;
    private float hitCooldown = 0.5f;
    public bool isDead = false;

    bool isTeleporting = false;

    bool isInitialized = false;

    bool isInMiniGameTrigger = false;
    MiniGameTrigger currentTrigger;

    [Header("UI")]
    public GameObject allDeadUI;      // 전원 사망 연출용
    public GameObject someDeadUI;       // 일부 사망
    public GameObject allEscapeUI; // 탈출 연출용
    public GameObject someEscapeUI; // 탈출 연출용
    public GameObject prisonEndingUI; // 감옥 연출용
    public static Dictionary<int, RunnerStatus> runnerStatuses = new();
    public float uiDuration = 5f;
    private bool hasTriggeredEnding = false;
}