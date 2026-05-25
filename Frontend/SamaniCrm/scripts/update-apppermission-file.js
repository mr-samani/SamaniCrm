/**
 * Generate TypeScript permissions from C# AppPermissions
 * 
 * Usage:
 *   node generate-permissions.js <csFilePath> <tsFilePath>
 * 
 * Example:
 *   node generate-permissions.js  ../../BackEnd/SamaniCrm.Core/Const/AppPermissions.cs ../src/shared/permissions/app-permissions.ts
 */

 
const fs = require('fs');
const path = require('path');

// ═══════════════════════════════════════════════════════════════════════
// Configuration
// ═══════════════════════════════════════════════════════════════════════

const args = process.argv.slice(2);

if (args.length < 2) {
    console.error('❌ Error: Please provide both C# and TypeScript file paths');
    console.log('\nUsage: node generate-permissions.js <csFilePath> <tsFilePath>');
    console.log('\nExample:');
    console.log('   node generate-permissions.js  ../../BackEnd/SamaniCrm.Core/Consts/AppPermissions.cs ../src/shared/permissions/app-permissions.ts');
    process.exit(1);
}




const csFilePath = path.resolve(args[0]);
const tsFilePath = path.resolve(args[1]);

// ═══════════════════════════════════════════════════════════════════════
// Validation
// ═══════════════════════════════════════════════════════════════════════

if (!fs.existsSync(csFilePath)) {
    console.error(`❌ Error: C# file not found: ${csFilePath}`);
    process.exit(1);
}

// ═══════════════════════════════════════════════════════════════════════
// Parse C# File
// ═══════════════════════════════════════════════════════════════════════
// ═══════════════════════════════════════════════════════════════════════
// Line-by-line Parser
// ═══════════════════════════════════════════════════════════════════════

function parseCSharpPermissions(filePath) {
    const content = fs.readFileSync(filePath, 'utf-8');
    const lines = content.split('\n');
    
    const permissions = [];
    
    // Stack to track nested class hierarchy
    const classStack = [];
    
    // Get the root class name (AppPermissions) to exclude it
    const rootClassMatch = content.match(/public\s+static\s+class\s+(\w+)\s*\{/);
    const rootClassName = rootClassMatch ? rootClassMatch[1] : null;
    
    for (let i = 0; i < lines.length; i++) {
        const line = lines[i];
        
        // Calculate indentation
        const indent = line.search(/\S/);
        if (indent === -1) continue;
        
        // ─────────────────────────────────────────────────────────────
        // Check for class declaration
        // ─────────────────────────────────────────────────────────────
        const classMatch = line.match(/^(\s*)public\s+static\s+class\s+(\w+)/);
        
        if (classMatch) {
            const classIndent = classMatch[1].length;
            const className = classMatch[2];
            
            // Skip the root class (AppPermissions itself)
            if (className === rootClassName) {
                continue;
            }
            
            // Pop classes from stack that are at same or deeper level
            while (classStack.length > 0 && classStack[classStack.length - 1].indent >= classIndent) {
                classStack.pop();
            }
            
            // Push current class to stack
            classStack.push({ className, indent: classIndent });
            
            continue;
        }
        
        // ─────────────────────────────────────────────────────────────
        // Check for const string declaration
        // ─────────────────────────────────────────────────────────────
        const constMatch = line.match(/^\s*public\s+const\s+string\s+(\w+)\s*=\s*"([^"]+)"/);
        
        if (constMatch) {
            const constName = constMatch[1];
            const constValue = constMatch[2];
            
            // Build TypeScript name based on class hierarchy
            let tsName;
            
            if (classStack.length === 0) {
                // Top-level constant (like Administrator = "Administrator")
                tsName = constName;
            } else {
                // Build name from class stack: Class1_Class2_..._ClassN_ConstName
                const classNames = classStack.map(c => c.className);
                tsName = [...classNames, constName].join('_');
            }
            
            // Build category name
            const category = classStack.length > 0 
                ? classStack.map(c => c.className).join('.')
                : 'Core';
            
            permissions.push({
                tsName,
                value: constValue,
                category
            });
        }
    }
    
    return permissions;
}

// ═══════════════════════════════════════════════════════════════════════
// Generate TypeScript
// ═══════════════════════════════════════════════════════════════════════

function generateTypeScript(permissions) {
    // Group by category
    const grouped = {};
    
    permissions.forEach(p => {
        if (!grouped[p.category]) {
            grouped[p.category] = [];
        }
        grouped[p.category].push(p);
    });
    
    // Build TypeScript content
    let tsContent = `/**
 * Auto-generated file - Do not edit manually
 * Generated from: ${path.basename(csFilePath)}
 * Generated at: ${new Date().toISOString()}
 */

export class AppPermissions {
`;

    // Sort categories: Core first, then alphabetically
    const sortedCategories = Object.keys(grouped).sort((a, b) => {
        if (a === 'Core') return -1;
        if (b === 'Core') return 1;
        return a.localeCompare(b);
    });

    sortedCategories.forEach(category => {
        const categoryPerms = grouped[category];
        
        // Add comment for category
        if (category !== 'Core') {
            const displayName = category.replace(/\./g, ' > ');
            tsContent += `\n    // ${displayName}\n`;
        }
        
        categoryPerms.forEach(p => {
            tsContent += `    static ${p.tsName} = "${p.value}";\n`;
        });
    });

    tsContent += `}\n`;
    
    return tsContent;
}

// ═══════════════════════════════════════════════════════════════════════
// Main
// ═══════════════════════════════════════════════════════════════════════

console.log('\n🔄 Generating TypeScript permissions...\n');

try {
    const permissions = parseCSharpPermissions(csFilePath);
    
    if (permissions.length === 0) {
        console.error('❌ Error: No permissions found in C# file');
        process.exit(1);
    }
    
    console.log(`✅ Found ${permissions.length} permissions`);
    
    const tsContent = generateTypeScript(permissions);
    
    // Ensure directory exists
    const tsDir = path.dirname(tsFilePath);
    if (!fs.existsSync(tsDir)) {
        fs.mkdirSync(tsDir, { recursive: true });
    }
    
    // Delete existing file if exists
    if (fs.existsSync(tsFilePath)) {
        fs.unlinkSync(tsFilePath);
        console.log(`🗑️  Deleted existing file`);
    }
    
    // Write new file
    fs.writeFileSync(tsFilePath, tsContent, 'utf-8');
    
    console.log(`✅ Generated: ${tsFilePath}`);
    
    // Show preview
    console.log('\n📋 Preview:');
    permissions.slice(0, 15).forEach(p => {
        console.log(`   ${p.tsName} = "${p.value}"`);
    });
    
    if (permissions.length > 15) {
        console.log(`   ... and ${permissions.length - 15} more`);
    }
    
    console.log('\n✨ Done!\n');
    
} catch (error) {
    console.error('❌ Error:', error.message);
    console.error(error.stack);
    process.exit(1);
}