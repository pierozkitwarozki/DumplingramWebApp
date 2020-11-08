import { HttpResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { error } from 'protractor';
import { runInThisContext } from 'vm';
import { User } from '../_models/User';
import { AlertifyService } from '../_services/alertify.service';
import { UserService } from '../_services/user.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  users: User[];

  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.data.subscribe((data) => {
      debugger;
      this.users = data['users'].body;
    });
  }

  loadUsers() {
    this.userService
      .getUsers()
      .pipe()
      .subscribe(
        (res: User[]) => {
          this.users = res;
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }
}
