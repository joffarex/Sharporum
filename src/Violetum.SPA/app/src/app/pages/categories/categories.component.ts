import {Component, OnInit, OnDestroy} from '@angular/core';

@Component({
  selector: 'app-categories',
  templateUrl: 'categories.component.html'
})
export class CategoriesComponent implements OnInit, OnDestroy {
  searchType = 'Category Name';
  categories: any[] = [
    {
      name: 'category 1',
    },
    {
      name: 'category 2',
    },
  ];

  constructor() {
  }

  ngOnInit() {

  }

  ngOnDestroy() {

  }
}
