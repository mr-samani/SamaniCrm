import { ChangeDetectorRef, inject, Injector } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ColorSchemaService } from '@shared/services/color-schema.service';
import { LanguageService } from '@shared/services/language.service';

export class BaseComponent {
  colorSchemaService: ColorSchemaService;
  languageService: LanguageService;
  route: ActivatedRoute;

  cd = inject(ChangeDetectorRef);

  constructor(injector: Injector) {
    this.colorSchemaService = injector.get(ColorSchemaService);
    this.languageService = injector.get(LanguageService);
    this.route = injector.get(ActivatedRoute);
  }
}
