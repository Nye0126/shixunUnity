using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
   
    public float maxHealth = 100f;
    private float currentHealth;
    public Animator anim;
    public Slider healthSlider;
    public bool isReported = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }


    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        // 限制血量不小于0
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // 【调试神器】：如果没掉血，控制台会告诉你
        Debug.Log($"{gameObject.name} 受到伤害，剩余血量: {currentHealth}");

        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth; 
        }
        anim.SetFloat("hp", currentHealth);
    }

    public void Die()
    {
       
        gameObject.tag = "DeadBody";
        gameObject.layer = LayerMask.NameToLayer("DeadBody");

        GetComponent<EnemyPatrol>().enabled = false;
        GetComponent<Newenemies>().enabled = false;

        
        if (healthSlider != null) healthSlider.gameObject.SetActive(false);

        //transform.Find("FOV_Renderer")?.gameObject.SetActive(false);

        
        GetComponent<SpriteRenderer>().color = Color.gray;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;       // 速度归零
            rb.angularVelocity = 0;          // 旋转速度归零
            rb.isKinematic = true;           
                                            
        }

        anim.SetTrigger("Die");

    }
}