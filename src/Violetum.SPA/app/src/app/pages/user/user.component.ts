import {Component, OnDestroy, OnInit} from '@angular/core';

@Component({
  selector: 'app-profile',
  templateUrl: 'user.component.html'
})
export class UserComponent implements OnInit, OnDestroy {
  source = 'profile';

  constructor() {
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
