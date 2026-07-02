import { Component, inject } from '@angular/core';
import { CropService } from '../../../services/crop/crop.service';
import { GetAllCropsComponent } from '../get-all-crops/get-all-crops.component'
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-add-crop',
  imports: [CommonModule, FormsModule],
  templateUrl: './add-crop.component.html'
})
export class AddCropComponent {

  isFormVisible: boolean = false;
  error: string | null = null;
  name = '';
  type = 'Select Crop Type';

  private cropService = inject(CropService);
  private getAllCrops = inject(GetAllCropsComponent);

  showForm(): void {
    this.isFormVisible = true;
    this.type = 'Select Crop Type';
    this.name = '';
    this.error = null;
  }

  closeForm(): void {
    this.isFormVisible = false;
  }

  addCrop(): void {
    this.cropService.addCrop(this.name, this.type).subscribe({
      next: (response) => {
        this.getAllCrops.crops.push(response.crop);
        this.isFormVisible = false;
      },
      error: err => {
        const errors = err.error?.errors;

        if (errors && typeof errors === 'object') {
          const allErrors = Object.values(errors)
            .flat()
            .filter(msg => typeof msg === 'string');

          if (allErrors.length > 1) {
            this.error= allErrors[1];
          }
          else {
            this.error ='Error adding crop';
          }
        }
        else if (typeof err.error === 'string') {
          this.error = err.error;
        }
        else {
          this.error = 'Error adding crop';
        }
      },
    });
  }
}
