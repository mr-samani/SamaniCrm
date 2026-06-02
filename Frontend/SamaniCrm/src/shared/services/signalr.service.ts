import { Injectable, Injector, OnDestroy } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { HubConnection, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

export enum ConnectionStatus {
  Stoped,
  Connecting,
  Reconnecting,
  Connected,
}

@Injectable({
  providedIn: 'root',
})
export class SignalRService implements OnDestroy {
  connectionStatus = ConnectionStatus.Stoped;
  hubConnection?: HubConnection;
  onConnectionChange = new BehaviorSubject<ConnectionStatus>(this.connectionStatus);
  constructor() {}

  public get isConnecting() {
    return (
      this.connectionStatus == ConnectionStatus.Connecting || this.connectionStatus == ConnectionStatus.Reconnecting
    );
  }

  ngOnDestroy(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
      this.onConnectionChange.unsubscribe();
    }
  }

  init(signalRUri: string) {
    this.connectionStatus = ConnectionStatus.Connecting;
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(signalRUri, {
        transport: signalR.HttpTransportType.WebSockets,
        withCredentials: true,
        skipNegotiation: true,
        // logger: LogLevel.Information,
       // accessTokenFactory: () => token.accessToken ?? '',
      })
      .configureLogging(LogLevel.Critical)
      .withAutomaticReconnect()
      .build();

    this.hubConnection.onreconnecting(() => {
      console.log('SignalR Reconnectiong', signalRUri);
      this.connectionStatus = ConnectionStatus.Reconnecting;
      this.onConnectionChange.next(this.connectionStatus);
    });
    this.hubConnection.onreconnected(() => {
      console.log('✅ SignalR Reconnected', signalRUri);
      this.connectionStatus = ConnectionStatus.Connected;
      this.onConnectionChange.next(this.connectionStatus);
    });

    this.hubConnection.start().then((result) => {
      if (this.hubConnection?.state == HubConnectionState.Connected) {
        this.connectionStatus = ConnectionStatus.Connected;
        this.onConnectionChange.next(this.connectionStatus);
        console.log('✅ SignalR connected via WebSocket', signalRUri);
        // abp.event.trigger('abp.signalr.connected');
      }
    });

    this.hubConnection.onclose((e) => {
      console.log('SignalR Disconnected', signalRUri);
      this.connectionStatus = ConnectionStatus.Stoped;
      this.onConnectionChange.next(this.connectionStatus);
      if (e) {
        console.error('❌ SignalR connection error:', e);
        // abp.log.debug('Chat connection closed with error: ' + e);
      } else {
        //  abp.log.debug('Chat disconnected');
      }
    });
    return this.hubConnection;
  }

  /**
   * send data with signalR
   * @param connection current HUB
   * @param event event name @example 'SendMessage'
   * @param data input data
   * @returns Promise
   */
  invoke(connection: HubConnection, event: string, data: any): Promise<any> {
    return new Promise((resolve, reject) => {
      if (this.checkState(connection) === false) {
        reject('SignalR not connected');
      }
      connection
        .invoke(event, data)
        .then((result) => {
          resolve(result);
        })
        .catch((error) => {
          reject(error);
        });
    });
  }

  /**
   * listen signalR events
   * @param connection current HUB
   * @param event event name @example 'GetMessageData'
   * @param callback calback function for handle returned data
   * @returns
   */
  on(connection: HubConnection, event: string, callback: any) {
    if (this.checkState(connection) === false) {
      // abp.notify.warn(this.l('ChatIsNotConnectedWarning'));
      return;
    }
    connection.on(event, (...data) => {
      if (callback) {
        callback(...data);
      }
    });
  }

  checkState(connection: HubConnection): boolean {
    if (connection && connection.state === signalR.HubConnectionState.Connected) {
      return true;
    } else {
      return false;
    }
  }
}
