import {Component, Input, OnDestroy, OnInit} from '@angular/core';

@Component({
  selector: 'app-comments',
  templateUrl: './comments.component.html',
})
export class CommentsComponent implements OnInit, OnDestroy {
  @Input() source: string;
  comments: any[] = [
    {
      content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vehicula ligula magna, non vulputate lacus portanec. Nunc at ultricies nibh. Nulla vitae finibus lectus. Orci varius natoque pe...'
    },
    {
      content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vehicula ligula magna, non vulputate lacus portanec. Nunc at ultricies nibh. Nulla vitae finibus lectus. Orci varius natoque pe...'
    },
    {
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
    body.classList.add('profile-page');
  }

  ngOnDestroy() {
    const body = document.getElementsByTagName('body')[0];
    body.classList.remove('profile-page');
  }

}
