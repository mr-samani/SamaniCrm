import { Component, EventEmitter, Injector, Input, OnInit, Output, forwardRef } from '@angular/core';
import {
  NG_VALUE_ACCESSOR,
  NG_VALIDATORS,
  ControlValueAccessor,
  Validator,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { FileManagerService } from '@app/file-manager/file-manager.service';
import { FileManagerDto } from '@app/file-manager/models/file-manager-dto';
import { FileManagerOptions, IOptions } from '@app/file-manager/options.interface';
import { AppConst } from '@shared/app-const';

@Component({
  selector: 'file-selector',
  templateUrl: './file-selector.component.html',
  styleUrls: ['./file-selector.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => FileSelectorComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: FileSelectorComponent,
    },
  ],
  standalone: false,
})
export class FileSelectorComponent extends AppComponentBase implements OnInit, ControlValueAccessor, Validator {
  _options: IOptions = new FileManagerOptions();
  @Input() set options(config: IOptions) {
    this._options = { ...this._options, ...config };
  }
  @Output() change = new EventEmitter<FileManagerDto>();

  loading = false;
  disabled = false;
  required = false;
  file?: FileManagerDto;
  fileInfo?: FileManagerDto;
  fileServerUrl = AppConst.fileServerUrl;

  private _onChange = (t: FileManagerDto) => {};
  private _onTouched = () => {};
  constructor(
    injector: Injector,
    private fileManagerService: FileManagerService,
  ) {
    super(injector);
  }

  ngOnInit(): void {}

  validate(control: AbstractControl): ValidationErrors | null {
    const content = control.value;
    if (!this.file && this.required) {
      return { required: true };
    } else {
      return null;
    }
  }

  writeValue(value: any): void {
    if (value && value !== this.file) {
      this.file = value;
      this.getInfo();
    }
  }
  registerOnChange(fn: any): void {
    this._onChange = fn;
  }
  registerOnTouched(fn: any): void {
    this._onTouched = fn;
  }
  setDisabledState?(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  openFileManager() {
    this.fileManagerService.openFileManager(this._options).then((result) => {
      this.file = result;
      this.getInfo();
      this._onChange(this.file);
      this.change.emit(this.file);
    });
  }

  getInfo() {
    if (!this.file || !this._options.showPreview) {
      return;
    }
    this.fileInfo = this.file;
    // this.loading = true;
    // this.fileManagerProxy.getInfo(this.fileId)
    //   .pipe(finalize(() => this.loading = false))
    //   .subscribe(response => {
    //     this.fileInfo = response.data;
    //   })
  }
}
