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
    //{"type":1,"target":"SendAsync","arguments":["ReceiveNotification",{"id":"333b83e0-eb11-45f0-bb0f-08addb35a73e","title":"5454","content":"thgbhnklm,\n","periority":0,"type":1,"recieverUserId":"93b02a5b-5b2e-4ae9-0def-08deb7bf8122","recieverName":null,"senderUserId":"93b02a5b-5b2e-4ae9-0def-08deb7bf8122","senderName":null,"read":false,"data":null,"creationTime":"2026-05-24T14:45:07.4597343Z"}]}
    this.signalRService.on(this.hubConnection, 'SendAsync', (method:string,msg: NotificationDto) => {
      console.log('📨 Notification:', msg);
      this.onRecieveMessage$.next(msg);
    });
  }
}
