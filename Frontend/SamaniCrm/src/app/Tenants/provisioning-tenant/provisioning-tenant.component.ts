import { Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { HubConnection } from '@microsoft/signalr';
import { AppConst } from '@shared/app-const';
import { SignalRService } from '@shared/services/signalr.service';

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
  constructor(private signalRService: SignalRService) {
    super();
    this.tenantId = this.route.snapshot.params['tenantId'];
    this.tenantSlug = this.route.snapshot.params['tenantSlug'];
  }

  ngOnInit() {
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
    this.hubConnection.on('OnProgress', (notification: any) => {
      console.log('📊 Progress:', notification);
      this.handleNotification(notification);
    });

    // Event برای تکمیل
    this.hubConnection.on('OnCompleted', (notification: any) => {
      console.log('✅ Completed:', notification);
      this.handleNotification(notification);
    });

    // Event برای خطا
    this.hubConnection.on('OnError', (notification: any) => {
      console.log('❌ Error:', notification);
      this.handleNotification(notification);
    });
  }

  // ✅ پردازش نوتیفیکیشن
  private handleNotification(notification: any): void {
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
