import {
  BrowserModule,
  HammerGestureConfig,
  HAMMER_GESTURE_CONFIG,
} from '@angular/platform-browser';
import { Injectable, NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { CollapseModule } from 'ngx-bootstrap/collapse';

import { AppComponent } from './app.component';
import { LoginPageComponent } from './login-page/login-page.component';
import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { RegisterPageComponent } from './register-page/register-page.component';
import { RouterModule } from '@angular/router';
import { appRoutes } from './routes';
import { NavComponent } from './nav/nav.component';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';
import { HomeComponent } from './home/home.component';
import { NotAuthGuard } from './_guards/not-auth.guard';
import { PostListResolver } from './_resolvers/post-list.resolver';
import { JwtModule } from '@auth0/angular-jwt';
import { PostCardComponent } from './post-card/post-card.component';
import { UserDetailComponent } from './user-detail/user-detail.component';
import { UserDetailResolver } from './_resolvers/user-detail.resolver';
import { ModalModule } from 'ngx-bootstrap/modal';
import { FileUploadModule } from 'ng2-file-upload';
import { UserEditComponent } from './user-edit/user-edit.component';
import { UserEditResolver } from './_resolvers/user-edit.resolver';
import { ThreadListComponent } from './thread-list/thread-list.component';
import { ConversationListResolver } from './_resolvers/conversation-list.resolver';
import { ToastrModule } from 'ngx-toastr';
import { StripeModule } from 'stripe-angular';
import { DonateComponent } from './donate/donate.component';
import { DonationSuccessComponent } from './donation-success/donation-success.component';

export function tokenGetter() {
  return localStorage.getItem('token');
}

@Injectable()
export class CustomHammerConfig extends HammerGestureConfig {
  overrides = {
    pinch: { enable: false },
    rotate: { enable: false },
  };
}

@NgModule({
  declarations: [				
    AppComponent,
    LoginPageComponent,
    RegisterPageComponent,
    NavComponent,
    HomeComponent,
    PostCardComponent,
    UserDetailComponent,
    UserEditComponent,
    ThreadListComponent,
      DonateComponent,
      DonationSuccessComponent
   ],
  imports: [
    BrowserModule,
    BsDatepickerModule.forRoot(),
    ToastrModule.forRoot(),
    CollapseModule.forRoot(),
    ReactiveFormsModule,
    StripeModule.forRoot('pk_test_51I5YvcJvKXNzTQcriYlZbz2AY6gfE97xSpJ7mUaL2UWOAABOjPBN9mlS3421iRf2nL25anPw6bgSSP5mxg7yI1Z600SJy8Gsm7'),
    ModalModule.forRoot(),
    BsDropdownModule.forRoot(),
    RouterModule.forRoot(appRoutes),
    BrowserAnimationsModule,
    FileUploadModule,
    HttpClientModule,
    FormsModule,
    JwtModule.forRoot({
      config: {
        tokenGetter,
        allowedDomains: ['localhost:5000'],
        disallowedRoutes: ['localhost:5000/api/auth'],
      },
    }),
  ],
  providers: [
    ErrorInterceptorProvider,
    PreventUnsavedChanges,
    NotAuthGuard,
    PostListResolver,
    UserDetailResolver,
    UserEditResolver,
    ConversationListResolver,
    { provide: HAMMER_GESTURE_CONFIG, useClass: CustomHammerConfig },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
