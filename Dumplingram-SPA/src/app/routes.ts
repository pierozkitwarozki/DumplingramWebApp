import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginPageComponent } from './login-page/login-page.component';
import { RegisterPageComponent } from './register-page/register-page.component';
import { UserDetailComponent } from './user-detail/user-detail.component';
import { AuthGuard } from './_guards/auth.guard';
import { NotAuthGuard } from './_guards/not-auth.guard';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';
import { PostListResolver } from './_resolvers/post-list.resolver';
import { UserDetailResolver } from './_resolvers/user-detail.resolver';

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
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      {
        path: 'home',
        resolve: { users: PostListResolver },
        component: HomeComponent,
      },
      {
        path: 'users/:id',
        resolve: { user: UserDetailResolver },
        component: UserDetailComponent,
      },
    ],
  },
  { path: '**', redirectTo: 'login', pathMatch: 'full' },
];
