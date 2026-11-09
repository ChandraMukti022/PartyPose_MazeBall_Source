using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Pastikan yang menyentuh lubang adalah Bola
        if (collision.gameObject.name == "Ball")
        {
            int bintangDidapat = 0;
            
            // Ambil jumlah bintang dari script BallCollector di bola
            BallCollector kolektor = collision.gameObject.GetComponent<BallCollector>();
            if (kolektor != null)
            {
                bintangDidapat = kolektor.jumlahBintang;
            }

            // Cari GameFlowManager di layar dan kirim sinyal menang beserta jumlah bintang
            GameFlowManager flowManager = FindObjectOfType<GameFlowManager>();
            if (flowManager != null)
            {
                flowManager.EndGame(true, bintangDidapat);
            }
        }
    }
}