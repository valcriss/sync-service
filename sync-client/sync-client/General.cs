using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace sync_client
{
    public partial class General : Form
    {

        private Config _config;
        private Dictionary<string, string> _localIndex;
        private Dictionary<string, FileEntry> _serverIndex;
        private readonly HttpClient _httpClient = new HttpClient();

        // Download tracking
        private int totalFiles;
        private int completedFiles;
        private Label[] labels;
        private Point StartingPosition = new Point(12, 303);
        private Size StartingSize = new Size(696, 71);

        public General()
        {
            InitializeComponent();
            labels = new Label[]
            {
                labelStatus,
                label1,
                label2,
                label3,
                label4
            };
        }

        private void HideLabels()
        {
            for (int i = 0; i < labels.Length; i++)
            {
                if (i == 0) continue;
                labels[i].Visible = false;
            }
            groupBox1.Location = StartingPosition;
            groupBox1.Size = StartingSize;
            Application.DoEvents();
        }

        private void UpdateStatus(string message)
        {
            labelStatus.Text = message;
            Application.DoEvents();
        }

        private void UpdateStatus(int index, string message)
        {
            labels[index].Visible = true;
            labels[index].Text = message;
            int visibleCount = labels.Count(l => l.Visible) - 1;
            groupBox1.Location = new Point(StartingPosition.X, StartingPosition.Y - ((visibleCount * 22) + (visibleCount - 1) * 3));
            groupBox1.Size = new Size(StartingSize.Width, StartingSize.Height + ((visibleCount * 22) + (visibleCount - 1) * 3));
            Application.DoEvents();
        }

        private void UpdateOverallProgress(int maximum, int completed)
        {
            progressBarOverall.Maximum = maximum;
            progressBarOverall.Value = completed;
            Application.DoEvents();
        }

        private void UpdateOverallProgress()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateOverallProgress));
                return;
            }
            UpdateOverallProgress(totalFiles, completedFiles);
            UpdateStatus($"Progression générale : {completedFiles}/{totalFiles}");
        }

        private void UpdateFileProgress(int workerId, int percent, string file)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateFileProgress(workerId, percent, file)));
                return;
            }
            UpdateStatus(workerId, $"Téléchargement de {file} : {percent}%");
        }

        private async Task SyncWithServerAsync()
        {
            try
            {
                button1.Enabled = false;
                await FetchServerIndexAsync();
                var toDownload = CompareIndices();

                totalFiles = toDownload.Count;
                completedFiles = 0;
                UpdateOverallProgress();

                await DownloadFilesAsync(toDownload);
                UpdateStatus("Synchronisation terminée.");
                HideLabels();
                button1.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la synchronisation : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task FetchServerIndexAsync()
        {
            var response = await _httpClient.GetAsync("/files/index");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<FileEntry>>(json);
            _serverIndex = list.ToDictionary(f => f.Path, f => f);
        }

        private void LoadConfig()
        {
            _config = Config.LoadOrCreate();
            if (string.IsNullOrEmpty(_config.LocalPath) || !Directory.Exists(_config.LocalPath))
            {
                using (var dlg = new FolderBrowserDialog())
                {
                    dlg.Description = "Sélectionnez le dossier où installer le jeu " + _config.GameName;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        _config.LocalPath = dlg.SelectedPath;
                        _config.Save();
                    }
                    else
                    {
                        MessageBox.Show("Le dossier est requis.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                    }
                }
            }
            label5.Text = _config.GameName;
            _config.ServerUrl = _config.ServerUrl;
            _config.Save();
            if(string.IsNullOrEmpty(_config.ServerUrl))
            {
                MessageBox.Show("L'URL du serveur est requise.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            _httpClient.BaseAddress = new Uri(_config.ServerUrl);
            HideLabels();
        }

        private void ScanLocalFolder()
        {
            _localIndex = new Dictionary<string, string>();
            var basePath = _config.LocalPath.TrimEnd(Path.DirectorySeparatorChar);
            var files = Directory.GetFiles(basePath, "*", SearchOption.AllDirectories);

            using (var sha = SHA256.Create())
            {
                foreach (var fullPath in files)
                {
                    UpdateStatus($"Analyse du fichier : {fullPath}");
                    var relPath = fullPath.Substring(basePath.Length + 1).Replace("\\", "/");
                    using (var stream = File.OpenRead(fullPath))
                    {
                        var hashBytes = sha.ComputeHash(stream);
                        var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                        _localIndex[relPath] = hash;
                    }
                }
            }
        }

        private List<string> CompareIndices()
        {
            var list = new List<string>();
            foreach (var kv in _serverIndex)
            {
                var path = kv.Key;
                var serverHash = kv.Value.Hash;
                if (!_localIndex.TryGetValue(path, out var localHash) || localHash != serverHash)
                {
                    list.Add(path);
                }
            }
            return list;
        }

        private async Task DownloadFilesAsync(List<string> toDownload)
        {
            int maxTasks = _config.MaxParallelDownloads > 0 ? _config.MaxParallelDownloads : 4;
            var queue = new ConcurrentQueue<string>(toDownload);

            var tasks = Enumerable.Range(0, maxTasks).Select(workerId => Task.Run(async () =>
            {
                while (queue.TryDequeue(out var relPath))
                {
                    try
                    {
                        await DownloadFileAsync(relPath, workerId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur sur {relPath} par worker {workerId} : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    finally
                    {
                        Interlocked.Increment(ref completedFiles);
                        UpdateOverallProgress();
                    }
                }
            }));

            await Task.WhenAll(tasks);
        }

        private async Task DownloadFileAsync(string relPath, int workerId)
        {
            var entry = _serverIndex[relPath];
            var url = $"/files/download?path={Uri.EscapeDataString(relPath)}";
            var localFullPath = Path.Combine(_config.LocalPath, relPath.Replace("/", Path.DirectorySeparatorChar.ToString()));
            Directory.CreateDirectory(Path.GetDirectoryName(localFullPath));

            long existingLength = 0;
            if (File.Exists(localFullPath))
            {
                existingLength = new FileInfo(localFullPath).Length;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (existingLength > 0)
            {
                request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(existingLength, null);
            }

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var totalSize = entry.Size;
            long totalRead = existingLength;

            using (var contentStream = await response.Content.ReadAsStreamAsync())
            using (var fs = new FileStream(
                localFullPath,
                existingLength > 0 ? FileMode.Append : FileMode.Create,
                FileAccess.Write,
                FileShare.None))
            {
                var buffer = new byte[81920];
                int read;
                while ((read = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fs.WriteAsync(buffer, 0, read);
                    totalRead += read;
                    int percent = (int)(totalRead * 100 / totalSize);
                    UpdateFileProgress(workerId + 1, percent, relPath);
                }
            }
            Console.WriteLine($"Téléchargé: {relPath}");
        }

        private void General_Load(object sender, EventArgs e)
        {
            this.Show();
            LoadConfig();
            ScanLocalFolder();
            _ = SyncWithServerAsync();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ExecutableLookup();
        }

        private void ExecutableLookup()
        {
            string? path = Directory.GetFiles(_config.LocalPath, "*.exe", SearchOption.TopDirectoryOnly)
                    .Where(c => Path.GetFileName(c) != "World of Warcraft Launcher.exe").FirstOrDefault();
            if (path == null)
            {
                MessageBox.Show("Aucun exécutable trouvé dans le dossier local.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Process process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.Arguments = _config.ExecArgs;
            process.StartInfo.WorkingDirectory = _config.LocalPath;
            process.Start();
            this.Close();
        }
    }
}
