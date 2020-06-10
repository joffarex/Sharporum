import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {BrowserModule} from '@angular/platform-browser';
import {RouterModule, Routes} from '@angular/router';

import {IndexComponent} from './pages/index/index.component';
import {UserComponent} from './pages/user/user.component';
import {PostsComponent} from './pages/posts/posts.component';
import {CategoriesComponent} from './pages/categories/categories.component';
import {UnauthorizedComponent} from './pages/unauthorized/unauthorized.component';

const routes: Routes = [
  {path: '', redirectTo: 'home', pathMatch: 'full'},
  {path: 'home', component: IndexComponent},
  {path: 'user', component: UserComponent},
  {path: 'posts', component: PostsComponent},
  {path: 'categories', component: CategoriesComponent},
  {path: 'forbidden', component: UnauthorizedComponent},
  {path: 'unauthorized', component: UnauthorizedComponent},
];

@NgModule({
  imports: [
    CommonModule,
    BrowserModule,
    RouterModule.forRoot(routes, {
      useHash: true
    })
  ],
  exports: []
})
export class AppRoutingModule {
}
