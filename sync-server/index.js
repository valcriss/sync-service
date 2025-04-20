const express = require('express');
const path = require('path');
const fs = require('fs');
const fg = require('fast-glob');
const crypto = require('crypto');
const mime = require('mime');

const app = express();
const port = process.env.PORT || 5000;

if (!process.argv[2]) {
  console.error('Usage: node index.js <directory_to_serve>');
  process.exit(1);
}

const rootDir = path.resolve(process.argv[2]);
console.log(`Serving files from ${rootDir}`);

let fileIndex = {};

// Initialise l'index des fichiers
async function initFileIndex() {
  const entries = await fg(['**/*'], { cwd: rootDir, dot: false, onlyFiles: true });
  for (const relPath of entries) {
    const fullPath = path.join(rootDir, relPath);
    const hash = await hashFile(fullPath);
    const size = (await fs.promises.stat(fullPath)).size;
    fileIndex[relPath] = { hash, size };
    console.log(`Indexed: ${relPath} (hash: ${hash})`);
  }
}

// Calcule le hash SHA-256 d'un fichier
function hashFile(filePath) {
  return new Promise((resolve, reject) => {
    const hash = crypto.createHash('sha256');
    const stream = fs.createReadStream(filePath);

    stream.on('error', reject);
    stream.on('data', chunk => hash.update(chunk));
    stream.on('end', () => resolve(hash.digest('hex')));
  });
}

// Endpoint pour récupérer l'index
app.get('/files/index', (req, res) => {
  const list = Object.entries(fileIndex).map(([path, { hash, size }]) => ({ path, hash, size }));
  res.json(list);
});

// Endpoint pour télécharger un fichier avec support des Range Requests
app.get('/files/download', (req, res) => {
  const relPath = req.query.path;
  if (!relPath || !fileIndex[relPath]) {
    return res.status(404).send('File not found');
  }
  const fullPath = path.join(rootDir, relPath);
  const stat = fs.statSync(fullPath);
  const range = req.headers.range;
  const contentType = mime.getType(fullPath) || 'application/octet-stream';
  const fileSize = stat.size;

  if (range) {
    const [startStr, endStr] = range.replace(/bytes=/, '').split('-');
    const start = parseInt(startStr, 10);
    const end = endStr ? parseInt(endStr, 10) : fileSize - 1;

    if (start >= fileSize || end >= fileSize) {
      res.status(416).header('Content-Range', `bytes */${fileSize}`).end();
      return;
    }

    res.status(206);
    res.set({
      'Content-Range': `bytes ${start}-${end}/${fileSize}`,
      'Accept-Ranges': 'bytes',
      'Content-Length': end - start + 1,
      'Content-Type': contentType,
    });

    fs.createReadStream(fullPath, { start, end }).pipe(res);
  } else {
    res.set({
      'Content-Length': fileSize,
      'Content-Type': contentType,
      'Accept-Ranges': 'bytes',
    });
    fs.createReadStream(fullPath).pipe(res);
  }
});

// Initialisation et démarrage
initFileIndex().then(() => {
  app.listen(port, () => console.log(`Sync-server listening on port ${port}`));
}).catch(err => {
  console.error('Error initializing file index:', err);
  process.exit(1);
});
