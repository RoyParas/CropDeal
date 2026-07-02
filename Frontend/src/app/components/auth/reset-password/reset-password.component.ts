import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { PasswordResetService } from '../../../services/auth/password-reset.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-reset-password',
  imports: [FormsModule, CommonModule],
  templateUrl: './reset-password.component.html'
})

export class ResetPasswordComponent implements OnInit {
  token: string = '';
  newPassword: string = '';
  confirmPassword: string = '';
  message: string = '';
  verifyingLinkError: string = '';
  isSuccess: boolean = false;
  isLoading: boolean = false; 

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private passwordResetService: PasswordResetService
  ) {}

  ngOnInit() {
    this.token = this.route.snapshot.queryParamMap.get('token')!;
    this.passwordResetService.verifyLink(this.token).subscribe({
      next: () => {},
      error: (err) => {
        console.log(err);
        this.verifyingLinkError = err.error.message;
      }
    })
  }

  isFormValid(): boolean {
    return (
      this.newPassword != '' &&
      this.confirmPassword != '' &&
      this.newPassword.length >= 6 &&
      this.newPassword === this.confirmPassword
    );
  }


  resetPassword() {
    if (!this.isFormValid()) {
      this.message = 'Please fix the errors above.';
      this.isSuccess = false;
      return;
    }

    this.isLoading = true;
    this.passwordResetService.resetPassword(this.token, this.newPassword).subscribe({
      next: () => {
        this.isSuccess = true;
        this.message = "Password reset successful! Redirecting to login...";
        this.isLoading = false;
        setTimeout(() => this.router.navigate(['/login']), 3000);
      },
      error: (err) => {
        const errors = err.error;

        if (errors && typeof errors === 'object') {
          const allErrors = Object.values(errors)
          .flat()
          .filter(msg => typeof msg === 'string');
          
          this.message = allErrors[0];
        }
        else if (typeof errors === 'string') {
          // Handle plain string error message from backend
          this.message = err.error;
        }
        else {
          console.log(err);
          this.message = 'Resetting Password failed : Check Console';
        }
        this.isLoading = false;
      }
    });
  }
}