import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class VideoService {
  constructor(private http: HttpClient) {}

  listUploads(userId: string): Observable<any> {
    return this.http.get('/api/videos', { headers: { 'X-User-Id': userId } });
  }

  getVideo(userId: string, id: string): Observable<any> {
    return this.http.get(`/api/videos/${id}`, { headers: { 'X-User-Id': userId } });
  }

  updateVideo(userId: string, id: string, data: { title: string; description: string }): Observable<any> {
    return this.http.put(`/api/videos/${id}`, data, { headers: { 'X-User-Id': userId } });
  }

  postComment(userId: string, videoId: string, text: string): Observable<any> {
    return this.http.post(`/api/videos/${videoId}/comment`, { text }, { headers: { 'X-User-Id': userId } });
  }

  replyToComment(userId: string, videoId: string, parentId: string, text: string): Observable<any> {
    return this.http.post(`/api/videos/${videoId}/comment/${parentId}/reply`, { text }, { headers: { 'X-User-Id': userId } });
  }

  deleteComment(userId: string, commentId: string): Observable<any> {
    return this.http.delete(`/api/comments/${commentId}`, { headers: { 'X-User-Id': userId } });
  }
}
