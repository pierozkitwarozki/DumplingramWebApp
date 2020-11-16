import { CoreEnvironment } from '@angular/compiler/src/compiler_facade_interface';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/Message';
import { User } from '../_models/User';
import { AlertifyService } from './alertify.service';
import { AuthService } from './auth.service';
import { MessageService } from './message.service';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();

  private conversations = new BehaviorSubject<Message[]>([]);
  conversations$ = this.conversations.asObservable();

  constructor(
    private alertify: AlertifyService,
    private messageService: MessageService
  ) {}

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().then(() => {
      this.messageService.getMessages(user.id)
        .subscribe((res: Message[]) => {
          this.conversations.next(res);
        });
    }).catch((error) => console.log(error));

    this.hubConnection.on('GetOnlineUsers', (ids: string[]) => {
      this.onlineUsersSource.next(ids);
    });

    this.hubConnection.on('NewMessageReceived', ({ username }) => {
      this.alertify.warning('@' + username + ' wysłał Ci wiadomość.');
      this.messageService.getMessages(user.id)
        .subscribe((res: Message[]) => {
          this.conversations.next(res);
        });
    });

    this.hubConnection.on('NewMessageReceivedNoNotification', () => {
      this.messageService.getMessages(user.id)
        .subscribe((res: Message[]) => {
          this.conversations.next(res);
        });
    });
  }

  stopHubConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }
}
