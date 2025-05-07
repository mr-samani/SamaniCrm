import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { CreateUserCommand, RoleResponseDTO, RoleServiceProxy, UserServiceProxy } from '@shared/service-proxies';
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
  constructor(
    injector: Injector,
    private dialogRef: MatDialogRef<CreateUserCommand>,
    @Inject(MAT_DIALOG_DATA) _data: any,
    private userService: UserServiceProxy,
    private roleService: RoleServiceProxy,
  ) {
    super(injector);
    this.form = this.fb.group({
      fullName: ['', [Validators.required]],
      userName: ['', [Validators.required]],
      email: ['', [Validators.required]],
      password: ['', [Validators.required]],
      confirmationPassword: ['', [Validators.required]],
    });
  }

  ngOnInit() {
    this.loading = true;
    this.roleService.getAllRoles().subscribe({
      next: (response) => {
        this.roles = response.data ?? [];
        if (this.roles.length === 0) {
          this.notify.warning(this.l('NotExistRoles!'));
          this.dialogRef.close();
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
    this.saving = true;
    const input = new CreateUserCommand();
    input.init(this.form.value);
    input.roles = [];
    for (let item of this.roles) {
      if (item.selected) {
        input.roles.push(item.id!);
      }
    }
    if (input.roles.length === 0) {
      this.notify.warning(this.l('NotSelectedAnyRoleForUser'));
      return;
    }
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
