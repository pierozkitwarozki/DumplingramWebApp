import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Message } from '../_models/Message';
import { User } from '../_models/User';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { MessageService } from '../_services/message.service';
import { UserService } from '../_services/user.service';

@Injectable()
export class ConversationListResolver implements Resolve<Message[]> {
  constructor(
    private messageSerivce: MessageService,
    private router: Router,
    private alertifyService: AlertifyService,
    private authService: AuthService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Message[]> {
    return this.messageSerivce.getMessages(this.authService.currentUser.id).pipe(
      catchError((error) => {
        this.alertifyService.error('Problem retrieving data');
        this.router.navigate(['/home']);
        return of(null);
      })
    );
  }
}