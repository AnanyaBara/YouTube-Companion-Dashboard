import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';
import { VideoService } from '../../services/video.service';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-comment-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, MatInputModule, MatButtonModule],
  template: `
    <h2>Add Comment</h2>
    <mat-form-field appearance="fill">
      <mat-label>Comment</mat-label>
      <textarea matInput [(ngModel)]="text"></textarea>
    </mat-form-field>
    <button mat-raised-button color="primary" (click)="save()">Post</button>
    <button mat-button (click)="dialogRef.close()">Cancel</button>
  `,
})
export class CommentDialogComponent {
  text: string = '';

  constructor(
    public dialogRef: MatDialogRef<CommentDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private videoService: VideoService
  ) {}

  save(): void {
    if (this.text.trim()) {
      this.videoService.postComment(this.data.userId, this.data.videoId, this.text).subscribe(() => {
        this.dialogRef.close();
      });
    }
  }
}
