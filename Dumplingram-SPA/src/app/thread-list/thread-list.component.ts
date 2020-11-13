import { Component, OnInit, TemplateRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Message } from '../_models/Message';
import { User } from '../_models/User';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { MessageService } from '../_services/message.service';
import { UserService } from '../_services/user.service';

@Component({
  selector: 'app-thread-list',
  templateUrl: './thread-list.component.html',
  styleUrls: ['./thread-list.component.css'],
})
export class ThreadListComponent implements OnInit {
  users: User[];
  messages: Message[];
  selectedUserId: number;
  modalRef: BsModalRef;

  chatMessages: Message[];
  message: string;
  newMessage: any = {};
  user: any;

  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    public authService: AuthService,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private modalService: BsModalService
  ) {}

  ngOnInit(): void {
    this.route.data.subscribe((data) => {
      this.messages = data['messages'];
    });
  }

  isLastMessageFromMe(message: Message): boolean {
    if (message.senderId === this.authService.currentUser.id) {
      return true;
    } else {
      return false;
    }
  }

  openConversation(id: number, template: TemplateRef<any>) {
    this.selectedUserId = id;
    this.userService.getUser(this.selectedUserId).subscribe(
      (user: User) => {
        this.user = user;
        this.messageService
          .getMessageThread(
            this.authService.currentUser.id,
            this.selectedUserId
          )
          .subscribe(
            (res: Message[]) => {
              this.chatMessages = res;
              this.modalRef = this.modalService.show(template);
            },
            (error) => {
              this.alertify.error(error);
            }
          );
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  close() {
    this.modalRef.hide();
    this.selectedUserId = null;
  }

  isFromMe(message: Message): boolean {
    if (message.senderId === this.authService.currentUser.id) {
      return true;
    } else {
      return false;
    }
  }

  onKey(event) {
    this.message = event.target.value;
  }

  sendMessage() {
    this.newMessage.recipientId = this.selectedUserId;
    this.newMessage.content = this.message;
    if (this.message !== '') {
      this.messageService
        .sendMessage(this.authService.currentUser.id, this.newMessage)
        .subscribe(
          (message: Message) => {
            this.chatMessages.push(message);
            this.newMessage.content = '';
          },
          (error) => {
            this.alertify.error(error);
          }
        );
    } else {
      this.alertify.error('Nie wysyłaj pustej wiadomości.');
    }
  }

}
