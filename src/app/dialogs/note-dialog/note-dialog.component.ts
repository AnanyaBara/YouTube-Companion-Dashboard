import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';
import { NotesService } from '../../services/notes.service';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-note-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, MatInputModule, MatButtonModule],
  template: `
    <h2>{{data.note ? 'Edit Note' : 'Add Note'}}</h2>
    <mat-form-field appearance="fill">
      <mat-label>Title</mat-label>
      <input matInput [(ngModel)]="note.title">
    </mat-form-field>
    <mat-form-field appearance="fill">
      <mat-label>Body</mat-label>
      <textarea matInput [(ngModel)]="note.body"></textarea>
    </mat-form-field>
    <mat-form-field appearance="fill">
      <mat-label>Tags (comma separated)</mat-label>
      <input matInput [(ngModel)]="tags">
    </mat-form-field>
    <button mat-raised-button color="primary" (click)="save()">Save</button>
    <button mat-button (click)="dialogRef.close()">Cancel</button>
  `,
})
export class NoteDialogComponent {
  note: any = { title: '', body: '', tags: [] };
  tags: string = '';

  constructor(
    public dialogRef: MatDialogRef<NoteDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private notesService: NotesService
  ) {
    if (data.note) {
      this.note = { ...data.note };
      this.tags = (data.note.tags || []).join(',');
    }
  }

  save(): void {
    this.note.tags = this.tags.split(',').map((t: string) => t.trim());
    const obs = this.note.id
      ? this.notesService.update(this.data.userId, this.note.id, this.note)
      : this.notesService.create(this.data.userId, this.note);
    obs.subscribe(() => this.dialogRef.close());
  }
}
