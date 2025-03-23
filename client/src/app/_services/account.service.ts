import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { User } from '../_models/user';
import { map } from 'rxjs/internal/operators/map';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);

  baseURL = 'https://localhost:5001/api/';

  curentUser = signal<User | null>(null);

  login(model: any) {
    return this.http.post<User>(this.baseURL + 'account/user/login', { userName: model.userName, password: model.password })
                    .pipe(
                      map(user => {
                        if(user){
                          localStorage.setItem("user", JSON.stringify(user));
                          this.curentUser.set(user)
                        }
                        return user;
                      })
                    );
  }

  logout() {
    localStorage.removeItem('user');
    this.curentUser.set(null);
  }

  register(model: any) {
    return this.http.post<User>(this.baseURL + 'account/user/register', { userName: model.userName, password: model.password })
                    .pipe(
                      map(user => {
                        if(user){
                          localStorage.setItem("user", JSON.stringify(user));
                          this.curentUser.set(user)
                        }
                        return user;
                      })
                    );
  }
}
