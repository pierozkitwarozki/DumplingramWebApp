import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { PhotoComment } from '../_models/Comment';
import { Photo } from '../_models/photo';
import { PhotoLike } from '../_models/photoLike';

@Injectable({
  providedIn: 'root',
})
export class PhotoService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}
  getPhotos(): Observable<any[]> {
    return this.http
      .get<any[]>(this.baseUrl + 'photos/', { observe: 'response' })
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
    return this.http
      .get<PhotoLike[]>(this.baseUrl + 'photos/' + photoId + '/likes/', {
        observe: 'response',
      })
      .pipe(
        map((response) => {
          const x = response.body;
          return x;
        })
      );
  }

  unlikePhoto(id: number, userId: number) {
    return this.http.delete(
      this.baseUrl + 'photos/' + id + '/unlike/' + userId
    );
  }

  getLike(photoId: number): Observable<PhotoLike> {
    return this.http
      .get<PhotoLike>(this.baseUrl + 'photos/' + photoId + '/getlike', {
        observe: 'response',
      })
      .pipe(
        map((response) => {
          return response.body;
        })
      );
  }

  deletePhoto(photoId: number) {
    return this.http.delete(
      this.baseUrl + 'photos/delete/' + photoId
    );
  }

  setMainPhoto(photoId: number) {
    return this.http.post(
      this.baseUrl + 'photos/setMain/' + photoId,
      {}
    );
  }

  getPhotosForUser(id: number): Observable<any[]> {
    return this.http
      .get<any[]>(this.baseUrl + 'photos/forUser/' + id, {
        observe: 'response',
      })
      .pipe(
        map((repsponse) => {
          const x = repsponse.body;
          return x;
        })
      );
  }

  commentPhoto(model: any) {
    return this.http.post(this.baseUrl + 'photos/comments', model);
  }

  getComments(photoId: number) : Observable<PhotoComment[]> {
    return this.http
      .get<PhotoComment[]>(this.baseUrl + 'photos/' + photoId + '/comments', {
        observe: 'response',
      })
      .pipe(
        map((repsponse) => {
          const x = repsponse.body;
          return x;
        })
      );
  }
  
  deleteComment(photoId: number) {
    return this.http.delete(this.baseUrl + 'photos/' + photoId + '/comments/delete');
  }
}
