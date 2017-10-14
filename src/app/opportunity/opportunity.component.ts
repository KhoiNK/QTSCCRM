import { Component, OnInit } from '@angular/core';
import {OpportunityService} from './opportunity.service';

@Component({
  selector: 'app-opportunity',
  templateUrl: './opportunity.component.html',
  providers: [OpportunityService],
  styleUrls: ['./opportunity.component.css']
})
export class OpportunityComponent implements OnInit {
  public OpportunityList: any[];

  constructor(private _markSrv: OpportunityService) { }

  ngOnInit() {
    this.loadData();
  }

  loadData(){
    this._markSrv.GetList().subscribe(res =>{
      this.OpportunityList = res;
    }, err =>{
      console.log(err);
    });
  }
}
