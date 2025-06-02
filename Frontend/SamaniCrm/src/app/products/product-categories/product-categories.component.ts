import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { finalize, Subscription } from 'rxjs';
import { ProductServiceProxy } from '@shared/service-proxies/api/product.service';
import { MatDialog } from '@angular/material/dialog';
import { FormGroup } from '@angular/forms';
import { FieldsType, SortEvent } from '@shared/components/table-view/fields-type.model';
import {
  DeleteProductCategoryCommand,
  ExportAllLocalizationValueDto,
  GetCategoriesForAdminQuery,
  PagedProductCategoryDto,
} from '@shared/service-proxies';
import { PageEvent } from '@shared/components/pagination/pagination.component';
import { CreateOrEditProductCategoryComponent } from './create-or-edit/create-or-edit.component';
import { DownloadService } from '@shared/services/download.service';
import { AppConst } from '@shared/app-const';
import { JsonFileReaderService } from '@shared/services/json-file-reader.service';

@Component({
  selector: 'app-product-categories',
  templateUrl: './product-categories.component.html',
  styleUrl: './product-categories.component.scss',
  standalone: false,
})
export class ProductCategoriesComponent extends AppComponentBase implements OnInit {
  loading = true;

  list: PagedProductCategoryDto[] = [];
  totalCount = 0;

  fields: FieldsType[] = [
    { column: 'image', title: this.l('Image'), width: 100, type: 'profilePicture' },
    // { column: 'id', title: this.l('id'), width: 100 },
    { column: 'title', title: this.l('Title') },
    { column: 'description', title: this.l('Description') },
    { column: 'isActive', title: this.l('IsActive'), type: 'yesNo' },
    { column: 'hasChild', title: this.l('HasChild'), type: 'yesNo' },
    { column: 'orderIndex', title: this.l('OrderIndex') },
    { column: 'slug', title: this.l('Slug') },
    { column: 'childCount', title: this.l('ChildCount') },
    { column: 'creationTime', title: this.l('CreationTime'), type: 'dateTime' },
  ];

  form: FormGroup;
  page = 1;
  perPage = 10;
  listSubscription$?: Subscription;
  showFilter = false;

  parentId = '';
  constructor(
    injector: Injector,
    private productService: ProductServiceProxy,
    private matDialog: MatDialog,
    private downloadService: DownloadService,
    private jsonFileReaderService: JsonFileReaderService,
  ) {
    super(injector);
    this.breadcrumb.list = [{ name: this.l('ProductCategories'), url: '/dashboard/products/categories' }];
    this.form = this.fb.group({
      filter: [''],
    });

    this.route.queryParams.subscribe((p) => {
      this.parentId = p['parentId'] ?? '';
      this.page = p['page'] ?? 1;
      this.perPage = p['perPage'] ?? 10;
      this.getList();
    });
  }

  ngOnInit(): void {}

  ngOnDestroy(): void {
    if (this.listSubscription$) {
      this.listSubscription$.unsubscribe();
    }
  }

  getList(ev?: SortEvent) {
    if (this.listSubscription$) {
      this.listSubscription$.unsubscribe();
    }
    this.loading = true;
    const input = new GetCategoriesForAdminQuery();
    input.parentId = this.parentId == '' ? undefined : this.parentId;
    input.filter = this.form.get('filter')?.value;
    input.pageNumber = this.page;
    input.pageSize = this.perPage;
    input.sortBy = ev ? ev.field : '';
    input.sortDirection = ev ? ev.direction : '';
    this.listSubscription$ = this.productService
      .getCategoriesForAdmin(input)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.list = response.data?.items ?? [];
        this.totalCount = response.data?.totalCount ?? 0;
        this.breadcrumb.list = [{ name: this.l('Categories'), url: '/dashboard/products/categories' }];
        if (response.data?.breadcrumbs) {
          for (let b of response.data.breadcrumbs.reverse()) {
            if (b.id == this.parentId) {
              this.breadcrumb.list.push({ name: b.title! });
            } else {
              this.breadcrumb.list.push({
                name: b.title!,
                url: '/dashboard/products/categories',
                queryParams: {
                  parentId: b.id!,
                  page: 1 + '',
                },
              });
            }
          }
        }
      });
  }

  reload(setFirstPage = true) {
    if (setFirstPage) {
      this.page = 1;
    }
    this.onPageChange();
  }
  resetFilter() {
    this.showFilter = false;
    this.form.patchValue({ filter: '' });
    this.reload();
  }

  onPageChange(ev?: PageEvent) {
    this.getList();
    this.router.navigate(['/dashboard/products/categories'], {
      queryParams: {
        page: this.page,
        parentId: this.parentId,
      },
    });
  }

  openCreateOrEditDialog(item?: PagedProductCategoryDto) {
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
          this.reload();
        }
      });
  }

  remove(item: PagedProductCategoryDto) {
    this.confirmMessage(`${this.l('Delete')}:${item?.title}`, this.l('AreUseSureForDelete')).then((result) => {
      if (result.isConfirmed) {
        this.showMainLoading();
        this.productService
          .deleteProductCategory(new DeleteProductCategoryCommand({ id: item.id }))
          .pipe(finalize(() => this.hideMainLoading()))
          .subscribe((response) => {
            if (response.success) {
              this.notify.success(this.l('DeletedSuccessfully'));
              this.reload();
            }
          });
      }
    });
  }

  viewSubCategories(item: PagedProductCategoryDto) {
    this.parentId = item.id!;
    this.router.navigate(['/dashboard/products/categories'], {
      queryParams: {
        page: 1,
        parentId: this.parentId,
      },
    });
  }

  exportAllLocalizations() {
    this.showMainLoading();
    this.productService
      .getAllProductCategoryTranslations()
      .pipe(finalize(() => this.hideMainLoading()))
      .subscribe((result) => {
        const data = result.data ?? {};
        debugger;
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
              this.reload();
            }
          });
      } catch (e) {
        this.notify.error(this.l('JsonFileIsInvalid'));
      }
    });
  }
}
