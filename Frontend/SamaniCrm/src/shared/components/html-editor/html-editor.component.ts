import { Component, EventEmitter, forwardRef, Injector, Input, OnInit, Output } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
} from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import {
  FileUsageEnum,
  ImageCropperDialogComponent,
  ImageCropperDialogData,
} from '@app/file-manager/image-cropper-dialog/image-cropper-dialog.component';
import { TusUploadService } from '@app/file-manager/tus-upload.service';
import { AppConst } from '@shared/app-const';
import { Editor, TinyMCE } from 'tinymce';

type EditorOptions = Parameters<TinyMCE['init']>[0];
export interface CustomButton {
  tooltip: string;
  icon: string;
  action: (editor: Editor) => Function;
}

@Component({
  standalone: false,
  selector: 'html-editor',
  templateUrl: './html-editor.component.html',
  styleUrls: ['./html-editor.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => HtmlEditorComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: HtmlEditorComponent,
    },
  ],
})
export class HtmlEditorComponent extends AppComponentBase implements OnInit, ControlValueAccessor, Validator {
  @Input() customButtons: CustomButton[] = [];
  @Input() required = false;
  @Input() simpleView = false;
  @Output() init = new EventEmitter<Editor>();
  private _onChange: (color: string) => void = () => {};
  private _onTouched: () => void = () => {};
  tinyMceEditor: any;
  simplePlugIn = ['wordcount'];
  fullPlugIn = ['link', 'image', 'preview', 'code', 'fullscreen', 'table', 'code', 'wordcount', 'directionality'];
  simpleToolbar =
    'fontsize | bold italic backcolor forecolor | \
  alignleft aligncenter alignright alignjustify ';

  // https://www.tiny.cloud/docs/tinymce/6/available-toolbar-buttons/
  get fullToolbar() {
    let cbtns = '';
    for (let i = 0; i < this.customButtons.length; i++) {
      cbtns += 'customButton_' + i + ' ';
    }
    return (
      'undo redo | ' +
      cbtns +
      ' customInsertImageButton customInsertVoiceButton customInsertVideoButton customInsertPdfButton table \
   |fontfamily fontsize styles | bold italic underline strikethrough backcolor forecolor | \
   alignleft aligncenter alignright alignjustify lineheight \
   hr bullist numlist | removeformat |ltr rtl link code fullscreen preview'
    );
  }

  tinymceInit: any;
  content = '';
  disabled = false;
  constructor(
    private matDialog: MatDialog,
    public tusUploadService: TusUploadService,
    injector: Injector,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.initTinyConfig();
  }

