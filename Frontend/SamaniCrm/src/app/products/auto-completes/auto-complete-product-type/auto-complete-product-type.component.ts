import { CommonModule } from '@angular/common';
import { Component, EventEmitter, forwardRef, Injector, Input, OnDestroy, OnInit, Output } from '@angular/core';
import {
  ReactiveFormsModule,
  NG_VALUE_ACCESSOR,
  NG_VALIDATORS,
  ControlValueAccessor,
  Validator,
  FormControl,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { MatAutocompleteModule, MatAutocompleteTrigger } from '@angular/material/autocomplete';
import { AppComponentBase } from '@app/app-component-base';
import { TranslateModule } from '@ngx-translate/core';
import { RequireMatch } from '@shared/custom-validator/requireMatch';
import { isNullOrEmpty } from '@shared/helper/null-or-empty';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { ProductServiceProxy, GuidAutoCompleteDto } from '@shared/service-proxies';
import { Observable, startWith, debounceTime, distinctUntilChanged, switchMap, of, finalize, map } from 'rxjs';

@Component({
  selector: 'app-auto-complete-product-type',
  templateUrl: './auto-complete-product-type.component.html',
  styleUrls: ['./auto-complete-product-type.component.scss'],
  standalone: true,
  imports: [CommonModule, MaterialCommonModule, MatAutocompleteModule, ReactiveFormsModule, TranslateModule],
  providers: [
    ProductServiceProxy,
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AutoCompleteProductTypeComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: AutoCompleteProductTypeComponent,
    },
  ],
})
export class AutoCompleteProductTypeComponent
  extends AppComponentBase
  implements OnInit, OnDestroy, ControlValueAccessor, Validator
{
  @Input() theme: 'material' | 'bootstrap' = 'material';
  @Output('selectionChange') selectionChange = new EventEmitter<GuidAutoCompleteDto>();
  myControl = new FormControl<GuidAutoCompleteDto>(new GuidAutoCompleteDto());
  filteredOptions = new Observable<GuidAutoCompleteDto[]>();
  loading = true;
  isEmpty: boolean = false;
  private _onChange: (val: GuidAutoCompleteDto | undefined) => void = () => {};
  private _onChangeValidate: () => void = () => {};
  private _onTouched: () => void = () => {};
  disabled = false;
  constructor(
    injector: Injector,
    private productService: ProductServiceProxy,
  ) {
    super(injector);
    this.myControl.setValidators([RequireMatch]);
  }

  ngOnInit(): void {
    this.update();
  }

  public update() {
    this.filteredOptions = this.myControl.valueChanges.pipe(
      startWith(''),
      debounceTime(200),
      distinctUntilChanged(),
      switchMap((value: any) => {
        return this._filter(value);
      }),
    );
  }

  ngOnDestroy(): void {}

  writeValue(val: GuidAutoCompleteDto): void {
    this.myControl.setValue(val);
  }

  registerOnChange(fn: any): void {
    this._onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this._onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    this.disabled = isDisabled;
    if (isDisabled) {
      this.myControl.disable();
    } else {
      this.myControl.enable();
    }
  }

  validate(control: AbstractControl): ValidationErrors | null {
    return this.myControl.errors;
  }

  registerOnValidatorChange(fn: any): void {
    this._onChangeValidate = fn;
  }

  _filter(value: string): Observable<GuidAutoCompleteDto[]> {
    if (isNullOrEmpty(value)) {
      value = '';
    }
    let filterValue = '';
    if (typeof value === 'string') {
      filterValue = value.toLowerCase();
    } else if (typeof value === 'object') {
      return of([value]);
    }
    this.loading = true;
    return this.productService.getAutoCompleteProductType(filterValue).pipe(
      finalize(() => (this.loading = false)),
      map((data) => {
        const list = data.data ?? [];
        this.isEmpty = list.length == 0;
        return list;
      }),
    );
  }

  displayFn(opt: GuidAutoCompleteDto): string {
    return opt && opt.title ? opt.title : '';
  }
  openPanel(trigger: MatAutocompleteTrigger) {
    trigger.openPanel();
  }

  onChange() {
    const val = this.myControl.value ?? undefined;
    this._onChange(val);
    this.selectionChange.emit(val);
  }

  onChangeTextBox() {
    this.onChange();
  }

  reset() {
    this.myControl.reset();
    this.onChange();
  }
}
