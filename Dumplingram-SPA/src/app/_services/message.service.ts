import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/Message';
import { User } from '../_models/User';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http: HttpClient) {}

  createHubConnection(user: User, otherUserId: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUserId, {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((error) => console.log(error));

    this.hubConnection.on('RecieveMessageThread', (messages) => {
      this.messageThreadSource.next(messages);
    });

    this.hubConnection.on('NewMessage', (message) => {
      this.messageThread$.pipe(take(1)).subscribe((messages) => {
        this.messageThreadSource.next([...messages, message]);
      });
    });
  }

  stopConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  getMessages(userId: number): Observable<Message[]> {
    return this.http
      .get<Message[]>(this.baseUrl + 'messages/' + userId + '/received', {
        observe: 'response',
      })
      .pipe(
        map((response) => {
          return response.body;
        })
      );
  }

  getMessageThread(userId: number, recipientId: number): Observable<Message[]> {
    return this.http
      .get<Message[]>(
        this.baseUrl + 'messages/' + userId + '/thread/' + recipientId,
        { observe: 'response' }
      )
      .pipe(
        map((response) => {
          return response.body;
        })
      );
  }

  async sendMessage(userId: number, message: Message) {
    return this.hubConnection
      .invoke('SendMessage', message)
      .catch((error) => console.log(error));
  }
}
