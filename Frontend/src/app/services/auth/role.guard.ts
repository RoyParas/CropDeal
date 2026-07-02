import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Check login
  if (!authService.isLoggedIn()) {
    router.navigate(['/login']);
    return false;
  }

  const role = authService.getRole();
  const url = state.url;

  // Role-based routing
  if(url.includes('farmer') && role !== 'Farmer') {
    router.navigate(['/unauthorized']);
    return false;
  }
  if(url.includes('dealer') && role !== 'Dealer') {
    router.navigate(['/unauthorized']);
    return false;
  }
  if(url.includes('admin') && role !== 'Admin') {
    router.navigate(['/unauthorized']);
    return false;
  }

  return true;
};
