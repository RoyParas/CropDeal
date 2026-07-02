import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  private readonly baseUrl = `${environment.apiBaseUrl}/Notification`;

  private http: HttpClient = inject(HttpClient);

  getUserNotifications(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/getNotifications`);
  }

  deleteNotification(notificationId: string): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/deleteNotification`, {params: {notificationIdString: notificationId}} )
  }
}
