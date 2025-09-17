import { Pipe, PipeTransform } from '@angular/core';
import { AuthService } from '@shared/services/auth.service';

@Pipe({ standalone: false, name: 'permissionAny' })
export class PermissionAnyPipe implements PipeTransform {
  constructor(private authService: AuthService) {}

  transform(arrPermissions: string[]): boolean {
    if (!arrPermissions) {
      return false;
    }

    for (const permission of arrPermissions) {
      if (this.authService.isGranted(permission)) {
        return true;
      }
    }

    return false;
  }
}
