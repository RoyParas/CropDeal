import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { SignupComponent } from './components/auth/signup/signup.component';
import { LoginComponent } from './components/auth/login/login.component';
import { ForgetPasswordComponent } from './components/auth/forget-password/forget-password.component';
import { roleGuard } from './services/auth/role.guard';
import { authGuard } from './services/auth/auth.guard';
import { ResetPasswordComponent } from './components/auth/reset-password/reset-password.component';

export const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
    canActivate: [authGuard]
  },
  {
    path: 'signup',
    component: SignupComponent,
    canActivate: [authGuard]
  },
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [authGuard]
  },
  {
    path: 'forget-password',
    component: ForgetPasswordComponent,
    canActivate: [authGuard]
  },
  {
    path: 'reset-password',
    component: ResetPasswordComponent,
    canActivate: [authGuard]
  },
  {
    path: 'admin/allowed-crops',
    loadComponent: () => import('./components/crops/get-all-crops/get-all-crops.component').then(m=> m.GetAllCropsComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'admin/all-cropList',
    loadComponent: () => import('./components/cropList/get-all-cropList/get-all-cropList.component').then(m=> m.GetAllCropsListComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'admin/cropList/:id',
    loadComponent: () => import('./components/cropList/get-crop-list-by-id/get-crop-list-by-id.component').then(m => m.GetCropListByIdComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'admin/all-transactions',
    loadComponent: () => import('./components/transaction/all-transactions/all-transactions.component').then(m => m.AllTransactionsComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'admin/transaction/:id',
    loadComponent: () => import('./components/transaction/get-transaction-by-id/get-transaction-by-id.component').then(m => m.GetTransactionByIdComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'admin/all-users',
    loadComponent: () => import('./components/user/get-all-users/get-all-users.component').then(m => m.GetAllUsersComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'admin/user/:id',
    loadComponent: () => import('./components/user/get-user-by-id/get-user-by-id.component').then(m => m.GetUserByIdComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'admin/myProfile',
    loadComponent: () => import('./components/user/user-profile/user-profile.component').then(m => m.UserProfileComponent),
    canActivate: [roleGuard]
  },


  {
    path: 'farmer/addCropToList',
    loadComponent: () => import('./components/cropList/add-crop-to-list/add-crop-to-list.component').then(m => m.AddCropToListComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'farmer/myListedCrops',
    loadComponent: () => import('./components/cropList/listed-crops-by-farmer/listed-crops-by-farmer.component').then(m => m.ListedCropsByFarmerComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'farmer/myTransactions',
    loadComponent: () => import('./components/transaction/all-transactions/all-transactions.component').then(m => m.AllTransactionsComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'farmer/transaction/:id',
    loadComponent: () => import('./components/transaction/get-transaction-by-id/get-transaction-by-id.component').then(m => m.GetTransactionByIdComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'farmer/myProfile',
    loadComponent: () => import('./components/user/user-profile/user-profile.component').then(m => m.UserProfileComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'farmer/reviews',
    loadComponent: () => import('./components/review/get-my-reviews/get-my-reviews.component').then(m => m.GetMyReviewsComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'dealer/listedCrops',
    loadComponent: () => import('./components/cropList/listed-available-crops/listed-available-crops.component').then(m => m.ListedAvailableCropsComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'dealer/cropList/:id',
    loadComponent: () => import('./components/cropList/get-crop-list-by-id/get-crop-list-by-id.component').then(m => m.GetCropListByIdComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'dealer/mySubscriptions',
    loadComponent: () => import('./components/subscription/subscription.component').then(m => m.SubscriptionComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'dealer/myTransactions',
    loadComponent: () => import('./components/transaction/all-transactions/all-transactions.component').then(m => m.AllTransactionsComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'dealer/transaction/:id',
    loadComponent: () => import('./components/transaction/get-transaction-by-id/get-transaction-by-id.component').then(m => m.GetTransactionByIdComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'dealer/myProfile',
    loadComponent: () => import('./components/user/user-profile/user-profile.component').then(m => m.UserProfileComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'change-password',
    loadComponent: () => import('./components/user/change-password/change-password.component').then(m=> m.ChangePasswordComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'farmer-dashboard',
    loadComponent: () => import('./components/dashboards/farmer-dashboard/farmer-dashboard.component').then(m => m.FarmerDashboardComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'dealer-dashboard',
    loadComponent: () => import('./components/dashboards/dealer-dashboard/dealer-dashboard.component').then(m => m.DealerDashboardComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'admin-dashboard',
    loadComponent: () => import('./components/dashboards/admin-dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent),
    canActivate: [roleGuard]
  },
  {
    path: 'unauthorized',
    loadComponent: () => import('./shared/unauthorized/unauthorized.component').then(m => m.UnauthorizedComponent)
  },
  {
    path: '**',
    loadComponent: () => import('./shared/notfound/notfound.component').then(m => m.NotFoundComponent)
  }
];
