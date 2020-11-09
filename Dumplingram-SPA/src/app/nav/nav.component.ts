import { Component, OnInit, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { User } from '../_models/User';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { UserService } from '../_services/user.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  modalRef: BsModalRef;
  searchedUsers: User[];
  word: string;

  constructor(
    public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router,
    private modalService: BsModalService,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    this.word = '';
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    this.alertify.message('logged out');
    this.router.navigate(['']);
  }

  loadUsers() {
    this.userService.getUsers(this.word).subscribe(
      (res: User[]) => {
        this.searchedUsers = res;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  openSearchedUsers(template: TemplateRef<any>) {
    this.loadUsers();
    this.modalRef = this.modalService.show(template);
  }

  onKey(event) {
    this.word = event.target.value;
    this.loadUsers();
  }

}
