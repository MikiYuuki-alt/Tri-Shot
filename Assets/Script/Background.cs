using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    public float scrollSpeed = 2f;  // スクロール速度
    public float imageHeight = 10f; // 背景画像の高さ

    private void Update()
    {
        // 下方向に移動
        transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);

        // 一定の位置まで下がったら上に戻す
        if (transform.position.y <= -imageHeight)
        {
            transform.position += new Vector3(0, imageHeight * 2f, 0);
        }
    }
}
