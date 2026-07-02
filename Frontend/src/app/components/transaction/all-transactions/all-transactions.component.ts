import { Component, OnInit, inject } from '@angular/core';
import { TransactionService } from '../../../services/transaction/transaction.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth/auth.service';
import { ShortenIdPipe } from '../../../Pipes/shorten-id.pipe';


@Component({
  selector: 'app-all-transactions',
  imports: [CommonModule, ShortenIdPipe],
  templateUrl: './all-transactions.component.html'
})
export class AllTransactionsComponent implements OnInit {
  transactions: any[] = []
  loading: boolean = true;

  private readonly transactionService: TransactionService = inject(TransactionService);
  private readonly authService: AuthService = inject (AuthService);
  private readonly router : Router = inject(Router);

  ngOnInit(): void {
    if(this.getRole() === 'Admin') this.getAllTransactions();

    else if(this.getRole() === 'Dealer') this.getDealerTransactions();

    else if(this.getRole() === 'Farmer') this.getFarmerTransactions();
  }

  getRole(){
    return this.authService.getRole();
  }

  getAllTransactions() {
    this.transactionService.getAllTransaction().subscribe({
      next: (data) => {
        this.transactions = data;
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        console.log(err);
      }
    })
  }

  getFarmerTransactions() {
    this.transactionService.getFarmerTransactions().subscribe({
      next: (data) => {
        this.transactions = data;
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        console.log(err);
      }
    })
  }

  getDealerTransactions() {
    this.transactionService.getDealerTransactions().subscribe({
      next: (data) => {
        this.transactions = data;
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
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

  viewTransaction(transactionId: string) {
    if(this.getRole() === 'Admin') this.router.navigate(['admin/transaction', transactionId]);

    else if(this.getRole() === 'Farmer') this.router.navigate(['farmer/transaction', transactionId]);

    else if(this.getRole() === 'Dealer') this.router.navigate(['dealer/transaction', transactionId]);
  }
}
