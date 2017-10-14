import { Component, OnInit } from '@angular/core';
import {ExportService} from './export.service';

@Component({
  selector: 'app-export',
  templateUrl: './export.component.html',
  providers: [ExportService],
  styleUrls: ['./export.component.css']
})
export class ExportComponent implements OnInit {
  public exportList: any[];

  constructor(private _markSrv: ExportService) { }

  ngOnInit() {
    this.loadData();
  }

  loadData(){
    this._markSrv.GetList().subscribe(res =>{
      this.exportList = res;
    }, err =>{
      console.log(err);
    });
  }
}
