using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Memutar urutan gambar (frame) pada UI Image, pengganti GIF.
/// Isi 'frames' dengan hasil pecahan GIF. Kalau 'frames' kosong, Image disembunyikan
/// (panel tetap berjalan dengan teks saja).
/// </summary>
[RequireComponent(typeof(Image))]
public class UISpriteAnimator : MonoBehaviour
{
    public Sprite[] frames;
    public float fps = 12f;
    public bool loop = true;

    Image _img;
    float _t;

    void Awake()
    {
        _img = GetComponent<Image>();
        _img.preserveAspect = true;
    }

    void OnEnable()
    {
        if (_img == null) _img = GetComponent<Image>();
        _t = 0f;
        bool has = frames != null && frames.Length > 0;
        _img.enabled = has;
        if (has) _img.sprite = frames[0];
    }

    void Update()
    {
        if (frames == null || frames.Length == 0) return;
        _t += Time.unscaledDeltaTime;
        int i = (int)(_t * fps);
        i = loop ? i % frames.Length : Mathf.Min(i, frames.Length - 1);
        _img.sprite = frames[i];
    }
}
