import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  imports: [],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  http = inject(HttpClient);
  title = 'Warmaster';
  users: any;

  ngOnInit(): void {
    this.http.get("https://localhost:5001/api/users/getusers").subscribe({
      next: response => this.users = response,
      error: error => console.log(error),
      complete: () => { console.log('This request has completed') }
    })
  }
}
