import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  TemplateRef,
} from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { PhotoComment } from '../_models/Comment';
import { Photo } from '../_models/photo';
import { PhotoLike } from '../_models/photoLike';
import { User } from '../_models/user';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { PhotoService } from '../_services/photo.service';
import { UserService } from '../_services/user.service';

@Component({
  selector: 'app-post-card',
  templateUrl: './post-card.component.html',
  styleUrls: ['./post-card.component.css'],
})
export class PostCardComponent implements OnInit {
  @Input() photo: any;
  @Output() getLikerClicked = new EventEmitter<string>();
  @Output() getPhotoDeleted = new EventEmitter<string>();
  photoLikes: PhotoLike[];
  currentUser: User;
  isLiked: any;
  modalRef: BsModalRef;
  comments: PhotoComment[] = [];
  isCollapsed = false;
  commentContent = '';

  constructor(
    private authService: AuthService,
    private photoService: PhotoService,
    private userService: UserService,
    private alertify: AlertifyService,
    private modalService: BsModalService
  ) {}

  ngOnInit(): void {
    // this.userService.getUser(this.photo.userid).subscribe((data) => {
    //   this.photo.user = data;
    // });
    this.currentUser = JSON.parse(localStorage.getItem('user'));
    this.getLikes();
    this.doILike();
    this.getComments();
  }

  getComments() {
    this.photoService.getComments(this.photo.id).subscribe((data) => {
      this.comments = data;
      debugger;
    },
    error => {
      console.log(error);
    })
  }

  addComment() {
    let comment = { 
      commenterId: this.currentUser.id,
      photoId: this.photo.id,
      content: this.commentContent 
    };
    
    this.photoService.commentPhoto(comment).subscribe((next) => {
      this.getComments();
      this.commentContent = '';
    },
    error => {
      this.alertify.error(error);
    })
  }

  deleteComment(commentId: number) {
    this.alertify.confirm("Czy na pewno chcesz usunąć ten komentarz?", () => {
      this.photoService.deleteComment(commentId).subscribe((next) => {
        this.getComments();
      },
      error => {
        this.alertify.error(error);
      })
    })
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
    this.photoService.getLike(this.photo.id).subscribe((res: any) => {
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
    if (this.photo.userId === this.authService.currentUser.id) {
      return true;
    } else {
      return false;
    }
  }

  deletePhoto() {
    this.alertify.confirm('Czy na pewno chcesz usunąć to zdjęcie? ', () => {
      this.photoService.deletePhoto(this.photo.id).subscribe(
        (data) => {
          this.getPhotoDeleted.emit();
          this.alertify.message('Pomyślnie usunięto');
        },
        (error) => {
          this.alertify.error(error);
        }
      );
    });
  }

  setMain() {
    debugger;
    this.alertify.confirm('Czy chcesz ustawić to zdjęcie jako główne?', () => {
      this.photoService.setMainPhoto(this.photo.id).subscribe(
        (next) => {
          this.getPhotoDeleted.emit();
          this.authService.changeMemberPhoto(this.photo.url);
        },
        (error) => {
          this.alertify.error(error);
        }
      );
    });
  }
}
