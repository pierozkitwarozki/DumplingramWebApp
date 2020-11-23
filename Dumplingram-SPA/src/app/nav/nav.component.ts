import { Component, OnInit, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { User } from '../_models/user';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { MessageService } from '../_services/message.service';
import { PresenceService } from '../_services/presence.service';
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
  photoUrl: string;

  constructor(
    public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router,
    private modalService: BsModalService,
    private userService: UserService,
    private presenceService: PresenceService,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    this.word = '';

    if (this.authService.currentUser.photoUrl) {
      this.authService.changeMemberPhoto(this.authService.currentUser.photoUrl);
    }

    this.authService.currentPhotoUrl.subscribe(
      (photoUrl) => (this.photoUrl = photoUrl)
    );
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.presenceService.stopHubConnection();
    this.messageService.stopConnection();
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    this.router.navigate(['/login']);
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
    this.modalRef = this.modalService.show(template);
  }

  onKey(event) {
    this.word = event.target.value;
    this.loadUsers();
  }

  close() {
    this.modalRef.hide();
    this.searchedUsers = null;
  }
}
