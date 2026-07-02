import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {
  private readonly baseUrl = `${environment.apiBaseUrl}/Transaction`;
  private readonly http: HttpClient = inject(HttpClient);

  getAllTransaction(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/getAllTransactions`);
  }

  getFarmerTransactions() : Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/getFarmerTransactions`);
  }

  getDealerTransactions() : Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/getDealerTransactions`);
  }

  getTransactionById(transactionId: string) : Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/getTransactionById`, {params: {transactionIdString : transactionId}});
  }

  generateUserReport(userId: string) : Observable<any> {
    return this.http.get(`${this.baseUrl}/generateReport`, 
    {
      headers : new HttpHeaders({
        'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
      }),
      params : {userIdString : userId},
      responseType: 'blob'
    });
  }

  initiateTransaction(cropListId: string, quantity: number, finalPrice: number): Observable<any> {
    return this.http.post<any>(
      `${this.baseUrl}/initiateTransaction`,
      {
        PricePerKg: finalPrice,
        QuantityInKg: quantity
      },
      {
        params: { CropListingIdString: cropListId }
      }
    );
  }

  acceptTransaction(transactionId: string) : Observable<any> {
    return this.http.patch<any>(`${this.baseUrl}/acceptTransaction`, null, {params: {transactionIdString : transactionId}});
  }

  rejectTransaction(transactionId: string) : Observable<any> {
    return this.http.patch<any>(`${this.baseUrl}/rejectTransaction`, null, {params: {transactionIdString : transactionId}});
  }

  makePayment(transactionId: string) : Observable<any> {
    return this.http.patch<any>(`${this.baseUrl}/makePayment`, null, {params: {transactionIdString : transactionId}});
  }

downloadReceipt(transactionId: string): Observable<Blob> {
  return this.http.get(`${this.baseUrl}/receipt/pdf`, {
    responseType: 'blob',
    params: { transactionIdString: transactionId }
  });
}

}
