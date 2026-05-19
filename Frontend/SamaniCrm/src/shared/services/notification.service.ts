// notification.service.ts
import { Injectable, OnDestroy } from '@angular/core';
import { AppConst } from '@shared/app-const';
import { NotificationDto } from '@shared/service-proxies/model/notification-dto';
import { ConnectionStatus, SignalRService } from './signalr.service';
import { Subject } from 'rxjs';
import { HubConnection } from '@microsoft/signalr';

@Injectable({ providedIn: 'root' })
export class NotificationService implements OnDestroy {
  private hubConnection?: HubConnection;

  onRecieveMessage$ = new Subject<NotificationDto>();
  constructor(
    private signalRService: SignalRService,
  ) {}

  ngOnDestroy(): void {
    if (this.onRecieveMessage$) {
      this.onRecieveMessage$.unsubscribe();
    }
  }

  public startConnection() {
    const uri = AppConst.apiUrl + '/hubs/notifications';
    this.hubConnection = this.signalRService.init(uri);
    this.signalRService.onConnectionChange.subscribe((state) => {
      if (state == ConnectionStatus.Connected) {
        this.registerEvents();
      }
    });
  }

  public stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  private registerEvents() {
    if (!this.hubConnection) return;
    this.signalRService.on(this.hubConnection, 'ReceiveNotification', (msg: NotificationDto) => {
      console.log('📨 Notification:', msg.title);
      this.onRecieveMessage$.next(msg);
    });
  }
}
