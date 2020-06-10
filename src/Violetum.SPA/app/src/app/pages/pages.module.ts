import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {RouterModule} from '@angular/router';

import {BsDropdownModule} from 'ngx-bootstrap/dropdown';
import {ProgressbarModule} from 'ngx-bootstrap/progressbar';
import {TooltipModule} from 'ngx-bootstrap/tooltip';
import {CollapseModule} from 'ngx-bootstrap/collapse';
import {TabsModule} from 'ngx-bootstrap/tabs';
import {PaginationModule} from 'ngx-bootstrap/pagination';
import {AlertModule} from 'ngx-bootstrap/alert';
import {BsDatepickerModule} from 'ngx-bootstrap/datepicker';
import {CarouselModule} from 'ngx-bootstrap/carousel';
import {ModalModule} from 'ngx-bootstrap/modal';
import {JwBootstrapSwitchNg2Module} from 'jw-bootstrap-switch-ng2';
import {PopoverModule} from 'ngx-bootstrap/popover';

import {IndexComponent} from './index/index.component';
import {UserComponent} from './user/user.component';
import {PostsComponent} from './posts/posts.component';
import {SharedModule} from '../shared/shared.module';
import {CategoriesComponent} from './categories/categories.component';
import {PostComponent} from './posts/post/post.component';
import {CategoryComponent} from './categories/category/category.component';
import {UnauthorizedComponent} from './unauthorized/unauthorized.component';
import {CommentsComponent} from './comments/comments.component';
import {CommentComponent} from './comments/comment/comment.component';

@NgModule({
  imports: [
    CommonModule,
    BrowserModule,
    FormsModule,
    RouterModule,
    BsDropdownModule.forRoot(),
    ProgressbarModule.forRoot(),
    TooltipModule.forRoot(),
    PopoverModule.forRoot(),
    CollapseModule.forRoot(),
    JwBootstrapSwitchNg2Module,
    TabsModule.forRoot(),
    PaginationModule.forRoot(),
    AlertModule.forRoot(),
    BsDatepickerModule.forRoot(),
    CarouselModule.forRoot(),
    ModalModule.forRoot(),
    SharedModule,
  ],
  declarations: [
    IndexComponent,
    UserComponent,
    PostsComponent,
    PostComponent,
    CategoriesComponent,
    CategoryComponent,
    UnauthorizedComponent,
    CommentsComponent,
    CommentComponent,
  ],
  exports: [
    IndexComponent,
    UserComponent,
    PostsComponent,
    PostComponent,
    CategoriesComponent,
    CategoryComponent,
    UnauthorizedComponent,
    CommentsComponent,
    CommentComponent,
  ],
  providers: []
})
export class PagesModule {
}
