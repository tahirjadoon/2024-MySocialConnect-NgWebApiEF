import { Component } from '@angular/core';
import { ZRoles } from 'src/app/core/enums/z-roles';

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css']
})
export class AdminPanelComponent {

  zRole = ZRoles;

}
