import { Component, inject } from '@angular/core';
import { CropListService } from '../../../services/cropList/crop-list.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-get-all-crops',
  imports: [FormsModule, CommonModule],
  templateUrl: './get-all-cropList.component.html',
})
export class GetAllCropsListComponent {
  cropLists: any[] = [];
  loading: boolean = true;

  router: Router = inject(Router);
  cropListService: CropListService = inject(CropListService);

  ngOnInit() {
    this.cropListService.getAllListedCrops().subscribe({
      next: (data) => {
        this.loading = false;
        this.cropLists = data; 
      },
      error: (err) => {
        this.loading = false;
        console.error('Failed to load crops', err);
      }
    });
  }

  getCropListById(cropListId: string){
    this.router.navigate(['/admin/cropList', cropListId]);
  }
}
