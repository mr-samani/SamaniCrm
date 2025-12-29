import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { AccountServiceProxy, ExternalProviderDto, SecuritySettingsServiceProxy, Verify2FARequest } from '@shared/service-proxies';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-external-provider',
  templateUrl: './external-provider.component.html',
  styleUrls: ['./external-provider.component.scss'],
})
export class CreateOrEditExternalProviderComponent extends AppComponentBase implements OnInit {
  loading = false;
  saving = false;
 form: FormGroup;
  isEditMode: boolean = false;
  showClientSecret: boolean = false;

  providerTypes = [
    { value: ExternalProviderTypeEnum.OpenIdConnect, label: 'OpenID Connect' },
    { value: ExternalProviderTypeEnum.OAuth2, label: 'OAuth 2.0' },
    { value: ExternalProviderTypeEnum.Google, label: 'Google' },
    { value: ExternalProviderTypeEnum.Microsoft, label: 'Microsoft' },
    { value: ExternalProviderTypeEnum.GitHub, label: 'GitHub' },
    { value: ExternalProviderTypeEnum.LinkedIn, label: 'LinkedIn' }
  ];

  responseTypes = [
    { value: 'code', label: 'Authorization Code (code)' },
    { value: 'id_token', label: 'Implicit (id_token)' },
    { value: 'id_token token', label: 'Implicit (id_token token)' },
    { value: 'code id_token', label: 'Hybrid (code id_token)' },
    { value: 'code token', label: 'Hybrid (code token)' },
    { value: 'code id_token token', label: 'Hybrid (code id_token token)' }
  ];

  responseModes = [
    { value: 'query', label: 'Query' },
    { value: 'fragment', label: 'Fragment' },
    { value: 'form_post', label: 'Form Post' }
  ];

  constructor(
    injector: Injector,
    @Inject(MAT_DIALOG_DATA) _data: { provider?: ExternalProviderDto },
    private dialogRef: MatDialogRef<CreateOrEditExternalProviderComponent>,
        private securitySettingsService: SecuritySettingsServiceProxy,
  ) {
    super(injector);
     this.isEditMode = !!_data?.provider;
  }

 ngOnInit(): void {
    this.initForm();
    
    if (this.isEditMode && this.data.provider) {
      this.loadProvider();
    }
  }

  private initForm(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(255)]],
      displayName: ['', [Validators.required, Validators.maxLength(255)]],
      scheme: ['', Validators.maxLength(255)],
      providerType: [ExternalProviderTypeEnum.OpenIdConnect, Validators.required],
      clientId: ['', [Validators.required, Validators.maxLength(255)]],
      clientSecret: ['', this.isEditMode ? [] : [Validators.required, Validators.maxLength(500)]],
      authorizationEndpoint: ['', [Validators.required, Validators.maxLength(255), Validators.pattern('https?://.+')]],
      tokenEndpoint: ['', [Validators.required, Validators.maxLength(255), Validators.pattern('https?://.+')]],
      userInfoEndpoint: ['', [Validators.required, Validators.maxLength(255), Validators.pattern('https?://.+')]],
      callbackPath: ['', Validators.maxLength(255)],
      logoutEndpoint: ['', [Validators.maxLength(255), Validators.pattern('https?://.+')]],
      scopes: ['openid profile email', [Validators.required, Validators.maxLength(500)]],
      responseType: ['code', Validators.required],
      responseMode: ['query', Validators.required],
      usePkce: [true],
      metadataJson: ['']
    });
  }

  private loadProvider(): void {
    const provider = this.data.provider!;
    this.form.patchValue({
      name: provider.name,
      displayName: provider.displayName,
      scheme: provider.scheme,
      providerType: provider.providerType,
      clientId: provider.clientId,
      authorizationEndpoint: provider.authorizationEndpoint,
      tokenEndpoint: provider.tokenEndpoint,
      userInfoEndpoint: provider.userInfoEndpoint,
      callbackPath: provider.callbackPath,
      logoutEndpoint: provider.logoutEndpoint,
      scopes: provider.scopes,
      responseType: provider.responseType,
      responseMode: provider.responseMode,
      usePkce: provider.usePkce,
      metadataJson: provider.metadataJson
    });
  }

  toggleClientSecretVisibility(): void {
    this.showClientSecret = !this.showClientSecret;
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notify.warning(this.l('CompleteFormFields'));
      return;
    }
     

    this.saving = true;
    const formValue = this.form.value;

    if (this.isEditMode) {
      const dto: UpdateExternalProviderDto = {
        id: this.data.provider!.id,
        ...formValue
      };
      
      this.service.update(dto.id, dto)
            .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: () => {
          this.dialogRef.close(true);
        },
        error: (err) => {
          console.error('Error updating provider:', err);
        }
      });
    } else {
      const dto: CreateExternalProviderDto = formValue;
      
      this.service.create(dto)
            .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: () => {
          this.dialogRef.close(true);
        },
        error: (err) => {
          console.error('Error creating provider:', err);
        }
      });
    }
  }




}
