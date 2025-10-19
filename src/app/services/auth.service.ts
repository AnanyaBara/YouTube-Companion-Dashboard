import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = 'https://localhost:7088/api/auth';

  constructor(private http: HttpClient) {}

  loginWithGoogle(): void {
    window.location.href = `${this.baseUrl}/google/login`;
  }

  getCallbackUser(code: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/google/callback?code=${code}`);
  }
}
