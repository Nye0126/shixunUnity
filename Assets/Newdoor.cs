using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Newdoor : MonoBehaviour
{
    [SerializeField] private BoxCollider2D solidCollider;
    private bool isOpen = false;
    private Transform playerTransform; // 用来储存玩家的位置信息

  
    public float openAngle = 90f;
    public float rotationSpeed = 2f;

   
    public float closeDistance = 5f; // 设定的关门距离

    void Start()
    {
        // 自动通过标签找到玩家，省去手动拖拽的麻烦
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        // 如果门是开着的，且玩家已经走远了
        if (isOpen && playerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);

            if (distance > closeDistance)
            {
                CloseDoor();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        isOpen = true;
      
        if (solidCollider != null) solidCollider.enabled = false;
        StopAllCoroutines(); // 停止当前的旋转（防止开关门冲突）
        StartCoroutine(RotateDoor(openAngle)); // 旋转到开门角度
    }

    void CloseDoor()
    {
        isOpen = false;
        
        // 注意：关门时要在旋转完成后再开启碰撞，或者立即开启
        if (solidCollider != null) solidCollider.enabled = true;
        StopAllCoroutines();
        StartCoroutine(RotateDoor(0)); // 旋转回 0 度（初始位置）
    }

    // 通用的旋转协程，可以同时处理开门和关门
    IEnumerator RotateDoor(float targetZAngle)
    {
        Quaternion startRotation = transform.rotation;
        // 计算目标四元数（基于初始状态旋转 targetZAngle 度）
        // 如果你的门初始不是0度，这里可能需要根据初始角度计算
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetZAngle);

        float timeElapsed = 0f;
        float duration = 1f / rotationSpeed;

        while (timeElapsed < duration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;
    }
}