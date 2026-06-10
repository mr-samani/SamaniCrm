import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { FileManagerService } from '@app/file-manager/file-manager.service';
import { AppConst } from '@shared/app-const';
import { AccountServiceProxy } from '@shared/service-proxies/api/account.service';
import { UserDTO } from '@shared/service-proxies/model/user-dto';
import { ColorSchemaService } from '@shared/services/color-schema.service';
import { finalize } from 'rxjs/operators';

@Component({
  standalone: false,
  selector: 'app-side-configs',
  templateUrl: './side-configs.component.html',
  styleUrl: './side-configs.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SideConfigsComponent extends AppComponentBase {
  user?: UserDTO;
  currentLang = AppConst.currentLanguage;
  languages = AppConst.languageList;

  tenancyName = AppConst.tenancyName;
  baseUrl = AppConst.apiUrl;

  private readonly accountService = inject(AccountServiceProxy);
  private readonly fileManager = inject(FileManagerService);

  constructor(public colorSchemaService: ColorSchemaService) {
    super();

    this.authService.currentUser.subscribe((u) => {
      this.user = u;
    });
  }

  changeLanguage(lang: string) {
    this.language.changeLanguage(lang);
    this.currentLang = lang;
  }

  logout() {
    this.authService.logout();
  }

  exitDelegation() {
    this.showMainLoading();
    this.showMainLoading();
    this.accountService
      .exitDelegation()
      .pipe(finalize(() => this.hideMainLoading()))
      .subscribe((result) => {
        this.router.navigate(['/panel']);
        setTimeout(() => {
          location.reload();
        }, 100);
      });
  }

  openFileManager() {
    this.fileManager.openFileManager();
  }
}
