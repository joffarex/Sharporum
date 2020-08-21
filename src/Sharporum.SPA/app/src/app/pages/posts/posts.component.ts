import {Component, Input, OnDestroy, OnInit} from '@angular/core';

@Component({
  selector: 'app-posts',
  templateUrl: 'posts.component.html'
})
export class PostsComponent implements OnInit, OnDestroy {
  @Input() source: string;
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

  getContentContainerClass(): string {
    if (this.source !== 'profile') {
      return 'content-container container';
    }
  }

  ngOnInit() {
    const body = document.getElementsByTagName('body')[0];
    body.classList.add('user-page');
  }

  ngOnDestroy() {
    const body = document.getElementsByTagName('body')[0];
    body.classList.remove('user-page');
  }
}
