using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    
    // 在 Inspector 面板中，把你的双开门父物体拖到这里
    public SlidingDoorController doorController;

```c#

public bool isCleaningRoomMode = false;


// 对应策划案：玩家进入后一小段时间后完全关闭
public float closeDelay = 1.5f; 
// 对应策划案：清洗间短暂关闭的时间
public float cleaningDuration = 2.0f; 

private bool hasTriggered = false;

private void OnTriggerEnter2D(Collider2D other)
{
   
    if (other.CompareTag("Player") && !hasTriggered)
    {
        if (doorController == null)
        {
            Debug.LogError("触发器未关联 DoorController！");
            return;
        }

        hasTriggered = true;

        // 2. 根据策划案需求选择逻辑分支
        if (isCleaningRoomMode)
        {
            // 执行特殊点②逻辑：清洗间效果
            doorController.StartCleaningCycle(cleaningDuration);
            Debug.Log("特殊点②：清洗间循环启动");
        }
        else
        {
            // 执行特殊点①逻辑：延迟关闭大门
            doorController.CloseWithDelay(closeDelay);
            Debug.Log("特殊点①：大门将在 " + closeDelay + " 秒后关闭");
            
            // 这里可以预留 UI 提示接口，告知玩家“另寻他路”
        }

        // 3. 触发后禁用碰撞体，防止逻辑冲突
        GetComponent<Collider2D>().enabled = false;
    }
}
```
}