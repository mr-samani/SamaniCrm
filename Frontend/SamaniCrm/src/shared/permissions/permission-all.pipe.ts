import {  Pipe, PipeTransform } from '@angular/core';
import { AuthService } from '@shared/services/auth.service';

@Pipe({ standalone: false, name: 'permissionAll' })
export class PermissionAllPipe implements PipeTransform {
  constructor(private authService: AuthService) {}

  transform(arrPermissions: string[]): boolean {
    if (!arrPermissions) {
      return false;
    }

    for (const permission of arrPermissions) {
      if (!this.authService.isGranted(permission)) {
        return false;
      }
    }

    return true; //all permissions are granted
  }
}
