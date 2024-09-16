import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { map } from 'rxjs';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  http = inject(HttpClient);
  urlBase = 'https://localhost:7246/';
  urlAccountServce = this.urlBase + 'api/account/';

  currentUser = signal<User | null>(null);
  
  login(model : any){
    return this.http.post<User>(this.urlAccountServce + 'login', model).pipe(
      map( user => {
        if(user){
          localStorage.setItem("user", JSON.stringify(user));
          this.currentUser.set(user);
        }
        return user;
      })
    )
  }

  register(model : any){
    return this.http.post<User>(this.urlAccountServce + 'register', model).pipe(
      map( user => {
        if(user){
          localStorage.setItem("user", JSON.stringify(user));
          this.currentUser.set(user);
        }
        return user;
      })
    )
  }
  
  logout(){
    localStorage.removeItem("user");
    this.currentUser.set(null);
  }
}
