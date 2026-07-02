import { UserService } from './../../../services/user/user.service';
import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-dashboard',
  imports: [CommonModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.componnent.css'
})
export class AdminDashboardComponent implements OnInit {
  dashBoardDto: any[] = [
    {
      title: "Registered Dealers",
      icon: "fas fa-user-tie",
      value: 0
    },
    {
      title: "Registered Farmers",
      icon: "fas fa-tractor",
      value: 0
    },
    {
      title: "Allowed Crop",
      icon: "fas fa-seedling",
      value: 0
    },
    {
      title: "Available Crop Listings",
      icon: "fas fa-leaf",
      value: 0
    },
    {
      title: "Completed Deals",
      icon: "fas fa-handshake",
      value: 0
    },
    {
      title: "Total Deal Value",
      icon: "fa-solid fa-money-check",
      value: 0
    },
  ]
  
  private readonly userService = inject(UserService);

  ngOnInit(): void {
    this.userService.getAdminDashBoard().subscribe({
      next: (data) => {
        console.log(data);
        this.dashBoardDto = this.dashBoardDto.map(item => {
          switch (item.title) {
            case "Registered Dealers":
              return { ...item, value: data.totalDealers };
            case "Registered Farmers":
              return { ...item, value: data.totalFarmers };
            case "Allowed Crop":
              return { ...item, value: data.totalAllowedCrops };
            case "Available Crop Listings":
              return { ...item, value: data.totalListedCrops };
            case "Completed Deals":
              return { ...item, value: data.completedDeals };
            case "Total Deal Value":
              return { ...item, value: "₹ "+ data.totalDealValue };
            default:
              return item;
          }
        });
      },
      error: (err) => {
        console.log(err);
      }
    })
    
  }
}
