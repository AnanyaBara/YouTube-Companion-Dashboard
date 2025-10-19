import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { VideoService } from '../../services/video.service';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { CommentDialogComponent } from '../../dialogs/comment-dialog/comment-dialog.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-video-details',
  standalone: true,
  imports: [CommonModule, FormsModule, MatCardModule, MatInputModule, MatButtonModule, MatDialogModule],
  template: `
    <h2>Video Details</h2>
    <mat-card *ngIf="video">
      <h3>{{video.snippet.title}}</h3>
      <textarea [(ngModel)]="video.snippet.description" rows="3" cols="50"></textarea>
      <br/>
      <button mat-raised-button color="primary" (click)="updateVideo()">Update</button>
      <button mat-raised-button color="accent" (click)="openCommentDialog()">Add Comment</button>
    </mat-card>
  `,
})
export class VideoDetailsComponent implements OnInit {
  video: any;
  userId = localStorage.getItem('userId') || '';
  videoId = '';

  constructor(private route: ActivatedRoute, private videoService: VideoService, private dialog: MatDialog) {}

  ngOnInit(): void {
    this.videoId = this.route.snapshot.params['id'];
    if (this.userId && this.videoId) {
      this.videoService.getVideo(this.userId, this.videoId).subscribe(v => this.video = v);
    }
  }

  updateVideo(): void {
    this.videoService.updateVideo(this.userId, this.videoId, {
      title: this.video.snippet.title,
      description: this.video.snippet.description
    }).subscribe();
  }

  openCommentDialog(): void {
    const dialogRef = this.dialog.open(CommentDialogComponent, {
      data: { videoId: this.videoId, userId: this.userId }
    });
    dialogRef.afterClosed().subscribe();
  }
}
