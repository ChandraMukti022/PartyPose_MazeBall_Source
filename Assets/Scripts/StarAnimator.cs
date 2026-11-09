using UnityEngine;

public class StarAnimator : MonoBehaviour
{
    [Header("Pengaturan Denyut")]
    public float kecepatan = 4f; 
    public float ukuranMaksimal = 0.2f; 
    
    private Vector3 ukuranAwal;

    void Start()
    {
        ukuranAwal = transform.localScale;
    }

    void Update()
    {
        // Membuat bintang berdenyut (membesar-mengecil) secara otomatis
        float denyut = Mathf.Sin(Time.time * kecepatan) * ukuranMaksimal;
        transform.localScale = ukuranAwal + new Vector3(denyut, denyut, 0f);
    }
}