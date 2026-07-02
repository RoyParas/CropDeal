import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  imports: [RouterLink, CommonModule],
  templateUrl: './home.component.html'
})
export class HomeComponent {
  logoWithoutbgBlackText = "assets/logo/logo-without-bg-black-text.png";


  features = [
    {
      title: 'No Middlemen',
      description: 'Farmers can directly sell their produce to dealers, eliminating unnecessary commissions and extra charges.',
      icon: 'fas fa-user-times',
      iconColor: 'text-success'
    },
    {
      title: 'Transparent Pricing',
      description: 'Get fair prices for your crops, and avoid hidden fees and commissions that traditional markets charge.',
      icon: 'fas fa-dollar-sign',
      iconColor: 'text-primary'
    },
    {
      title: 'Easy Transactions',
      description: 'All transactions are handled directly through the app, ensuring secure payments and reducing any financial concerns.',
      icon: 'fas fa-credit-card',
      iconColor: 'text-warning'
    }
  ];

  testimonials = [
    {
      quote: '"CropDeal has transformed the way we sell crops. The platform is easy to use, and I don\'t have to worry about market commissions anymore!"',
      author: 'Rajesh Kumar, Farmer'
    },
    {
      quote: '"This is a game-changer for dealers. I can directly connect with farmers and get fresh produce at great prices."',
      author: 'Arvind Patel, Crop Dealer'
    }
  ];
}
