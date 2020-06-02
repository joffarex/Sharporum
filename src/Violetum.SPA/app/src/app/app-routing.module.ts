import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {BrowserModule} from '@angular/platform-browser';
import {Routes, RouterModule} from '@angular/router';

import {IndexComponent} from './pages/index/index.component';
import {ProfileComponent} from './pages/profile/profile.component';
import {PostsComponent} from './pages/posts/posts.component';
import {CategoriesComponent} from './pages/categories/categories.component';

const routes: Routes = [
  {path: '', redirectTo: 'home', pathMatch: 'full'},
  {path: 'home', component: IndexComponent},
  {path: 'profile', component: ProfileComponent},
  {path: 'posts', component: PostsComponent},
  {path: 'categories', component: CategoriesComponent},
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
