import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SubscriptionService } from '../../services/subscription/subscription.service';
import { formatDistanceToNow } from 'date-fns';

@Component({
  selector: 'app-subscription',
  imports: [CommonModule],
  templateUrl: './subscription.component.html',
})
export class SubscriptionComponent implements OnInit {
  loading!: boolean;
  errorMessage: string = '';
  subscribedCrops: any[] = [];

  subscriptionService: SubscriptionService = inject(SubscriptionService);

  ngOnInit(): void {
    this.loading = true;
    this.subscriptionService.getSubscribedCropsByDealer().subscribe({
      next: (data) => {
        this.loading = false;
        this.subscribedCrops = data;
      },
      error: (err) => {
        this.loading = false;
        console.log(err);
      }
    })
  }

  getTimeAgo(postedAt: string | Date): string {
    return formatDistanceToNow(new Date(postedAt), { addSuffix: true });
  }

  deleteSubscription(subscriptionId: string){
    this.subscriptionService.deleteSubscription(subscriptionId).subscribe({
      next: (data) => {
        this.subscribedCrops = this.subscribedCrops.filter(sc => sc.id != subscriptionId);
      },
      error: (err) => console.log(err)
    })
  }
}
