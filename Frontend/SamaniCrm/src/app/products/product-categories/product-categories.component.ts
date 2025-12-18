import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { finalize, Subscription } from 'rxjs';
import { ProductServiceProxy } from '@shared/service-proxies/api/product.service';
import { MatDialog } from '@angular/material/dialog';
import {
  DeleteProductCategoryCommand,
  ExportAllLocalizationValueDto,
  GetCategoriesForAdminQuery,
  ProductCategoryDto,
} from '@shared/service-proxies';
import { CreateOrEditProductCategoryComponent } from './create-or-edit/create-or-edit.component';
import { DownloadService } from '@shared/services/download.service';
import { AppConst } from '@shared/app-const';
import { JsonFileReaderService } from '@shared/services/json-file-reader.service';
export class TreeProductCategory extends ProductCategoryDto {
  override children?: TreeProductCategory[] = [];
  isOpen?: boolean;
  hasUnSelectedChildren?: boolean;
}

@Component({
  selector: 'app-product-categories',
  templateUrl: './product-categories.component.html',
  styleUrl: './product-categories.component.scss',
  standalone: false,
})
export class ProductCategoriesComponent extends AppComponentBase implements OnInit {
  loading = true;

  list: TreeProductCategory[] = [];

  listSubscription$?: Subscription;
  showFilter = false;

  baseUrl = AppConst.fileServerUrl;
  constructor(
    injector: Injector,
    private productService: ProductServiceProxy,
    private matDialog: MatDialog,
    private downloadService: DownloadService,
    private jsonFileReaderService: JsonFileReaderService,
  ) {
    super(injector);
    this.breadcrumb.list = [{ name: this.l('ProductCategories'), url: '/panel/products/categories' }];
  }

  ngOnInit(): void {
    this.getList();
  }

  ngOnDestroy(): void {
    if (this.listSubscription$) {
      this.listSubscription$.unsubscribe();
    }
  }

  getList() {
    if (this.listSubscription$) {
      this.listSubscription$.unsubscribe();
    }
    this.loading = true;
    this.list = [];
    const input = new GetCategoriesForAdminQuery();
    this.listSubscription$ = this.productService
      .getTreeProductCategoriesForAdmin(input)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.list = response.data ?? [];
      });
  }

  openCreateOrEditDialog(item?: TreeProductCategory) {
    this.matDialog
      .open(CreateOrEditProductCategoryComponent, {
        data: {
          id: item?.id,
        },
        width: '768px',
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.getList();
        }
      });
  }

  remove(item: TreeProductCategory, parent?: TreeProductCategory) {
    this.confirmMessage(`${this.l('Delete')}:${item?.title}`, this.l('AreYouSureForDelete')).then((result) => {
      if (result.isConfirmed) {
        this.showMainLoading();
        this.productService
          .deleteProductCategory(new DeleteProductCategoryCommand({ id: item.id }))
          .pipe(finalize(() => this.hideMainLoading()))
          .subscribe((response) => {
            if (response.success) {
              this.notify.success(this.l('DeletedSuccessfully'));
              if (!parent) {
                let index = this.list.findIndex((x) => x.id == item.id);
                this.list.splice(index, 1);
              } else {
                if (parent.children) {
                  let index = parent.children.findIndex((x) => x.id == item.id);
                  parent.children.splice(index, 1);
                } else {
                  this.getList();
                }
              }
            }
          });
      }
    });
  }

  exportAllLocalizations() {
    this.showMainLoading();
    this.productService
      .getAllProductCategoryTranslations()
      .pipe(finalize(() => this.hideMainLoading()))
      .subscribe((result) => {
        const data = result.data ?? {};
        this.downloadService.generateDownloadJson(data, 'category_' + AppConst.currentLanguage + '.json');
      });
  }

  importLocalizations() {
    this.jsonFileReaderService.selectAndReadJson().then((data: ExportAllLocalizationValueDto[]) => {
      console.table(data);
      try {
        this.showMainLoading();
        this.productService
          .importProductCategoryLocalization(data)
          .pipe(finalize(() => this.hideMainLoading()))
          .subscribe((result) => {
            if (result) {
              this.getList();
            }
          });
      } catch (e) {
        this.notify.error(this.l('JsonFileIsInvalid'));
      }
    });
  }

  /*-------------------------------------------------------------------*/

  onChange(item: TreeProductCategory) {
    // کلیک اول
    // خودش فقط انتخاب بشه
    if (!item.isActive) {
      item.isActive = true;
      //this.changeChild.emit(item);
      if (!item.isOpen) {
        this.openCloseFolder(item);
      }
      return;
    }
    // کلیک دوم
    // خودش و بچه هاش انتخاب بشن
    if (
      item.isActive == true &&
      item.children &&
      item.children.length > 0 &&
      item.children.every((x) => x.isActive) == false
    ) {
      this.toggleAllChild(item, true);
      return;
    }
    // کلیک سوم
    // خودش و بچه هاش از انتخاب خارج بشن
    if (
      item.isActive === true &&
      item.children &&
      item.children.length > 0 &&
      item.children.every((x) => x.isActive) == true
    ) {
      this.toggleAllChild(item, false);
      return;
    }

    if (item.isActive == true && (!item.children || item.children.length === 0)) {
      item.isActive = false;
      // this.changeChild.emit(item);
      return;
    }
  }

  toggleAllChild(item: TreeProductCategory, isActive: boolean) {
    item.isActive = isActive;

    item.hasUnSelectedChildren = false;
    if (item.children) {
      this.checkOrUncheckAll(item.children, item.isActive);
    }
  }
  checkOrUncheckAll(item: TreeProductCategory[], isChecked: boolean) {
    for (let i of item) {
      i.isActive = isChecked;

      i.hasUnSelectedChildren = false;
      // this.changeChild.emit(i);
      if (i.children) {
        this.checkOrUncheckAll(i.children, isChecked);
      }
    }
  }

  checkChild(ev: TreeProductCategory, parent: TreeProductCategory) {
    //  console.log(ev, 'parent', parent);
    if (parent.children?.every((x) => x.isActive)) {
      parent.isActive = true;

      parent.hasUnSelectedChildren = false;
    } else if (parent.children?.some((x) => x.isActive || x.hasUnSelectedChildren)) {
      parent.hasUnSelectedChildren = true;
    }
    // this.changeChild.emit(this.parent);
  }

  openCloseFolder(item: TreeProductCategory) {
    item.isOpen = !item.isOpen;
    if (item.isOpen && item.children && item.children.length === 0) {
      // this.getData(item);
    }
  }

  //--------------------------------------------

  openOrCloseAll(tree: TreeProductCategory[], open: boolean) {
    for (let item of tree) {
      item.isOpen = open;
      if (item.children) {
        this.openOrCloseAll(item.children, open);
      }
    }
  }
}
