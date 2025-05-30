import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import { CustomValidators } from '@shared/custom-validator/form-validation';
import {
  CreateUserCommand,
  EditUserCommand,
  PasswordComplexityDTO,
  RoleDTO,
  RoleServiceProxy,
  SecuritySettingsServiceProxy,
  UserDTO,
  UserServiceProxy,
} from '@shared/service-proxies';
import { forkJoin } from 'rxjs';
import { finalize } from 'rxjs/operators';

export class SelectableRole extends RoleDTO {
  selected?: boolean;
}

@Component({
  selector: 'app-create-or-edit-user',
  templateUrl: './create-or-edit-user.component.html',
  styleUrls: ['./create-or-edit-user.component.scss'],
  standalone: false,
})
export class CreateOrEditUserComponent extends AppComponentBase implements OnInit {
  isUpdate = false;
  loading = true;
  saving = false;
  form: FormGroup;
  roles: SelectableRole[] = [];
  languageList = AppConst.languageList;
  passwordPolicy?: PasswordComplexityDTO;
  constructor(
    injector: Injector,
    private dialogRef: MatDialogRef<CreateUserCommand>,
    @Inject(MAT_DIALOG_DATA) private _data: { user?: UserDTO },
    private userService: UserServiceProxy,
    private roleService: RoleServiceProxy,
    private securitySettingsService: SecuritySettingsServiceProxy,
  ) {
    super(injector);
    this.form = this.fb.group({
      id: [''],
      userName: ['', [Validators.required, CustomValidators.checkEnglishAndNumberCharacters]],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.required, CustomValidators.checkEmail]],
      phoneNumber: ['', [Validators.required]],
      password: ['', [Validators.required]],
      lang: [AppConst.currentLanguage],
      address: [''],
    });
    if (_data.user) {
      this.isUpdate = true;
      this.form.patchValue(_data.user);
      this.form.get('userName')?.disable();
      this.form.get('password')?.clearValidators();
      this.form.get('password')?.updateValueAndValidity();
    }
  }

  ngOnInit() {
    this.loading = true;

    forkJoin([this.roleService.getAllRoles(), this.securitySettingsService.getPasswordComplexity()])
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: ([roles, passwordComplexity]) => {
          this.roles = roles.data ?? [];
          if (this.roles.length === 0) {
            this.notify.warning(this.l('NotExistRoles!'));
            this.dialogRef.close();
            return;
          }
          this.passwordPolicy = passwordComplexity.data;
          if (this.isUpdate) {
            for (let r of this.roles) {
              if (
                this._data.user &&
                this._data.user.roles &&
                this._data.user.roles.findIndex((x) => x == r.roleName) > -1
              ) {
                r.selected = true;
              } else {
                r.selected = false;
              }
            }
          }
        },
        error: () => {
          this.dialogRef.close();
        },
      });
  }

  save() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notify.warning(this.l('CompleteFormField'));
      return;
    }
    if (this.isUpdate) {
      this.updateUser();
    } else {
      this.createNewUser();
    }
  }

  createNewUser() {
    const input = new CreateUserCommand();
    input.init(this.form.value);
    input.roles = [];
    for (let item of this.roles) {
      if (item.selected) {
        input.roles.push(item.roleName!);
      }
    }
    if (input.roles.length === 0) {
      this.notify.warning(this.l('NotSelectedAnyRoleForUser'));
      return;
    }

    this.saving = true;
    this.userService
      .createUser(input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.notify.success(this.l('SaveSuccessFully'));
            this.dialogRef.close(true);
          }
        },
      });
  }

  updateUser() {
    const input = new EditUserCommand();
    input.init(this.form.value);
    input.roles = [];
    for (let item of this.roles) {
      if (item.selected) {
        input.roles.push(item.roleName!);
      }
    }
    if (input.roles.length === 0) {
      this.notify.warning(this.l('NotSelectedAnyRoleForUser'));
      return;
    }
    this.saving = true;
    this.userService
      .updateUser(input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.notify.success(this.l('SaveSuccessFully'));
            this.dialogRef.close(true);
          }
        },
      });
  }
}
