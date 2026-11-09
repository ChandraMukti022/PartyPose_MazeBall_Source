using UnityEngine;
using UnityEngine.EventSystems; // Wajib untuk mendeteksi sentuhan jari/mouse

public class EfekKlikTombol : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 ukuranAwal;
    
    [Tooltip("Seberapa kecil tombol saat ditekan. 0.9 berarti ukurannya jadi 90%")]
    public float skalaMengecil = 0.9f;

    void Start()
    {
        // Simpan ukuran asli tombol saat game mulai
        ukuranAwal = transform.localScale;
    }

    // Fungsi ini otomatis terpanggil saat tombol DITEKAN (jari/mouse turun)
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = ukuranAwal * skalaMengecil;
    }

    // Fungsi ini otomatis terpanggil saat jari/mouse DILEPAS
    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = ukuranAwal;
    }
}