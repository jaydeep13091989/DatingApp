import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanDeactivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { ConfirmService } from '../_services/confirm.service';
import { map, take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {
  constructor(private confirmService : ConfirmService){}
  retValue : boolean;
  canDeactivate(component: MemberEditComponent):  Observable<boolean> {

    if(component.editForm.dirty)
    {

       return this.confirmService.confirm();
    }
    else
    {
      return new BehaviorSubject<boolean>(true);
    }
  }
  
}
