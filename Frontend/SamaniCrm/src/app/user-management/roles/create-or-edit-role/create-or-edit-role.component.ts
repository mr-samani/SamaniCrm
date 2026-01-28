import { Component, Inject,  OnInit } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import { CustomValidators } from '@shared/custom-validator/form-validation';
import {
  PasswordComplexityDto,
  RoleCreateCommand,
  RoleDTO,
  RoleServiceProxy,
  UpdateRoleCommand,
} from '@shared/service-proxies';
import { forkJoin, finalize } from 'rxjs';

@Component({
  selector: 'app-create-or-edit-role',
  templateUrl: './create-or-edit-role.component.html',
  styleUrls: ['./create-or-edit-role.component.scss'],
  standalone: false,
})
export class CreateOrEditRoleComponent extends AppComponentBase implements OnInit {
  isUpdate = false;
  saving = false;
  form: FormGroup;
  constructor(
    private dialogRef: MatDialogRef<CreateOrEditRoleComponent>,
    @Inject(MAT_DIALOG_DATA) private _data: { role?: RoleDTO },
    private roleService: RoleServiceProxy,
  ) {
    super();
    this.form = this.fb.group({
      id: [''],
      roleName: ['', [Validators.required, CustomValidators.checkEnglishAndNumberCharacters]],
      // displayName: ['', []],
    });
    if (_data.role) {
      this.isUpdate = true;
      this.form.patchValue(_data.role);
    }
  }

  ngOnInit() {}

  save() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notify.warning(this.l('CompleteFormField'));
      return;
    }
    if (this.isUpdate) {
      this.updateRole();
    } else {
      this.createNewRole();
    }
  }

  createNewRole() {
    const input = new RoleCreateCommand();
    input.init(this.form.value);
    this.saving = true;
    this.roleService
      .createRole(input)
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

  updateRole() {
    const input = new UpdateRoleCommand();
    input.init(this.form.value);
    this.saving = true;
    this.roleService
      .editRole(input)
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
