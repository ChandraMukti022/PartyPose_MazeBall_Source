using UnityEngine;

/// <summary>
/// Jembatan data: menyimpan posisi landmark pose terbaru untuk 2 PLAYER.
/// Player 0 = Kiri, Player 1 = Kanan.
/// </summary>
public static class PoseBridge
{
    public const int Nose = 0;
    public const int LeftEar = 7, RightEar = 8;
    public const int LeftWrist = 15, RightWrist = 16;
    public const int LeftIndex = 19, RightIndex = 20;
    public const int LeftShoulder = 11, RightShoulder = 12;
    public const int LeftHip = 23, RightHip = 24;
    public const int LeftKnee = 25, RightKnee = 26;
    public const int LeftAnkle = 27, RightAnkle = 28;

    static readonly object _lock = new object();

    // Array untuk 2 pemain (0 = Kiri, 1 = Kanan)
    static readonly Vector2[][] _points = new Vector2[2][]
    {
        new Vector2[33], // Slot Player 0
        new Vector2[33]  // Slot Player 1
    };

    static readonly System.Diagnostics.Stopwatch _clock = System.Diagnostics.Stopwatch.StartNew();
    static readonly bool[] _hasPose = new bool[2];
    static readonly double[] _lastUpdate = new double[2];

    public static void SetPoint(int playerIndex, int index, float x, float y)
    {
        if (playerIndex < 0 || playerIndex > 1) return;
        if (index < 0 || index >= 33) return;
        lock (_lock) { _points[playerIndex][index] = new Vector2(x, y); }
    }

    public static void MarkUpdated(int playerIndex, bool found)
    {
        if (playerIndex < 0 || playerIndex > 1) return;

        // [GAME] Kalau deteksi gagal, JANGAN langsung dimatikan.
        // Biar timeout di IsFresh() yang menentukan kapan pemain dianggap hilang.
        // Ini mencegah UI AR kedip saat 1 frame deteksi gagal (misal gerak cepat).
        if (!found) return;

        lock (_lock)
        {
            _hasPose[playerIndex] = true;
            _lastUpdate[playerIndex] = _clock.Elapsed.TotalSeconds;
        }
    }

    // Fungsi dengan parameter playerIndex (0 atau 1)
    public static Vector2 Get(int playerIndex, int index)
    {
        if (playerIndex < 0 || playerIndex > 1) return Vector2.zero;
        lock (_lock) { return _points[playerIndex][index]; }
    }

    // [GAME] timeout default 0.35 detik (sebelumnya 0.5). Naikkan kalau masih kedip.
    public static bool IsFresh(int playerIndex, float timeout = 0.35f)
    {
        if (playerIndex < 0 || playerIndex > 1) return false;
        lock (_lock) { return _hasPose[playerIndex] && (_clock.Elapsed.TotalSeconds - _lastUpdate[playerIndex]) < timeout; }
    }

    // --- Fungsi bawaan (Backward Compatibility) agar script lama tidak error ---
    // Sementara akan membaca data Player 0 secara otomatis.
    public static Vector2 Get(int index) { return Get(0, index); }
    public static bool IsFresh(float timeout = 0.35f) { return IsFresh(0, timeout) || IsFresh(1, timeout); }
}