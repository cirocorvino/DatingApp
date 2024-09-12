import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'DatingApp';
  users : any;
  http = inject(HttpClient);

  

  ngOnInit(): void {
    //Called after the constructor, initializing input properties, and the first call to ngOnChanges.
    //Add 'implements OnInit' to the class.
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
