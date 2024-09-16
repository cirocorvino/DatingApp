import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [FormsModule, BsDropdownModule],
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.css'
})
export class NavBarComponent {

  model: any = {};
  accountService = inject(AccountService)
  
  login() {
    this.accountService.login(this.model).subscribe({
      next: user => {
        console.log(user);
      },
      error: error => {console.log(error)},
      complete : () => {console.log("login request completed")}
    });
  }

  logout(){
    this.accountService.logout();
  }
}
