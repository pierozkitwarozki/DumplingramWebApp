import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { error } from 'protractor';
import { Observable } from 'rxjs';
import { map, repeat, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Follow } from '../_models/Follow';
import { User } from '../_models/User';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) {}

  getUsers(): Observable<User[]> {
    return this.http
      .get<User[]>(this.baseUrl + 'users', { observe: 'response' })
      .pipe(
        map((response) => {
          const x = response.body;
          return x;
        })
      );
  }

  getUser(id: number): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }

  followUser(id: number, followeeId: number) {
    return this.http.post(
      this.baseUrl + 'users/' + id + '/follow/' + followeeId,
      {}
    );
  }

  getFollow(id: number, followeeId: number): Observable<Follow> {
    return this.http
      .get<Follow>(this.baseUrl + 'users/' + id + '/getfollow/' + followeeId, {
        observe: 'response',
      })
      .pipe(
        map((response) => {
          return response.body;
        })
      );
  }

  unfollow(id: number, followeeId: number) {
    return this.http.delete(
      this.baseUrl + 'users/' + id + '/unfollow/' + followeeId
    );
  }

  getFollowees(id: number): Observable<User[]> {
    return this.http
      .get<User[]>(this.baseUrl + 'users/' + id + '/followees', {
        observe: 'response',
      })
      .pipe(
        map((response) => {
          return response.body;
        })
      );
  }

  getFollowers(id: number): Observable<User[]> {
    return this.http
      .get<User[]>(this.baseUrl + 'users/' + id + '/followers', {
        observe: 'response',
      })
      .pipe(
        map((response) => {
          return response.body;
        })
      );
  }
}
