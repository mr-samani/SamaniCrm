import { FormTools, BlockDefinition } from '../blocks/block-registry';

export function createTreeFormTools(definitios: BlockDefinition[]): FormTools[] {
  let tree: FormTools[] = [];
  for (let item of definitios) {
    if (!item.category) item.category = 'Other';

    let index = tree.findIndex((x) => x.category == item.category);
    if (index > -1) {
      tree[index].items.push(item);
    } else {
      tree.push({
        category: item.category,
        items: [item],
      });
    }
  }
  return tree;
}
