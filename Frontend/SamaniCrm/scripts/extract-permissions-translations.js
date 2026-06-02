#!/usr/bin/env node

/**
 * extract-permissions-translations.js
 *
 * Usage:
 * node extract-permissions-translations.js apppermissions.ts permissions.json
 */

const fs = require('fs').promises;
const path = require('path');

const INPUT =
    process.argv[2] || '../src/shared/permissions/app-permissions.ts';

const OUTPUT =
    process.argv[3] || './translations/permissions.json';

async function main() {

    const content =
        await fs.readFile(INPUT, 'utf8');

    let existing = {};

    try {

        existing = JSON.parse(
            await fs.readFile(OUTPUT, 'utf8')
        );
    }
    catch {
        existing = {};
    }

    const result = {};

    const regex =
        /static\s+([A-Za-z0-9_]+)\s*=\s*["']([^"']+)["']/g;

    let match;

    while ((match = regex.exec(content)) !== null) {

        const permissionValue = match[2];

        const key =
            'Permission_' +
            permissionValue.replace(/\./g, '_');

        result[key] = '';
    }

    for (const [key, value] of Object.entries(result)) {

        if (!(key in existing)) {
            existing[key] = value;
        }
    }

    await fs.mkdir(
        path.dirname(OUTPUT),
        { recursive: true }
    );

    await fs.writeFile(
        OUTPUT,
        JSON.stringify(existing, null, 2),
        'utf8'
    );

    console.log(
        `Permissions found: ${Object.keys(result).length}`
    );

    console.log(
        `Output: ${OUTPUT}`
    );
}

main().catch(err => {

    console.error(err);
    process.exit(1);
});