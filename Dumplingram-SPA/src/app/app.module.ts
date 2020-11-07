import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';

import { AppComponent } from './app.component';
import { LoginPageComponent } from './login-page/login-page.component';
import { AuthService } from './_services/auth.service';
import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { RegisterPageComponent } from './register-page/register-page.component';
import { RouterModule } from '@angular/router';
import { appRoutes } from './routes';
import { NavComponent } from './nav/nav.component';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';
import { HomeComponent } from './home/home.component';
import { NotAuthGuard } from './_guards/not-auth.guard';

@NgModule({
  declarations: [AppComponent, LoginPageComponent, RegisterPageComponent, NavComponent, HomeComponent],
  imports: [
    BrowserModule,
    BsDatepickerModule.forRoot(),
    ReactiveFormsModule,
    BsDropdownModule.forRoot(),
    RouterModule.forRoot(appRoutes),
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
  ],
  providers: [ErrorInterceptorProvider, AuthService, PreventUnsavedChanges, NotAuthGuard],
  bootstrap: [AppComponent],
})
export class AppModule {}
