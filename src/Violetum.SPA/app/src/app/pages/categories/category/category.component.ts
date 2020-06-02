import {Component, OnInit, OnDestroy, Input} from '@angular/core';

@Component({
  selector: 'app-category',
  templateUrl: 'category.component.html'
})
export class CategoryComponent implements OnInit, OnDestroy {
  @Input() category: any;

  constructor() {
  }

  ngOnInit() {

  }

  ngOnDestroy() {

  }
}
