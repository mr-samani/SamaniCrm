import { Component, ElementRef, inject, ViewChild } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { FileManagerService } from '@app/file-manager/file-manager.service';
import { AppConst } from '@shared/app-const';
import { AccountServiceProxy } from '@shared/service-proxies/api/account.service';
import { UserDTO } from '@shared/service-proxies/model/user-dto';
import { ColorSchemaService } from '@shared/services/color-schema.service';
import { finalize } from 'rxjs/operators';
declare var bootstrap: any;

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
  baseUrl = AppConst.apiUrl;
  AppConst = AppConst;

  tenancyName = AppConst.tenancyName;
  @ViewChild('navbarCollapse') navbarCollapse!: ElementRef;

  accountService = inject(AccountServiceProxy);
  constructor(
    public colorSchemaService: ColorSchemaService,
    private fileManager: FileManagerService,
  ) {
    super();
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
    this.authService.logout();
  }

  closeNavbar() {
    const collapse = new bootstrap.Collapse(this.navbarCollapse.nativeElement, {
      toggle: false,
    });
    collapse.hide();
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
}
