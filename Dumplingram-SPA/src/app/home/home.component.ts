import { Component, OnInit } from '@angular/core';
<<<<<<< HEAD
import { User } from '../_models/User';
import { UserService } from '../_services/user.service';
=======
>>>>>>> 6994ca86e4e02563288f30b6de76290deb848696

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
<<<<<<< HEAD
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  users: User[];

  constructor(private userService: UserService) {}

  ngOnInit(): void {}
=======
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

>>>>>>> 6994ca86e4e02563288f30b6de76290deb848696
}
