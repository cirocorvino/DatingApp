import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { map } from 'rxjs';
import { User } from '../_models/user';
import { environment } from '../../environments/environment';
import { LikesService } from './likes.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private http = inject(HttpClient);
  private likeService = inject(LikesService);

  urlBase = environment.apiUrl;
  urlAccountServce = this.urlBase + 'account/';

  currentUser = signal<User | null>(null);
  
  login(model : any){
    return this.http.post<User>(this.urlAccountServce + 'login', model).pipe(
      map( user => {
        if(user){
          this.setCurrentUser(user);
        }
        return user;
      })
    )
  }

  register(model : any){
    return this.http.post<User>(this.urlAccountServce + 'register', model).pipe(
      map( user => {
        if(user){
          this.setCurrentUser(user);
        }
      })
    )
  }

  setCurrentUser(user: User) {
    localStorage.setItem("user", JSON.stringify(user));
    this.currentUser.set(user);
    this.likeService.getLikeIds();
  }

  logout(){
    localStorage.removeItem("user");
    this.currentUser.set(null);
  }
}
