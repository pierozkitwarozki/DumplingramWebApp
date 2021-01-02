import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from '../_models/user';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  baseUrl = environment.apiUrl + 'auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../../assets/user.png');
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(
    private http: HttpClient,
    private presenceService: PresenceService
  ) {}

  changeMemberPhoto(photoUrl: string) {
    localStorage.setItem('photoUrl', photoUrl);
    this.photoUrl.next(photoUrl);
  }

  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.currentUser = user.user;
          this.currentUser.token = user.token;
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(this.currentUser));

          if (this.currentUser.photoUrl) {
            this.changeMemberPhoto(this.currentUser.photoUrl);
            localStorage.setItem('photoUrl', this.currentUser.photoUrl);           
          }

          this.presenceService.createHubConnection(this.currentUser);
        }
      })
    );
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }

  register(user: User) {
    return this.http.post(this.baseUrl + 'register', user);
  }
}
