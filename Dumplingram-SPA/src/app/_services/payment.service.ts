import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class PaymentService {
  url = environment.apiUrl + 'Checkout';
  constructor(private http: HttpClient) {}

  charge(model: any): Observable<any> {
    return this.http.post<any>(this.url, model).pipe(
      map((response: any) => {
        debugger;
        const x = response;
        return x;
      })
    );
  }
}
