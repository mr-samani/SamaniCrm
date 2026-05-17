// notification.service.ts
import { ChangeDetectorRef, Injectable, OnDestroy } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { AuthService } from './auth.service';
import { TokenService } from './token.service';
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
    private tokenService: TokenService,
    private signalRService: SignalRService,
    private chdr: ChangeDetectorRef,
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
      this.chdr.detectChanges();
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
