import { Component, Input, OnInit } from '@angular/core';
import { Photo } from '../_models/Photo';
import { User } from '../_models/User';

@Component({
  selector: 'app-post-card',
  templateUrl: './post-card.component.html',
  styleUrls: ['./post-card.component.css']
})
export class PostCardComponent implements OnInit {
  @Input() photo: Photo;
  constructor() { }

  ngOnInit(): void {
  }

}
