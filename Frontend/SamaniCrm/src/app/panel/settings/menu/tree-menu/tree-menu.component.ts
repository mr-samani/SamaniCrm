
import { Component, Inject, Injector, Input, OnDestroy, OnInit, ViewEncapsulation, DOCUMENT } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { CdkDragDrop, CdkDragMove } from '@angular/cdk/drag-drop';
import { Subject, debounceTime, finalize } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { FileManagerService } from '@app/file-manager/file-manager.service';
import { CreateOrEditMenuComponent } from '../create-or-edit/create-or-edit.component';
import { MenuServiceProxy } from '@shared/service-proxies/api/menu.service';
import { ChangeActiveMenuCommand, DeleteMenuCommand } from '@shared/service-proxies';
import { TreeNode } from '../TreeNode';

export interface DropInfo {
  targetId: string;
  action?: string;
}

@Component({
  selector: 'app-tree-menu',
  templateUrl: './tree-menu.component.html',
  styleUrl: './tree-menu.component.scss',
  encapsulation: ViewEncapsulation.None,
  standalone: false,
})
export class TreeMenuComponent extends AppComponentBase implements OnInit, OnDestroy {
  nodes: TreeNode[] = [];
  isExpandAll = false;
  @Input() set list(val: TreeNode[]) {
    this.nodes = val ?? [];
    this.isExpandAll = false;
    this.toggleAll(this.nodes);
    this.isExpandAll = !this.isExpandAll;
    this.prepareDragDrop(this.nodes);
  }

  // ids for connected drop lists
  dropTargetIds: string[] = [];
  nodeLookup: any = {};
  dropActionTodo?: DropInfo;

  dragMoveSubject = new Subject<CdkDragMove<any>>();
  loading?: boolean;
  constructor(
    injector: Injector,
    @Inject(DOCUMENT) private document: Document,
    private dialog: MatDialog,
    private fileManagerService: FileManagerService,
    private menuService: MenuServiceProxy,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.dragMoveSubject.pipe(debounceTime(100)).subscribe((ev: CdkDragMove<any>) => this.onDragMoved(ev));
  }

  ngOnDestroy() {
    this.dragMoveSubject.complete();
  }

