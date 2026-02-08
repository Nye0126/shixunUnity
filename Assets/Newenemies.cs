using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Newenemies : MonoBehaviour
{
   
    public float viewRadius = 5f;       // 视野距离
   
    public float viewAngle = 90f;      // 视野角度
    public Animator anim;
  
    public LayerMask playerMask;       // 玩家图层
    public LayerMask obstacleMask;     // 墙壁/障碍物图层（非常重要！请确保只选Obstacle层）

    
    public bool canSeePlayer;          // 是否看到玩家
    public GameObject alertIcon;       // 提示图标
    public float iconDuration = 1.0f;

   
    public NewEnemiesGroup myGroup;
    private bool lastCanSeePlayer;
    private bool alertedByGroup = false;

    // 综合判定：自己看到或小组通报
    public bool IsAlerted => canSeePlayer || alertedByGroup || (myGroup != null && myGroup.isGroupAlerted);

    private bool isGroupAlerted => myGroup != null && myGroup.isGroupAlerted;

    void Start()
    {
        if (myGroup == null)
        {
            myGroup = FindObjectOfType<NewEnemiesGroup>();
        }
    }

    void Update()
    {
        anim.SetBool("jingjie", canSeePlayer);
        // 增加一层保护：如果本身就是尸体，就不再执行任何逻辑
        // 虽然在 Die() 里禁用脚本更彻底，但这里加一层更稳健
        if (gameObject.CompareTag("DeadBody")) return;

        FieldOfViewCheck();
        HandleAlert();

        // 建议把尸体检测频率降低，不需要每帧都找尸体
        // 这符合你对性能优化的研究兴趣
        DetectDeadBodies();
    }

    private void FieldOfViewCheck()
    {
        // 1. 玩家侦测逻辑
        Collider2D rangeCheck = Physics2D.OverlapCircle(transform.position, viewRadius, playerMask);

        if (rangeCheck != null)
        {
            Transform target = rangeCheck.transform;
            Vector2 directionToTarget = (target.position - transform.position).normalized;

            if (Vector2.Angle(transform.up, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector2.Distance(transform.position, target.position);

                // 核心修复点：射线检测墙壁。如果撞到了障碍物，说明被挡住了
                // 注意：obstacleMask 绝对不能包含 Player 层，否则射线会撞到玩家导致判定失败
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask);

                if (hit.collider == null)
                {
                    canSeePlayer = true; // 没撞到墙，看到玩家
                }
                else
                {
                    canSeePlayer = false; // 撞到墙了
                }
            }
            else
            {
                canSeePlayer = false;
            }
        }
        else
        {
            canSeePlayer = false;
        }

     
    }

    //private void DetectBodies()
    //{
    //    Collider2D[] rangeChecks = Physics2D.OverlapCircleAll(transform.position, viewRadius);
    //    foreach (var check in rangeChecks)
    //    {
    //        if (check.CompareTag("DeadBody"))
    //        {
    //            Vector2 dirToTarget = (check.transform.position - transform.position).normalized;
    //            if (Vector2.Angle(transform.up, dirToTarget) < viewAngle / 2)
    //            {
    //                float dist = Vector2.Distance(transform.position, check.transform.position);
    //                // 统一使用 obstacleMask
    //                if (!Physics2D.Raycast(transform.position, dirToTarget, dist, obstacleMask))
    //                {
    //                    if (!isGroupAlerted) OnSeeBody();
    //                }
    //            }
    //        }
    //    }
    //}

    private void HandleAlert()
    {
        if (canSeePlayer && !lastCanSeePlayer)
        {
            if (myGroup != null) myGroup.ReportDetection();
            StartCoroutine(ShowAlertIcon());
        }
        lastCanSeePlayer = canSeePlayer;
    }

    // 绘制视野线，方便调试
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 viewAngle01 = DirectionFromAngle(-viewAngle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(viewAngle / 2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + viewAngle01 * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngle02 * viewRadius);

        if (canSeePlayer)
        {
            Gizmos.color = Color.red; // 看到时连线变红
            Collider2D p = Physics2D.OverlapCircle(transform.position, viewRadius, playerMask);
            if (p != null) Gizmos.DrawLine(transform.position, p.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float angleInDegrees)
    {
        angleInDegrees += transform.eulerAngles.z;
        return new Vector3(-Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    IEnumerator ShowAlertIcon()
    {
        Debug.Log("触发警报中...");
        if (alertIcon != null)
        {
            alertIcon.SetActive(true);
            Debug.Log("icon true...");
            yield return new WaitForSeconds(iconDuration);
           
        }
    }

    public void OnGroupAlert()
    {
        if (canSeePlayer) return;
        alertedByGroup = true;
        if (alertIcon != null)
        {
            StopAllCoroutines();
            StartCoroutine(ShowAlertIcon());
        }
    }

    public void ResetAlertStatus()
    {
        alertedByGroup = false;
    }

    private void OnSeeBody()
    {
        if (!isGroupAlerted)
        {
            if (myGroup != null) myGroup.ReportDetection();
          
        }
    }
    private void DetectDeadBodies()
    {
        // 【补充 1：状态拦截】如果已经处于全组警报，为了性能直接返回，不再每帧发射线
        if (isGroupAlerted) return;

        // 1. 范围搜索：找出周围所有的碰撞体
        Collider2D[] rangeChecks = Physics2D.OverlapCircleAll(transform.position, viewRadius);

        foreach (var check in rangeChecks)
        {
            // 2. 身份确认：不是我自己，且标签必须是 DeadBody
            if (check.gameObject != gameObject && check.CompareTag("DeadBody"))
            {
                EnemyHealth corpse = check.GetComponent<EnemyHealth>();

                // 【补充 2：唯一性检测】只处理还没被报过警的尸体，实现“只检测一次”
                if (corpse != null && !corpse.isReported)
                {
                    Vector2 dir = (check.transform.position - transform.position).normalized;

                    // 【补充 3：视野角度限制】确保敌人不会“背后长眼”看到尸体
                    // 只有在视野角度 (viewAngle) 范围内才继续探测
                    if (Vector2.Angle(transform.up, dir) < viewAngle / 2)
                    {
                        float dist = Vector2.Distance(transform.position, check.transform.position);

                        // 4. 遮挡检查：只看你定义的 obstacleMask
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, obstacleMask);

                        if (hit.collider == null)
                        {
                            // 发现尸体后的逻辑链
                            corpse.isReported = true; // 锁定状态
                            OnGroupAlert();

                            // 视觉反馈联动
                            if (alertIcon != null) StartCoroutine(ShowAlertIcon());

                            Debug.Log(gameObject.name + " 发现了同伴的尸体！");
                            break; // 发现一个就够了，跳出循环节省性能
                        }
                    }
                }
            }
        }
    }
}