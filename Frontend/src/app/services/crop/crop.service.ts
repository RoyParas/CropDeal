import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface CropDto {
  id: string;
  name: string;
  type: string;
}


@Injectable({
  providedIn: 'root'
})

export class CropService {
  private readonly baseUrl = `${environment.apiBaseUrl}/Crop`; // Replace with actual backend URL

  private http: HttpClient = inject(HttpClient);

  // Get all crops
  getAllCrops(): Observable<CropDto[]> {
    return this.http.get<CropDto[]>(`${this.baseUrl}/getAllCrops`);
  }

  getCropsType(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/getCropsType`);
  }

  getCropsName(cropType:string): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/getCropsName`, {params: {cropType}});
  }

  getDistinctCropsName(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/getDistinctCropsName`);
  }

  // Add a new crop
  addCrop(name: string, type: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/addCrop`, {name, type});
  }

  // Edit an existing crop
  editCrop(cropId: string, crop: CropDto): Observable<any> {
    const params = new HttpParams().set('cropIdString', cropId);
    return this.http.patch<any>(`${this.baseUrl}/editCrop`, crop, { params });
  }

  // Delete a crop
  deleteCrop(cropName: string): Observable<any> {
    const params = new HttpParams().set('cropName', cropName);
    return this.http.delete<any>(`${this.baseUrl}/deleteCrop`, { params });
  }
}
