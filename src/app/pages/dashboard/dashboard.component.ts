import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { VideoService } from '../../services/video.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, RouterLink],
  template: `
    <h2>Your Uploaded Videos</h2>
    <mat-card *ngFor="let video of videos">
      <h3>{{video.snippet.title}}</h3>
      <p>{{video.snippet.description}}</p>
      <button mat-raised-button color="primary" [routerLink]="['/video', video.id]">Details</button>
    </mat-card>
  `,
})
export class DashboardComponent implements OnInit {
  videos: any[] = [];
  userId = localStorage.getItem('userId') || '';

  constructor(private videoService: VideoService) {}

  ngOnInit(): void {
    if (this.userId) {
      this.videoService.listUploads(this.userId).subscribe(res => this.videos = res.items || []);
    }
  }
}
