import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  Resolve,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Photo } from '../_models/Photo';
import { User } from '../_models/User';
import { AlertifyService } from '../_services/alertify.service';
import { UserService } from '../_services/user.service';

@Injectable()
export class PostListResolver implements Resolve<Photo[]> {
  constructor(
    private userService: UserService,
    private router: Router,
    private alertifyService: AlertifyService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Photo[]> {
    return this.userService.getPhotos().pipe(
      catchError((error) => {
        this.alertifyService.error(error);
        // this.router.navigate([])
        return of(null);
      })
    );
  }
}
