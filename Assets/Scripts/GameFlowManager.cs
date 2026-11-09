using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class GameFlowManager : MonoBehaviour
{
    public enum StateAR { Off, Setup, Gameplay }

    [Header("--- PENGATURAN LEVEL ---")]
    public LevelData[] daftarLevelData;
    public GameObject[] daftarTutorial;
    
    // Menyimpan indeks level yang sedang berjalan.
    // Dibuat 'static' agar tidak mereset jadi 0 saat scene dimuat ulang (pindah level).
    private static int indeksLevelAktif = 0;

    [Header("--- PANEL UTAMA CANVAS ---")]
    public GameObject scanPanel;
    public GameObject tutorialPanel;
    public GameObject countdownPanel;
    public GameObject gameplayPanel;
    public GameObject retryPanel;
    public GameObject resultPanel;
    public GameObject gameOverPanel;
    public GameObject photoCountdownPanel;

    [Header("--- UI TEKS & STATUS ---")]
    public TMP_Text timerText;
    public TMP_Text tutorialCountdownText;
    public TMP_Text countdownText;
    public TMP_Text setupStatusText;
    public TMP_Text photoCountdownText;
    public TMP_Text resultTimeText;
    public TMP_Text notifikasiDownload;

    [Header("--- UI RESULT & FOTO ---")]
    public RawImage resultPhotoDisplay;
    public GameObject[] bintangKuningList;
    private Texture2D fotoTerakhir;

    [Header("--- AUDIO ---")]
    public AudioSource bgmSource;
    public AudioSource sfxManager;
    public AudioClip sfxJepretKamera;
    public AudioClip sfxMenang;
    public AudioClip sfxKalah;
    public AudioClip vo321;
    public AudioClip voGo;

    [Header("--- AR PLAYER ---")]
    public GameObject arSetupP1;
    public GameObject arGameplayP1;

    // Variabel Internal Gameplay
    private GameObject activeMiniGame;
    private bool isGameRunning = false;
    private float timeLeft;
    private float timeElapsed;

    void Start()
    {
        // 1. Matikan semua panel di awal permainan agar layar bersih
        MatikanSemuaPanel();
        
        foreach (GameObject bintang in bintangKuningList)
        {
            if (bintang != null) bintang.SetActive(false);
        }

        if (notifikasiDownload) notifikasiDownload.text = "";

        // 2. Matikan kamera AR sementara
        SetARMode(StateAR.Off);

        // 3. Mulai alur permainan
        StartCoroutine(StartGameFlow());
    }

    private void MatikanSemuaPanel()
    {
        if (scanPanel) scanPanel.SetActive(false);
        if (tutorialPanel) tutorialPanel.SetActive(false);
        if (countdownPanel) countdownPanel.SetActive(false);
        if (gameplayPanel) gameplayPanel.SetActive(false);
        if (retryPanel) retryPanel.SetActive(false);
        if (resultPanel) resultPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (photoCountdownPanel) photoCountdownPanel.SetActive(false);
    }

    private void SetARMode(StateAR mode)
    {
        if (arSetupP1) arSetupP1.SetActive(mode == StateAR.Setup);
        if (arGameplayP1) arGameplayP1.SetActive(mode == StateAR.Gameplay);
    }

    IEnumerator StartGameFlow()
    {
        // JIKA LEVEL 1 (Indeks 0): Lakukan Setup Kamera & Tutorial
        if (indeksLevelAktif == 0)
        {
            SetARMode(StateAR.Setup);
            if (scanPanel) scanPanel.SetActive(true);
            if (setupStatusText) setupStatusText.text = "Waiting P1...";

            // Tunggu pemain masuk frame kamera
            yield return new WaitUntil(() => PoseBridge.IsFresh(0));

            if (setupStatusText) setupStatusText.text = "READY!";
            yield return new WaitForSeconds(1.5f);
            if (scanPanel) scanPanel.SetActive(false);

            // Tampilkan Tutorial
            if (tutorialPanel)
            {
                tutorialPanel.SetActive(true);

                foreach (GameObject isiTutor in daftarTutorial)
                {
                    if (isiTutor != null) isiTutor.SetActive(false);
                }
                
                if (indeksLevelAktif >= 0 && indeksLevelAktif < daftarTutorial.Length)
                {
                    if (daftarTutorial[indeksLevelAktif] != null)
                        daftarTutorial[indeksLevelAktif].SetActive(true);
                }

                for (int i = 4; i > 0; i--)
                {
                    if (tutorialCountdownText) tutorialCountdownText.text = "GAME WILL START IN " + i + " SEC..";
                    yield return new WaitForSeconds(1f);
                }
                tutorialPanel.SetActive(false);
            }
        }
        else
        {
            // JIKA LEVEL 2 ATAU 3: Langsung siap main tanpa Setup/Tutorial lagi
            SetARMode(StateAR.Gameplay);
        }

        // --- HITUNG MUNDUR 3, 2, 1, GO ---
        if (countdownPanel) countdownPanel.SetActive(true);
        if (countdownText) countdownText.text = "3";
        if (sfxManager != null && vo321 != null) sfxManager.PlayOneShot(vo321);
        yield return new WaitForSeconds(1f);

        if (countdownText) countdownText.text = "2";
        yield return new WaitForSeconds(1f);

        if (countdownText) countdownText.text = "1";
        yield return new WaitForSeconds(1f);

        if (countdownText) countdownText.text = "GO!";
        if (sfxManager != null && voGo != null) sfxManager.PlayOneShot(voGo);
        yield return new WaitForSeconds(0.5f);

        if (countdownPanel) countdownPanel.SetActive(false);

        // --- MULAI GAMEPLAY ---
        if (bgmSource != null) bgmSource.Play();
        if (gameplayPanel) gameplayPanel.SetActive(true);

        LevelData levelAktif = null;
        if (indeksLevelAktif < daftarLevelData.Length)
        {
            levelAktif = daftarLevelData[indeksLevelAktif];
        }

        if (levelAktif != null && levelAktif.miniGamePrefab != null)
        {
            activeMiniGame = Instantiate(levelAktif.miniGamePrefab, gameplayPanel.transform);
            activeMiniGame.transform.localPosition = Vector3.zero;
            activeMiniGame.transform.localScale = levelAktif.miniGamePrefab.transform.localScale;
            timeLeft = levelAktif.batasWaktu;
        }
        else
        {
            timeLeft = 30f; // Default jika data kosong
        }

        timeElapsed = 0f;
        isGameRunning = true;
    }

    void Update()
    {
        if (isGameRunning)
        {
            timeLeft -= Time.deltaTime;
            timeElapsed += Time.deltaTime;

            if (timerText) timerText.text = Mathf.Max(0, Mathf.CeilToInt(timeLeft)).ToString() + "s";

            // Jika Waktu Habis -> Game Over (Kalah)
            if (timeLeft <= 0)
            {
                EndGame(false, 0);
            }
        }
    }

    public void EndGame(bool isWin, int totalBintang = 0)
    {
        if (!isGameRunning) return;
        isGameRunning = false;

        if (bgmSource != null) bgmSource.Stop();

        if (isWin)
        {
            StartCoroutine(WinSequence(totalBintang, timeElapsed));
        }
        else
        {
            if (sfxManager != null && sfxKalah != null) sfxManager.PlayOneShot(sfxKalah);
            if (gameplayPanel) gameplayPanel.SetActive(false);
            if (activeMiniGame) Destroy(activeMiniGame);
            
            SetARMode(StateAR.Off);
            if (retryPanel) retryPanel.SetActive(true);
        }
    }

    IEnumerator WinSequence(int totalBintang, float waktuTerselesaikan)
    {
        // 1. Hitung mundur foto
        if (photoCountdownPanel) photoCountdownPanel.SetActive(true);
        if (photoCountdownText) photoCountdownText.text = "3";
        yield return new WaitForSeconds(1f);
        if (photoCountdownText) photoCountdownText.text = "2";
        yield return new WaitForSeconds(1f);
        if (photoCountdownText) photoCountdownText.text = "1";
        yield return new WaitForSeconds(1f);

        if (photoCountdownText) photoCountdownText.text = "";
        if (gameplayPanel) gameplayPanel.SetActive(false);
        
        // 2. Jepret Kamera
        if (sfxManager != null && sfxJepretKamera != null) sfxManager.PlayOneShot(sfxJepretKamera);
        yield return new WaitForEndOfFrame();
        
        // Simpan screenshot ke memori internal game
        fotoTerakhir = ScreenCapture.CaptureScreenshotAsTexture();

        // 3. Tampilkan teks kemenangan
        if (photoCountdownText) photoCountdownText.text = "STAGE COMPLETE!";
        if (sfxManager != null && sfxMenang != null) sfxManager.PlayOneShot(sfxMenang);
        yield return new WaitForSeconds(1.5f);
        if (photoCountdownPanel) photoCountdownPanel.SetActive(false);

        // Bersihkan scene
        if (activeMiniGame) Destroy(activeMiniGame);
        SetARMode(StateAR.Off);

        // 4. Buka Panel Result
        if (resultPanel) resultPanel.SetActive(true);
        if (resultPhotoDisplay) resultPhotoDisplay.texture = fotoTerakhir;
        if (resultTimeText) resultTimeText.text = "Waktu Selesai: " + Mathf.CeilToInt(waktuTerselesaikan).ToString() + "s";
        if (notifikasiDownload) notifikasiDownload.text = "";

        // Tampilkan Bintang
        for (int i = 0; i < bintangKuningList.Length; i++)
        {
            if (bintangKuningList[i] != null) bintangKuningList[i].SetActive(i < totalBintang);
        }
    }

    // ==========================================
    // FUNGSI-FUNGSI TOMBOL (ON CLICK)
    // ==========================================

    public void DownloadFoto()
    {
        if (fotoTerakhir != null)
        {
            byte[] dataGambar = fotoTerakhir.EncodeToPNG();
            string waktu = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string namaFile = "GAMAJAM_" + waktu + ".png";
            
            // 1. Cari lokasi folder game ini berada (bersebelahan dengan file .exe nantinya)
            string folderGame = System.IO.Directory.GetParent(Application.dataPath).FullName;
            
            // 2. Buat nama folder khusus untuk menyimpan foto
            string folderSS = System.IO.Path.Combine(folderGame, "Foto SS Game");

            // 3. Cek apakah foldernya sudah ada? Kalau belum, buatkan otomatis!
            if (!System.IO.Directory.Exists(folderSS))
            {
                System.IO.Directory.CreateDirectory(folderSS);
            }

            // 4. Gabungkan nama folder dan nama file fotonya
            string jalurLengkap = System.IO.Path.Combine(folderSS, namaFile);

            // 5. Simpan file
            File.WriteAllBytes(jalurLengkap, dataGambar);
            Debug.Log("Foto tersimpan di: " + jalurLengkap);

            if (notifikasiDownload != null)
            {
                notifikasiDownload.text = "Tersimpan di folder 'Foto SS Game'!";
            }
        }
    }

    public void TombolLanjut()
    {
        indeksLevelAktif++;

        // Cek apakah pemain baru saja menyelesaikan level terakhir
        if (indeksLevelAktif >= daftarLevelData.Length)
        {
            if (resultPanel) resultPanel.SetActive(false);
            if (gameOverPanel) gameOverPanel.SetActive(true);
            return;
        }

        // Muat ulang scene untuk lanjut ke level berikutnya (tanpa mereset indeks level)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TombolUlangi()
    {
        // Ulang level yang sama saat kalah
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TombolMainLagiDariAwal()
    {
        indeksLevelAktif = 0; // Reset ke level pertama
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TombolKeMainMenu()
    {
        indeksLevelAktif = 0; // Reset agar saat main lagi mulai dari level 1
        
        
        SceneManager.LoadScene("MainMenu");
    }
}