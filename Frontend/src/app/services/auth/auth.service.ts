import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = `${environment.apiBaseUrl}/Auth`;

  constructor(private http: HttpClient, private router: Router) {}

  signup(data: any) {
    return this.http.post(`${this.baseUrl}/signup`, data);
  }

  signin(data: any) {
    return this.http.post(`${this.baseUrl}/signin`, data);
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    this.router.navigate(['/']);
  }

  setToken(token: string) {
    localStorage.setItem('token', token);

    const role = this.getRoleFromToken();
    if(role){
      localStorage.setItem('role' , role);
    }
    else alert('cannot set role in LocalStorage');
  }

  getToken() {
    const token = localStorage.getItem('token');

    if (token && this.isTokenExpired(token)) {
      this.logout();
      alert('Your token has expired! Login again.');
      return null;
    }

    return token;
  }

  getRole(){
    return localStorage.getItem('role');
  }

  private getRoleFromToken(): string | null {
    const token = this.getToken();
    if (!token) return null;

    try {

      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || null;

    } catch (e) {
      console.error('Error decoding token', e);
      return null;
    }
  }

  private isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const exp = payload.exp;
      if (!exp) return true;

      const now = Math.floor(Date.now() / 1000);
      return now >= exp;
    } catch (e) {
      console.error('Error checking token expiration', e);
      return true;
    }
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}
