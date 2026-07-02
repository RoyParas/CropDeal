import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-unauthorized',
  templateUrl: './unauthorized.component.html',
})

export class UnauthorizedComponent {
  unauthorizedLogo =  "../../../assets/unauthorized.jpg"

  constructor(private router: Router) {
  }

  goBack(): void {
    // Go back two steps in history
    if (window.history.length > 2) {
      window.history.go(-2);
    } else {
      this.router.navigate(['/']); // fallback
    }
  }
}
