import { Component, inject, OnInit} from '@angular/core';
import { Router, RouterLink, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth/auth.service';
import { NotificationService } from '../../services/notification/notification.service';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, CommonModule, RouterModule],
  templateUrl: './navbar.component.html'
})
export class NavbarComponent implements OnInit{

  logoWithoutbgBlackText = "assets/logo/logo-without-bg-black-text.png"
  userName !: string;
  notificationClicked!: boolean;
  notifications: any[] = [];

  private readonly authService: AuthService = inject(AuthService);
  private readonly notificationService: NotificationService = inject(NotificationService);
  private readonly router: Router = inject(Router);

  ngOnInit(): void {
    this.notificationClicked = false;
    if(this.isLoggedIn() && this.getRole() == 'Dealer'){
      this.getNotifications();
    }
  }

  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  getRole(): string | null {
    return this.authService.getRole();
  }

  getNotifications() {
    this.notificationService.getUserNotifications().subscribe({
      next: (data) => {
        this.notifications = data;
      },
      error: (err) => console.log(err)
    })
  }

  deleteNotification(messageId : string){
    this.notificationService.deleteNotification(messageId).subscribe({
      next: () => {
        this.notifications = this.notifications.filter(mes => mes.id != messageId);
      },
      error: (err) => {
        console.log(err);
      }
    })
  }

  logout(): void {
    this.authService.logout(); // clear token/session
    this.router.navigate(['/']);
  }

  isLoginPage(): boolean {
    return this.router.url === '/login';
  }

  isSignUpPage() : boolean {
    return this.router.url === '/signup';
  }

  goBack(){
    this.router.navigateByUrl('/');
  }
}
