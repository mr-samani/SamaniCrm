import { NgModule, ModuleWithProviders, SkipSelf, Optional } from '@angular/core';
import { Configuration } from './configuration';
import { HttpClient } from '@angular/common/http';

import { AccountServiceProxy } from './api/account.service';
import { CaptchaServiceProxy } from './api/captcha.service';
import { CommonServiceProxy } from './api/common.service';
import { LanguageServiceProxy } from './api/language.service';
import { MaintenanceServiceProxy } from './api/maintenance.service';
import { MenuServiceProxy } from './api/menu.service';
import { PagesServiceProxy } from './api/pages.service';
import { ProductServiceProxy } from './api/product.service';
import { RoleServiceProxy } from './api/role.service';
import { SecuritySettingsServiceProxy } from './api/security-settings.service';
import { UserServiceProxy } from './api/user.service';
import { WeatherForecastServiceProxy } from './api/weather-forecast.service';

@NgModule({
  imports:      [],
  declarations: [],
  exports:      [],
  providers: [
    AccountServiceProxy,
    CaptchaServiceProxy,
    CommonServiceProxy,
    LanguageServiceProxy,
    MaintenanceServiceProxy,
    MenuServiceProxy,
    PagesServiceProxy,
    ProductServiceProxy,
    RoleServiceProxy,
    SecuritySettingsServiceProxy,
    UserServiceProxy,
    WeatherForecastServiceProxy ]
})
export class SamaniApiModule {
    public static forRoot(configurationFactory: () => Configuration): ModuleWithProviders<SamaniApiModule> {
        return {
            ngModule: SamaniApiModule,
            providers: [ { provide: Configuration, useFactory: configurationFactory } ]
        };
    }

    constructor( @Optional() @SkipSelf() parentModule: SamaniApiModule,
                 @Optional() http: HttpClient) {
        if (parentModule) {
            throw new Error('SamaniApiModule is already loaded. Import in your base AppModule only.');
        }
        if (!http) {
            throw new Error('You need to import the HttpClientModule in your AppModule! \n' +
            'See also https://github.com/angular/angular/issues/20575');
        }
    }
}
