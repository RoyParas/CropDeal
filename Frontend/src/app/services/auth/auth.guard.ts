import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isLoggedIn()) {
    return true; // Allow access to guest-only pages (e.g., home)
  }

  const role = authService.getRole();
  const url = state.url;

  // Role-based routing
  if(url.includes('') && (role === 'Farmer' || role === 'Dealer'|| role === 'Admin')) {
    router.navigate([`/${role.toLowerCase()}-dashboard`]);
    return false;
  }

  return true;
};
