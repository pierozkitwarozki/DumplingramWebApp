import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  Resolve,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Photo } from '../_models/photo';
import { User } from '../_models/user';
import { AlertifyService } from '../_services/alertify.service';
import { PhotoService } from '../_services/photo.service';

@Injectable()
export class PostListResolver implements Resolve<any[]> {
  constructor(
    private photoService: PhotoService,
    private alertifyService: AlertifyService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<any[]> {
    return this.photoService.getPhotos().pipe(
      catchError((error) => {
        this.alertifyService.error(error);
        // this.router.navigate([])
        return of(null);
      })
    );
  }
}
