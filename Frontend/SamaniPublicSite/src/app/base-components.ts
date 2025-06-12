import { Injector } from '@angular/core';
import { ColorSchemaService } from '@shared/services/color-schema.service';
import { LanguageService } from '@shared/services/language.service';

export class BaseComponent {
  colorSchemaService: ColorSchemaService;
  languageService: LanguageService;

  constructor(injector: Injector) {
    this.colorSchemaService = injector.get(ColorSchemaService);
    this.languageService = injector.get(LanguageService);
  }
}
