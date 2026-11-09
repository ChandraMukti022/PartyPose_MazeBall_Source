using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void KeluarGame()
    {
        // Menampilkan pesan di Console saat dites di Unity Editor
        Debug.Log("GAME OVER / KELUAR DARI APLIKASI!"); 
        
        // Perintah wajib untuk menutup game di versi Build (.exe) PC
        Application.Quit(); 
    }
}