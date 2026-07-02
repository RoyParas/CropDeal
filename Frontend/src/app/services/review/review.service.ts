import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ReviewDto  {
    comment:string;
    rating:number;
}

@Injectable({
  providedIn: 'root'
})
export class ReviewService {

  private readonly baseUrl = `${environment.apiBaseUrl}/Review`;
  private readonly http: HttpClient = inject(HttpClient);

  getFarmerReviews(farmerId: string): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/getFarmerReviews`, {params: {farmerIdString: farmerId}});
  }

  getMyReviews() : Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/myReviews`);
  }

  getReviewById(transactionId: string) : Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/getReviewById`, {params: {transactionIdString: transactionId}});
  }

  addReview(review: ReviewDto, transactionId: string): Observable<any>{
    return this.http.post<any>(`${this.baseUrl}/addReview`, review, {params: {transactionIdString: transactionId}});
  }
}
