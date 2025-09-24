import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TreeViewModel } from './tree-view.model';

@Component({
  selector: 'tree-view',
  templateUrl: './tree-view.component.html',
  styleUrls: ['./tree-view.component.scss'],
  standalone: true,
  imports: [CommonModule],
})
export class TreeViewComponent implements OnInit {
  list: TreeViewModel[] = [];
  @Input('Items') set Items(items: TreeViewModel[]) {
    this.list = items ?? [];
    this.openOrCloseAll(this.list, true);
    this.init();
  }
  @Input() parent?: TreeViewModel;
  @Output() changeChild = new EventEmitter<TreeViewModel>();

  constructor() {}

  ngOnInit() {}

  init() {
    for (let i = 0; i < this.list.length; i++) {
      if (!this.list[i].children) {
        this.list[i].children = [];
      }
    }
  }

  onChange(item: TreeViewModel) {
    // کلیک اول
    // خودش فقط انتخاب بشه
    if (!item.selected) {
      item.selected = true;
      this.changeChild.emit(item);
      if (!item.isOpen) {
        this.openCloseFolder(item);
      }
      return;
    }
    // کلیک دوم
    // خودش و بچه هاش انتخاب بشن
    if (
      item.selected == true &&
      item.children &&
      item.children.length > 0 &&
      item.children.every((x) => x.selected) == false
    ) {
      this.toggleAllChild(item, true);
      return;
    }
    // کلیک سوم
    // خودش و بچه هاش از انتخاب خارج بشن
    if (
      item.selected === true &&
      item.children &&
      item.children.length > 0 &&
      item.children.every((x) => x.selected) == true
    ) {
      this.toggleAllChild(item, false);
      return;
    }

    if (item.selected == true && (!item.children || item.children.length === 0)) {
      item.selected = false;
      this.changeChild.emit(item);
      return;
    }
  }

  toggleAllChild(item: TreeViewModel, selected: boolean) {
    item.selected = selected;

    item.hasUnSelectedChildren = false;
    if (item.children) {
      this.checkOrUncheckAll(item.children, item.selected);
    }
  }
  checkOrUncheckAll(item: TreeViewModel[], isChecked: boolean) {
    for (let i of item) {
      i.selected = isChecked;

      i.hasUnSelectedChildren = false;
      this.changeChild.emit(i);
      if (i.children) {
        this.checkOrUncheckAll(i.children, isChecked);
      }
    }
  }

  checkChild(ev: TreeViewModel, parent: TreeViewModel) {
    //  console.log(ev, 'parent', parent);
    if (parent.children?.every((x) => x.selected)) {
      parent.selected = true;

      parent.hasUnSelectedChildren = false;
    } else if (parent.children?.some((x) => x.selected || x.hasUnSelectedChildren)) {
      parent.hasUnSelectedChildren = true;
    }
    this.changeChild.emit(this.parent);
  }

  openCloseFolder(item: TreeViewModel) {
    item.isOpen = !item.isOpen;
    if (item.isOpen && item.children.length === 0) {
      // this.getData(item);
    }
  }

  //--------------------------------------------

  openOrCloseAll(tree: TreeViewModel[], open: boolean) {
    for (let item of tree) {
      item.isOpen = open;
      if (item.children) {
        this.openOrCloseAll(item.children, open);
      }
    }
  }
}
