using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemiesGroup : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Newenemies> members = new List<Newenemies>();

    // 记录整个小组是否处于警报状态
    public bool isGroupAlerted { get; private set; }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isGroupAlerted)
        {
            CheckGroupStatus();
        }
    }
    public void ReportDetection()
    {
        if (isGroupAlerted) return;

        isGroupAlerted = true;

        // --- 修改点 1：增加空引用检查 ---
        // 如果你在游戏中打死了一个敌人，enemy 就会变成 null。
        // 如果不检查 null，执行 OnGroupAlert() 时会直接报错崩溃。
        foreach (var enemy in members)
        {
            if (enemy != null)
            {
                enemy.OnGroupAlert();
            }
        }
        Debug.Log("execute");
    }

    
    private void CheckGroupStatus()
    {
        bool anyoneSeesPlayer = false;

        // 遍历所有成员，看看有没有人还盯着玩家
        foreach (var enemy in members)
        {
            if (enemy != null && enemy.canSeePlayer)
            {
                anyoneSeesPlayer = true;
                break; // 只要有一个人看见，警报就不能解除
            }
        }

        // 如果所有人都看不见玩家了
        if (!anyoneSeesPlayer)
        {
            ResetGroupAlert();
        }
    }
    private void ResetGroupAlert()
    {
        isGroupAlerted = false;
        foreach (var enemy in members)
        {
            if (enemy != null) enemy.ResetAlertStatus(); // 通知成员重置状态
        }
    }

}

