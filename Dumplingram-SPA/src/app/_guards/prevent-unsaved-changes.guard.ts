import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { RegisterPageComponent } from '../register-page/register-page.component';

@Injectable()
export class PreventUnsavedChanges implements CanDeactivate<RegisterPageComponent> {

    canDeactivate(component: RegisterPageComponent) {
        if (component.editForm.dirty && !component.editForm.valid) {
            return confirm('Jesteś pewien? Niezapisane zmiany zostaną utracone.');
        }
        return true;
    }
}