import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  TemplateRef,
} from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Photo } from '../_models/Photo';
import { PhotoLike } from '../_models/PhotoLike';
import { User } from '../_models/User';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { PhotoService } from '../_services/photo.service';

@Component({
  selector: 'app-post-card',
  templateUrl: './post-card.component.html',
  styleUrls: ['./post-card.component.css'],
})
export class PostCardComponent implements OnInit {
  @Input() photo: Photo;
  @Output() getLikerClicked = new EventEmitter<string>();
  photoLikes: PhotoLike[];
  isLiked: any;
  modalRef: BsModalRef;

  constructor(
    private authService: AuthService,
    private photoService: PhotoService,
    private alertify: AlertifyService,
    private modalService: BsModalService
  ) {}

  ngOnInit(): void {
    this.getLikes();
    this.doILike();
  }

  likePhoto(photoId: number) {
    this.photoService.likePhoto(photoId).subscribe(
      (data) => {
        this.getLikes();
        this.doILike();
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  getLikes() {
    this.photoService
      .getLikes(this.photo.id)
      .pipe()
      .subscribe(
        (res: PhotoLike[]) => {
          this.photoLikes = res;
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  countLikes(): number {
    if (typeof this.photoLikes !== 'undefined' && this.photoLikes.length > 0) {
      return this.photoLikes.length;
    } else {
      return 0;
    }
  }

  doILike() {
    this.photoService
      .getLike(this.photo.id, this.authService.currentUser.id)
      .subscribe((res: any) => {
        this.isLiked = res;
      });
  }

  dislikePhoto(photoId: number) {
    this.photoService
      .unlikePhoto(photoId, this.authService.decodedToken.nameid)
      .subscribe(
        (data) => {
          this.getLikes();
          this.doILike();
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  likeOrDislike(photoId: number) {
    if (this.isLiked !== null) {
      this.dislikePhoto(photoId);
    } else {
      this.likePhoto(photoId);
    }
  }

  openFoloweesModal(template: TemplateRef<any>) {
    this.getLikerClicked.emit();
    this.modalRef = this.modalService.show(template);
  }

  isPhotoMine(): boolean {
    if (this.photo.user.id === this.authService.currentUser.id) {
      return true;
    } else {
      return false;
    }
  }

  deletePhoto() {
    this.alertify.confirm('czy na pewno chcesz usunąć to zdjęcie? ', () => {
      this.photoService
        .deletePhoto(this.authService.currentUser.id, this.photo.id)
        .subscribe(
          (data) => {
            this.getLikerClicked.emit();
          },
          (error) => {
            this.alertify.error(error);
          }
        );
    });
  }
}
