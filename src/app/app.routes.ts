import { provideRouter, Routes } from '@angular/router';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { VideoDetailsComponent } from './pages/video-details/video-details.component';
import { NotesComponent } from './pages/notes/notes.component';

export const appRoutes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'video/:id', component: VideoDetailsComponent },
  { path: 'notes', component: NotesComponent },
  { path: '**', redirectTo: 'dashboard' }, // fallback
];

// Provide the router for standalone bootstrap
export const appRouterProviders = [
  provideRouter(appRoutes)
];
