import { Component, Injector } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { FileManagerService } from '@app/file-manager/file-manager.service';
import { AppConst } from '@shared/app-const';
import { UserDTO } from '@shared/service-proxies/model/user-dto';
import { ColorSchemaService } from '@shared/services/color-schema.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  standalone: false,
})
export class HeaderComponent extends AppComponentBase {
  user?: UserDTO;
  currentLang = AppConst.currentLanguage;
  languages = AppConst.languageList;
  backEndUrl = AppConst.apiUrl;
  AppConst = AppConst;
  constructor(
    injector: Injector,
    public colorSchemaService: ColorSchemaService,
    private fileManager: FileManagerService,
  ) {
    super(injector);
    this.authService.currentUser.subscribe((u) => {
      this.user = u;
    });
  }

  changeLanguage(lang: string) {
    this.language.changeLanguage(lang);
    this.currentLang = lang;
  }

  openFileManager() {
    this.fileManager.openFileManager();
  }

  logout() {
    this.authService.logOut();
  }
}