  reload() {
    this.loading = true;
    this.menuService
      .getAllMenus()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.list = response.data ?? ([] as any);
      });
  }

  prepareDragDrop(nodes: TreeNode[]) {
    nodes.forEach((node) => {
      this.dropTargetIds.push(node.id!);
      this.nodeLookup[node.id!] = node;
      this.prepareDragDrop(node.children);
    });
  }

  dragMoved(event: CdkDragMove<any>) {
    this.dragMoveSubject.next(event);
  }
  onDragMoved(event: CdkDragMove<any>) {
    let e = this.document.elementFromPoint(event.pointerPosition.x, event.pointerPosition.y);

    if (!e) {
      this.clearDragInfo();
      return;
    }
    let container = e.classList.contains('node-item') ? e : e.closest('.node-item');
    if (!container) {
      this.clearDragInfo();
      return;
    }
    this.dropActionTodo = {
      targetId: container.getAttribute('data-id') ?? '',
    };
    const targetRect = container.getBoundingClientRect();
    const oneThird = targetRect.height / 3;

    if (event.pointerPosition.y - targetRect.top < oneThird) {
      // before
      this.dropActionTodo['action'] = 'before';
    } else if (event.pointerPosition.y - targetRect.top > 2 * oneThird) {
      // after
      this.dropActionTodo['action'] = 'after';
    } else {
      // inside
      this.dropActionTodo['action'] = 'inside';
    }
    this.showDragInfo();
  }

  drop(event: CdkDragDrop<any, any, any>) {
    if (!this.dropActionTodo) return;

    const draggedItemId = event.item.data;
    const parentItemId = event.previousContainer.id;
    const targetListId = this.getParentNodeId(this.dropActionTodo.targetId, this.nodes, 'main');

    // console.log(
    //   '\nmoving\n[' + draggedItemId + '] from list [' + parentItemId + ']',
    //   '\n[' + this.dropActionTodo.action + ']\n[' + this.dropActionTodo.targetId + '] from list [' + targetListId + ']');

    const draggedItem = this.nodeLookup[draggedItemId];

    const oldItemContainer = parentItemId != 'main' ? this.nodeLookup[parentItemId].children : this.nodes;
    const newContainer = targetListId != 'main' ? this.nodeLookup[targetListId].children : this.nodes;

    let i = oldItemContainer.findIndex((c: { id: any }) => c.id === draggedItemId);
    oldItemContainer.splice(i, 1);

    switch (this.dropActionTodo.action) {
      case 'before':
      case 'after':
        const targetIndex = newContainer.findIndex(
          (c: { id: string | undefined }) => c.id === this.dropActionTodo?.targetId,
        );
        if (this.dropActionTodo.action == 'before') {
          newContainer.splice(targetIndex, 0, draggedItem);
        } else {
          newContainer.splice(targetIndex + 1, 0, draggedItem);
        }
        break;

      case 'inside':
        this.nodeLookup[this.dropActionTodo.targetId].children.push(draggedItem);
        this.nodeLookup[this.dropActionTodo.targetId].isExpanded = true;
        break;
    }

    this.clearDragInfo(true);
  }
  getParentNodeId(id: string, nodesToSearch: TreeNode[], parentId: string): string {
    for (let node of nodesToSearch) {
      if (node.id == id) return parentId;
      let ret = this.getParentNodeId(id, node.children, node.id!);
      if (ret) return ret;
    }
    return '';
  }
  showDragInfo() {
    this.clearDragInfo();
    if (this.dropActionTodo) {
      this.document
        .getElementById('node-' + this.dropActionTodo.targetId)!
        .classList.add('drop-' + this.dropActionTodo.action);
    }
  }
  clearDragInfo(dropped = false) {
    if (dropped) {
      this.dropActionTodo = undefined;
    }
    this.document.querySelectorAll('.drop-before').forEach((element) => element.classList.remove('drop-before'));
    this.document.querySelectorAll('.drop-after').forEach((element) => element.classList.remove('drop-after'));
    this.document.querySelectorAll('.drop-inside').forEach((element) => element.classList.remove('drop-inside'));
  }

  openCreateOrEditDialog(item?: TreeNode) {
    this.dialog
      .open(CreateOrEditMenuComponent, {
        data: {
          id: item?.id,
        },
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.reload();
        }
      });
  }

  delete(item: TreeNode) {
    this.confirmMessage('AreYouSureDelete', item.title ?? '').then((r) => {
      if (r.isConfirmed) {
        const input = new DeleteMenuCommand();
        input.id = item.id;
        this.menuService.deleteMenu(input).subscribe((response) => {
          this.notify.success('DeleteSuccessfully');
          this.deleteFromTree(this.nodes, item.id!);
        });
      }
    });
  }

  deleteFromTree(tree: TreeNode[], id: string) {
    for (let i = 0; i < tree.length; i++) {
      if (tree[i].id == id) {
        tree.splice(i, 1);
        return;
      }
      if (tree[i].children) {
        this.deleteFromTree(tree[i].children, id);
      }
    }
  }

  toggleAll(tree: TreeNode[]) {
    for (let m of tree) {
      m.isExpanded = !this.isExpandAll;
      if (m.children) this.toggleAll(m.children);
    }
  }

  changeActive(item: TreeNode) {
    item.loading = true;
    const input = new ChangeActiveMenuCommand();
    input.id = item.id;
    input.isActive = !item.isActive;
    this.menuService
      .activeOrDeactive(input)
      .pipe(finalize(() => (item.loading = false)))
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.notify.success(this.l('SaveSuccessFully'));
            item.isActive = !item.isActive;
          }
        },
        error: (err) => {},
      });
  }

  stopPagination(ev: Event) {
    ev.stopPropagation();
  }
}
