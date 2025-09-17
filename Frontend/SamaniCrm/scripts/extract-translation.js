#!/usr/bin/env node

/**
 * extract-translations.js
 *
 * Usage:
 *   node extract-translations.js [sourceDir] [outputJson]
 *
 * Defaults:
 *   sourceDir = "src"
 *   outputJson = "translations/extracted-translations.json"
 *
 * Scans files for translation keys used with:
 *  - 'key' | translate
 *  - this.l('key')
 *  - translate.instant('key'), this.translate.instant('key')
 *  - translateService.get('key'), translate.get('key')
 *
 * Writes a JSON object: { "key1": "", "key2": "", ... }
 */

const fs = require('fs').promises;
const path = require('path');

const SRC = process.argv[2] || '../src';
const OUT = process.argv[3] || path.join('translations', 'extracted-translations.json');

// file extensions to scan
const EXTENSIONS = new Set(['.html', '.htm', '.ts', '.js', '.jsx', '.tsx', '.vue']);

// regex patterns to find translation keys
const patterns = [
  // 'home' | translate  (allow single/double/backtick quotes)
  /(['"`])([^'"`\\\n\r]+?)\1\s*\|\s*translate\b/gm,

  // this.l('home')  or this.l("home") or this.l(`home`)
  /this\.l\s*\(\s*(['"`])([^'"`\\\n\r]+?)\1\s*\)/gm,

  // translate.instant('home') or this.translate.instant('home')
  /(?:\btranslate(?:Service|\b)?|this\.translate)\.instant\s*\(\s*(['"`])([^'"`\\\n\r]+?)\1\s*\)/gm,

  // translateService.get('home') or translate.get('home')
  /(?:translate(?:Service)?|this\.translate)\.get\s*\(\s*(['"`])([^'"`\\\n\r]+?)\1\s*\)/gm,

  // i18n attribute like i18n="home" or i18n-title="home" (optional, helpful)
  /i18n(?:-[\w-]+)?\s*=\s*(['"`])([^'"`\\\n\r]+?)\1/gm,
];

async function walk(dir, fileList = []) {
  let entries;
  try {
    entries = await fs.readdir(dir, { withFileTypes: true });
  } catch (err) {
    console.error(`Error in reading directory ${dir}:`, err.message);
    return fileList;
  }

  for (const entry of entries) {
    const full = path.join(dir, entry.name);
    if (entry.isDirectory()) {
      // skip node_modules and dist/build folders by default
      if (entry.name === 'node_modules' || entry.name === 'dist' || entry.name === 'build') continue;
      await walk(full, fileList);
    } else if (entry.isFile()) {
      if (EXTENSIONS.has(path.extname(entry.name).toLowerCase())) {
        fileList.push(full);
      }
    }
  }
  return fileList;
}

async function extractKeysFromFile(filePath, keySet) {
  try {
    const content = await fs.readFile(filePath, 'utf8');

    for (const re of patterns) {
      let m;
      // reset lastIndex for safety
      re.lastIndex = 0;
      while ((m = re.exec(content)) !== null) {
        // capture group 2 is the key in our patterns
        const key = m[2] && m[2].trim();
        if (key && key.length > 0) {
          keySet.add(key);
        }
      }
    }

    // additional heuristic: look for l('key') without this. (optional)
    // (uncomment next block if you want to include plain l('...') calls)
    /*
    const plainL = /(?:\b|[^.\w])l\s*\(\s*(['"`])([^'"`\\\n\r]+?)\1\s*\)/gm;
    let m2;
    plainL.lastIndex = 0;
    while ((m2 = plainL.exec(content)) !== null) {
      const key = m2[2] && m2[2].trim();
      if (key) keySet.add(key);
    }
    */

  } catch (err) {
    console.warn(`Error reading file ${filePath}: ${err.message}`);
  }
}

async function main() {
  console.log(`Searching in folder: ${SRC}`);
  const files = await walk(SRC);
  console.log(`Files scanned: ${files.length}`);

  const keys = new Set();

  for (const f of files) {
    await extractKeysFromFile(f, keys);
  }

  const sortedKeys = Array.from(keys).sort((a, b) => a.localeCompare(b, 'en', { numeric: true }));
  const outObj = {};
  for (const k of sortedKeys) {
    outObj[k] = "";
  }

  // ensure output directory exists
  const outDir = path.dirname(OUT);
  await fs.mkdir(outDir, { recursive: true });

  await fs.writeFile(OUT, JSON.stringify(outObj, null, 2), 'utf8');
  console.log(`Keys extracted: ${sortedKeys.length}`);
  console.log(`Output file: ${OUT}`);
}

main().catch(err => {
  console.error('Error:', err);
  process.exit(1);
});
