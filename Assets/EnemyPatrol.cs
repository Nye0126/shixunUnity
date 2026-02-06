using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    //zhuizongwanjia
    public Transform[] waypoints;      // 巡逻点
    public float patrolSpeed = 2f;    // 巡逻速度
    public float chaseSpeed = 4f;     // 追击速度（通常比巡逻快）
    public float waitTime = 1f;       // 到达路点的停留时间
    public float stoppingDistance = 1.2f; // 距离玩家多远时停止（防止重叠）

    //sheji
    public GameObject bulletPrefab;   // 拖入你的子弹预制体
    public Newbullet bullet;  //子弹预制体脚本
    public Transform firePoint;      // 在敌人前方创建一个空物体作为开火点
    public float fireRate = 1f;      // 每秒开几枪
    public float attackDistance = 2f; // 射击触发距离
    private float nextFireTime;
    

    private Newenemies fov;           // 引用视野脚本
    private Transform playerTransform; // 玩家的坐标

    private int currentWaypointIndex = 0;
    private float waitCounter;
    private bool isWaiting;

    void Start()
    {
        fov = GetComponent<Newenemies>();
        // 自动寻找带有 Player 标签的对象
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        if (waypoints.Length > 0) transform.position = waypoints[0].position;
    }

    void Update()
    {
        // 判断是否处于警报状态且玩家存在
        if (fov != null && fov.IsAlerted && playerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);

            // 始终看向玩家：无论动不动，都要保证视野和枪口对准目标
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            RotateTowards(direction);

            // 2. 移动控制：只有距离大于 4 时才追击
            if (distance >= 4f)
            {
                // 向玩家移动
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, chaseSpeed * Time.deltaTime);
            }
            // 当距离 < 4 时，这里什么都不做，敌人就会停在原地

            // 3. 射击逻辑
            // 注意：如果你希望他在停止追击时就能开火，记得把 Inspector 里的 Attack Distance 改为 4 或更高
            if (distance < attackDistance && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
        else
        {
            // 没发现玩家时，继续日常巡逻
            PatrolLogic();
        }
    }

    void ChasePlayer()
    {
        isWaiting = false; // 追击时不再等待

        // 计算方向
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        //只有距离大于停止距离时才移动
        if (distance > stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, chaseSpeed * Time.deltaTime);
        }

        // 始终看向玩家（确保视野 Mesh 跟着转）
        RotateTowards(direction);
    }

    void PatrolLogic()  //巡逻机制
    {
        if (waypoints.Length < 2) return;

        if (isWaiting)
        {
            waitCounter -= Time.deltaTime;
            if (waitCounter <= 0) isWaiting = false;
            return;
        }

        Transform target = waypoints[currentWaypointIndex];
        Vector2 direction = (target.position - transform.position).normalized;

        transform.position = Vector2.MoveTowards(transform.position, target.position, patrolSpeed * Time.deltaTime);
        RotateTowards(direction);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            isWaiting = true;
            waitCounter = waitTime;
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    void RotateTowards(Vector2 direction)  //旋转方向
    {
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }
    void Shoot()  //开枪逻辑
    { 
        if (bulletPrefab != null && firePoint != null && playerTransform != null)
        {
           
            Vector2 directionToPlayer = (playerTransform.position - firePoint.position).normalized;

            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

          
            Quaternion bulletRotation = Quaternion.Euler(0, 0, angle );

            Instantiate(bulletPrefab, firePoint.position, bulletRotation);

        
        }
    }
}