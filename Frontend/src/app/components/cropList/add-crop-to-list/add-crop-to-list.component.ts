import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CropListService } from '../../../services/cropList/crop-list.service';
import { CropService } from '../../../services/crop/crop.service';

@Component({
  selector: 'app-add-crop-to-list',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './add-crop-to-list.component.html'
})
export class AddCropToListComponent implements OnInit {
  loading: boolean = false;
  listCrop = "assets/ListCrop.jpg"
  cropTypes: string[] = []
  cropNames: string[] = []
  selectedFile: File | null = null;
  errorMessage: string  = '';
  imageInvalid = false;
  submitting: boolean = false;

  private router: Router = inject (Router);
  private cropService = inject(CropService);
  private fb = inject(FormBuilder);
  private cropListService = inject(CropListService);

  cropForm = this.fb.group({
    name: ['', Validators.required],
    type: ['', Validators.required],
    pricePerKg: ['', [Validators.required, Validators.min(0.01)]],
    quantityInKg: ['', [Validators.required, Validators.min(0.01)]],
    description: ['', Validators.required]
  });


  ngOnInit(): void {
    this.cropService.getCropsType().subscribe({
      next:(data) => {
        this.loading = false;
        this.cropTypes = data;
      },
      error:(err) => {
        this.errorMessage = err.error;
        this.loading = false;
      }
    })
  }

  loadName(event: Event): void{
    this.cropForm.get('name')?.setValue('');
    this.cropService.getCropsName((event.target as HTMLSelectElement).value).subscribe({
      next:(data) => {
        this.cropNames = data;
      },
      error: (err) => {
        console.log(err);
      }
    })
  }

  onFileChange(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (file) {
      this.selectedFile = file;
      this.imageInvalid = false;
    } else {
      this.selectedFile = null;
      this.imageInvalid = true;
    }
  }

  onSubmit() {
    if (this.cropForm.invalid || !this.selectedFile) {
      this.imageInvalid = !this.selectedFile;
      return;
    }

    this.submitting = true;
    const formData = new FormData();
    Object.entries(this.cropForm.value).forEach(([key, value]) => {
      formData.append(key, value?.toString() || '');
    });

    formData.append('image', this.selectedFile!);

    this.cropListService.listCrop(formData).subscribe({
      next: (res) => {
        this.submitting = false;
        alert('Crop listed successfully!');
        this.router.navigateByUrl('farmer/myListedCrops')
      },
      error: (err) => {
        this.submitting = false;
        const error = err.error;
        if (error && typeof error === 'object') {
          const allErrors = Object.values(error)
            .flat()
            .filter(msg => typeof msg === 'string');

          this.errorMessage = allErrors[0];
        }
      }
    });
  }

  goBack(){
    window.history.go(-1);         // Navigate back
    this.cropForm.reset();         // Reset form controls
    this.selectedFile = null;      // Reset file selection
    this.imageInvalid = false;     // Reset image validation state
  }

  closeError(){
    this.errorMessage = '';
    this.ngOnInit();
  }
}
