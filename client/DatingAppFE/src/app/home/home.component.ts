import { Component, inject } from '@angular/core';
import { RegisterComponent } from "../register/register.component";
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RegisterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  registerMode = false;
  http = inject(HttpClient);
  users : any;

  registerToggle(){
      this.registerMode = ! this.registerMode;
  }

  closeRegisterForm(){
    this.registerMode = false;
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
