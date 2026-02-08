using System.Collections;
using System.Collections.Generic;
using TMPro; // TextMeshPro命名空间
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class Newcircle : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotationSpeed = 700f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    public GameObject bullet;
    public Animator anim;
    public GameObject deathPicture;
    public Image screenFader;

    // 玩家血量相关变量
    public float maxHp = 100f;
    private float currentHp;

    // TextMeshPro文本组件
    public TMP_Text hpText;
    public GameObject gameOverUI; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHp = maxHp;

        // 可选：手动赋值（如果拖拽仍有问题，取消下面注释）
        // GameObject hpTextObj = GameObject.Find("HpText");
        // if (hpTextObj != null)
        // {
        //     hpText = hpTextObj.GetComponent<TMP_Text>();
        // }
        // else
        // {
        //     Debug.LogError("UI text object named HpText not found!");
        // }

        // 初始化血量显示
        UpdateHpUI();
        Debug.Log("Player initial HP: " + currentHp); // 中文→英文
    }

    void Update()
    {
        if (rb != null)
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
        SmoothRotate();
    }

    void SmoothRotate()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = (Vector2)mousePosition - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg-90f;
        float smoothedAngle = Mathf.MoveTowardsAngle(rb.rotation, angle, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(smoothedAngle);
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            TakeDamage(10f); 
            Destroy(collision.gameObject);
            return;
        }

       
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Newbullet bulletScript = collision.gameObject.GetComponent<Newbullet>();
            if (bulletScript != null && bulletScript.owner != "player")
            {
                TakeDamage(50f);
                Destroy(collision.gameObject);
            }
        }
    }

    // 扣血方法
    public void TakeDamage(float damageAmount)
    {
        currentHp -= damageAmount;
        currentHp = Mathf.Max(currentHp, 0);

        // 更新血量UI显示
        UpdateHpUI();

        Debug.Log("Player took " + damageAmount + " damage, remaining HP: " + currentHp); 

        if (currentHp <= 0)
        {
            Debug.Log("Player died!");
            // Destroy(gameObject);
            //加入 死亡ui
            Die();
        }
    }

    
    private void UpdateHpUI()
    {
        if (hpText != null)
        {
           
            hpText.text = $"HP: {currentHp}/{maxHp}";
        }
    }

    public float GetCurrentHp()
    {
        return currentHp;
    }
    void Die()
    {

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
            StartCoroutine(DeathSequence());
           
        }
    }
    IEnumerator DeathSequence()
    {
        // 等待一小会儿，让玩家先看清楚角色倒地的动画
        yield return new WaitForSeconds(1.0f);

        // --- 画面渐黑动画 ---
        float duration = 1.5f; // 变黑过程持续 1.5 秒
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            // 让遮罩的透明度从 0 渐变到 0.85 (85% 黑)
            if (screenFader != null)
            {
                float alpha = Mathf.Lerp(0, 0.85f, timer / duration);
                screenFader.color = new Color(0, 0, 0, alpha);
            }
            yield return null; // 等待下一帧
        }

        // --- 弹出 UI ---
        // 画面变黑后，同时显示文字和图片
        if (gameOverUI != null) gameOverUI.SetActive(true);
        if (deathPicture != null) deathPicture.SetActive(true);

        // --- 等待并退出 ---
        // 让玩家盯着悲伤的画面看 3 秒
        yield return new WaitForSeconds(3.0f);

        Debug.Log("Quitting Game...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
