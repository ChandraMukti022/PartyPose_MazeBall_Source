using UnityEngine;

public class BallCollector : MonoBehaviour
{
    public int jumlahBintang = 0;

    [Header("Pengaturan Suara")]
    public AudioSource sfxSource;      // Tarik komponen Audio Source milik bola ke sini
    public AudioClip sfxAmbilBintang;  // Masukkan file MP3/WAV suara "Ting!" bintang ke sini

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bintang"))
        {
            jumlahBintang++; 
            
            // Mainkan suara tanpa memotong suara bintang sebelumnya
            if (sfxSource != null && sfxAmbilBintang != null)
            {
                sfxSource.PlayOneShot(sfxAmbilBintang);
            }
            
            Destroy(collision.gameObject); 
        }
    }
}