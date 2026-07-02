import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { PasswordDto, UserService } from '../../../services/user/user.service';
import { AuthService } from '../../../services/auth/auth.service';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './change-password.component.html',
})
export class ChangePasswordComponent {
  private fb = inject(FormBuilder);
  private userService = inject(UserService);
  private authService = inject(AuthService);

  changePasswordForm = this.fb.group({
    oldPassword: ['', Validators.required],
    newPassword: ['', [Validators.required, Validators.minLength(6)]],
    confirmNewPassword: ['', Validators.required]
  });

  submitted = false;
  loading = false;
  errorMessage = '';
  successMessage = '';

  isBothPasswordsMatched() : boolean {
    return this.changePasswordForm.get('confirmNewPassword')?.value === this.changePasswordForm.get('newPassword')?.value
  }
  
  onSubmit() {
    this.errorMessage = '';

    this.loading = true;

    const formValue = this.changePasswordForm.value;

    const payload: PasswordDto = {
      oldPassword: formValue.oldPassword || '',
      newPassword: formValue.newPassword || '',
      confirmNewPassword: formValue.confirmNewPassword || ''
    };

    this.userService.changePassword(payload).subscribe({
      next: (res) => {
        this.successMessage = "Password Changed Successfully! Redirecting you to login...";
        setTimeout(() => this.authService.logout(), 3000);
      },
      error: (err) => {
        console.log(err);
        this.loading = false;
        const errorMessages = err.error?.message;
        
        if (errorMessages && typeof errorMessages === 'object') {
          const allErrors = Object.values(errorMessages)
          .flat()
          .filter(msg => typeof msg === 'string');
          this.errorMessage = allErrors[0];
        }
        else if (typeof errorMessages === 'string') {
          // Handle plain string error message from backend
          this.errorMessage = errorMessages;
        }
        else {
          console.log(err);
          this.errorMessage = 'Changing Password failed : Check Console';
        }
      }
    });
  }
}
