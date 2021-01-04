import { Injectable } from '@angular/core';
import * as alertify from 'alertifyjs';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root',
})
export class AlertifyService {
  constructor(private toastr: ToastrService) {}

  confirm(message: string, okCallback: () => any) {
    alertify.confirm(message, (e: any) => {
      if (e) {
        okCallback();
      } else {
      }
    }).setHeader('🥟 Wiadomość od Dumplingram 🥟');;
  }

  success(message: string) {
    this.toastr.success(message);
  }

  error(message: string) {
    this.toastr.error(message);
  }

  warning(message: string) {
    this.toastr.warning(message);
  }

  message(message: string) {
    this.toastr.info(message);
  }
}

