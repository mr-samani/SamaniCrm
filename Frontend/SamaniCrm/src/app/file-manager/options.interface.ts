export interface IOptions {
  /**
   * نوع فایل های مجاز برای انتخاب
   */
  type: 'Image' | 'Video' | 'Excel' | 'Doc' | 'Pdf' | 'All';
  /**
   * حداقل حجم سایز
   * - byte
   */
  minSize?: number;
  /**
   * حداکثر حجم سایز
   * - byte
   */
  maxSize?: number;

  /**
   * پیش نمایش برای فایل انتخابی
   * - اگر در برنامه بخواهیم چنیدن فایل انتخاب کنیم بهتر است این گزینه روی False تنظیم شود
   */
  showPreview?: boolean;
}
export class FileManagerOptions implements IOptions {
  type: 'Image' | 'Video' | 'Excel' | 'Doc' | 'Pdf' | 'All' = 'All';
  minSize = 0;
  maxSize = 0;
  showPreview = true;
}
