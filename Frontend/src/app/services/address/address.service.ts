import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

export interface AddressDto {
  id: string;
  city: string;
  district: string;
  location: string;
  state: string;
  zipCode: string;
}

@Injectable({
  providedIn: 'root'
})
export class AddressService {
  private readonly addressBaseUrl = `${environment.apiBaseUrl}/Address`

  http: HttpClient = inject( HttpClient);

  addAdress(payload: AddressDto): Observable<any>{
    return this.http.post(`${this.addressBaseUrl}/createAddress`,payload);
  }

  editAddress(payload: AddressDto): Observable<AddressDto> {
    return this.http.patch<AddressDto>(`${this.addressBaseUrl}/updateAddress`, payload);
  }

  deleteAddress(): Observable<any>{
    return this.http.delete(`${this.addressBaseUrl}/deleteAddress`);
  }
}
