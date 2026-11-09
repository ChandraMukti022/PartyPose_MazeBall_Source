# MainMekanik - AR Puzzle Game 🎮

[![Play on itch.io](https://img.shields.io/badge/Play_on-itch.io-FA5C5C?style=for-the-badge&logo=itch.io)](https://shinchanstudio.itch.io/partypose-maze-ball-ar)

MainMekanik adalah mini game puzzle 2D/3D interaktif yang dibuat menggunakan Unity. Game ini memanfaatkan teknologi Augmented Reality (AR) *body tracking* sebagai kontroler utamanya. Proyek ini dikembangkan buat ngeksplorasi *core gameplay loop* yang asik, arsitektur kode yang rapi, dan bereksperimen dengan input kontrol game yang beda dari biasanya.

**👉 Kamu bisa main dan download build-nya di sini:** [https://shinchanstudio.itch.io/partypose-maze-ball-ar](https://shinchanstudio.itch.io/partypose-maze-ball-ar)

## 📄 Dokumen Arsitektur & Desain
Untuk penjelasan lebih mendalam mengenai keputusan desain, *core loop*, dan struktur kode dari proyek ini, silakan cek dokumen berikut:
**[Baca Game Architecture & Design Document (Google Docs)](https://docs.google.com/document/d/1JUaDsl-SbLrpbYEo6CHDB2WN3MsDIex0/edit?usp=sharing)**

## 🌟 Fitur Utama
- **Kontrol Unik:** Pakai *body tracking* berbasis kamera (MediaPipe) buat ngendaliin puzzle, ngasih pengalaman yang jauh lebih seru dibanding cuma main pakai *keyboard* atau *mouse*.
- **Sistem Level Progresif:** Punya 3 level "Maze Ball" dengan tingkat kesulitan yang makin naik. Semuanya diatur rapi pakai `ScriptableObject` (`LevelData`).
- **Fitur Jepret & Simpan:** Ada kamera bawaan yang bakal nangkep momen pose kemenangan pemain dan otomatis nyimpen fotonya (format `.png`) langsung ke folder `Pictures` di Windows atau ke dalam folder *game*-nya.
- **Feedback Audio & Visual:** Makin hidup dengan transisi UI (Fade In/Out), petunjuk visual, *Voice Over* (3-2-1 GO!), serta efek suara (SFX) dan BGM buat merespons setiap aksi pemain.
- **Struktur Kode Rapi:** Alur game diatur sama `GameFlowManager` (*state machine*) terpusat dan pakai *dynamic prefab instantiation*, bikin kodenya modular dan gampang dikembangin lagi.

## 🔄 Core Gameplay Loop
Game ini dibangun dengan siklus *Action-Feedback-Progression* yang bikin ketagihan:
1. **Aksi:** Pemain berdiri di depan kamera, nunggu proses *scan* AR selesai, terus miringin badannya buat arahin bola di dalam labirin secara *real-time*.
2. **Feedback:** Waktu (timer) terus jalan, UI dinamis nampilin status saat ini, dan ada efek suara waktu pemain berhasil masukin bola atau pas waktunya keburu habis.
3. **Progresi:** Pas menang, game bakal ambil foto gaya pemain dan nampilin *Result Screen*. Waktu pemain ngeklik "Lanjut", mereka langsung masuk ke level berikutnya yang lebih susah tanpa harus *scan* ulang, jadi *feel arcade*-nya dapet banget.

## 🏗️ Struktur Kode & Desain
- **GameFlowManager.cs:** Bertindak sebagai "otak" dari game ini pakai logika *State* simpel (`Setup`, `Gameplay`, `Result`). Script ini yang ngurus buka-tutup panel UI, putar audio, sampai transisi antar level.
- **ScriptableObjects (`LevelData`):** Dipakai buat misahin konfigurasi level dari logika utama game. Tiap level punya batas waktu dan *prefab* labirinnya sendiri. Ini ngebantu banget kalau desainer game mau nambahin ratusan level baru tanpa perlu ngutak-ngatik kodingan lagi.
- **Dynamic Instantiation:** Level baru bakal di-*spawn* (instantiate) di dalam panel UI cuma pas dibutuhin aja, dan langsung dihapus pas levelnya kelar biar memorinya tetap hemat dan *hierarchy*-nya bersih.

## 🚀 Rencana Pengembangan (Ke Depannya)
Kalau ada waktu luang, proyek ini bisa banget ditambahin fitur kayak:
- **Multi-Mini-Game Hub:** Nambahin mini-game lain, misalnya game kartu memori berbasis perintah suara (*Voice Command Memory Card*) pakai Windows Speech Recognition buat variasi.
- **Local Multiplayer:** Bikin sistem kameranya bisa ngebaca dua orang sekaligus, jadi pemain bisa tanding sebelah-sebelahan.
- **Online Leaderboard:** Nambahin sistem skor online (ke *cloud backend*) berdasarkan waktu penyelesaian tercepat biar
