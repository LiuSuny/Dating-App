import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { initialState } from 'ngx-bootstrap/timepicker/reducer/timepicker.reducer';
import { ConfirmDialogComponent } from '../modals/confirm-dialog/confirm-dialog.component';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
//modals confirm notifcation
export class ConfirmService {
bsModalRef:BsModalRef

  constructor(private bsModalService: BsModalService) { }

  confirm(title = 'Comfirmation', 
    message = 'Are you sure you want do this?',
    btnOkText = 'Ok',
    btnCancelText = 'Cancel'
  ): Observable<boolean>
  {
    const config = {
      initialState: {
        title,
        message,
        btnOkText,
        btnCancelText
      }
    }
    this.bsModalRef = this.bsModalService.show(ConfirmDialogComponent, config);
    return new Observable<boolean>(this.getResult());
  }
 
//this method is responsible for passing of observable as boolean
  private getResult()
  {
       return (observer) => {
        const subscription = this.bsModalRef.onHidden.subscribe(() => {
          observer.next(this.bsModalRef.content.result);
          observer.complete();
        });
       return {
        unsubscribe() {
           subscription.unsubscribe();
        }
       }
       }
  }
}

