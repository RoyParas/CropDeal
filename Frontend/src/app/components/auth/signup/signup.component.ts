import { Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../services/auth/auth.service';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-signup',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './signup.component.html',
})
export class SignupComponent {

  logo = "assets/logo/logo-without-bg-black-text.png";
  signupImg = "assets/SignUp.png";
  isLoading = false;
  errorMessage: string = '';

  private fb: FormBuilder = inject(FormBuilder);
  private auth: AuthService = inject(AuthService);
  private router: Router = inject(Router);

  form = this.fb.group({
    fullName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['', [Validators.required, Validators.minLength(10)]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', Validators.required],
    role: ['Farmer']
  });

  submit() {
    if (this.form.invalid) {
      const controls = this.form.controls as { [key: string]: AbstractControl };

      const validationMessages: { [key: string]: string } = {
        fullName: 'Enter the full name',
        email: 'Enter the email address',
        phoneNumber: 'Enter the phone number',
        password: 'Enter the password',
        confirmPassword: 'Re-enter the password',
      };

      for (const controlName in validationMessages) {
        const control = controls[controlName];
        if (control.hasError('required')) {
          this.errorMessage = validationMessages[controlName];
          return;
        }
      }

      if(this.form.controls['email'].hasError('email') === true){
        this.errorMessage = "Entered email is not valid";
        return;
      };

      if(this.form.controls['phoneNumber'].hasError('minlength') === true){
        this.errorMessage = "PhoneNumber must be of length 10";
        return;
      };

      if(this.form.controls['password'].hasError('minlength') === true){
        this.errorMessage = "Passwords must be at least 6 characters.";
        return;
      };  
    }

    if(this.form.controls['password'].value !== this.form.controls['confirmPassword'].value) {
      this.errorMessage = "Both passwords do not match";
      return;
    }

    this.isLoading = true;

    this.auth.signup(this.form.value).subscribe({
      next: () => this.router.navigate(['/login']),
      error: err => {

        this.isLoading = false;
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
          this.errorMessage = 'Signup failed : Check Console';
        }
      },

      complete: ()=> this.isLoading = false
    });
  }
}
