import { Component, OnInit } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from './_models/User';
import { AuthService } from './_services/auth.service';
import { MessageService } from './_services/message.service';
import { PresenceService } from './_services/presence.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  jwtHelper = new JwtHelperService();

  constructor(
    private authService: AuthService,
    private presenceService: PresenceService,
    private messageService: MessageService
  ) {}

  ngOnInit() {
    const token = localStorage.getItem('token');
    const user: User = JSON.parse(localStorage.getItem('user'));
    if (token) {
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }
    if (user) {
      this.authService.currentUser = user;
      this.presenceService.createHubConnection(user);
    }
  }

  ngOnDestroy() {
    this.presenceService.stopHubConnection();
    this.messageService.stopConnection();
  }
}
