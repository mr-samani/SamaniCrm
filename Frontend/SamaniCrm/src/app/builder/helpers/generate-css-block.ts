import { BlockDefinition } from '../blocks/block-registry';

export function generateCSSFromBlocks(blocks: BlockDefinition[]): string {
  const cssRules: string[] = [];

  /**
   *
   * @param block
   */
  function traverse(block: BlockDefinition) {
    const selector = `#${block.id}`;
    const styles: string[] = [];

    if (block.data?.css) {
      styles.push(`${block.data?.css};`);
      //   for (const [key, value] of Object.entries(block.data.style)) {
      //     if (value !== undefined && value !== null && value !== '') {
      //       styles.push(`${kebabCase(key)}: ${value};`);
      //     }
      //   }
    }

    if (styles.length > 0) {
      cssRules.push(`${selector} {\n  ${styles.join('\n  ')}\n}`);
    }

    // تکرار برای فرزندان
    for (const child of block.children ?? []) {
      traverse(child);
    }
  }

  for (const block of blocks) {
    traverse(block);
  }

  return cssRules.join('\n\n');
}

function kebabCase(str: string): string {
  return str.replace(/([a-z])([A-Z])/g, '$1-$2').toLowerCase();
}
