import { Component, Inject,  OnInit } from '@angular/core';
import { FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { TranslateModule } from '@ngx-translate/core';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import {
  CreateExternalProviderCommand,
  ExternalProvidersServiceProxy,
  UpdateExternalProviderCommand,
} from '@shared/service-proxies';
import { ExternalProviderDto } from '@shared/service-proxies/model/external-provider-dto';
import { ExternalProviderTypeEnum } from '@shared/service-proxies/model/external-provider-type-enum';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-external-provider',
  templateUrl: './external-provider.component.html',
  styleUrls: ['./external-provider.component.scss'],
  imports: [ReactiveFormsModule, MaterialCommonModule, TranslateModule],
  providers: [ExternalProvidersServiceProxy],
})
export class CreateOrEditExternalProviderComponent extends AppComponentBase implements OnInit {
  loading = false;
  saving = false;
  form!: FormGroup;
  isEditMode: boolean = false;
  showClientSecret: boolean = false;

  providerTypes = [
    { value: ExternalProviderTypeEnum.OpenIdConnect, label: 'OpenID Connect' },
    { value: ExternalProviderTypeEnum.OAuth2, label: 'OAuth 2.0' },
    { value: ExternalProviderTypeEnum.Google, label: 'Google' },
    { value: ExternalProviderTypeEnum.Microsoft, label: 'Microsoft' },
    { value: ExternalProviderTypeEnum.GitHub, label: 'GitHub' },
    { value: ExternalProviderTypeEnum.LinkedIn, label: 'LinkedIn' },
  ];

  responseTypes = [
    { value: 'code', label: 'Authorization Code (code)' },
    { value: 'id_token', label: 'Implicit (id_token)' },
    { value: 'id_token token', label: 'Implicit (id_token token)' },
    { value: 'code id_token', label: 'Hybrid (code id_token)' },
    { value: 'code token', label: 'Hybrid (code token)' },
    { value: 'code id_token token', label: 'Hybrid (code id_token token)' },
  ];

  responseModes = [
    { value: 'query', label: 'Query' },
    { value: 'fragment', label: 'Fragment' },
    { value: 'form_post', label: 'Form Post' },
  ];

  providerId = '';

  constructor(
    @Inject(MAT_DIALOG_DATA) _data: { provider?: ExternalProviderDto },
    private dialogRef: MatDialogRef<CreateOrEditExternalProviderComponent>,
    private service: ExternalProvidersServiceProxy,
  ) {
    super();
    this.isEditMode = !!_data?.provider;
    if (this.isEditMode && _data.provider) {
      this.providerId = _data.provider.id!;
      this.loadProvider(_data.provider.id!);
    }
    this.initForm();
  }

  ngOnInit(): void {}

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
      metadataJson: [''],
    });
  }

  private loadProvider(id: string): void {
    this.loading = true;
    this.service
      .getById(id)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (result) => {
          if (result.data) {
            let provider = result.data;
            this.form.patchValue({
              name: provider.name,
              displayName: provider.displayName,
              scheme: provider.scheme,
              providerType: provider.providerType,
              clientId: provider.clientId,
              clientSecret: provider.clientSecret,
              authorizationEndpoint: provider.authorizationEndpoint,
              tokenEndpoint: provider.tokenEndpoint,
              userInfoEndpoint: provider.userInfoEndpoint,
              callbackPath: provider.callbackPath,
              logoutEndpoint: provider.logoutEndpoint,
              scopes: provider.scopes,
              responseType: provider.responseType,
              responseMode: provider.responseMode,
              usePkce: provider.usePkce,
              metadataJson: provider.metadataJson,
            });
          } else {
            console.error('Error updating provider:', result);
            this.dialogRef.close();
          }
        },
        error: (err) => {
          console.error('Error updating provider:', err);
          this.dialogRef.close();
        },
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
      const input = new UpdateExternalProviderCommand();
      input.init(formValue);
      input.id = this.providerId;
      this.service
        .update(input)
        .pipe(finalize(() => (this.saving = false)))
        .subscribe({
          next: () => {
            this.dialogRef.close(true);
          },
          error: (err) => {
            console.error('Error updating provider:', err);
          },
        });
    } else {
      const input = new CreateExternalProviderCommand();
      input.init(formValue);

      this.service
        .create(input)
        .pipe(finalize(() => (this.saving = false)))
        .subscribe({
          next: () => {
            this.dialogRef.close(true);
          },
          error: (err) => {
            console.error('Error creating provider:', err);
          },
        });
    }
  }
}
