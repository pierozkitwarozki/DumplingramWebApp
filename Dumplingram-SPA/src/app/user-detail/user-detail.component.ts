import { PathLocationStrategy } from '@angular/common';
import { FnParam } from '@angular/compiler/src/output/output_ast';
import { Component, OnInit, TemplateRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BsModalRef, BsModalService, ModalModule } from 'ngx-bootstrap/modal';
import { error } from 'protractor';
import { take } from 'rxjs/operators';
import { Follow } from '../_models/Follow';
import { Message } from '../_models/Message';
import { Photo } from '../_models/Photo';

import { User } from '../_models/User';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { MessageService } from '../_services/message.service';
import { PresenceService } from '../_services/presence.service';
import { UserService } from '../_services/user.service';

@Component({
  selector: 'app-user-detail',
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.css'],
})
export class UserDetailComponent implements OnInit {
  user: User;
  follow: Follow;
  modalRef: BsModalRef;
  followeeItems: any;
  followerItems: any;
  photo: Photo;
  message: string;
  newMessage: any = {};

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private authService: AuthService,
    private alertify: AlertifyService,
    private modalService: BsModalService,
    private messageService: MessageService,
    public presenceService: PresenceService
  ) {}

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.user = data['user'];
      this.getFollowees();
      this.getFollowers();
      this.getFollow(this.user.id);
    });
  }

  followUser(id: number) {
    this.userService
      .followUser(this.authService.decodedToken.nameid, id)
      .subscribe(
        (data) => {
          this.alertify.success(
            'Zaobserwowałeś użytkownika: ' + this.user.username
          );
          this.getFollow(id);
          this.getFollowers();
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  getFollow(id: number) {
    this.userService
      .getFollow(this.authService.decodedToken.nameid, id)
      .pipe()
      .subscribe(
        (res: Follow) => {
          this.follow = res;
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  unfollow(id: number) {
    this.alertify.confirm(
      'czy na pewno chcesz przestać obserwować użytkownika ' +
        this.user.username,
      () => {
        this.userService
          .unfollow(this.authService.decodedToken.nameid, id)
          .subscribe(
            (data) => {
              this.getFollow(id);
              this.getFollowers();
            },
            (error) => {
              this.alertify.error(error);
            }
          );
      }
    );
  }

  openFoloweesModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template);
  }

  getFollowees() {
    this.userService
      .getFollowees(this.user.id)
      .pipe()
      .subscribe(
        (res: User[]) => {
          this.followeeItems = res;
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  getFollowers() {
    this.userService
      .getFollowers(this.user.id)
      .pipe()
      .subscribe(
        (res: User[]) => {
          this.followerItems = res;
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  isProfileMine(): boolean {
    if (this.user.id === this.authService.currentUser.id) {
      return true;
    } else {
      return false;
    }
  }

  countFollowees(): number {
    if (
      typeof this.followeeItems !== 'undefined' &&
      this.followeeItems.length > 0
    ) {
      return this.followeeItems.length;
    } else {
      return 0;
    }
  }

  countFollowers(): number {
    if (
      typeof this.followerItems !== 'undefined' &&
      this.followerItems.length > 0
    ) {
      return this.followerItems.length;
    } else {
      return 0;
    }
  }

  countPhotos(): number {
    if (
      typeof this.user.photos !== 'undefined' &&
      this.user.photos.length > 0
    ) {
      return this.user.photos.length;
    } else {
      return 0;
    }
  }

  openModalPhotoPreview(template: TemplateRef<any>, photo: Photo) {
    this.photo = {
      id: photo.id,
      dateAdded: photo.dateAdded,
      user: this.user,
      isMain: photo.isMain,
      url: photo.url,
      description: photo.description,
    };
    this.modalRef = this.modalService.show(template);
  }

  closeDialog() {
    this.modalRef.hide();
  }

  refreshPage() {
    this.userService.getUser(this.user.id).subscribe(
      (res: User) => {
        this.user = res;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
    this.modalRef.hide();
  }

  onKey(event) {
    this.message = event.target.value;
  }

  sendMessage() {
    this.newMessage.recipientId = this.user.id;
    this.newMessage.content = this.message;
    if (this.newMessage.content !== '' && this.newMessage.content) {
      this.messageService
        .sendMessage(this.authService.currentUser.id, this.newMessage)
        .then(() => {
          this.newMessage.content = '';
          this.modalRef.hide();
          this.alertify.success('Wysłano.');
        });
    } else {
      this.alertify.error('Nie wysyłaj pustej wiadomości.');
    }
  }

  openCreateMessageModal(template: TemplateRef<any>) {
    this.messageService.createHubConnection(
      this.authService.currentUser,
      this.user.id.toString()
    );
    this.modalRef = this.modalService.show(template);
    this.modalService.onHide.pipe(take(1)).subscribe(() => {
      this.messageService.stopConnection();
    });
  }

  ngOnDestroy() {
    this.messageService.stopConnection();
  }
}