  initTinyConfig() {
    this.tinymceInit = <EditorOptions>{
      license_key: 'gpl',
      branding: false,
      height: 250,
      draggable_modal: true,
      dragDropUpload: false,
      block_unsupported_drop: true,
      menubar: false,
      plugins: this.simpleView ? this.simplePlugIn : this.fullPlugIn,
      toolbar: this.simpleView ? this.simpleToolbar : this.fullToolbar,
      fontsize_formats: '8pt 10pt 12pt 14pt 18pt 24pt 36pt',
      setup: (editor: Editor) => {
        editor.ui.registry.addIcon(
          'pdf',
          '<svg  width="24" height="24" viewBox="0 0 384 512"><path d="M320 464C328.8 464 336 456.8 336 448V416H384V448C384 483.3 355.3 512 320 512H64C28.65 512 0 483.3 0 448V416H48V448C48 456.8 55.16 464 64 464H320zM256 160C238.3 160 224 145.7 224 128V48H64C55.16 48 48 55.16 48 64V192H0V64C0 28.65 28.65 0 64 0H229.5C246.5 0 262.7 6.743 274.7 18.75L365.3 109.3C377.3 121.3 384 137.5 384 154.5V192H336V160H256zM88 224C118.9 224 144 249.1 144 280C144 310.9 118.9 336 88 336H80V368C80 376.8 72.84 384 64 384C55.16 384 48 376.8 48 368V240C48 231.2 55.16 224 64 224H88zM112 280C112 266.7 101.3 256 88 256H80V304H88C101.3 304 112 293.3 112 280zM160 240C160 231.2 167.2 224 176 224H200C226.5 224 248 245.5 248 272V336C248 362.5 226.5 384 200 384H176C167.2 384 160 376.8 160 368V240zM192 352H200C208.8 352 216 344.8 216 336V272C216 263.2 208.8 256 200 256H192V352zM336 224C344.8 224 352 231.2 352 240C352 248.8 344.8 256 336 256H304V288H336C344.8 288 352 295.2 352 304C352 312.8 344.8 320 336 320H304V368C304 376.8 296.8 384 288 384C279.2 384 272 376.8 272 368V240C272 231.2 279.2 224 288 224H336z"/></svg>',
        );
        editor.ui.registry.addIcon(
          'variable',
          '<svg  width="24" height="24" viewBox="0 0 640 512"><path d="M205.1 52.76C201.3 40.3 189.3 32.01 176 32.01S150.7 40.3 146 52.76l-144 384c-6.203 16.56 2.188 35 18.73 41.22c16.55 6.125 34.98-2.156 41.2-18.72l28.21-75.25h171.6l28.21 75.25C294.9 472.1 307 480 320 480c3.734 0 7.531-.6562 11.23-2.031c16.55-6.219 24.94-24.66 18.73-41.22L205.1 52.76zM114.2 320L176 155.1l61.82 164.9H114.2zM608 160c-13.14 0-24.37 7.943-29.3 19.27C559.2 167.3 536.5 160 512 160c-70.58 0-128 57.41-128 128l.0007 63.93c0 70.59 57.42 128.1 127.1 128.1c24.51 0 47.21-7.266 66.7-19.26C583.6 472.1 594.9 480 608 480c17.67 0 32-14.31 32-32V192C640 174.3 625.7 160 608 160zM576 352c0 35.28-28.7 64-64 64s-64-28.72-64-64v-64c0-35.28 28.7-63.1 64-63.1s64 28.72 64 63.1V352z"/></svg>',
        );
        if (!this.simpleView) {
          editor.ui.registry.addButton('customInsertImageButton', this.customInsertImageButton(editor));
          editor.ui.registry.addButton('customInsertVoiceButton', this.customInsertVoiceButton(editor));
          editor.ui.registry.addButton('customInsertVideoButton', this.customInsertVideoButton(editor));
          editor.ui.registry.addButton('customInsertPdfButton', this.customInsertPdfButton(editor));
        }
        if (this.customButtons.length > 0) {
          for (let i = 0; i < this.customButtons.length; i++) {
            editor.ui.registry.addButton('customButton_' + i, this.customButton(editor, i));
          }
        }
        this.tinyMceEditor = editor;
      },
      language_url: 'tinymce/langs/' + this.getLang + '.js',
      language: this.getLang,
      font_family_formats:
        'IranSans =iranSans;Axiforma; times; Arial=arial,helvetica,sans-serif; Arial Black=arial black;Tahoma=tahoma,arial',
      skin: AppConst.isDarkMode ? 'oxide-dark' : 'oxide',
      content_css: AppConst.isDarkMode ? 'dark' : undefined,
    };
  }
  validate(control: AbstractControl): ValidationErrors | null {
    const content = control.value;
    if (!content && this.required) {
      return { required: true };
    } else {
      return null;
    }
  }

