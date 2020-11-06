import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { RegisterPageComponent } from '../register-page/register-page.component';

@Injectable()
export class PreventUnsavedChanges implements CanDeactivate<RegisterPageComponent> {

    canDeactivate(component: RegisterPageComponent) {
        if (component.editForm.dirty) {
            return confirm('Are you sure you want to continue? Any unsaved changes will be lost.');
        }
        return true;
    }
}