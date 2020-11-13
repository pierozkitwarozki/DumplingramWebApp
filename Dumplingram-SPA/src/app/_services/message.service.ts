import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/Message';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

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

  sendMessage(userId: number, message: Message) {
    return this.http.post(this.baseUrl + 'messages/' + userId, message);
  }
}
