import { Component, Injector, OnInit } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { CustomValidators } from '@shared/custom-validator/form-validation';
import { TenantsServiceProxy } from '@shared/service-proxies/api/tenants.service';
import { CreateTenantCommand } from '@shared/service-proxies/model/create-tenant-command';
import { DatabaseStrategy } from '@shared/service-proxies/model/database-strategy';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-create-tenant',
  templateUrl: './create-tenant.component.html',
  styleUrls: ['./create-tenant.component.scss'],
  standalone: false,
})
export class CreateTenantComponent extends AppComponentBase implements OnInit {
  form: FormGroup;
  loading = false;
  isUpdate = false;
  saving = false;
  id: string;
  allowedIpAddresses?: string[] = [];
  featureFlags?: { [key: string]: boolean };

  constructor(private tenantService: TenantsServiceProxy) {
    super();
    this.breadcrumb.list = [{ name: this.l('Tenants'), url: '/panel/tenants' }];

    this.form = this.fb.group({
      name: [null, [Validators.required]],
      slug: [null, [Validators.required]],
      email: [null, [Validators.required, CustomValidators.email]],
      phone: [null, [Validators.required, CustomValidators.phone]],
      mobile: [null, [Validators.required, CustomValidators.mobile]],
      legalName: [null, [Validators.required]],

      adminFirstName: [null, [Validators.required]],
      adminLastName: [null, [Validators.required]],
      adminEmail: [null, [Validators.required]],
      adminUserName: [null, [Validators.required]],
      adminPassword: [null, [Validators.required]],
      adminMobile: [null, [Validators.required]],

      registrationNumber: [null, []],
      taxId: [null, []],
      website: [null, []],
      country: ['Iran', []],
      city: [null, []],
      address: [null, []],
      postalCode: [null, []],
      latitude: [null, []],
      longitude: [null, []],

      databaseStrategy: [DatabaseStrategy.Shared, []],
      subscriptionPlan: [null, []],
      maxUsers: [100, []],
      maxStorageMB: [1024, []],
      isTrial: [false, []],
      trialDays: [0, []],

      primaryColor: [''],
      secondaryColor: [''],

      require2FA: [false],
      sessionTimeoutMinutes: [60],
      passwordMinLength: [6],
      passwordRequireSpecialChar: [false],
    });

    this.id = this.route.snapshot.params['id'];

    if (this.id) {
      this.isUpdate = true;
      this.getForEdit(this.id);
    } else {
      this.isUpdate = false;
      this.getForCreate();
    }
  }

  ngOnInit(): void {}

  getForCreate() {}

  getForEdit(id: string) {
    this.loading = true;
    this.tenantService
      .getTenantById(id)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.chdr.detectChanges();
        }),
      )
      .subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.form.patchValue(response.data);
          } else {
            window.history.back();
          }
        },
        error: (err) => {
          window.history.back();
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
    const formValue = this.form.value;
    const input = new CreateTenantCommand();
    input.init(formValue);
    input.featureFlags = this.featureFlags;
    input.allowedIpAddresses = this.allowedIpAddresses;

    this.tenantService
      .createTenant(input)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.chdr.detectChanges();
        }),
      )
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.notify.success(this.l('SaveSuccessFully'));
            this.router.navigate(['/panel/tenants']);
          }
        },
      });
  }
}
