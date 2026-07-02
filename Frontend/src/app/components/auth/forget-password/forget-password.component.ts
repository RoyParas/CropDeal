import { PasswordResetService } from './../../../services/auth/password-reset.service';
import { Component, inject, OnInit } from '@angular/core';
import { EmailService } from '../../../services/auth/email.service';
import {FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-forget-password',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './forget-password.component.html'
})
export class ForgetPasswordComponent implements OnInit {
  errorMessage: string = '';
  successMessage: string = '';
  resetLink: string = '';
  isLoading: boolean = false;
  

  private emailService: EmailService = inject(EmailService);
  private passwordResetService: PasswordResetService = inject(PasswordResetService);
  private router: Router = inject(Router);
  private fb: FormBuilder = inject(FormBuilder);

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]]
  });

  
  ngOnInit() {
    this.form.controls['email'].valueChanges.subscribe(() => {
      if (this.errorMessage) {
        this.errorMessage = '';
      }
    });
  }
  

  requestReset() {
    this.isLoading = true;
    if(this.form.valid) {
      const email = this.form.controls['email'].value!;
      this.passwordResetService.generateResetLink(email).subscribe({
        next: (data) => {
          this.resetLink = data.resetLink;
          this.isLoading = false;
          this.sendMail();
        },
        error: (err) => {
          console.log(err);
          
          this.isLoading = false;
          this.errorMessage = err.error?.message;
        },
      });
    }
    else {
      console.log("Invalid form");
    }
  }

  sendMail() {
    if (this.form.valid) {
      const email = this.form.controls['email'].value!;
      this.emailService.sendPasswordResetEmail(email, this.resetLink, "User")
        .then(() => {
          this.isLoading = false;
          this.successMessage = "If the mail exist on our app, Password reset link has been sent to your email.";
          setTimeout(() => this.router.navigate(['/login']), 3000);
        })
        .catch(err => {
          this.isLoading = false;
          console.log(err);
          this.errorMessage = "Failed to send email : Check Console";
        }
      );
    }
    else {
      console.log("Invalid form");
    }
  }
}