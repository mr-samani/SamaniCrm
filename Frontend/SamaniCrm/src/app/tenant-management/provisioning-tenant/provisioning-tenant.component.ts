import { Component, OnDestroy, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { HubConnection } from '@microsoft/signalr';
import { AppConst } from '@shared/app-const';
import { ConnectionStatus, SignalRService } from '@shared/services/signalr.service';
import { ProvisioningNotification } from './provisioning-notification';
import { TenantsServiceProxy } from '@shared/service-proxies/api/tenants.service';
import { finalize } from 'rxjs';
import { ProvisioningStatusDto } from '@shared/service-proxies/model/provisioning-status-dto';
import { ProvisioningStepStatus } from '@shared/service-proxies/model/provisioning-step-status';

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

  steps: ProvisioningStatusDto[] = [];

  retring = false;
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
    this.hubConnection = this.signalRService.init(uri);

    this.signalRService.onConnectionChange.subscribe((state) => {
      if (state == ConnectionStatus.Connected) {
        setTimeout(() => {
          this.registerEvents();
          this.joinTenantGroup();
        }, 100);
      }
      this.chdr.detectChanges();
    });
  }

  ngOnDestroy(): void {
    if (this.hubConnection) {
      this.leaveTenantGroup();
      this.hubConnection.stop();
    }
  }

  getProvisioningStatuses() {
    this.loading = true;
    this.tenantService
      .getProvisioningTenantStatus(this.tenantId)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.chdr.detectChanges();
        }),
      )
      .subscribe((result) => {
        this.steps = result.data ?? [];
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
    console.log('recived', notification);
    if (notification.tenantSlug == this.tenantSlug) {
      if (notification.message) {
        this.notify.error(notification.message);
      }
      const f = this.steps.findIndex((x) => x.step == notification.currentStep);
      if (f > -1) {
        this.steps[f].errorMessage = notification.message;
        this.steps[f].stepStatus = notification.status;
      }
    }
    this.chdr.detectChanges();
  }

  retryProvisioningData() {
    this.retring = true;
    this.tenantService
      .retryProvisioningTenant(this.tenantId)
      .pipe(
        finalize(() => {
          this.retring = false;
          this.chdr.detectChanges();
        }),
      )
      .subscribe((result) => {
        if (result.success) {
          this.steps.forEach((e) => (e.stepStatus = ProvisioningStepStatus.Pending));
          this.notify.success(this.l('ProvisioningTenantDataJobStarted'));
          this.chdr.detectChanges();
        }
      });
  }
}
