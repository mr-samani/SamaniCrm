import { Component, EventEmitter,  Input, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import {
  GetProductAttributesQuery,
  ProductAttributeDataTypeEnum,
  ProductAttributeDto,
  ProductAttributeValueDto,
  ProductServiceProxy,
} from '@shared/service-proxies';
import { finalize } from 'rxjs';
export class ProductAttributeDtoExtended extends ProductAttributeDto {
  value?: any;
}
@Component({
  selector: 'add-or-update-product-attributes',
  templateUrl: './add-or-update-product-attributes.component.html',
  styleUrls: ['./add-or-update-product-attributes.component.scss'],
  standalone: false,
})
export class AddOrUpdateProductAttributesComponent extends AppComponentBase implements OnInit {
  @Input() set productTypeId(val: string) {
    if (val) {
      this.getAttributeList(val);
    } else {
      this.attributeList = [];
    }
  }
  @Input() attributeValues?: Array<ProductAttributeValueDto> = [];
  @Output() onChange = new EventEmitter<ProductAttributeValueDto[]>();

  loading = false;
  attributeList: ProductAttributeDtoExtended[] = [];
  constructor(
    private productService: ProductServiceProxy,
  ) {
    super();
  }

  ngOnInit() {}

  public get ProductAttributeDataTypeEnum(): typeof ProductAttributeDataTypeEnum {
    return ProductAttributeDataTypeEnum;
  }

  getAttributeList(productTypeId: string) {
    this.loading = true;
    this.productService
      .getProductAttributes(
        new GetProductAttributesQuery({
          productTypeId: productTypeId,
        }),
      )
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((result) => {
        this.attributeList = result.data?.items ?? [];
        for (let item of this.attributeList) {
          let found = this.attributeValues?.find((x) => x.attributeId == item.id);
          if (found) {
            item.value = found.value;
          }
        }
        this.changed();
      });
  }

  changed() {
    this.onChange.emit(
      this.attributeList.map((x) => {
        return new ProductAttributeValueDto({
          attributeId: x.id,
          value: x.value + '',
        });
      }),
    );
  }
}
