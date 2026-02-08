using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Newcircle : MonoBehaviour //define 玩家操控物体
{
    public float moveSpeed = 2f;
    public float rotationSpeed = 700f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    public GameObject bullet;
    public Animator anim;

    // 新增：玩家血量相关变量
    public float maxHp = 100f; // 最大血量
    private float currentHp;   // 当前血量

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 初始化血量为最大值
        currentHp = maxHp;
        Debug.Log("玩家初始血量: " + currentHp);
    }

    // Update is called once per frame
    void Update()
    {
        if (rb != null)//wasd input
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            anim.SetFloat("horizontal", Mathf.Abs(moveX));
            anim.SetFloat("vertical", Mathf.Abs(moveY));
            anim.SetFloat("hp", Mathf.Abs(currentHp));
            moveInput = new Vector2(moveX, moveY).normalized;
            SmoothRotate();
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }
    }

    void Shoot()
    {
        // 计算头部偏移量
        Vector3 spawnPosition = transform.position + transform.up * 0.5f;

        GameObject b = Instantiate(bullet, spawnPosition, transform.rotation);

        Newbullet script = b.GetComponent<Newbullet>();
        if (script != null)
        {
            script.owner = "player";
        }

        Debug.Log("player shoot toward: " + transform.up);
    }

    void Moveplayer()
    {
        rb.velocity = moveInput * moveSpeed;
    }

    void FixedUpdate()
    {
        Moveplayer();
        SmoothRotate();//add smooth ratation
    }

    void SmoothRotate()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = (Vector2)mousePosition - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        float smoothedAngle = Mathf.MoveTowardsAngle(rb.rotation, angle, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(smoothedAngle);
    }

    // 新增：处理碰撞检测
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 检查碰撞对象是否是子弹（根据子弹预制体的标签或名称判断）
        // 建议给子弹预制体添加 "Bullet" 标签，这样判断更准确
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // 获取子弹的owner，避免玩家被自己的子弹击中
            Newbullet bulletScript = collision.gameObject.GetComponent<Newbullet>();
            if (bulletScript != null && bulletScript.owner != "player")
            {
                TakeDamage(50f); // 受到50点伤害
                // 销毁击中玩家的子弹
                Destroy(collision.gameObject);
            }
        }
    }

    // 新增：扣血方法
    public void TakeDamage(float damageAmount)
    {
        // 扣血
        currentHp -= damageAmount;
        // 确保血量不会低于0
        currentHp = Mathf.Max(currentHp, 0);

        Debug.Log("玩家受到 " + damageAmount + " 点伤害，剩余血量: " + currentHp);

        // 血量为0时的处理（可根据需求扩展，比如死亡动画、游戏结束等）
        if (currentHp <= 0)
        {
            Debug.Log("玩家死亡！");
            // 这里可以添加死亡逻辑，比如销毁玩家、触发游戏结束等
            // Destroy(gameObject);
        }
    }

    // 可选：获取当前血量的方法（方便UI显示等）
    public float GetCurrentHp()
    {
        return currentHp;
    }
}