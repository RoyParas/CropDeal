import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-notfound',
  templateUrl: './notfound.component.html',
})

export class NotFoundComponent {
  notFoundLogo =  "../../../assets/notfound.png"

  constructor(private router: Router) {
  }

  goBack(): void {
    // Go back two steps in history
    if (window.history.length > 1) {
      window.history.go(-1);
    } else {
      this.router.navigate(['/']); // fallback
    }
  }
}
