YouTube Companion Dashboard

This project is a YouTube Companion Dashboard built with:

Frontend: Angular 19 + Angular Material

Backend: .NET 8 Web API + Entity Framework Core + PostgreSQL

YouTube API integration to manage videos, comments, and notes.

It allows users to:

List their uploaded YouTube videos

Update video title and description

Post, reply, and delete comments

Add and search notes with tags

Track event logs

Backend Setup (.NET 8)
Prerequisites

.NET 8 SDK

PostgreSQL

Google OAuth credentials with YouTube Data API v3 enabled

Quickstart

Clone the repo and navigate to backend folder
Set environment variables
Restore packages:

dotnet restore


Run EF Core migrations:

dotnet tool install --global dotnet-ef --version 8.0.0
dotnet ef migrations add InitialCreate -p src/YouTubeCompanion -s src/YouTubeCompanion
dotnet ef database update -p src/YouTubeCompanion -s src/YouTubeCompanion


Start the backend server:

dotnet run --project src/YouTubeCompanion


Backend Structure

src/YouTubeCompanion/
├── Controllers/         # API controllers: AuthController, VideosController, NotesController
├── Data/                # EF Core DbContext
├── Models/              # User, Note, EventLog, UserToken
├── Repositories/        # CRUD repositories for Users and Notes
├── Services/            # YouTube API service, Event Logger
├── Program.cs           # Application entry point
├── appsettings.json     # Configuration
└── YouTubeCompanion.csproj


API Endpoints

GET /api/auth/google/login → Google OAuth login

GET /api/auth/google/callback → OAuth callback

GET /api/videos → List user uploads

GET /api/videos/{id} → Get video details

PUT /api/videos/{id} → Update video title/description

POST /api/videos/{id}/comment → Post comment

POST /api/videos/{id}/comment/{parent}/reply → Reply to comment

DELETE /api/comments/{commentId} → Delete comment

GET /api/notes → Search notes

POST /api/notes → Create note

PUT /api/notes/{id} → Update note

DELETE /api/notes/{id} → Delete note

Frontend Setup (Angular 19)
Prerequisites

Node.js
 (v20+)

Angular CLI 19

Quickstart

Navigate to frontend folder
Install dependencies:

npm install


Run the Angular development server:

ng serve


Open http://localhost:4200
 in your browser. The app reloads automatically on file changes.

Frontend Structure

src/app/
├── pages/
│   ├── dashboard/           # Dashboard page with video list
│   ├── video-details/       # Video detail page with comments
│   └── notes/               # Notes management page
├── dialogs/
│   ├── note-dialog/         # Add/Edit note dialog
│   └── comment-dialog/      # Post/reply comment dialog
├── services/
│   ├── auth.service.ts      # Google OAuth login
│   ├── video.service.ts     # Video API calls
│   ├── note.service.ts      # Notes API calls
│   └── comment.service.ts   # Comments API calls
├── app.component.ts
└── app.routes.ts            # Standalone Angular 19 routing

⚡ Connecting Frontend to Backend

The backend expects X-User-Id header for authentication (replace with JWT in production).

OAuth flow:

User clicks Login with Google → redirects to backend/api/auth/google/login

After successful OAuth, backend redirects to frontend/auth/success?userId=<id>

Frontend stores userId and sends it in X-User-Id header for all API requests

Building
Frontend
ng build


Builds production-ready files in dist/.

Backend
dotnet publish -c Release -o ./publish

Testing

Frontend unit tests:

ng test


Backend unit tests: Implement using xUnit or preferred framework.

Additional Resources

Angular CLI Docs

Angular Material Docs

YouTube Data API v3 Docs

EF Core Docs



