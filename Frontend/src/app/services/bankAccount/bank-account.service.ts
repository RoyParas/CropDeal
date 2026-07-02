import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

export interface BankAccountDto {
  id: string;
  accountNumber: string;
  bankName: string;
  branchName: string;
  ifscCode: string;
}

@Injectable({
  providedIn: 'root'
})
export class BankAccountService {
  private readonly bankBaseUrl = `${environment.apiBaseUrl}/Bank`;

  http: HttpClient = inject( HttpClient);


  addBankAccount(bankAccount : BankAccountDto): Observable<any>{
    return this.http.post(`${this.bankBaseUrl}/addAccount`, bankAccount);
  }

  deleteBankAccount(): Observable<any>{
    return this.http.delete(`${this.bankBaseUrl}/deleteAccount`);
  }
}
