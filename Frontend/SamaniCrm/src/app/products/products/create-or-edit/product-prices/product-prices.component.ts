import { Component, EventEmitter, Injector, Input, OnInit, Output } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import {
  CurrencyDto,
  PriceTypeEnum,
  ProductPriceDto,
  ProductServiceProxy,
  StringAutoCompleteDto,
} from '@shared/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'product-prices',
  templateUrl: './product-prices.component.html',
  styleUrls: ['./product-prices.component.scss'],
  standalone: false,
})
export class ProductPricesComponent extends AppComponentBase implements OnInit {
  @Input() prices: ProductPriceDto[] = [];
  @Output() onChange = new EventEmitter<ProductPriceDto[]>();

  currencyList: StringAutoCompleteDto[] = [];
  loadingCurrency = true;

  form: FormGroup;
  constructor(
    injector: Injector,
    private productService: ProductServiceProxy,
  ) {
    super(injector);
    this.form = this.fb.group({
      currency: ['', Validators.required],
      price: ['', Validators.required],
      type: [PriceTypeEnum.Reqular, Validators.required],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
    });
  }

  ngOnInit() {
    this.getCurrencies();
  }

  public get PriceTypeEnum(): typeof PriceTypeEnum {
    return PriceTypeEnum;
  }

  getCurrencies() {
    this.loadingCurrency = true;
    this.productService
      .getActiveCurrencies()
      .pipe(finalize(() => (this.loadingCurrency = false)))
      .subscribe((response) => {
        this.currencyList = response.data ?? [];
      });
  }

  save() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notify.warning(this.l('CompleteFormField'));
      return;
    }
    this.prices.unshift(this.form.value);
    this.onChange.emit(this.prices);
    this.form.reset();
  }

  remove(i: number) {
    this.confirmMessage(`${this.l('Delete')}:${this.prices[i].price}`, this.l('AreYouSureForDelete')).then((result) => {
      if (result.isConfirmed) {
        this.prices.splice(i, 1);
        this.onChange.emit(this.prices);
      }
    });
  }
}
