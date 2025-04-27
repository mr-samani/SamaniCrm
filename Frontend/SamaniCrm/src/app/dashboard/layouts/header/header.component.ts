import { Component, Injector } from '@angular/core';
import { UserResponseDTO } from '@app/account/models/login-dto';
import { FileManagerService } from '@app/file-manager/file-manager.service';
import { ColorSchemaService } from '@shared/services/color-schema.service';
import { AppComponentBase } from 'src/app/app-component-base';
import { AppConst } from 'src/shared/app-const';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent extends AppComponentBase {
  user?: UserResponseDTO;
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
}
