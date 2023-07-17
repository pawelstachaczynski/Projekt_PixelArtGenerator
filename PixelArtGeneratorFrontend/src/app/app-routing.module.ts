import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserPanelComponent } from './user-panel/user-panel.component';


//const routes: Routes = [];
const routes: Routes = [
  {
    path: '', title: 'PixelArtGenerator - Witaj na stronie', component: UserPanelComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
