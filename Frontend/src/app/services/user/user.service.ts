import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

// Common properties for creation/updating
interface BaseUserDto {
  fullName: string;
  email: string;
  phoneNumber: string;
}

// Full user object, extends the base and adds system-specific fields
export interface UserDto extends BaseUserDto {
  id: string;
  role: string;
  isActive: boolean;
  addressId: string;
  bankAccountId: string;
}

// DTO for updates (only includes modifiable fields)
export interface UpdateUserDto extends BaseUserDto {}

export interface PasswordDto {
  oldPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly baseUrl = `${environment.apiBaseUrl}/User`;

  http: HttpClient = inject(HttpClient);

  getAllUsers(): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(`${this.baseUrl}/getAllUser`);
  }

  getAdminDashBoard() : Observable<any>{
    return this.http.get(`${this.baseUrl}/getAdminDashBoard`);
  }

  getDealerDashBoard() : Observable<any>{
    return this.http.get(`${this.baseUrl}/getDealerDashBoard`);
  }

  getFarmerDashBoard() : Observable<any>{
    return this.http.get(`${this.baseUrl}/getFarmerDashBoard`);
  }

  getUserById(userIdString: string) : Observable<any>{
    return this.http.get<any>(`${this.baseUrl}/getUserById`, {params: {userIdString}});
  }

  changeUserStatus(email: string): Observable<any> {
    const params = new HttpParams().set('email', email);
    return this.http.patch(`${this.baseUrl}/changeUserStatus`, null, { params });
  }

  changePassword(payload: PasswordDto): Observable<any> {
    return this.http.patch(`${this.baseUrl}/changePassword`, payload);
  }

  getProfile(): Observable<any>{
    return this.http.get<any>(`${this.baseUrl}/myProfile`);
  }

  updateProfile(payload: UpdateUserDto): Observable<any>{
    return this.http.patch(`${this.baseUrl}/editProfile`, payload);
  }
}
