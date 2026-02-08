using UnityEngine;
using UnityEngine.UI; // 必须引入
using System.Collections;

public class FinishPoint : MonoBehaviour
{
    public GameObject finishUI;
    public GameObject endPicture;
    public Image screenFader; // 拖入刚才创建的黑色 Image

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(EndSequence());
        }
    }

    IEnumerator EndSequence()
    {
        
        FindObjectOfType<Newcircle>().enabled = false;

        //平滑变暗
        float duration = 1.0f; // 变暗持续 1 秒
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 0.8f, timer / duration); // 从透明到 80% 黑
            screenFader.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // 画面变暗后，弹出文字和图片
        if (finishUI != null) finishUI.SetActive(true);
        if (endPicture != null) endPicture.SetActive(true);

       
        yield return new WaitForSeconds(5.0f);
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}