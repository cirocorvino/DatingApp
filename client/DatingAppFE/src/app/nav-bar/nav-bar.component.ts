import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { MembersService } from '../_services/members.service';
import { HasRoleDirective } from '../_directives/has-role.directive';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [FormsModule, BsDropdownModule, RouterLink, RouterLinkActive, HasRoleDirective],
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.css'
})
export class NavBarComponent {

  model: any = {};
  accountService = inject(AccountService)
  membersService = inject(MembersService);
  router = inject(Router);
  toastr = inject(ToastrService);
  
  login() {
    this.accountService.login(this.model).subscribe({
      next: _ => {
        this.router.navigateByUrl('/members');
      },
      error: error => this.toastr.error(error.error)
    });
  }

  logout(){
    this.accountService.logout();
    this.membersService.paginatedResult.set(null);
    this.router.navigateByUrl('/');
  }
}
