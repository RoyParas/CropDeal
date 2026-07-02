import { Component, OnInit, inject } from '@angular/core';
import { CropService, CropDto } from '../../../services/crop/crop.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {AddCropComponent } from '../add-crop/add-crop.component'


@Component({
  selector: 'app-get-all-crops',
  imports: [CommonModule, FormsModule, AddCropComponent] ,
  templateUrl: './get-all-crops.component.html'
})
export class GetAllCropsComponent implements OnInit {
  private cropService = inject(CropService);  // Use inject() for standalone components

  crops: CropDto[] = [];   // This will hold the list of crops
  loading: boolean = true;  // Show loading spinner while fetching data
  error: string = ''; // Error message, if any
  editingCropId: string | null = null;

  ngOnInit(): void {
    this.fetchCrops();
  }

  fetchCrops(): void {
    this.cropService.getAllCrops().subscribe({
      next: (data) => {
        this.crops = data;
        this.loading = false;
      },
      error: (err) => {
        console.log(err);
        if(err){
          if(typeof err === 'object'){
            this.error = err.error ;
          }
          if(typeof err === 'string'){
            this.error = err;
          }
        }
        this.loading = false;
      },
      complete : () => this.loading = false
    });
  }

  startEditing(cropId: string): void {
    this.editingCropId = cropId;
  }

  saveCrop(crop: CropDto): void{
    this.cropService.editCrop(crop.id,crop).subscribe({
      next: (data) => {
        var cropFound = this.crops.find(c => c.id === crop.id);
        if(cropFound && data.cropDtoObj) {
          cropFound.name = data.cropDtoObj.name;
          cropFound.type = data.cropDtoObj.type;
        }

        // Update the crop in the list
        this.crops = [...this.crops]; // Force change detection to update the view

        this.editingCropId = null;
      },
      error: (err) => {
        console.log(err);
        this.error = err ;
        this.loading = false;
        this.editingCropId = null;
      }
    })
  }

  deleteCrop(cropName: string): void {
    this.cropService.deleteCrop(cropName).subscribe({
      next:() => {
        this.crops = this.crops.filter(crop => crop.name !== cropName);
      },
      error: (err) => {
        console.log(err);
        this.error = err.error;
        this.loading = false;
      }
    })
  }

  closeError(){
    this.error = '';
  }
}
