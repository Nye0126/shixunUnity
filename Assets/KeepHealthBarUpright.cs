using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepHealthBarUpright : MonoBehaviour
{
    private Quaternion initialRotation;

    void Start()
    {
        // 记录游戏开始时的旋转（通常是 0 度）
        initialRotation = Quaternion.identity;
    }

    void LateUpdate()
    {
        // 强制让画布的旋转始终保持固定，不随父物体旋转
        transform.rotation = initialRotation;
    }
}
