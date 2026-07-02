import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';



@Injectable({
  providedIn: 'root'
})
export class SubscriptionService {

  private readonly baseUrl = `${environment.apiBaseUrl}/Subscription`; // Replace with actual backend URL
  private readonly http: HttpClient = inject(HttpClient);

  getSubscribedCropsByDealer(): Observable<any>{
    return this.http.get<any>(`${this.baseUrl}/getSubscribedCropsByDealer`);
  }

  addSubscription(cropName: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/addSubscription`, null, { params: {cropName} });
  }

  deleteSubscription(subscriptionId: string) : Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/deleteSubscription`, {params: {subscriptionIdString: subscriptionId}});
  }

}
