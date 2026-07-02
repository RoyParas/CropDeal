import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { formatDistanceToNow } from 'date-fns';
import { CropListService } from '../../../services/cropList/crop-list.service';
import { CropService } from '../../../services/crop/crop.service';
import { SubscriptionService } from '../../../services/subscription/subscription.service';
import { subscriptionLogsToBeFn } from 'rxjs/internal/testing/TestScheduler';
import { Router } from '@angular/router';

@Component({
  selector: 'app-listed-available-crops',
  imports: [CommonModule],
  templateUrl: './listed-available-crops.component.html'
})
export class ListedAvailableCropsComponent {
  loading!: boolean;
  crops: any[] = [];
  cropsNames: string[] = [];

  cropToSearch!: string;
  isSearching: boolean = false;
  noSearchFoundForCrop!: string;
  isAll!: boolean ;
  errorMessage!: string;

  private readonly router: Router = inject(Router);
  private readonly cropListService: CropListService = inject(CropListService);
  private readonly cropService: CropService = inject(CropService);
  private readonly subscriptionService: SubscriptionService = inject(SubscriptionService);

  ngOnInit(): void {
    this.errorMessage = '';
    this.cropToSearch = '';
    this.noSearchFoundForCrop = ''
    this.loading = true;

    this.cropService.getDistinctCropsName().subscribe({
      next: (data) => {
        this.cropsNames = data;
      },
      error: (err) => {
        console.log(err);
        this.errorMessage = err
      }
    })

    this.cropListService.getAvailableCrops().subscribe({
      next: (data) => {
        this.isAll = true;
        this.loading = false;
        this.crops = data;
      },
      error: (err) => {
        this.loading = false;
        console.log(err);
        const error = err.error;
        if (error && typeof error === 'object') {
          const allErrors = Object.values(error)
            .flat()
            .filter(msg => typeof msg === 'string');

          this.errorMessage = allErrors[0];
        }
      }
    });
  }

  onNameChange(event: Event) {
    this.cropToSearch = (event.target as HTMLSelectElement).value;
  }

  search(): void {
    if (!this.cropToSearch) return;

    this.isSearching = true;
    this.loading = true;

    this.cropListService.searchListedCrops(this.cropToSearch).subscribe({
      next: (data) => {
        this.errorMessage = '';
        this.isSearching = false;
        this.loading = false;
        this.isAll = false;

        if(data.length == 0){
          this.noSearchFoundForCrop = this.cropToSearch;
        }
        else{
          this.noSearchFoundForCrop = '';
        }
        this.crops = data;
      },
      error: (err) => {
        console.log(err);
        const error = err.error;
        if (error && typeof error === 'object') {
          const allErrors = Object.values(error)
            .flat()
            .filter(msg => typeof msg === 'string');

          this.errorMessage = allErrors[0];
        }
        this.loading = false;
        this.isAll = false;
        this.isSearching = false;
      }
    })
  }

  getAvailableCrops() {
    this.ngOnInit();
  }

  subscribe() {
    this.subscriptionService.addSubscription(this.noSearchFoundForCrop).subscribe({
      next: (data) => {
        this.noSearchFoundForCrop = '';
        this.ngOnInit();
      },
      error: (err) => {
        console.log(err);
        this.errorMessage = err.error.message;
        this.noSearchFoundForCrop = '';
      }
    });
  }

  getTimeAgo(postedAt: string | Date): string {
    return formatDistanceToNow(new Date(postedAt), { addSuffix: true });
  }

  getCropListById(cropListingId : string) {
    this.router.navigate(['dealer/cropList', cropListingId]);
  }

  closeError() {
    this.errorMessage = '';
    this.cropToSearch = '';
    this.ngOnInit();
  }
}
