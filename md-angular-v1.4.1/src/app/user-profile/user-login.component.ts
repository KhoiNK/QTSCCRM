import {Component} from '@angular/core';

@Component({
    selector: 'user-login',
    templateUrl: 'user-login.component.html'
})

export class UserLogin{
    public user: any;

    constructor(){
        this.user = {};
    }
}