import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { RecentActivitiesComponent } from './components/recent-activities/recent-activities.component';
import { MyDevicesComponent } from './components/my-devices/my-devices.component';
import { MyLinksComponent } from './components/my-links/my-links.component';
import { MyServicesComponent } from './components/my-services/my-services.component';
import { HeaderComponent } from './components/shared/header/header.component';
import { FooterComponent } from './components/shared/footer/footer.component';
import { DeviceDetailComponent } from './components/my-devices/device-detail/device-detail.component';
import { LoadingComponent } from './components/shared/loading/loading.component';

import { DevicesService } from './services/devices.service';

@NgModule({
   declarations: [
      AppComponent,
      RecentActivitiesComponent,
      MyDevicesComponent,
      MyLinksComponent,
      MyServicesComponent,
      HeaderComponent,
      FooterComponent,
      DeviceDetailComponent,
      LoadingComponent
   ],
   imports: [
      CommonModule,
      HttpClientModule,
      FormsModule,
      RouterModule.forRoot([
         { path: '', redirectTo: 'recent-activities', pathMatch: 'full' },
         { path: 'recent-activities', component: RecentActivitiesComponent },
         { path: 'my-devices', component: MyDevicesComponent },
         { path: 'my-links', component: MyLinksComponent },
         { path: 'my-services', component: MyServicesComponent },
         { path: '**', redirectTo: 'recent-activities' }
      ])
   ],
   providers: [
      DevicesService
   ]
})
export class AppModuleShared {
}
