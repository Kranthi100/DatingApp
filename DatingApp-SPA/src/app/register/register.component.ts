import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { HomeComponent } from '../home/home.component';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();
  model: any = {};

  constructor(private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  register() {
   // console.log(this.model);
   this.authService.register(this.model).subscribe(() => {
   this.alertify.success('registration Successful');
   }, error => { console.error(); }
   );
  }
  cancel() {
    // this.comp.registerToggle();
    console.log('cancelled');
    this.cancelRegister.emit(false);
  }

}
