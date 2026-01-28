import {  Pipe, PipeTransform } from '@angular/core';
import { AuthService } from '@shared/services/auth.service';

@Pipe({ standalone: false, name: 'permission' })
export class PermissionPipe implements PipeTransform {
  constructor(
    private authService: AuthService,
  ) {}

  transform(permission?: string): boolean {
    if (!permission) return true;
    return this.authService.isGranted(permission);
  }
}
