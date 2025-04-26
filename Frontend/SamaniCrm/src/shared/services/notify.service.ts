import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig, MatSnackBarRef, TextOnlySnackBar } from '@angular/material/snack-bar';
@Injectable({
  providedIn: 'root',
})
export class NotifyService {
  private defaultConfiguration: MatSnackBarConfig<any> = {
    duration: 4000,
    horizontalPosition: 'center',
    verticalPosition: 'bottom',
  };
  constructor(private snackBar: MatSnackBar) {}

  info(text: string): MatSnackBarRef<TextOnlySnackBar> {
    return this.snackBar.open(text, '×', {
      ...this.defaultConfiguration,
      panelClass: 'notify-info',
    });
  }
  warning(text: string): MatSnackBarRef<TextOnlySnackBar> {
    return this.snackBar.open(text, '×', {
      ...this.defaultConfiguration,
      panelClass: 'notify-warning',
    });
  }

  error(text: string): MatSnackBarRef<TextOnlySnackBar> {
    return this.snackBar.open(text, '×', {
      ...this.defaultConfiguration,
      panelClass: 'notify-error',
    });
  }

  success(text: string): MatSnackBarRef<TextOnlySnackBar> {
    return this.snackBar.open(text, '×', {
      ...this.defaultConfiguration,
      panelClass: 'notify-success',
    });
  }
}
