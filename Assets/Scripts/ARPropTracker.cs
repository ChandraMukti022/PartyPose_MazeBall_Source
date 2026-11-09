using UnityEngine;
using UnityEngine.UI;

public class ARPropTracker : MonoBehaviour
{
    public enum BagianTubuh { Hidung, TanganKiri, TanganKanan }

    [Header("Pengaturan Pelacakan")]
    public int playerIndex = 0;
    public BagianTubuh targetLacak;
    public float smoothSpeed = 15f;

    [Header("Pengaturan Posisi & Layar")]
    public bool kameraMirrored = true;
    public Vector2 offsetPosisi;
    
    [Tooltip("Waktu toleransi (detik) sebelum gambar hilang saat kamera nge-lag")]
    public float waktuHilang = 0.5f;

    private RectTransform rectTransform;
    private RectTransform canvasRect;
    private Graphic komponenVisual; 
    
    // Timer untuk mengatasi frame kosong
    private float waktuDeteksiTerakhir = 0f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        komponenVisual = GetComponent<Graphic>();
        
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null) canvasRect = canvas.GetComponent<RectTransform>();
        
        if (komponenVisual != null) komponenVisual.enabled = false;
    }

    void Update()
    {
        // 1. Jika ada sinyal pergerakan BARU dari AI
        if (PoseBridge.IsFresh(playerIndex))
        {
            waktuDeteksiTerakhir = Time.time; // Reset timer ke waktu sekarang
            
            // Nyalakan gambar jika sedang mati
            if (komponenVisual != null && !komponenVisual.enabled)
                komponenVisual.enabled = true;

            // Jalankan logika posisi
            Vector2 landmarkPos = Vector2.zero;
            bool landmarkDitemukan = false;

            switch (targetLacak)
            {
                case BagianTubuh.Hidung:
                    landmarkPos = PoseBridge.Get(playerIndex, 0); 
                    landmarkDitemukan = true; break;
                case BagianTubuh.TanganKiri:
                    landmarkPos = PoseBridge.Get(playerIndex, PoseBridge.LeftWrist);
                    landmarkDitemukan = true; break;
                case BagianTubuh.TanganKanan:
                    landmarkPos = PoseBridge.Get(playerIndex, PoseBridge.RightWrist);
                    landmarkDitemukan = true; break;
            }

            if (landmarkDitemukan && canvasRect != null)
            {
                float normalizedX = kameraMirrored ? (1f - landmarkPos.x) : landmarkPos.x;
                float normalizedY = landmarkPos.y; 

                float uiX = (normalizedX * canvasRect.rect.width) - (canvasRect.rect.width * 0.5f);
                float uiY = -((normalizedY * canvasRect.rect.height) - (canvasRect.rect.height * 0.5f));

                Vector2 targetPos = new Vector2(uiX, uiY) + offsetPosisi;
                rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPos, Time.deltaTime * smoothSpeed);
            }
        }
        else
        {
            // 2. Jika TIDAK ada sinyal (lag/kamera mati), cek timer
            // Gambar hanya dimatikan jika sudah lewat dari waktu batas toleransi (0.5 detik)
            if (Time.time - waktuDeteksiTerakhir > waktuHilang)
            {
                if (komponenVisual != null && komponenVisual.enabled)
                    komponenVisual.enabled = false;
            }
        }
    }
}