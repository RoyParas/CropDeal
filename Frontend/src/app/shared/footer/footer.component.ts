import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router} from '@angular/router';

@Component({
  selector: 'app-footer',
  imports: [CommonModule],
  templateUrl: './footer.component.html',
  // styleUrl: './footer.component.css'
})
export class FooterComponent {

  constructor(private router: Router) {}

  // Method to check if the current route is login or signup
  isLoginOrSignup(): boolean {
    const currentUrl = this.router.url;
    return currentUrl === '/login' || currentUrl === '/signup';
  }
}
