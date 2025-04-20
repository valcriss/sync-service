# SyncService: The Directory Updater That Doesn’t Make You Rage-Quit 🐉

Welcome, heroic adventurer! You’ve braved the dark corners of GitHub to find a reliable, fast, and slightly sassy World of Warcraft updater. If you’re tired of outdated launchers that move slower than a turtle on a good day, you’ve come to the right place.

---

## 🧐 What Is SyncService?

SyncService is a client-server synchronization tool designed specifically for patching massive games like World of Warcraft. It leverages a lightweight Node.js server and a WinForms C# client to:

1. **Index** all your game files on the server (SHA-256 hash, size).  
2. **Scan** the client’s local copy, detect missing or changed files.  
3. **Download** only the bits that changed—no more full repatches!  
4. **Parallelize** downloads across multiple threads to saturate your bandwidth (choose how many workers you want).

All that without making you pull out your hair.

---

## 🏗️ Architecture Overview

```text
┌──────────────┐         HTTP        ┌───────────────┐
│  Sync-Server │ ◀────────────────▶ │  Sync-Client  │
│   (Node.js)  │                     │ (C# WinForms) │
└──────────────┘                     └───────────────┘
```  

- **Server** (in `sync-server/`):
  - **Fast-Glob** to recursively scan files.  
  - **Crypto** (SHA-256) for file integrity.  
  - **Express** for REST endpoints:
    - `GET /files/index` → list of `{ path, hash, size }`  
    - `GET /files/download?path=` → stream with HTTP Range support

- **Client** (in `sync-client/`):
  - **Config** JSON for server URL, local folder, max threads.  
  - **SHA256** scan of local directory.  
  - **HttpClient** with `Range` headers for resume and chunked downloads.  
  - **WinForms GUI** with progress bars, status messages, and error handling.

---

## 🚀 Getting Started

### 1. Clone the Repo

```bash
git clone https://github.com/valcriss/sync-service.git
cd SyncMaster
```

### 2. Run the Server

```bash
cd sync-server
tnpm install
node index.js /path/to/your/files
```

### 3. Configure & Launch the Client

1. Open `sync-client` in Visual Studio.  
2. Build & run.  
3. When prompted, select your local install folder.  
4. (Optional) Edit `config.json` to tweak `MaxParallelDownloads` or server URL.

### 4. Watch the Magic Happen

You’ll see a progress bar for overall sync, and per-file label progress. Sit back, sip a coffee, or summon a dragon.

---

## 🔧 Future features

- **Hot-Reloading**: Want the server to watch your file tree and auto-index new patches? Add `chokidar` and trigger `initFileIndex()` on change.
- **Logging**: Integrate `morgan` or `winston` on the server for fancy log output.  
- **Dockerize**: Spin up your server in a container so you can deploy on any cloud with zero brain cells used.

---

## 🤝 Contributing

1. Fork the repository.  
2. Create a feature branch (`git checkout -b feature/awesome-patch`).  
3. Commit your changes (`git commit -m "feat: add unicorn support 🦄"`).  
4. Push to the branch (`git push origin feature/awesome-patch`).  
5. Open a Pull Request.

Please keep it tasteful and avoid committing your real Steam password. 😉

---

## 📜 License

This project is licensed under the MIT License — feel free to patch it, fork it, or unleash it on Azeroth!

---

*May your patches be small and your frame rates high!* 🎮

