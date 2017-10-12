import { Component, OnInit } from '@angular/core';
import {MarketingService} from './marketing.service';

@Component({
  selector: 'app-marketing',
  templateUrl: './marketing.component.html',
  providers: [MarketingService],
  styleUrls: ['./marketing.component.css']
})
export class MarketingComponent implements OnInit {
  public marketingList: any[];

  constructor(private _markSrv: MarketingService) { }

  ngOnInit() {
    this.loadData();
  }

  loadData(){
    this._markSrv.GetList().subscribe(res =>{
      this.marketingList = res;
    }, err =>{
      console.log(err);
    });
  }
}
