import { Component, inject, OnInit } from '@angular/core';
import { UserService } from '../../../services/user/user.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dealer-dashboard',
  imports: [CommonModule],
  templateUrl: './dealer-dashboard.component.html',
  styleUrl: './dealer-dashboard.componnent.css'
})
export class DealerDashboardComponent implements OnInit {

  dashBoardDto: any[] = [
    {
      title: "Total Quantity Purchased in Kg",
      icon: "fas fa-shopping-basket",
      value: 0
    },
    {
      title: "Total Deals Done",
      icon: "fas fa-handshake",
      value: 0
    },
    {
      title: "Total Expenditures",
      icon: "fa-solid fa-money-check-dollar",
      value: 0
    }
  ]

  private readonly userService = inject(UserService);
  ngOnInit(): void {
    this.userService.getDealerDashBoard().subscribe({
      next: (data) => {
        console.log(data);
        this.dashBoardDto = this.dashBoardDto.map(item => {
          switch (item.title) {
            case "Total Quantity Purchased in Kg":
              return { ...item, value: data.totalQuantityPurchased };
            case "Total Deals Done":
              return { ...item, value: data.completedDeals };
            case "Total Expenditures":
              return { ...item, value: "₹ " + data.totalSpent };
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
