import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Photo } from '../_models/Photo';
import { PhotoLike } from '../_models/PhotoLike';

@Injectable({
  providedIn: 'root',
})
export class PhotoService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}
  getPhotos(): Observable<Photo[]> {
    return this.http
      .get<Photo[]>(this.baseUrl + 'photos/', { observe: 'response' })
      .pipe(
        map((response) => {
          const x = response.body;
          return x;
        })
      );
  }

  likePhoto(photoId: number) {
    return this.http.post(this.baseUrl + 'photos/' + photoId + '/like/', {});
  }

  getLikes(photoId: number): Observable<PhotoLike[]> {
    return this.http.get<PhotoLike[]>(this.baseUrl + 'photos/' + photoId + '/likes/', {observe: 'response'})
      .pipe(map((response => {
        const x = response.body;
        return x;
      })));
  }

  unlikePhoto(id: number, userId: number) {
    return this.http.delete(
      this.baseUrl + 'photos/' + id + '/unlike/' + userId
    );
  }

  getLike(photoId: number, userId: number): Observable<PhotoLike> {
    return this.http
      .get<PhotoLike>(this.baseUrl + 'photos/' + photoId + '/getlike/' + userId, {
        observe: 'response',
      })
      .pipe(
        map((response) => {
          return response.body;
        })
      );
  }
}
