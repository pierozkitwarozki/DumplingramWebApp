import { FnParam } from '@angular/compiler/src/output/output_ast';
import { Component, OnInit, TemplateRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
  NgxGalleryOptions,
  NgxGalleryImage,
  NgxGalleryAnimation,
  NgxGalleryImageSize,
} from '@kolkov/ngx-gallery';
import { BsModalRef, BsModalService, ModalModule } from 'ngx-bootstrap/modal';
import { error } from 'protractor';
import { Follow } from '../_models/Follow';

import { User } from '../_models/User';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { UserService } from '../_services/user.service';

@Component({
  selector: 'app-user-detail',
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.css'],
})
export class UserDetailComponent implements OnInit {
  user: User;
  follow: Follow;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  modalRef: BsModalRef;
  followeeItems: User[];
  followerItems: User[];

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private authService: AuthService,
    private alertify: AlertifyService,
    private modalService: BsModalService
  ) {}

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.user = data['user'];
      this.getFollowees();
      this.getFollowers();
      this.getFollow(this.user.id);
      this.galleryImages = this.getImages();
    });

    this.galleryOptions = [
      {
        height: '180px',
        thumbnailsPercent: 25,
        thumbnails: true,
        thumbnailsColumns: 3,
        thumbnailSize: NgxGalleryImageSize.Cover,
        preview: true,
        image: false,
      },
    ];
  }

  getImages() {
    const imageUrls = [];
    for (const photo of this.user.photos) {
      imageUrls.push({
        small: photo.url,
        medium: photo.url,
        big: photo.url,
        description: photo.description,
      });
    }

    return imageUrls;
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

  isMyProfile(): boolean {
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
}
