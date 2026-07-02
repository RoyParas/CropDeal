import { Component, inject, OnInit } from '@angular/core';
import { UserDto, UserService } from '../../../services/user/user.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-get-all-users',
  imports: [FormsModule, CommonModule],
  templateUrl: './get-all-users.component.html'
})

export class GetAllUsersComponent implements OnInit {

  private readonly userService = inject(UserService);
  private readonly router = inject(Router);

  users: UserDto[] = [];
  error: string | null = null;
  userIdString: string = '';
  loading: boolean = true;

  ngOnInit(): void {
    this.fetchUsers();
  }

  closeError(): void {
    this.error = '';
  }

  getUserDetails(userId: string) {
    this.router.navigate(['/admin/user', userId]);
  }


  fetchUsers(): void {
    this.userService.getAllUsers().subscribe({
      next: (data) => {
        this.users = data;
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.handleError(err);
      }
    });
  }


  // Handle user status change
  onStatusChange(user: UserDto): void {
    this.userService.changeUserStatus(user.email).subscribe({
      next: () => {
        user.isActive = !user.isActive;
      },
      error: (err) => this.handleError(err),
    });
  }

  // Generalized error handling
  handleError(err: any): void {
    console.log(err);
    if(err){
      if(typeof err === 'object'){
        this.error = err.error ;
      }
      if(typeof err === 'string'){
        this.error = err;
      }
    }
    else console.error(err);
  }

}
