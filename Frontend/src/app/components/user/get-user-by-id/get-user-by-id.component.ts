import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UserService } from '../../../services/user/user.service';
import { CommonModule } from '@angular/common';
import { TransactionService } from './../../../services/transaction/transaction.service';

@Component({
  selector: 'app-get-user-by-id',
  imports: [CommonModule],
  templateUrl: './get-user-by-id.component.html'
})
export class GetUserByIdComponent implements OnInit {

  private readonly route = inject(ActivatedRoute);
  private readonly userService = inject(UserService);
  private readonly transactionService = inject(TransactionService);

  id!: string;
  errorMessage!: string;
  user: any;
  isLoading: boolean = true;
  downloading: boolean = false;

  ngOnInit(): void {
    this.errorMessage = '';
    this.id = this.route.snapshot.paramMap.get('id')!;
    this.loadUser();
  }

  loadUser(): void {
    this.userService.getUserById(this.id).subscribe({
      next: (data) => {
        this.user = data; // Assign the fetched data to the user object
        this.isLoading = false; // Set loading status to false once data is fetched
      },
      error: (err) => {
        console.log(err);
        this.errorMessage = 'Error loading user data. Please try again later.';
        this.isLoading = false;
      }
    });
  }

  goBack() {
    window.history.go(-1)
  }

  generateReport() {
      this.downloading = true;
      this.transactionService.generateUserReport(this.id).subscribe({
        next: (blob : Blob) => {
          const url = window.URL.createObjectURL(blob);
          this.downloading = false;
          window.open(url);
          // const a = document.createElement('a');
          // a.href = url;
          // a.download = 'UserReport.xlsx'; // desired filename
          // a.click();
  
          // window.URL.revokeObjectURL(url); // cleanup
        },
        error: (err) => {
          this.downloading = false;
          console.log(err);
        }
      })
    }

  closeError() {
    this.ngOnInit();
  }
}
