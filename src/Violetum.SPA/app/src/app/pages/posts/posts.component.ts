import {Component, OnInit, OnDestroy} from '@angular/core';

@Component({
  selector: 'app-posts',
  templateUrl: 'posts.component.html'
})
export class PostsComponent implements OnInit, OnDestroy {
  searchType = 'Post Title';
  posts: any[] = [
    {
      title: 'SOME LONG POST TITLE AAAYAYAYA 1',
      content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vehicula ligula magna, non vulputate lacus portanec. Nunc at ultricies nibh. Nulla vitae finibus lectus. Orci varius natoque pe...'
    },
    {
      title: 'SOME LONG POST TITLE AAAYAYAYA 2',
      content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vehicula ligula magna, non vulputate lacus portanec. Nunc at ultricies nibh. Nulla vitae finibus lectus. Orci varius natoque pe...'
    },
    {
      title: 'SOME LONG POST TITLE AAAYAYAYA 3',
      content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vehicula ligula magna, non vulputate lacus portanec. Nunc at ultricies nibh. Nulla vitae finibus lectus. Orci varius natoque pe...'
    },
    {
      title: 'SOME LONG POST TITLE AAAYAYAYA 4',
      content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vehicula ligula magna, non vulputate lacus portanec. Nunc at ultricies nibh. Nulla vitae finibus lectus. Orci varius natoque pe...'
    },
    {
      title: 'SOME LONG POST TITLE AAAYAYAYA 5',
      content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vehicula ligula magna, non vulputate lacus portanec. Nunc at ultricies nibh. Nulla vitae finibus lectus. Orci varius natoque pe...'
    },
  ];

  constructor() {
  }

  ngOnInit() {
    const body = document.getElementsByTagName('body')[0];
    body.classList.add('profile-page');
  }

  ngOnDestroy() {
    const body = document.getElementsByTagName('body')[0];
    body.classList.remove('profile-page');
  }
}
