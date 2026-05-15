import { Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { HubConnection } from '@microsoft/signalr';
import { AppConst } from '@shared/app-const';
import { SignalRService } from '@shared/services/signalr.service';
import {
  ProvisioningNotification,
  ProvisioningStepStatus,
  TenantProvisionStepsEnum,
} from './provisioning-notification';
import { TenantsServiceProxy } from '@shared/service-proxies/api/tenants.service';
import { finalize } from 'rxjs';
import { result } from 'lodash-es';

@Component({
  selector: 'app-provisioning-tenant',
  templateUrl: './provisioning-tenant.component.html',
  styleUrls: ['./provisioning-tenant.component.scss'],
  standalone: false,
})
export class ProvisioningTenantComponent extends AppComponentBase implements OnInit, OnDestroy {
  hubConnection?: HubConnection;
  tenantId: string;
  tenantSlug: string;

  loading = false;

  steps = [
    { name: TenantProvisionStepsEnum.CreateTenant, status: ProvisioningStepStatus.Pending },
    { name: TenantProvisionStepsEnum.CreateAdminUser, status: ProvisioningStepStatus.Pending },
    { name: TenantProvisionStepsEnum.ProvisionDatabase, status: ProvisioningStepStatus.Pending },
    { name: TenantProvisionStepsEnum.RunMigrations, status: ProvisioningStepStatus.Pending },
    { name: TenantProvisionStepsEnum.SeedData, status: ProvisioningStepStatus.Pending },
    { name: TenantProvisionStepsEnum.Finalize, status: ProvisioningStepStatus.Pending },
  ];
  constructor(
    public signalRService: SignalRService,

    private tenantService: TenantsServiceProxy,
  ) {
    super();
    this.tenantId = this.route.snapshot.params['tenantId'];
    this.tenantSlug = this.route.snapshot.params['tenantSlug'];
  }
  public get ProvisioningStepStatus(): typeof ProvisioningStepStatus {
    return ProvisioningStepStatus;
  }

  ngOnInit() {
    this.getProvisioningStatuses();

    const uri = AppConst.apiUrl + '/hubs/provisioning';

    this.signalRService.init(uri, (connection: HubConnection) => {
      this.hubConnection = connection;

      // ✅ اول گوش بده به events
      this.registerEvents();

      // ✅ بعد join گروه
      this.joinTenantGroup();
    });
  }

  ngOnDestroy(): void {
    if (this.hubConnection) {
      // ✅ قبل از بستن، از گروه خارج شو
      this.leaveTenantGroup();
      this.hubConnection.stop();
    }
  }

  getProvisioningStatuses() {
    this.loading = true;
    this.tenantService
      .getProvisioningTenantStatus(this.tenantId)
      .pipe(finalize(() => (this.loading = false)))

      .subscribe((result) => {
        this.steps = result.data ?? [] as any;

      });
  }

  // ✅ عضویت در گروه
  private joinTenantGroup(): void {
    if (!this.hubConnection) return;

    this.hubConnection
      .invoke('JoinTenantGroup', this.tenantSlug)
      .then(() => {
        console.log(`✅ Joined group: tenant-${this.tenantSlug}`);
      })
      .catch((err) => {
        console.error('❌ Failed to join group:', err);
      });
  }

  // ✅ خروج از گروه
  private leaveTenantGroup(): void {
    if (!this.hubConnection) return;

    this.hubConnection
      .invoke('LeaveTenantGroup', this.tenantSlug)
      .then(() => {
        console.log(`👋 Left group: tenant-${this.tenantSlug}`);
      })
      .catch((err) => {
        console.error('❌ Failed to leave group:', err);
      });
  }

  // ✅ ثبت event listeners
  private registerEvents(): void {
    if (!this.hubConnection) return;

    // Event برای پیشرفت
    this.hubConnection.on('OnProgress', (notification: ProvisioningNotification) => {
      console.log('📊 Progress:', notification);
      this.handleNotification(notification);
    });

    // Event برای تکمیل
    this.hubConnection.on('OnCompleted', (notification: ProvisioningNotification) => {
      console.log('✅ Completed:', notification);
      this.handleNotification(notification);
    });

    // Event برای خطا
    this.hubConnection.on('OnError', (notification: ProvisioningNotification) => {
      console.log('❌ Error:', notification);
      this.handleNotification(notification);
    });
  }

  // ✅ پردازش نوتیفیکیشن
  private handleNotification(notification: ProvisioningNotification): void {
    console.log('Tenant:', notification.tenantSlug);
    console.log('Status:', notification.status);
    console.log('Step:', notification.currentStep);
    console.log('Message:', notification.message);
    console.log('Time:', notification.timestamp);

    // اینجا UI رو آپدیت کن
    // this.progress = notification.currentStep;
    // this.status = notification.status;
  }
}
