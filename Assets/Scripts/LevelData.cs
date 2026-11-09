using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataBaru", menuName = "Puzzle/Data Level")]
public class LevelData : ScriptableObject
{
    [Header("Info Level")]
    public string namaLevel = "Level 1";
    public float batasWaktu = 30f;

    [Header("Modul Mini Game")]
    [Tooltip("Masukkan Prefab Labirin yang sudah disetting UI-nya")]
    public GameObject miniGamePrefab;
}