using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Pengaturan Scene")]
    [Tooltip("Ketik nama scene game utamamu persis seperti di file")]
    public string sceneMekanikName = "MainMekanik"; 

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip clickSound;

    [Header("Efek Transisi")]
    [Tooltip("Masukkan panel hitam untuk efek fade ke sini")]
    public CanvasGroup fadePanel;
    public float fadeDuration = 1f; // Lama waktu layar menggelap

    void Start()
    {
        // Bonus: Saat Main Menu baru dibuka, layar dari hitam akan perlahan terang
        if (fadePanel != null)
        {
            fadePanel.alpha = 1f; // Mulai dari hitam pekat
            fadePanel.blocksRaycasts = true; // Blokir klik sementara
            StartCoroutine(FadeIn());
        }
    }

    public void MulaiGame()
    {
        // Putar suara klik
        if (sfxSource != null && clickSound != null)
        {
            sfxSource.PlayOneShot(clickSound);
        }

        // Mulai proses layar menggelap lalu pindah scene
        if (fadePanel != null)
        {
            StartCoroutine(FadeOutAndLoad());
        }
        else
        {
            // Fallback kalau lupa masukin panel
            Invoke("PindahScene", 0.3f);
        }
    }

    IEnumerator FadeIn()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration); // Hitam ke transparan
            yield return null;
        }
        fadePanel.alpha = 0f;
        fadePanel.blocksRaycasts = false; // Buka blokir klik agar pemain bisa tekan tombol Play
    }

    IEnumerator FadeOutAndLoad()
    {
        // Blokir klik agar pemain tidak spam tombol Play berkali-kali
        fadePanel.blocksRaycasts = true; 

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration); // Transparan ke hitam
            yield return null;
        }
        fadePanel.alpha = 1f;
        
        // Tunggu sedikit setelah layar gelap gulita biar lebih sinematik
        yield return new WaitForSeconds(0.2f);
        
        PindahScene();
    }

    void PindahScene()
    {
        SceneManager.LoadScene(sceneMekanikName);
    }
}