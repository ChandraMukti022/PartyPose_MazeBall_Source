using UnityEngine;

public class UIMenuAnimator : MonoBehaviour
{
    [Header("Mode Animasi (Centang yang mau dipakai)")]
    public bool animasiScale = false;      // Untuk gede-kecil (Logo)
    public bool animasiNaikTurun = false;  // Untuk atas-bawah (Karakter)

    [Header("Pengaturan Scale (Membesar/Mengecil)")]
    [Tooltip("Kecepatan membesar/mengecil. Makin kecil angka, makin santai.")]
    public float scaleSpeed = 2f;
    [Tooltip("Seberapa besar perubahannya.")]
    public float scaleAmount = 0.05f;

    [Header("Pengaturan Naik Turun")]
    [Tooltip("Kecepatan naik/turun. Makin kecil angka, makin santai.")]
    public float floatSpeed = 2f;
    [Tooltip("Jarak naik turunnya dalam pixel.")]
    public float floatAmount = 15f; 

    private Vector3 startPos;
    private Vector3 startScale;

    void Start()
    {
        startPos = transform.localPosition;
        startScale = transform.localScale;
    }

    void Update()
    {
        // Animasi Gede Kecil (Breathing)
        if (animasiScale)
        {
            float s = Mathf.Sin(Time.time * scaleSpeed) * scaleAmount;
            transform.localScale = startScale + new Vector3(s, s, s);
        }

        // Animasi Atas Bawah (Floating)
        if (animasiNaikTurun)
        {
            float y = Mathf.Sin(Time.time * floatSpeed) * floatAmount;
            transform.localPosition = startPos + new Vector3(0, y, 0);
        }
    }
}