import { Injectable, signal, computed } from '@angular/core';
import { BlockDefinition } from '../blocks/block-registry';
import { cloneDeep, isEqual } from 'lodash-es';

type HistoryAction = 'add' | 'delete' | 'edit';

interface HistoryState {
  snapshot: BlockDefinition;
  description?: string;
  timestamp: number;
  action: HistoryAction;
}

@Injectable()
export class HistoryService {
  private _history = signal<HistoryState[]>([]);
  private _currentIndex = signal<number>(-1);

  readonly canUndo = computed(() => this._currentIndex() >= 0);
  readonly canRedo = computed(() => this._currentIndex() < this._history().length - 1);

  /** وضعیت فعلی بلاک‌ها */
  readonly currentState = computed(() => this._history()[this._currentIndex()]?.snapshot ?? []);

  /**
   * ذخیره وضعیت جدید فقط اگر تغییر واقعی وجود داشته باشد
   */
  save(action: HistoryAction, state: BlockDefinition, description?: string) {
    const clone = cloneDeep(state);
    const last = this._history()[this._currentIndex()];
    if (last && isEqual(last.snapshot, clone)) return;

    const newHistory = this._history().slice(0, this._currentIndex() + 1);
    newHistory.push({
      action,
      snapshot: clone,
      description,
      timestamp: Date.now(),
    });

    this._history.set(newHistory);
    this._currentIndex.set(newHistory.length - 1);
  }

  /**
   * بازگردانی وضعیت قبلی
   */
  undo(currentBlocks: BlockDefinition[]): BlockDefinition[] {
    if (!this.canUndo()) return currentBlocks;

    this._currentIndex.set(this._currentIndex() - 1);
    return this._applySnapshot(currentBlocks, this.currentState());
  }

  /**
   * بازگردانی وضعیت بعدی
   */
  redo(currentBlocks: BlockDefinition[]): BlockDefinition[] {
    if (!this.canRedo()) return currentBlocks;

    this._currentIndex.set(this._currentIndex() + 1);
    return this._applySnapshot(currentBlocks, this.currentState());
  }

  clear() {
    this._history.set([]);
    this._currentIndex.set(-1);
  }

  getHistory(): HistoryState[] {
    return this._history();
  }

  /**
   * اعمال وضعیت جدید روی لیست فعلی
   */
  private _applySnapshot(current: BlockDefinition[], next: BlockDefinition | null): BlockDefinition[] {
    debugger;
    const updated = cloneDeep(current);

    const findIndexById = (
      blocks: BlockDefinition[],
      id: string,
    ): { parent: BlockDefinition | null; index: number; container: BlockDefinition[] } | null => {
      for (let i = 0; i < blocks.length; i++) {
        if (blocks[i].id === id) {
          return { parent: null, index: i, container: blocks };
        }
        if (blocks[i].children?.length) {
          const result = deepFind(blocks[i], blocks[i].children!, id);
          if (result) return result;
        }
      }
      return null;
    };

    const deepFind = (
      parent: BlockDefinition,
      children: BlockDefinition[],
      id: string,
    ): { parent: BlockDefinition; index: number; container: BlockDefinition[] } | null => {
      for (let i = 0; i < children.length; i++) {
        if (children[i].id === id) {
          return { parent, index: i, container: children };
        }
        if (children[i].children?.length) {
          const result = deepFind(children[i], children[i].children!, id);
          if (result) return result;
        }
      }
      return null;
    };

    if (!next) {
      // اگر بلاک حذف شده
      // باید آن را پیدا کرده و حذف کنیم
      console.warn('History snapshot is null – deletion expected. Skipping.');
      return updated;
    }

    const target = findIndexById(updated, next.id!);

    if (!target) {
      // بلاک در حال حاضر وجود ندارد ⇒ اضافه کردن
      if (next.parent?.id) {
        const parent = findIndexById(updated, next.parent.id);
        if (parent) {
          parent.container.splice(parent.index + 1, 0, cloneDeep(next));
        } else {
          console.warn('Parent not found for added block', next);
        }
      } else {
        // parentId ندارد ⇒ سطح بالا
        updated.push(cloneDeep(next));
      }
    } else {
      if (!next) {
        // حذف کردن
        target.container.splice(target.index, 1);
      } else {
        // ویرایش یا جایگزینی کامل
        if (!isEqual(target.container[target.index], next)) {
          target.container[target.index] = cloneDeep(next);
        }
      }
    }

    return updated;
  }
}
