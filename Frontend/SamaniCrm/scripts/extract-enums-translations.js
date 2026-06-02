#!/usr/bin/env node

/**
 * extract-enums-translations.js
 *
 * Usage:
 * node extract-enums-translations.js [sourceDir] [outputJson]
 *
 * Example:
 * node extract-enums-translations.js ../src translations/extracted-enums.json
 */

const fs = require('fs').promises;
const path = require('path');
const ts = require('typescript');

const SRC = process.argv[2] || '../src/shared/service-proxies';
const OUT = process.argv[3] || 'translations/extracted-enums.json';

const EXTENSIONS = new Set([
    '.ts'
]);

async function walk(dir, files = []) {
    let entries;

    try {
        entries = await fs.readdir(dir, { withFileTypes: true });
    }
    catch {
        return files;
    }

    for (const entry of entries) {

        const fullPath = path.join(dir, entry.name);

        if (entry.isDirectory()) {

            if ([
                'node_modules',
                'dist',
                '.angular',
                '.git',
                'coverage',
                'build'
            ].includes(entry.name))
                continue;

            await walk(fullPath, files);
        }
        else if (entry.isFile()) {

            if (EXTENSIONS.has(path.extname(entry.name)))
                files.push(fullPath);
        }
    }

    return files;
}

async function extractEnums(filePath, result) {

    const content = await fs.readFile(filePath, 'utf8');

    const sourceFile = ts.createSourceFile(
        filePath,
        content,
        ts.ScriptTarget.Latest,
        true
    );

    function visit(node) {

        if (ts.isEnumDeclaration(node)) {

            const enumName = node.name.text;

            let autoValue = 0;

            for (const member of node.members) {

                const memberName = member.name.getText(sourceFile);

                let value;

                if (member.initializer) {

                    if (ts.isNumericLiteral(member.initializer)) {

                        value = Number(member.initializer.text);
                        autoValue = value + 1;
                    }
                    else if (ts.isStringLiteral(member.initializer)) {

                        value = member.initializer.text;
                    }
                    else {

                        continue;
                    }
                }
                else {

                    value = autoValue;
                    autoValue++;
                }

                result[`${enumName}_${value}`] = memberName;
            }
        }

        ts.forEachChild(node, visit);
    }

    visit(sourceFile);
}

async function main() {

    console.log(`Scanning: ${SRC}`);

    const files = await walk(SRC);

    console.log(`Files found: ${files.length}`);

    let existing = {};

    try {

        existing = JSON.parse(
            await fs.readFile(OUT, 'utf8')
        );
    }
    catch {
        existing = {};
    }

    const extracted = {};

    for (const file of files) {
        await extractEnums(file, extracted);
    }

    for (const [key, value] of Object.entries(extracted)) {

        if (!(key in existing)) {
            existing[key] = value;
        }
    }

    await fs.mkdir(
        path.dirname(OUT),
        { recursive: true }
    );

    await fs.writeFile(
        OUT,
        JSON.stringify(existing, null, 2),
        'utf8'
    );

    console.log(`Enums extracted: ${Object.keys(extracted).length}`);
    console.log(`Output: ${OUT}`);
}

main().catch(err => {
    console.error(err);
    process.exit(1);
});