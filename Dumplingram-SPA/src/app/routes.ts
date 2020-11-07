import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginPageComponent } from './login-page/login-page.component';
import { RegisterPageComponent } from './register-page/register-page.component';
import { AuthGuard } from './_guards/auth.guard';
import { NotAuthGuard } from './_guards/not-auth.guard';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';

export const appRoutes: Routes = [
  {
    path: 'login',
    component: LoginPageComponent,
    runGuardsAndResolvers: 'always',
    canActivate: [NotAuthGuard],
  },
  {
    path: 'register',
    component: RegisterPageComponent,
    canDeactivate: [PreventUnsavedChanges],
    runGuardsAndResolvers: 'always',
    canActivate: [NotAuthGuard],
  },
  {
    path: 'home',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    component: HomeComponent,
  },

  { path: '**', redirectTo: 'login', pathMatch: 'full' },
];
