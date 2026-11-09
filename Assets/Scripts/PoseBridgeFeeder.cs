using Mediapipe.Tasks.Vision.PoseLandmarker;
using System.Collections.Generic;
using LandmarkList = System.Collections.Generic.IReadOnlyList
<Mediapipe.Tasks.Components.Containers.NormalizedLandmark>;

public static class PoseBridgeFeeder
{
    // Mengatur apakah input kamera dibalik seperti cermin (biasanya true untuk webcam)
    public static bool isCameraMirrored = true;

    public static void Feed(PoseLandmarkerResult result)
    {
        var poses = result.poseLandmarks;
        
        // Jika tidak ada orang terdeteksi di kamera
        if (poses == null || poses.Count == 0)
        {
            NoPose();
            return;
        }

        // SOLO MODE: Kita HANYA memproses orang pertama yang terdeteksi (poses[0])
        ProcessPose(0, poses[0].landmarks);
    }

    static void ProcessPose(int playerIndex, LandmarkList lms)
    {
        // Pastikan jumlah titik sendi lengkap (33 titik dari MediaPipe)
        if (lms == null || lms.Count < 33)
        {
            PoseBridge.MarkUpdated(playerIndex, false);
            return;
        }
        
        // Simpan koordinat ke PoseBridge
        for (int i = 0; i < 33; i++)
        {
            PoseBridge.SetPoint(playerIndex, i, lms[i].x, lms[i].y);
        }
        PoseBridge.MarkUpdated(playerIndex, true);
    }

    // Fungsi ini dipanggil oleh PoseLandmarkerRunner saat kamera tidak mendeteksi pose apa pun
    public static void NoPose()
    {
        PoseBridge.MarkUpdated(0, false);
        // PoseBridge.MarkUpdated(1, false); // Dimatikan karena kita hanya main 1 player
    }
}