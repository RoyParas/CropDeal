import { Component, inject, OnInit } from '@angular/core';
import { CropListService, cropToUpdateDto } from '../../../services/cropList/crop-list.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';


@Component({
  selector: 'app-listed-crops-by-farmer',
  imports: [CommonModule, FormsModule],
  templateUrl: './listed-crops-by-farmer.component.html'
})

export class ListedCropsByFarmerComponent implements OnInit{

  cropLists: any[] = [];
  loading: boolean = true;
  saving: boolean = false;
  uploading: boolean = false;
  editingId: string | null = null;
  imageToUpdateId: string | null = null;
  selectedFile: File | null = null;
  errorMessage: string = '';

  cropListService: CropListService = inject(CropListService);
  router: Router = inject(Router);

  ngOnInit(): void {
    this.cropListService.getListedCropsByFarmer().subscribe({
      next: (data) => {
        this.loading = false;
        this.cropLists = data;
      },
      error: (err) => {
        this.loading = false;
        console.log(err);
        this.errorMessage = err.error;
      }
    })
  }

  navigateToAddCrop(){
    this.router.navigateByUrl('farmer/addCropToList');
  }

  triggerImageUpdate(cropListId: string){
    this.editingId = null;
    this.imageToUpdateId = cropListId;
  }

  // Handle file selection
  onImageSelected(event: any): void {
    if( event.target.files && event.target.files[0]){
      this.selectedFile = event.target.files[0];
    } else {
      alert('Please upload a valid image file.');
    }
  }

  // Trigger file input click
  triggerFileInput(cropItem: any): void {
    if (!this.selectedFile) return;

    const reader = new FileReader();
    reader.onload = (e: any) => {
      cropItem.imageUrl = e.target.result;
    };

    reader.readAsDataURL(this.selectedFile); // Now we pass a guaranteed File object

    this.uploading = true;
    this.cropListService.updateListedCropImage(cropItem.id, this.selectedFile).subscribe({
      next: (data) => {
        this.uploading = false;
        this.imageToUpdateId = null;
        this.selectedFile = null;
      },
      error: (err) => {
        this.uploading = false;
        console.log(err);
        this.errorMessage = "Image upload failed" + err.error;
      }
    });
  }


  triggerCropEdit(cropListId: string){
    this.editingId = cropListId;
    this.imageToUpdateId = null;
  }

  saveUpdate(cropItem: cropToUpdateDto, cropListId: string){
    this.saving = true;
    this.cropListService.updateListedCrop(cropItem, cropListId).subscribe({
      next: (data) => {
        this.editingId = null;
        this.saving = false;
        this.ngOnInit();
      },
      error: (err) => {
        console.log(err);
        this.saving = false;
        this.errorMessage = err.error;
      }
    })
  }

  cancelEdit(){
    this.editingId = null;
    this.imageToUpdateId = null;
  }

  deleteListedCrop(cropListId: string){
    this.cropListService.deleteListedCrop(cropListId).subscribe({
      next:() => {
        this.cropLists = this.cropLists.filter(cropList => cropList.id !== cropListId);
      },
      error: (err) => {
        console.log(err);
        this.errorMessage = err.error.message;
      }
    })
  }

  closeError(){
    this.errorMessage = '';
  }

}
