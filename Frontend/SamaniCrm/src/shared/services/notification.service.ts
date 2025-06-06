// notification.service.ts
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { AuthService } from './auth.service';
import { TokenService } from './token.service';
import { AppConst } from '@shared/app-const';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private hubConnection!: signalR.HubConnection;

  constructor(private tokenService: TokenService) {}

  public startConnection(): Promise<boolean> {
    return new Promise((resolve, reject) => {
      const token = this.tokenService.get();

      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(`${AppConst.apiUrl}/hubs/notifications`, {
          transport: signalR.HttpTransportType.WebSockets,
          accessTokenFactory: () => token.accessToken ?? '',
          skipNegotiation: true,
        })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

      this.hubConnection
        .start()
        .then(() => {
          console.log('✅ SignalR connected via WebSocket');
          resolve(true);
        })
        .catch((err) => {
          console.error('❌ SignalR connection error:', err);
          reject(err);
        });
    });
  }

  public stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  public onReceiveNotification(callBack: Function) {
    if (!this.hubConnection) {
      return;
    }
    this.hubConnection.on('ReceiveNotification', (msg: string) => {
      console.log('📨 Notification:', msg);
      if (callBack) {
        callBack(msg);
      }
    });
  }
}
