import {Injectable} from '@angular/core';
import {Observable} from 'rxjs/Observable';
import { RequestOptions, Http, URLSearchParams, Headers } from '@angular/http';
import * as GloBalVariable from '../globalvar.component';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

@Injectable()
export class ExportService{
    private apiUrl = GloBalVariable.apiUrl + "export";
    constructor(private _http: Http){

    }

    GetList(): Observable<any>{
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return this._http.get(this.apiUrl, options).map(res => res.json());
    }
}
