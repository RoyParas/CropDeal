import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CropListService } from '../../../services/cropList/crop-list.service';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth/auth.service';
import { FormsModule } from '@angular/forms';
import { TransactionService } from '../../../services/transaction/transaction.service';

@Component({
  selector: 'app-get-crop-list-by-id',
  imports: [CommonModule, FormsModule],
  templateUrl: './get-crop-list-by-id.component.html'
})
export class GetCropListByIdComponent implements OnInit{
  isBuyClicked: boolean = false;
  loading: boolean = true;
  id!: string;
  product: any;
  quantity!: number;
  finalPrice!: number;
  errorMessage: string = '';

  private readonly route: ActivatedRoute = inject(ActivatedRoute);
  private readonly router: Router = inject(Router);

  private readonly cropListService: CropListService = inject(CropListService);
  private readonly authService: AuthService = inject(AuthService);
  private readonly transactionService: TransactionService = inject(TransactionService);

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id')!;
    this.cropListService.getListedCropById(this.id).subscribe({
      next: (data) => {
        this.loading = false;
        this.product = data;
      },
      error: (err) => {
        this.loading = false;
        console.log(err);
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

  getRole() {
    return this.authService.getRole();
  }

  initiateTransaction(cropListId: string) {
    this.transactionService.initiateTransaction(cropListId, this.quantity, this.finalPrice).subscribe({
      next: (data) => {
        this.router.navigateByUrl('dealer/myTransactions');
      },
      error: (err) => {
        this.isBuyClicked = false;
        console.log(err);
        const error = err.error;
        if (error && typeof error === 'object') {
          const allErrors = Object.values(error)
            .flat()
            .filter(msg => typeof msg === 'string');

          this.errorMessage = allErrors[0];
        }
      }
    })
  }

  goBack() {
    window.history.go(-1);
  }

  closeError(){
    this.errorMessage = ''

  }
}
