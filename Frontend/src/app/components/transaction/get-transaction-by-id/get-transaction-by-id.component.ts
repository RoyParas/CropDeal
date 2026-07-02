import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TransactionService } from '../../../services/transaction/transaction.service';
import { CommonModule } from '@angular/common';
import { formatDistanceToNow } from 'date-fns';
import { AuthService } from '../../../services/auth/auth.service';
import { FormsModule } from '@angular/forms';
import { ReviewDto, ReviewService } from '../../../services/review/review.service';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-get-transaction-by-id',
  imports: [CommonModule, FormsModule],
  templateUrl: './get-transaction-by-id.component.html'
})
export class GetTransactionByIdComponent implements OnInit {

  id!: string;
  loading: boolean = true;
  downloading: boolean = false;
  openReviewForm: boolean = false;
  transaction: any;
  farmerDetails: any;
  dealerDetails: any;
  errorMessage: string = '';
  review: ReviewDto = {
    comment:'',
    rating: 0
  };

  private readonly route: ActivatedRoute = inject (ActivatedRoute);
  private readonly router: Router = inject(Router);
  private readonly authService: AuthService = inject (AuthService);
  private readonly transactionService: TransactionService = inject(TransactionService);
  private readonly reviewService: ReviewService = inject(ReviewService);

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id')!;
    this.loadTransaction();
    this.getReview();
  }

  loadTransaction() {
    this.loading = true;
    this.transactionService.getTransactionById(this.id).subscribe({
      next: (data) => {
        this.loading = false;
        this.transaction = data;
        this.farmerDetails = data.listing.farmer;
        this.dealerDetails = data.dealer;
      },
      error: (err) => {
        this.loading = false;
        console.log(err);
      }
    });
  }

  getReview() {
    this.reviewService.getReviewById(this.id).subscribe({
      next: (data) => {
        this.review.comment = data.comment;
        this.review.rating = data.rating;
      },
      error: (err) => {
        console.log(err);
      }
    })
  }

  getTimeAgo(updatedAt: string | Date): string {
    return formatDistanceToNow(new Date(updatedAt), { addSuffix: true });
  }

  getRole() {
    return this.authService.getRole();
  }

  goBack(){
    window.history.go(-1);
  }

  getReceipt(transactionId: string) {
    this.downloading = true;
    this.transactionService.downloadReceipt(transactionId).subscribe({
      next: (blob : Blob) => {
        this.downloading = false;
        const url = window.URL.createObjectURL(blob);
        
        window.open(url);
        
        // const fileName = `Receipt_${transactionId}.pdf`;
        // const a = document.createElement('a');
        // a.href = url;
        // a.download = fileName;
        // a.click();

        // // Clean up URL object
        // window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        this.downloading = false;
        console.log(err);
      }
    })
  }


  acceptTransaction(transactionId: string) {
    this.transactionService.acceptTransaction(transactionId).subscribe({
      next: (data) => {
        alert(data.message);
        this.ngOnInit();
      },
      error: (err) => {
        console.log(err);
      }
    })
  }

  rejectTransaction(transactionId: string) {
    this.transactionService.rejectTransaction(transactionId).subscribe({
      next: (data) => {
        alert(data.message);
        this.ngOnInit();
      },
      error: (err) => {
        console.log(err);
      }
    })
  }

  razorPayment(transactionId: string) {
    const options: any = {
      key: environment.razorpay_key,
      amount: this.transaction.totalPrice * 100,
      currency: 'INR',
      name: 'CropDeal',
      description: 'Test Transaction',
      handler: (response: any) => {
        console.log(response);
        this.transactionService.makePayment(transactionId).subscribe({
          next: (data: any) => {
            alert(data.message);
            this.router.navigate(['dealer/myTransactions']);
          },
          error: (err: any) => {
            console.log(err);
          }
        });
      },
      prefill: {
        contact: this.dealerDetails.phoneNumber,
      },
      notes: {
        address: 'Test Address',
      },
      theme: {
        color: '#198754',
      },
    };

    const rzp = new (window as any).Razorpay(options);
    rzp.open();
  }

  submitReview(review: ReviewDto, transactionId: string) {
    this.reviewService.addReview(review, transactionId).subscribe({
      next: (data) => {
        console.log(data);
        this.openReviewForm = false;
      },
      error: (err) => {
        console.log(err);
      }
    })
  }
}
