import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class NotesService {
  constructor(private http: HttpClient) {}

  search(userId: string, videoId?: string, q?: string, tags?: string[]): Observable<any> {
    let params = new HttpParams();
    if (videoId) params = params.set('videoId', videoId);
    if (q) params = params.set('q', q);
    if (tags) params = params.set('tags', tags.join(','));
    return this.http.get('/api/notes', { headers: { 'X-User-Id': userId }, params });
  }

  create(userId: string, note: any): Observable<any> {
    return this.http.post('/api/notes', note, { headers: { 'X-User-Id': userId } });
  }

  update(userId: string, id: string, note: any): Observable<any> {
    return this.http.put(`/api/notes/${id}`, note, { headers: { 'X-User-Id': userId } });
  }

  delete(userId: string, id: string): Observable<any> {
    return this.http.delete(`/api/notes/${id}`, { headers: { 'X-User-Id': userId } });
  }
}
