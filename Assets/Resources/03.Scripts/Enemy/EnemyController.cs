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
    }


    [Header("Enemy")]

    public GameObject enemy = null;
    [HideInInspector] public Rigidbody2D rb2d = null;



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

    [SerializeField] GameObject target = null;

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
    }



    private void Move()
    {
        if(moveMin != Vector2.zero && moveMin != Vector2.zero)
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
                Debug.Log("max");
                Flip();
            }
            else if(nowPos.x < moveMin.x && direction < 0)
            {
                Debug.Log("min");
                Flip();
            }
        }
    }


    private void FollowTarget()
    {
        if(target != null)
        {

        }
    }


    
    private void Flip()
    {
        transform.eulerAngles = new Vector3(0, Mathf.Abs(transform.eulerAngles.y - 180), 0);
        direction *= -1;
    }





    private void Attack()
    {

    }



    






    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == Tag.Player.ToString())
        {
            other.gameObject.SendMessage("PlayerHit");
        }
    }

}
