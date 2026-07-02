import { ShortenIdPipe } from './../../../Pipes/shorten-id.pipe';
import { Component, OnInit, inject } from '@angular/core';
import { ReviewService } from '../../../services/review/review.service';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-get-my-reviews',
  imports: [CommonModule, ShortenIdPipe],
  templateUrl: './get-my-reviews.component.html'
})
export class GetMyReviewsComponent implements OnInit {
  reviews:any[] = [];
  loading: boolean = true;
  errorMessage: string = '';

  private readonly reviewService = inject(ReviewService);

  ngOnInit(): void {
    this.reviewService.getMyReviews().subscribe({
      next: (data) => {
        this.loading = false;
        this.reviews = data;
      },
      error: (err) => {
        console.log(err);
        this.loading = false;
        this.errorMessage = err.error;
      }
    })
  }
}
