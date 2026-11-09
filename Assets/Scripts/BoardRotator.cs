using UnityEngine;
using UnityEngine.UI;

public class BoardRotator : MonoBehaviour
{
    [Header("Pengaturan Papan")]
    [Tooltip("Kecepatan rotasi saat tombol disentuh")]
    public float kecepatanRotasi = 35f; 
    
    [Header("Zona Tombol Udara")]
    [Tooltip("Batas X untuk tombol Kiri layar (Tangan di kiri tengah)")]
    public float batasZonaKiriX = 0.4f; 
    
    [Tooltip("Batas X untuk tombol Kanan layar (Tangan di kanan tengah)")]
    public float batasZonaKananX = 0.6f; 
    
    [Tooltip("Batas Y tangan (Tangan harus di ATAS perut. 0=Atas, 1=Bawah)")]
    public float zonaMaxY = 0.75f; 

    [Header("Visual Tombol UI")]
    public RectTransform leftButtonUI;
    public RectTransform rightButtonUI;

    private float currentTilt = 0f;
    private float logTimer = 0f; 
    
    void Update()
    {
        bool pencetKiri = false;
        bool pencetKanan = false;

        float keyboardInput = Input.GetAxis("Horizontal"); 
        
        if (Mathf.Abs(keyboardInput) > 0.1f)
        {
            if (keyboardInput < 0) pencetKiri = true;
            if (keyboardInput > 0) pencetKanan = true;
        }
        else if (PoseBridge.IsFresh(0)) 
        {
            Vector2 leftWrist = PoseBridge.Get(0, PoseBridge.LeftWrist);
            Vector2 rightWrist = PoseBridge.Get(0, PoseBridge.RightWrist);

            // Cek setiap tangan. Apakah ada di Zona Kiri (X < 0.4) ATAU Zona Kanan (X > 0.6)?
            // Kita cek kedua pergelangan tangan (kiri atau kanan asli pemain bebas)
            
            // Evaluasi Pergelangan Kiri Pemain
            if (leftWrist.y < zonaMaxY) 
            {
                if (leftWrist.x < batasZonaKiriX) pencetKiri = true;
                else if (leftWrist.x > batasZonaKananX) pencetKanan = true;
            }

            // Evaluasi Pergelangan Kanan Pemain
            if (rightWrist.y < zonaMaxY)
            {
                if (rightWrist.x < batasZonaKiriX) pencetKiri = true;
                else if (rightWrist.x > batasZonaKananX) pencetKanan = true;
            }

            // Jika tangan kirinya dan kanannya menyilang, kita biarkan saja (tombol ditekan)
        }

        // --- LOGIKA ROTASI BEBAS (TANPA BATAS MAKSIMAL) ---
        if (pencetKiri && !pencetKanan)
        {
            currentTilt += kecepatanRotasi * Time.deltaTime; // Miring ke Kiri
        }
        else if (pencetKanan && !pencetKiri)
        {
            currentTilt -= kecepatanRotasi * Time.deltaTime; // Miring ke Kanan
        }

        // Terapkan Rotasi (Bisa 360 derajat)
        transform.rotation = Quaternion.Euler(0f, 0f, currentTilt);

        // Animasi UI
        AnimateButton(leftButtonUI, pencetKiri);
        AnimateButton(rightButtonUI, pencetKanan);
    }

    void AnimateButton(RectTransform button, bool isPressed)
    {
        if (button != null)
        {
            float targetScale = isPressed ? 1.25f : 1.0f; 
            button.localScale = Vector3.Lerp(button.localScale, new Vector3(targetScale, targetScale, targetScale), Time.deltaTime * 12f);
        }
    }
}