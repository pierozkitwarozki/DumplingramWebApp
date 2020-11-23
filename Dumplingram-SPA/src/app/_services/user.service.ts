import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { error } from 'protractor';
import { Observable } from 'rxjs';
import { map, repeat, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Follow } from '../_models/follow';
import { Photo } from '../_models/photo';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) {}

  getUsers(word?: string): Observable<User[]> {

    const params = new HttpParams().set('word', word);

    return this.http
      .get<User[]>(this.baseUrl + 'users', { observe: 'response', params })
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
      this.baseUrl + 'users/follow/' + followeeId,
      {}
    );
  }

  getFollow(followeeId: number): Observable<Follow> {
    return this.http
      .get<Follow>(this.baseUrl + 'users/getfollow/' + followeeId, {
        observe: 'response',
      })
      .pipe(
        map((response) => {
          return response.body;
        })
      );
  }

  unfollow(followeeId: number) {
    return this.http.delete(
      this.baseUrl + 'users/unfollow/' + followeeId
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

  updateUser(user: User) {
    return this.http.put(this.baseUrl + 'users', user);
  }
}
