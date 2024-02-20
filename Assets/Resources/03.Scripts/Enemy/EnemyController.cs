using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController ins = null;

    private void Awake()
    {
        ins = this;

    }

    private void Start()
    {
        GetStart();
        GetComponent();
    }

    private void Update()
    {
        Move();
        FindTarget();
        FollowTarget();
    }


    [Header("Enemy")]

    public GameObject enemy = null;
    [HideInInspector] public Rigidbody2D rb2d = null;
    private BoxCollider2D bc2d = null;



    [Header("Move")]

    [SerializeField] float speed = 10f;
    [Space(8f)]

    [SerializeField] Vector2 firPos, nowPos;
    [Space(8f)]

    [SerializeField] Vector2 moveMin = Vector2.zero;
    [SerializeField] Vector2 moveMax = Vector2.zero;
    [Space(8f)]

    [SerializeField] bool isMove = false;
    [Space(8f)]

    [SerializeField] float direction = 1;
    [Space(8f)]




    [Header("Target")]

    [SerializeField] Transform eye = null;
    [SerializeField] Vector2 boxSize;
    public LayerMask targetLayer;
    [SerializeField] GameObject target = null;
    [SerializeField] bool isFollow = false;




    [Header("Attack")]

    // 공격 가능 여부
    [SerializeField] bool isAttack = false;
    [SerializeField] bool isGetPos = false;
    [SerializeField] Vector2 enemyPos;
    [SerializeField] Vector2 targetPos;



    private void GetStart()
    {
        firPos = transform.position;
        moveMin += firPos;
        moveMax += firPos;
    }



    private void GetComponent()
    {
        rb2d = enemy.GetComponent<Rigidbody2D>();
        bc2d = enemy.GetComponent<BoxCollider2D>();
    }



    private void Move()
    {
        if(moveMin != Vector2.zero && moveMin != Vector2.zero && !isFollow)
        {

            rb2d.AddForce(Vector2.right * direction, ForceMode2D.Impulse);

            nowPos = transform.position;
            
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

            if(nowPos.x > moveMax.x && direction > 0)
            {
                Flip();
            }
            else if(nowPos.x < moveMin.x && direction < 0)
            {
                Flip();
            }
        }
    }


    private void FindTarget()
    {
        if(target == null)
        {
            Collider2D hit = Physics2D.OverlapBox(eye.position, boxSize, 0, targetLayer);
            
            if(hit != null)
            {
                Debug.Log(hit.name);
                target = hit.gameObject;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(eye.position, boxSize);
    }

    private void FollowTarget()
    {
        if(target != null)
        {
            isFollow = true;

            rb2d.AddForce(Vector2.right * direction, ForceMode2D.Impulse);

            float h = 1;

            if(target.transform.position.x > enemy.transform.position.x)
            {
                rb2d.velocity = new Vector2(speed, rb2d.velocity.y);
                h = 1;
            }
            else if(target.transform.position.x < enemy.transform.position.x)
            {
                rb2d.velocity = new Vector2(speed * (-1), rb2d.velocity.y);
                h = -1;
            }
            
            if ((h > 0 && direction < 0) || (h < 0 && direction > 0))
            {
                Flip();
            }
        }
        else
        {
            isFollow = false;
        }
    }


    
    private void Flip()
    {
        transform.eulerAngles = new Vector3(0, Mathf.Abs(transform.eulerAngles.y - 180), 0);
        direction *= -1;
    }





    private void Attack()
    {
        if(isAttack)
        {
            bc2d.enabled = true;

        }
        else
        {  
            bc2d.enabled = false;

        }
    }

    public void Dead()
    {
        this.gameObject.SetActive(false);
    }



    






    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == Tag.Player.ToString())
        {
            string state = other.gameObject.GetComponent<PlayerController>().state.ToString();
            
            if (state == "Attack")
            {
                Dead();
            }
            else
                other.gameObject.SendMessage("PlayerHit");
            


        }
    }

}
