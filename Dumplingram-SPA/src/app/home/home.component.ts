import { HttpResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { error } from 'protractor';
import { runInThisContext } from 'vm';
import { Photo } from '../_models/Photo';
import { User } from '../_models/User';
import { AlertifyService } from '../_services/alertify.service';
import { PhotoService } from '../_services/photo.service';
import { UserService } from '../_services/user.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  photos: Photo[];

  constructor(
    private userService: UserService,
    private photoService: PhotoService,
    private alertify: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.photos = data['users'];
    });
  }

  loadUsers() {
    this.photoService
      .getPhotos()
      .pipe()
      .subscribe(
        (res: Photo[]) => {
          this.photos = res;
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }
}
