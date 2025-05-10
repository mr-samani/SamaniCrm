import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import { CustomValidators } from '@shared/custom-validator/form-validation';
import {
  CreateUserCommand,
  PasswordComplexityDTO,
  RoleResponseDTO,
  RoleServiceProxy,
  SecuritySettingsServiceProxy,
  UserServiceProxy,
} from '@shared/service-proxies';
import { forkJoin } from 'rxjs';
import { finalize } from 'rxjs/operators';

export class SelectableRole extends RoleResponseDTO {
  selected?: boolean;
}

@Component({
  selector: 'app-create-user',
  templateUrl: './create-user.component.html',
  styleUrls: ['./create-user.component.scss'],
  standalone: false,
})
export class CreateUserComponent extends AppComponentBase implements OnInit {
  loading = true;
  saving = false;
  form: FormGroup;
  roles: SelectableRole[] = [];
  languageList = AppConst.languageList;
  passwordPolicy?: PasswordComplexityDTO;
  constructor(
    injector: Injector,
    private dialogRef: MatDialogRef<CreateUserCommand>,
    @Inject(MAT_DIALOG_DATA) _data: any,
    private userService: UserServiceProxy,
    private roleService: RoleServiceProxy,
    private securitySettingsService: SecuritySettingsServiceProxy,
  ) {
    super(injector);
    this.form = this.fb.group({
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      userName: ['', [Validators.required, CustomValidators.checkEnglishAndNumberCharacters]],
      email: ['', [Validators.required, CustomValidators.checkEmail]],
      phoneNumber: ['', [Validators.required]],
      password: ['', [Validators.required]],
      lang: [AppConst.currentLanguage],
    });
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
          }
          this.passwordPolicy = passwordComplexity.data;
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
}
