import { isPlatformBrowser } from '@angular/common';
import { DOCUMENT, inject, Inject, Injectable, PLATFORM_ID, Renderer2, RendererFactory2 } from '@angular/core';
import { AppConst } from '@shared/app-const';
import { StoreService } from './localstore.service';
import { MediaMatcher } from '@angular/cdk/layout';
declare type ColorMode = 'dark' | 'light';

@Injectable({
  providedIn: 'root',
})
export class ColorSchemaService {
  private renderer: Renderer2;
  private colorScheme: ColorMode | '' = '';
  // Define prefix for clearer and more readable class names in scss files
  private colorSchemePrefix = 'color-scheme-';

  constructor(
    rendererFactory: RendererFactory2,
    @Inject(DOCUMENT) private _document: Document,
    private mediaMatcher: MediaMatcher,
    private storeService: StoreService
  ) {
    // Create new renderer from renderFactory, to make it possible to use renderer2 in a service
    this.renderer = rendererFactory.createRenderer(null, null);
  }

  _detectPrefersColorScheme() {
    let schema: ColorMode = 'dark';
    // Detect if prefers-color-scheme is supported
    if (this.mediaMatcher.matchMedia('(prefers-color-scheme)').media !== 'not all') {
      // Set colorScheme to Dark if prefers-color-scheme is dark. Otherwise, set it to Light.
      schema = this.mediaMatcher.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }
    this._setColorScheme(schema);
  }

  _setColorScheme(scheme: ColorMode) {
    this.colorScheme = scheme;
    AppConst.isDarkMode = scheme === 'dark';

    // Save prefers-color-scheme
    this.storeService.setItem('prefers-color', scheme);
  }

  _getColorScheme() {
    const colorScheme: ColorMode = this.storeService.getItem('prefers-color') as ColorMode;
    if (colorScheme) {
      this.colorScheme = colorScheme;
    } else {
      this._detectPrefersColorScheme();
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
