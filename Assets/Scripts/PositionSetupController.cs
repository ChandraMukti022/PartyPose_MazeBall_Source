using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PositionSetupController : MonoBehaviour
{
    public enum RequiredBody { HeadShoulders, UpperBody, FullBody }

    [Header("Referensi UI")]
    public RectTransform videoRect;          
    public GameObject panel;                 
    public TMP_Text statusText; 
    
    [Tooltip("Penting: Centang ini jika kamera Anda seperti cermin")]
    public bool mirrorX = true;             

    [Header("Siluet P1 & P2 (Kedap-kedip)")]
    public Image guideImageP1;                 
    public Image guideImageP2;                 
    public float blinkSpeed = 1.5f;
    public float maxAlpha = 0.5f;

    [Header("Aturan posisi")]
    public RequiredBody required = RequiredBody.HeadShoulders; 
    public float holdSeconds = 0.8f; 
    public float readyShowSeconds = 1.0f;
    [Range(0f, 0.2f)] public float frameMargin = 0.02f;

    [Header("Anti-Kedip Siluet")]
    [Tooltip("Pemain yang sudah dianggap OK tetap dianggap OK selama waktu ini (detik) walau deteksi goyang sesaat. Naikkan kalau siluet masih kedip.")]
    [Range(0f, 1f)] public float okGraceSeconds = 0.3f;

    [Header("Debug / Testing (KHUSUS DEVELOPER)")]
    [Tooltip("Centang ini jika main 1 orang saja (Solo Mode). Siluet P2 dan deteksi P2 akan dimatikan total.")]
    public bool isSoloMode = false;
    
    [Tooltip("Centang ini agar tombol SPASI bisa dipakai untuk SKIP layar ini.")]
    public bool enableDeveloperMode = false;

    [Header("Event")]
    public UnityEvent onReady;

    bool _phaseReady = false;
    float _hold;
    readonly float[] _lastOkTime = { -999f, -999f }; // [GAME] waktu terakhir tiap pemain dinyatakan OK

    void Start()
    {
        PoseBridgeFeeder.isCameraMirrored = mirrorX; 
    }

    public void Restart()
    {
        StopAllCoroutines();
        _phaseReady = false;
        _hold = 0f; 
        _lastOkTime[0] = -999f;
        _lastOkTime[1] = -999f;
        
        PoseBridgeFeeder.isCameraMirrored = mirrorX; 
        
        if (statusText) 
            statusText.text = isSoloMode ? "Waiting P1..." : "Waiting P1 & P2...";
        
        if (guideImageP1) guideImageP1.enabled = true;
        
        // Jika mode solo, siluet P2 dimatikan selamanya di layar setup
        if (guideImageP2) guideImageP2.enabled = !isSoloMode;

        if (panel) panel.SetActive(true);
    }

    void Update()
    {
        if (_phaseReady || videoRect == null) return;

        // --- CHEAT SPASI HANYA JALAN JIKA DEVELOPER MODE DICENTANG ---
        if (enableDeveloperMode && Input.GetKeyDown(KeyCode.Space))
        {
            if (statusText) statusText.text = "SKIPPED!";
            StartCoroutine(ReadySequence());
            return;
        }
        
        bool okP1 = EvaluatePlayer(0);
        bool okP2 = isSoloMode ? true : EvaluatePlayer(1); // Jika solo mode, P2 otomatis dianggap aman tanpa syarat

        // --- LOGIKA TEKS STATUS ---
        if (statusText)
        {
            if (isSoloMode)
            {
                statusText.text = okP1 ? "READY!" : "Waiting P1...";
            }
            else
            {
                if (okP1 && okP2) statusText.text = "READY!";
                else if (okP1 && !okP2) statusText.text = "Waiting P2...";
                else if (!okP1 && okP2) statusText.text = "Waiting P1...";
                else statusText.text = "Waiting P1 & P2...";
            }
        }

        // --- LOGIKA SILUET KEDAP-KEDIP ---
        float alpha = Mathf.PingPong(Time.time * blinkSpeed, maxAlpha);
        
        if (guideImageP1) 
        {
            guideImageP1.enabled = !okP1; 
            guideImageP1.color = new Color(1f, 1f, 1f, alpha);
        }

        if (guideImageP2) 
        {
            if (isSoloMode)
            {
                guideImageP2.enabled = false; // Sembunyikan siluet P2 jika mode solo
            }
            else
            {
                guideImageP2.enabled = !okP2; 
                guideImageP2.color = new Color(1f, 1f, 1f, alpha);
            }
        }

        // --- LOGIKA WAKTU TUNGGU ---
        bool bothOk = okP1 && okP2;
        float dt = Time.deltaTime;
        _hold = bothOk ? _hold + dt : Mathf.Max(0f, _hold - dt * 2f);

        if (_hold >= holdSeconds) StartCoroutine(ReadySequence());
    }

    IEnumerator ReadySequence()
    {
        _phaseReady = true;
        yield return new WaitForSeconds(readyShowSeconds);
        if (panel) panel.SetActive(false);
        onReady.Invoke();
    }

    // [GAME] Pembungkus anti-kedip: kalau pemain barusan OK, tetap dianggap OK selama okGraceSeconds
    bool EvaluatePlayer(int pIndex)
    {
        if (EvaluatePlayerRaw(pIndex))
        {
            _lastOkTime[pIndex] = Time.time;
            return true;
        }
        return (Time.time - _lastOkTime[pIndex]) < okGraceSeconds;
    }

    bool EvaluatePlayerRaw(int pIndex)
    {
        if (!PoseBridge.IsFresh(pIndex)) return false; 

        float shoulderX = (PoseBridge.Get(pIndex, PoseBridge.LeftShoulder).x + PoseBridge.Get(pIndex, PoseBridge.RightShoulder).x) / 2f;
        float normalizedX = mirrorX ? (1f - shoulderX) : shoulderX;

        // Jika mode solo, P1 bebas berdiri di mana saja (tidak dibatasi garis tengah)
        if (!isSoloMode)
        {
            if (pIndex == 0 && normalizedX > 0.60f) return false; 
            if (pIndex == 1 && normalizedX < 0.40f) return false; 
        }

        foreach (int idx in NeededLandmarks())
        {
            Vector2 n = PoseBridge.Get(pIndex, idx);
            if (n.x < frameMargin || n.x > 1f - frameMargin || n.y < frameMargin || n.y > 1f - frameMargin)
            {
                return false;
            }
        }

        return true;
    }

    int[] NeededLandmarks()
    {
        switch (required)
        {
            case RequiredBody.HeadShoulders: 
                return new[] { PoseBridge.Nose, PoseBridge.LeftShoulder, PoseBridge.RightShoulder };
            case RequiredBody.UpperBody:
                return new[] { PoseBridge.Nose, PoseBridge.LeftShoulder, PoseBridge.RightShoulder,
                               PoseBridge.LeftHip, PoseBridge.RightHip };
            default:
                return new[] { PoseBridge.Nose, PoseBridge.LeftShoulder, PoseBridge.RightShoulder,
                               PoseBridge.LeftHip, PoseBridge.RightHip,
                               PoseBridge.LeftAnkle, PoseBridge.RightAnkle };
        }
    }
}