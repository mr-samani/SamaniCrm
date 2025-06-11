import { isPlatformBrowser } from '@angular/common';
import { DOCUMENT, inject, Inject, Injectable, PLATFORM_ID, Renderer2, RendererFactory2 } from '@angular/core';
import { AppConst } from '@shared/app-const';

declare type ColorMode = 'dark' | 'light';

@Injectable({
  providedIn: 'root',
})
export class ColorSchemaService {
  private renderer: Renderer2;
  private colorScheme: ColorMode | '' = '';
  // Define prefix for clearer and more readable class names in scss files
  private colorSchemePrefix = 'color-scheme-';
  platformId = inject(PLATFORM_ID);

  constructor(rendererFactory: RendererFactory2, @Inject(DOCUMENT) private _document: Document) {
    // Create new renderer from renderFactory, to make it possible to use renderer2 in a service
    this.renderer = rendererFactory.createRenderer(null, null);
  }

  _detectPrefersColorScheme() {
    let schema: ColorMode = 'dark';
    // Detect if prefers-color-scheme is supported
    if (window.matchMedia('(prefers-color-scheme)').media !== 'not all') {
      // Set colorScheme to Dark if prefers-color-scheme is dark. Otherwise, set it to Light.
      schema = window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }
    this._setColorScheme(schema);
  }

  _setColorScheme(scheme: ColorMode) {
    this.colorScheme = scheme;
    AppConst.isDarkMode = scheme === 'dark';

    // Save prefers-color-scheme to localStorage
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem('prefers-color', scheme);
    }
  }

  _getColorScheme() {
    if (isPlatformBrowser(this.platformId)) {
      const localStorageColorScheme: ColorMode = localStorage.getItem('prefers-color') as ColorMode;
      // Check if any prefers-color-scheme is stored in localStorage
      if (localStorageColorScheme) {
        // Save prefers-color-scheme from localStorage
        this.colorScheme = localStorageColorScheme;
      } else {
        // If no prefers-color-scheme is stored in localStorage, try to detect OS default prefers-color-scheme
        this._detectPrefersColorScheme();
      }
    }
  }

  load() {
    this._getColorScheme();
    this.renderer.addClass(this._document.body, this.colorSchemePrefix + this.colorScheme);
    this.renderer.setAttribute(this._document.documentElement, 'data-bs-theme', this.colorScheme);
    AppConst.isDarkMode = this.colorScheme === 'dark';
  }

  update(scheme: ColorMode) {
    this._setColorScheme(scheme);
    // Remove the old color-scheme class
    this.renderer.removeClass(
      this._document.body,
      this.colorSchemePrefix + (this.colorScheme === 'dark' ? 'light' : 'dark')
    );
    // Add the new / current color-scheme class
    this.renderer.addClass(this._document.body, this.colorSchemePrefix + scheme);
    this.renderer.setAttribute(this._document.documentElement, 'data-bs-theme', this.colorScheme);
  }

  public toggleMode() {
    if (this.colorScheme === 'dark') {
      this.update('light');
    } else {
      this.update('dark');
    }
  }

  public get isDarkMode() {
    return this.colorScheme === 'dark';
  }
}
