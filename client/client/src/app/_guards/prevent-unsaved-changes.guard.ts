import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { ConfirmService } from '../_services/confirm.service';
import { inject } from '@angular/core';
import { Observable } from 'rxjs';

export const preventUnsavedChangesGuard: CanDeactivateFn<unknown> = 
(component: MemberEditComponent): Observable<boolean> | boolean => {

  const confirmService = inject(ConfirmService);

  if(component.editForm.dirty){
    return confirmService.confirm()
    //return confirm('Are you sure you want to continue? Any unsaved changes will be lose');
  }
  return true;
};
 