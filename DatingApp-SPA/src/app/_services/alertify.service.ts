import { Injectable } from '@angular/core';
import * as alertfy from 'alertifyjs';

@Injectable({
  providedIn: 'root'
})
export class AlertifyService {

constructor() { }
  confirm(message: string, okcallback: () => any) {
    alertfy.confirm(message, (e: any) => {
      if (e) {
        okcallback();
      } else {}
    });
}
success(message: string) {
  alertfy.success(message);
}

warning(message: string) {
  alertfy.warning(message);
}

error(message: string) {
  alertfy.error(message);
}

message(message: string) {
  alertfy.message(message);
}
}
