using UnityEngine;

public class BossShot : MonoBehaviour
{
    private Vector3 m_velocity;
    //ボス弾1,2
    
    public void Init(float angleDeg, float speed)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
        m_velocity = direction * speed;

        var rot = transform.localEulerAngles;
        rot.z = angleDeg - 90 + 180f;
        transform.localEulerAngles = rot;

        Destroy(gameObject, 4f);
    }

    void Update()
    {
        

        transform.position += m_velocity * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Hit Circle")
        {
            Debug.Log("プレイヤーに命中！");
            HitCounter counter = FindObjectOfType<HitCounter>();
            if (counter != null)
            {
                counter.AddHit();
            }
           

            Destroy(gameObject);
        }
    }

    
}
