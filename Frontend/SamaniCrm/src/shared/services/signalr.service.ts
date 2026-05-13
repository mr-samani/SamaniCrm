import { Injectable, Injector } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { HubConnection } from '@microsoft/signalr';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  constructor(private tokenService: TokenService) {
  }

  init(signalRUri: string, callback?: any) {
    const token = this.tokenService.get();
    let connection = new signalR.HubConnectionBuilder()
      .withUrl(signalRUri, {
        transport: signalR.HttpTransportType.WebSockets,
        withCredentials: true,
        skipNegotiation: true,
        accessTokenFactory: () => token.accessToken ?? '',
      })
      .configureLogging(signalR.LogLevel.Critical)
      .withAutomaticReconnect()
      .build();

    connection.start().then((result) => {
      console.log('✅ SignalR connected via WebSocket');
      // abp.event.trigger('abp.signalr.connected');
      if (callback) {
        callback(connection);
      }
    });

    connection.onclose((e) => {
      if (e) {
        console.error('❌ SignalR connection error:', e);
        // abp.log.debug('Chat connection closed with error: ' + e);
      } else {
        //  abp.log.debug('Chat disconnected');
      }
    });
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
