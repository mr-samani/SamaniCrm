import { ChangeDetectorRef, Component, ElementRef, Injector, OnInit, signal, ViewChild } from '@angular/core';
import { BaseComponent } from '@app/base-components';
import { AppConst } from '@shared/app-const';
import { MenuDTO } from '@shared/service-proxies/model/menu-dto';
import { finalize } from 'rxjs';
import { MenuItemsComponent } from '../menu-items/menu-items.component';
import { PublicServiceProxy } from '@shared/service-proxies/api/public.service';
import { LanguageDTO } from '@shared/service-proxies';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
declare var bootstrap: any;
@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  imports: [CommonModule, MenuItemsComponent, FormsModule],

  providers: [PublicServiceProxy],
})
export class HeaderComponent extends BaseComponent implements OnInit {
  menuList = signal<MenuDTO[]>([]);
  languages = AppConst.languageList;
  currentLanguage = AppConst.currentLanguage;
  loading = true;
  apiUrl = AppConst.apiUrl;
  isRtl = AppConst.isRtl;

  @ViewChild('navbarCollapse') navbarCollapse!: ElementRef;
  public get AppConst() {
    return AppConst;
  }

  constructor(
    injector: Injector,
    private publicService: PublicServiceProxy,
    private changeDetectRef: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit() {
    this.getMenuList();
  }

  getMenuList() {
    this.loading = true;
    this.publicService
      .getAllActiveMenus()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((result) => {
        this.menuList.update(() => result.data ?? []);
      });
  }

  changeLanguage(lang: LanguageDTO) {
    this.languageService.changeLanguage(lang.culture);
    this.currentLanguage = lang.culture;
  }

  closeNavbar() {
    const collapse = new bootstrap.Collapse(this.navbarCollapse.nativeElement, {
      toggle: false,
    });
    collapse.hide();
  }
}
