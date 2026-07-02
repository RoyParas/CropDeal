import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface cropToUpdateDto{
  description: string;
  quantityInKg: number;
  pricePerKg: number;
}

@Injectable({
  providedIn: 'root'
})
export class CropListService {
  private readonly baseUrl = `${environment.apiBaseUrl}/CropList`;

  private http:HttpClient = inject(HttpClient);


  listCrop(data: FormData): Observable<any> {
    return this.http.post(`${this.baseUrl}/listCrop`, data);
  }

  getAllListedCrops(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/getAllListedCrops`);
  }

  getListedCropById(cropListId: string): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/getListedCropById`, {
      params: {cropListingIdString: cropListId}
    });
  }

  getListedCropsByFarmer(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/getListedCropsByFarmer`)
  }

  getAvailableCrops(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/getAvailableCropsFromTheList`);
  }

  searchListedCrops(cropName: string): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/search`, {params: {cropName}});
  }

  updateListedCrop(cropItem: cropToUpdateDto, cropListId: string): Observable<any>{
    const formData = new FormData();
    formData.append('description', cropItem.description);
    formData.append('quantityInKg', cropItem.quantityInKg.toString());  // FormData expects string values
    formData.append('pricePerKg', cropItem.pricePerKg.toString());

    return this.http.patch<any>(`${this.baseUrl}/updateListedCrop`, formData, {
      params: {cropListingIdString: cropListId}
    })
  }

  updateListedCropImage(cropListingId: string, imageFile: File): Observable<any> {
    const formData = new FormData();
    formData.append('Image', imageFile);

    const params = new HttpParams().set('cropListingIdString', cropListingId);

    return this.http.patch(`${this.baseUrl}/updateImageOfListedCrop`, formData, { params });
  }

  deleteListedCrop(cropListingIdString: string): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/deleteListedCrop`, {
      params: {cropListingIdString}
    })
  }
}
