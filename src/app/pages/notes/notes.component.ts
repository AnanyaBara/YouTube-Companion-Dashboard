import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotesService } from '../../services/notes.service';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { NoteDialogComponent } from '../../dialogs/note-dialog/note-dialog.component';

@Component({
  selector: 'app-notes',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatDialogModule],
  template: `
    <h2>Notes</h2>
    <button mat-raised-button color="primary" (click)="openDialog()">Add Note</button>
    <table mat-table [dataSource]="notes" class="mat-elevation-z8">
      <ng-container matColumnDef="title">
        <th mat-header-cell *matHeaderCellDef>Title</th>
        <td mat-cell *matCellDef="let note">{{note.title}}</td>
      </ng-container>
      <ng-container matColumnDef="body">
        <th mat-header-cell *matHeaderCellDef>Body</th>
        <td mat-cell *matCellDef="let note">{{note.body}}</td>
      </ng-container>
      <ng-container matColumnDef="actions">
        <th mat-header-cell *matHeaderCellDef>Actions</th>
        <td mat-cell *matCellDef="let note">
          <button mat-button color="accent" (click)="editNote(note)">Edit</button>
          <button mat-button color="warn" (click)="deleteNote(note.id)">Delete</button>
        </td>
      </ng-container>
      <tr mat-header-row *matHeaderRowDef="['title','body','actions']"></tr>
      <tr mat-row *matRowDef="let row; columns: ['title','body','actions']"></tr>
    </table>
  `,
})
export class NotesComponent implements OnInit {
  notes: any[] = [];
  userId = localStorage.getItem('userId') || '';

  constructor(private notesService: NotesService, private dialog: MatDialog) {}

  ngOnInit(): void {
    this.loadNotes();
  }

  loadNotes(): void {
    if (this.userId) {
      this.notesService.search(this.userId).subscribe(res => this.notes = res);
    }
  }

  openDialog(): void {
    const dialogRef = this.dialog.open(NoteDialogComponent, { data: { userId: this.userId } });
    dialogRef.afterClosed().subscribe(() => this.loadNotes());
  }

  editNote(note: any): void {
    const dialogRef = this.dialog.open(NoteDialogComponent, { data: { note, userId: this.userId } });
    dialogRef.afterClosed().subscribe(() => this.loadNotes());
  }

  deleteNote(id: string): void {
    this.notesService.delete(this.userId, id).subscribe(() => this.loadNotes());
  }
}
