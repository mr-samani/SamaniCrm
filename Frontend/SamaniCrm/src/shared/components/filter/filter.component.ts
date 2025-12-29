
import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
  output,
} from '@angular/core';
import { ControlContainer, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'filter',
  standalone: true,
  imports: [TranslateModule, MatButtonModule, ReactiveFormsModule, MatDialogModule],
  templateUrl: './filter.component.html',
  styleUrl: './filter.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  viewProviders: [
    {
      provide: ControlContainer,
      useExisting: FormGroup,
    },
  ],
})
export class FilterComponent implements OnInit {
  @Input() form!: FormGroup;
  @Output() change = new EventEmitter<FormGroup>();
  @ViewChild('formContent', { static: true })
  formContent!: ElementRef<HTMLFormElement>;
  showFilter = false;
  constructor(private matDialog: MatDialog) {}
  ngOnInit(): void {}

  toggleFilter() {
    this.showFilter = !this.showFilter;
  }

  onReload() {
    console.log('filterForm', this.form.value);
    this.change.emit(this.form);
  }
}
