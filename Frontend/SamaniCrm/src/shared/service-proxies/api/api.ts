export * from './account.service';
import { AccountServiceProxy } from './account.service';
export * from './role.service';
import { RoleServiceProxy } from './role.service';
export * from './user.service';
import { UserServiceProxy } from './user.service';
export * from './weather-forecast.service';
import { WeatherForecastServiceProxy } from './weather-forecast.service';
export const APIS = [AccountServiceProxy, RoleServiceProxy, UserServiceProxy, WeatherForecastServiceProxy];
