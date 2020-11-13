import { Component, Input, OnInit } from '@angular/core';
import { Message } from '../_models/Message';
import { User } from '../_models/User';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { MessageService } from '../_services/message.service';
import { UserService } from '../_services/user.service';

@Component({
  selector: 'app-conversation',
  templateUrl: './conversation.component.html',
  styleUrls: ['./conversation.component.css'],
})
export class ConversationComponent implements OnInit {
  @Input() userId: number;
  messages: Message[];
  message: string;
  newMessage: any = {};
  user: User;

  constructor(
    private messageService: MessageService,
    private userService: UserService,
    private alertify: AlertifyService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadUser();
    this.loadConversation();
  }

  loadUser() {
    this.userService.getUser(this.userId).subscribe(
      (user: User) => {
        this.user = user;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  loadConversation() {
    this.messageService
      .getMessageThread(this.authService.currentUser.id, this.userId)
      .subscribe(
        (res: Message[]) => {
          this.messages = res;
        },
        (error) => {
          this.alertify.error(error);
        }
      );
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
    this.newMessage.recipientId = this.userId;
    this.newMessage.content = this.message;
    if (this.message !== '') {
      this.messageService
        .sendMessage(this.authService.currentUser.id, this.newMessage)
        .subscribe(
          (message: Message) => {
            this.messages.push(message);
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
