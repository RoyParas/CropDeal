import { Component, inject, OnInit } from '@angular/core';
import { UserService } from '../../../services/user/user.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-farmer-dashboard',
  imports: [CommonModule],
  templateUrl: './farmer-dashboard.component.html',
  styleUrl : './farmer-dashboard.componnent.css'
})
export class FarmerDashboardComponent implements OnInit{

  dashBoardDto: any[] = [
    {
      title: "My Listed Crops",
      icon: "fas fa-leaf",
      value: 0
    },
    {
      title: "Deals Done",
      icon: "fas fa-handshake",
      value: 0
    },
    {
      title: "Average Rating",
      icon: "fas fa-star",
      value: 0
    },
    {
      title: "Total Earnings",
      icon: "fas fa-wallet",
      value: 0
    }
  ]

  private readonly userService = inject(UserService);
  ngOnInit(): void {
    this.userService.getFarmerDashBoard().subscribe({
      next: (data) => {
        console.log(data);
        this.dashBoardDto = this.dashBoardDto.map(item => {
          switch (item.title) {
            case "My Listed Crops":
              return { ...item, value: data.totalListedCrops };
            case "Deals Done":
              return { ...item, value: data.completedDeals };
            case "Average Rating":
              return { ...item, value: data.averageRating + "/5" };
            case "Total Earnings":
              return { ...item, value: "₹ " + data.totalEarnings };
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
