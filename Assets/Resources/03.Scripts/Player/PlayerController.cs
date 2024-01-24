using System.IO;
using System.Data.SqlTypes;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Runtime.InteropServices.ComTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public static PlayerController ins = null;

    

    private void Awake()
    {
        ins = this;


    }


    private void Start()
    {
        GetComponent();
        StartSetting();
    }

    private void Update()
    {
        PlyerMove();

        PlayerJump();

        PlayerWallSliding();
        PlayerWallJump();

        PlayerDodge();

        PlayerStop();

        PlayerAttack();
        AttackParticle();

        BulletTime();
    }

    private void FixedUpdate()
    {
        GronudRay();
    }



    [Header("Player")]
    public GameObject player = null;                                        // 플레이어
    [HideInInspector] public Rigidbody2D rb2d = null;                       // 플레이어 Rigidbody2D
    private BoxCollider2D boxCollider2D = null;

    private enum State { Normal, Dodge, Attack,}
    [Space(8f)]
    [SerializeField] State state;



    [Header("Move")]
    [SerializeField] float speed = 10f;                                     // 이동속도
    [Space(8f)]

    [SerializeField] bool isMove = false;                                   // 플레이어가 이동 중인지 확인
    [SerializeField] bool isDontMove = false;                               // 플레이어 이동 불가능
    [SerializeField] float playerDir = 0;                                   // 플레이어가 바라보는 방향 확인 ( 좌 = -1 , 우 = 1 )



    [Header("Jump")]
    [SerializeField] bool isGround = false;                                 // 바닥 확인
    [SerializeField] bool isJump = false;                                   // 플레이어가 점프중인지 확인
    [Space(8f)]

    [SerializeField] float groundChkRadius = 0f;                            // 바닥 확인용 반지름
    [SerializeField] LayerMask groundLM;                                    // 바닥 확인용 레이어 마스크
    [SerializeField] float power = 15f;                                     // 점프파워
    [Space(8f)]

    [SerializeField] Transform footPos;                                     // 발의 위치



    [Header("Wall Jump")]
    [SerializeField] bool isWall = false;                                   // 벽에 닿았는지 확인
    [SerializeField] bool isWallSliding = false;                            // 벽에 슬라이딩 중인지 확인
    [SerializeField] bool isWallJump = false;                               // 벽 점프가 가능한지 확인
    [Space(8f)]

    [SerializeField] float wallStopTimer = 0.5f;
    [SerializeField] float wallSlidigSpeed = 0;                             // 벽에 닿았을 때 떨어지는 속도


    [Header("Dadge")]
    [SerializeField] bool isDodge = false;                                  // 구르기 중인지 확인
    [SerializeField] float dodgeSpeed = 0;



    [Header("Attack")]
    [SerializeField] float attack = 0f;                                     // 공격파워
    [Space(8f)]

    [SerializeField] float cooltime = 0f;                                   // 공격 쿨타임 고정
    [SerializeField] float curtime = 0f;                                    // 공격 쿨타임
    [Space(8f)]

    [SerializeField] bool isAttack = false;                                 // 공격 중인지 확인
    [SerializeField] bool isFirstAtk = false;                               // 바닥에서 공격하는지 확인
    [Space(8f)]

    [SerializeField] Vector2 atk_dis_chk;                                   // 공격시 플레이어 위치 (플레이어 공격사거리 확인용)
    [SerializeField] float atk_dis_x = 0f, atk_dis_y = 0;                   // 공격사거리
    private bool axp = false, axm = false, ayp = false, aym = false;        // 플레이어 공격 x+ x- y+ y- 거리 확인용
    [Space(8f)]

    [SerializeField] Vector3 mousePos;                                      // 마우스 위치
    [SerializeField] Vector3 mousePosDir;                                   // 마우스 방향
    [Space(8f)]

    [SerializeField] GameObject attack_guide = null;                        // 공격 방향

    [Space(8f)]
    [SerializeField] GameObject attack_particle_obj = null;                 // 공격시 나오는 이펙트
    private ParticleSystem.MainModule attack_particle;



    /// <summary>
    /// 컴포넌트 받아오기
    /// </summary>
    private void GetComponent()
    {
        rb2d = player.GetComponent<Rigidbody2D>();
        attack_particle = attack_particle_obj.GetComponent<ParticleSystem>().main;
        boxCollider2D = GetComponent<BoxCollider2D>();
    }




    /// <summary>
    /// 시작시 적용되는 설정
    /// </summary>
    private void StartSetting()
    {
        atk_dis_x = 1.5f;
        state = State.Normal;
    }




    /// <summary>
    /// 플레이어가 바닥에 닿았는지 확인
    /// </summary>
    private void GronudRay()
    {
        // 플레이어의 발 위치에 원을 생성, 원이 바닥에 닿아있으면 isGround = true 그렇지 않으면 isGround = false
        isGround = Physics2D.OverlapCircle(footPos.position, groundChkRadius, groundLM);

        if (isGround && !isAttack)
        {
            isFirstAtk = true;
        }

    }




    // scene창에서만 보임
    private void OnDrawGizmos()
    {
        // 바닥을 확인하는 원 생성
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(footPos.position, groundChkRadius);

    }




    /// <summary>
    /// 플레이어 이동
    /// </summary>
    public void PlyerMove()
    {
        if (isDontMove)
            return;

        if (isAttack)
            return;

        if (isDodge)
            return;


        if (Input.GetButton("Horizontal"))
        {
            isMove = true;
            float h = Input.GetAxisRaw("Horizontal");


            rb2d.AddForce(Vector2.right * h, ForceMode2D.Impulse);

            if ((h > 0 && playerDir < 0) || (h < 0 && playerDir > 0))
                PlayerFlip();

            // 오른쪽으로 이동
            if (rb2d.velocity.x > speed)
            {
                rb2d.velocity = new Vector2(speed, rb2d.velocity.y);

            }
            // 왼쪽으로 이동
            else if (rb2d.velocity.x < speed * (-1))
            {
                rb2d.velocity = new Vector2(speed * (-1), rb2d.velocity.y);
            }

        }
        // 이동 종료, 점프중 이라면 작동 안함
        if (Input.GetButtonUp("Horizontal"))
        {
            rb2d.velocity = new Vector2(rb2d.velocity.normalized.x * 0f, rb2d.velocity.y);
            isMove = false;
        }
    }





    private void PlayerFlip()
    {
        transform.eulerAngles = new Vector3(0, Mathf.Abs(transform.eulerAngles.y - 180), 0);
        playerDir *= -1;
    }




    // 플레이어 미끄럼 방지
    private void PlayerStop()
    {
        if (Input.GetAxisRaw("Horizontal") == 0)
            isMove = false;

        if (!isAttack && isGround && !isMove && !isDodge)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.normalized.x * 0f, rb2d.velocity.y);
            // Debug.Log("[ SUCCESS ]    " + isAttack + "  " + isGround + "  " + isMove);
        }
    }





    /// <summary>
    /// 플레이어 점프
    /// </summary>
    private void PlayerJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (!isGround)
                return;

            isJump = true;

            rb2d.AddForce(Vector2.up * power, ForceMode2D.Impulse);

            // Debug.Log("Player Jump");

        }
        else if (rb2d.velocity.y <= 0)
        {
            isJump = false;
        }
    }





    /// <summary>
    /// 플레이어 벽 슬라이딩
    /// </summary>
    private void PlayerWallSliding()
    {
        if (isWall)
        {
            if (Input.GetButton("Horizontal") && !isWallJump)
                isWallSliding = true;

            if (isWallJump && rb2d.velocity.x == 0)
                isWallSliding = true;

            if (isAttack)
            {
                isWallSliding = false;
                isWallJump = false;
            }


            if (!isGround && !isJump && !isAttack)
            {
                if (isWallSliding)
                    isWallJump = false;

                wallStopTimer -= Time.deltaTime;

                if (isWallSliding && wallStopTimer > 0)
                {
                    rb2d.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                }
                else if (isWallSliding)
                {
                    rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
                    rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * wallSlidigSpeed);
                }
                else
                {
                    rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
            }
            else
            {
                rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
                isWallSliding = false;
            }
        }
        else
        {
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
            wallStopTimer = 0.5f;

            isWallSliding = false;
        }
    }


    /// <summary>
    /// 플레이어 벽 점프
    /// </summary>
    private void PlayerWallJump()
    {
        if (isWallSliding)
        {
            if (Input.GetButtonDown("Jump"))
            {
                isWallJump = true;
                isDontMove = true;
                isWallSliding = false;

                rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;

                rb2d.velocity = new Vector2(-playerDir * power, 1f * power);
                PlayerFlip();

            }

        }

        if (isGround)
            isWallJump = false;


        if (rb2d.velocity.x == 0 || isGround && !isDodge)
            isDontMove = false;
    }


    /// <summary>
    /// 플레이어가 벽에 닿았는지 확인
    /// </summary>
    private void WallCheck(bool chk)
    {
        isWall = chk;
    }






    /// <summary>
    /// 플레이어 구르기, 지형 내려가기
    /// </summary>
    private void PlayerDodge()
    {
        

        if (Input.GetButtonDown("Vertical") && isMove)
        {
            if (!isDodge)
            {
                if (isGround)
                {
                    isDodge = true;
                    dodgeSpeed = 30f;
                    state = State.Dodge;
                    
                }
            }
        }

        if (isDodge)
        {
            isDontMove = true;
            attack_particle_obj.SetActive(true);
            attack_particle.startLifetime = 0.13f;


            float speedDrop = 10f;
            dodgeSpeed -= dodgeSpeed * speedDrop * Time.deltaTime;

            float minSpeed = 3f;


            if (dodgeSpeed < minSpeed)
            {
                isDodge = false;
                state = State.Normal;
                attack_particle.startLifetime = 0f;
            }
            rb2d.velocity = Vector2.right * playerDir * dodgeSpeed;


            // Debug.Log(" Success : Dodge");
        }
    }






    /// <summary>
    /// 플레이어 공격
    /// </summary>
    public void PlayerAttack()
    {
        if (!isAttack)
        {
            // 마우스 위치 저장
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // 마우스 방향
            mousePosDir = (mousePos - player.transform.position).normalized;
            // 마우스 각도
            float z = Mathf.Atan2(mousePosDir.y, mousePosDir.x) * Mathf.Rad2Deg;
            // attack_guide를 회전 
            attack_guide.transform.rotation = Quaternion.Euler(0, 0, z);


            if (Input.GetMouseButtonDown(0))
            {
                dodgeSpeed = 0;
                isDodge = false;
                state = State.Normal;
                
                    state = State.Attack;
                    attack_guide.SetActive(true);

                    atk_dis_chk = transform.position;
                    axp = true; axm = true; ayp = true; aym = true;

                    StartCoroutine("AttackCoolTime");

                    isAttack = true;

                    // 게속해서 높이 올라가지 않게 설정
                    if (isFirstAtk)
                    {
                        atk_dis_y = 1.5f;
                        rb2d.velocity = Vector2.zero;
                        isFirstAtk = false;
                    }
                    else
                    {
                        atk_dis_y = 0.3f;
                    }

                    // attack_guide의 앞으로 공격 
                    rb2d.AddForce(attack_guide.transform.right * attack, ForceMode2D.Impulse);
                
            }

        }

        AttackDistance();
    }

    // 공격 거리 제한
    private void AttackDistance()
    {
        if (axp)
        {
            if (atk_dis_chk.x + atk_dis_x < transform.position.x)
            {
                axp = false;
                rb2d.velocity = new Vector2(rb2d.velocity.x / 2, rb2d.velocity.y);
                // Debug.Log("x+   " + rb2d.velocity.x);
            }
        }
        if (axm)
        {
            if (atk_dis_chk.x - atk_dis_x > transform.position.x)
            {
                axm = false;
                rb2d.velocity = new Vector2(rb2d.velocity.x / 2, rb2d.velocity.y);
                // Debug.Log("x-   " + rb2d.velocity.x);
            }
        }
        if (ayp)
        {
            if (atk_dis_chk.y + atk_dis_y < transform.position.y)
            {
                ayp = false;
                rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y / 3);
                // Debug.Log("y+   " + rb2d.velocity.y);
            }
        }
        if (aym)
        {
            if (atk_dis_chk.y - atk_dis_y > transform.position.y)
            {
                aym = false;
                rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y / 3);
                // Debug.Log("y-   " + rb2d.velocity.y);
            }
        }

    }

    private IEnumerator AttackCoolTime()
    {
        curtime = cooltime;

        while (true)
        {
            if (curtime <= 0)
            {
                attack_guide.SetActive(false);
                state = State.Normal;
                isAttack = false;
                yield break;
            }
            else
            {
                yield return new WaitForSecondsRealtime(0.01f);
                curtime -= 0.01f;
            }
        }
    }

    /// <summary>
    /// 플레이어 공격 이펙트
    /// </summary>
    private void AttackParticle()
    {
        if (isAttack || isDodge)
        {
            attack_particle_obj.SetActive(true);
            attack_particle.startLifetime = 0.13f;
        }
        else
        {
            attack_particle.startLifetime = 0f;
        }
    }



    
    /// <summary>
    /// 플레이어 공격받을 때
    /// </summary>
    private void PlayerHit()
    {
        switch(state)
        {
            case State.Normal:
                Debug.Log("죽음");
            break;

            case State.Dodge:
                Debug.Log("회피");
            break;

            case State.Attack:
                Debug.Log("공격");
            break;
        }
    }







    private void BulletTime()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Time.timeScale = 0.5f;


            Debug.Log("Success :  Bullet Time On   ");
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Time.timeScale = 1f;

            Debug.Log("Success :  Bullet Time Off   ");
        }
    }




    







    private void OnCollisionEnter2D(Collision2D other)
    {

        if (other.transform.tag == Tag.DeadZone.ToString())
        {
            // Debug.Log("NOTICE : Get Tag, " + other.transform.tag);
            // Debug.Log("NOTICE : Player Dead");

            player.transform.position = new Vector2(0, 1);
        }

    }


    private void OnCollisionStay2D(Collision2D other)
    {

        WallCheck(other.gameObject.layer == LayerMask.NameToLayer("Wall"));

    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            WallCheck(false);
            isWallSliding = false;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {


        if (other.transform.tag == Tag.DeadZone.ToString())
        {
            // Debug.Log("NOTICE : Get Tag, " + other.transform.tag);
            // Debug.Log("NOTICE : Player Dead");

            GameManager.ins.Restart();
        }

        if (other.transform.tag == Tag.Object.ToString())
        {
            // Debug.Log("NOTICE : Get Tag, " + other.transform.tag);
            // Debug.Log("NOTICE : isAttack = false");

            other.gameObject.SetActive(false);
            // isAttack = false;
        }
    }

}
