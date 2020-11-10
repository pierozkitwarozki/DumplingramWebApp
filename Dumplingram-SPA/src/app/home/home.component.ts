import { HttpResponse } from '@angular/common/http';
import { Component, OnInit, TemplateRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FileUploader, FileUploaderOptions } from 'ng2-file-upload';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { error } from 'protractor';
import { environment } from 'src/environments/environment';
import { runInThisContext } from 'vm';
import { Photo } from '../_models/Photo';
import { User } from '../_models/User';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { PhotoService } from '../_services/photo.service';
import { UserService } from '../_services/user.service';
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  photos: Photo[];
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  currentMain: Photo;
  modalRef: BsModalRef;
  photoDescription = '';

  constructor(
    private modalService: BsModalService,
    private userService: UserService,
    private authService: AuthService,
    private photoService: PhotoService,
    private alertify: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.photos = data['users'];
    });
    this.initializeUploader();
  }

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
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

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'photos/' + this.authService.decodedToken.nameid,
      authToken: 'Bearer ' + localStorage.getItem('token'),
      additionalParameter: { description: this.photoDescription },
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onBeforeUploadItem = (file) => {
      this.uploader.options.additionalParameter = {
        description: this.photoDescription,
      };
    };

    this.uploader.onCompleteAll = () => {
      this.modalRef.hide();
      this.alertify.success("dodano.");
    };
  }

  openAddPhotoModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template);
  }

  onKey(event) {
    this.photoDescription = event.target.value;
  }
}
