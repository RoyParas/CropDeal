import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PasswordResetService {
  private http:HttpClient = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/PasswordReset`;

  generateResetLink(email : string) : Observable<any> {
    const formData = new FormData();
    formData.append("email", email);
    return this.http.post(`${this.baseUrl}/forget-password`, formData);
  }

  verifyLink(token: string) : Observable<any> {
    return this.http.get(`${this.baseUrl}/verify-link`, {params: {token: token}});
  }

  resetPassword(token: string, newPassword: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/reset-password`, JSON.stringify(newPassword),
      {
        params: { token: token },
        headers: { 'Content-Type': 'application/json' }
      }
    );
  }

}
