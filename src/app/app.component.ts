import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-root',
  standalone: true,
  template: `
    <mat-toolbar color="primary">
      <span>YouTube Companion Dashboard</span>
      <span class="spacer"></span>
      <button mat-icon-button routerLink="/dashboard">
        <mat-icon>dashboard</mat-icon>
      </button>
      <button mat-icon-button routerLink="/notes">
        <mat-icon>note</mat-icon>
      </button>
    </mat-toolbar>
    <router-outlet></router-outlet>
  `,
  styles: [`
    .spacer { flex: 1 1 auto; }
  `],
  imports: [RouterModule, MatToolbarModule, MatButtonModule, MatIconModule]
})
export class AppComponent {}
