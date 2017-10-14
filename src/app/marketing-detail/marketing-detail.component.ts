import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-marketing-detail',
  templateUrl: './marketing-detail.component.html',
  styleUrls: ['./marketing-detail.component.css'],
 
})
export class MarketingDetailComponent implements OnInit {
  public b64: string ="";
  constructor() { }



  ngOnInit() {
  }

  convertFileToB64(e){
    let file = e.target.files[0];
    let reader = new FileReader();
    reader.readAsDataURL(file);
    let convertFile = new Promise((res, rej)=>{
      reader.onload =  () => {
        res(btoa(reader.result));
      };
    });
    convertFile.then(res => console.log(res));
    reader.onerror = function (error) {
      console.log('Error: ', error);
    };
  }

}