  private get getLang() {
    if (['fa', 'fa-IR'].indexOf(AppConst.currentLanguage) > -1) {
      return 'fa_IR';
    } else {
      return AppConst.currentLanguage;
    }
  }
  writeValue(value: any): void {
    if (value && value !== this.content) {
      // console.log(
      //     AppConsts.fileServerVariableName,
      //     AppConsts.fileServerUrl,
      //     new RegExp(AppConsts.fileServerVariableName, 'g'),
      // );
      // replace file address variable to  absolute address
      this.content = value; //.replace(new RegExp(AppConsts.fileServerVariableName, 'g'), AppConsts.fileServerUrl);
      // this.content.setValue(value);
    }
  }
  registerOnChange(fn: any): void {
    this._onChange = fn;
  }
  registerOnTouched(fn: any): void {
    this._onTouched = fn;
  }
  setDisabledState?(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  change(event: string) {
    if (event) {
      // console.log(
      //     AppConsts.fileServerVariableName,
      //     AppConsts.fileServerUrl,
      //     new RegExp(AppConsts.fileServerVariableName, 'g'),
      // );
      // replace file server address to variable
      // event = event.replace(new RegExp(AppConsts.fileServerUrl, 'g'), AppConsts.fileServerVariableName);
    }
    this._onChange(event);
  }

  customInsertImageButton(editor: Editor) {
    return {
      icon: 'image',
      tooltip: this.l('AddImage'),
      onAction: () => {
        this.matDialog
          .open(ImageCropperDialogComponent, {
            data: <ImageCropperDialogData>{},
          })
          .afterClosed()
          .subscribe((result) => {
            if (result) {
              editor.insertContent('&nbsp;<img src="' + AppConst.baseUrl + '/' + result + '" />&nbsp;');
            }
          });
      },
    };
  }

  customInsertVoiceButton(editor: Editor) {
    return {
      icon: 'embed',
      tooltip: this.l('AddAudioFile'),
      onAction: () => {
        let fileselector: any = document.querySelector('#voicefileselector');
        if (fileselector) {
          fileselector.click();
        }
      },
    };
  }
  customInsertVideoButton(editor: Editor) {
    return {
      icon: 'gallery',
      tooltip: this.l('AddVideoFile'),
      onAction: () => {
        let fileselector: any = document.querySelector('#videofileselector');
        if (fileselector) {
          fileselector.click();
        }
      },
    };
  }

  customInsertPdfButton(editor: Editor) {
    return {
      icon: 'pdf',
      tooltip: this.l('AddPdfFile'),
      onAction: () => {
        let fileselector: any = document.querySelector('#pdffileselector');
        if (fileselector) {
          fileselector.click();
        }
      },
    };
  }

  /*------------------------------------------*/

  selectFile(ev: any, fileTypes: string[]): void {
    let fileList = ev.currentTarget.files;
    if (fileList.length) {
      let type = '.' + fileList[0].name.split('.').pop().toLowerCase();
      console.log('allowedTypes:', fileTypes, 'userFile:', type);
      if (fileTypes.indexOf(type) > -1) {
        this.tusUploadService
          .uploadFile(fileList[0], '', FileUsageEnum.HTML_EDITOR)
          .then((result) => {
            result = AppConst.baseUrl + '/' + result;
            switch (type) {
              case '.mp3':
                this.tinyMceEditor.insertContent(
                  '&nbsp;<audio  controls><source src="' + result + '" type="audio/mp3"></audio>&nbsp;',
                );
                break;
              case '.mp4':
                this.tinyMceEditor.insertContent(
                  '&nbsp;<video  controls><source src="' + result + '" type="video/mp4"></video>&nbsp;',
                );
                break;
              case '.pdf':
                this.tinyMceEditor.insertContent(
                  '&nbsp;<a href="' + result + '" target="_blank" download>' + this.l('FileDownload') + '</a>&nbsp;',
                );
                break;
            }
          })
          .catch((error) => {
            this.notify.error(this.l('AnErrorOccurred'));
          });
      } else {
        this.notify.warning(this.l('SelectOnlyFile{0}', fileTypes.join(',')) + '\n' + this.l('FileTypeIsNotAllowed'));
      }
    }
  }

  customButton(editor: Editor, index: number) {
    return {
      icon: this.customButtons[index].icon,
      tooltip: this.customButtons[index].tooltip,
      onAction: () => this.customButtons[index].action(editor),
    };
  }

  onInitTinyMce(ev: any) {
    // console.log(ev.editor.ui.registry.getAll().buttons);
    this.init.emit(ev.editor);
  }
}
