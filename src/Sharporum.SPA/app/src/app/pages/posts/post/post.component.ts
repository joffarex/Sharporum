import {Component, OnInit, OnDestroy, Input} from '@angular/core';

@Component({
  selector: 'app-post',
  templateUrl: 'post.component.html'
})
export class PostComponent implements OnInit, OnDestroy {
  @Input() post: any;

  constructor() {
  }

  ngOnInit() {

  }

  ngOnDestroy() {

  }
}
