import { Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../services/auth/auth.service';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
})
export class LoginComponent {

  loginImg = "assets/SignUp.png"
  logo = "assets/logo/logo-without-bg-black-text.png";
  isLoading = false;
  errorMessage: string = '';

  private fb: FormBuilder = inject(FormBuilder);
  private auth: AuthService = inject(AuthService);
  private router: Router = inject(Router);

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });

  submit() {
    if (this.form.invalid) {
      const controls = this.form.controls as { [key: string]: AbstractControl };

      const validationMessages: { [key: string]: string } = {
        email: 'Enter the email address',
        password: 'Enter the password',
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

    };

    this.isLoading = true; // Start spinner

    this.auth.signin(this.form.value).subscribe({
      next: (res: any) => {

        this.auth.setToken(res.token);

        const role = this.auth.getRole();

        if (role === 'Farmer' || role === 'Dealer'|| role === 'Admin'){
          this.router.navigate([`/${role.toLowerCase()}-dashboard`]);
        }
        else {
          this.router.navigate(['/']);
        }
      },

      error: err => {
        const errors = err.error;
        
        if (errors && typeof errors === 'object') {
          const allErrors = Object.values(errors)
          .flat()
          .filter(msg => typeof msg === 'string');
          
          this.errorMessage = allErrors[0];
        }
        else if (typeof errors === 'string') {
          // Handle plain string error message from backend
          this.errorMessage = err.error;
        }
        else {
          console.log(err);
          this.errorMessage = 'Login failed : Check Console';
        }

        this.isLoading = false;
      },

      complete: () => this.isLoading = false
    });
  }

}
