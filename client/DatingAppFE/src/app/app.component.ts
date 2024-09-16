import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavBarComponent } from "./nav-bar/nav-bar.component";
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavBarComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {

  http = inject(HttpClient);
  accountService = inject(AccountService);
  title = 'DatingApp';
  users : any;
  
  ngOnInit(): void {
    //Called after the constructor, initializing input properties, and the first call to ngOnChanges.
    //Add 'implements OnInit' to the class.
    this.setCurrentUser();
    this.getUsers();
  }

  setCurrentUser() {
    const user = localStorage.getItem('user');
    if(user) {
      this.accountService.currentUser.set(JSON.parse(user));
    }
  }

  getUsers(){
    this.http.get('https://localhost:7246/api/users').subscribe ({
      next: (data) => { 
        this.users = data;
        console.log(data)
      },
      error:(err)=> { console.log(err)},
      complete: () => { console.log('request get users completed')}
  });
  }

}
