import { CurrencyPipe, DecimalPipe, PercentPipe } from '@angular/common';
import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AgentService } from '../../../../_services/group-settle-services/agent.service';
import { debounceTime, distinctUntilChanged, finalize, map, Observable, startWith, switchMap } from 'rxjs';
import { MatAutocompleteTrigger } from '@angular/material/autocomplete';

@Component({
  selector: 'app-add-statement-description',
  templateUrl: './add-statement-description.component.html',
  styleUrls: ['./add-statement-description.component.scss'],
  providers: [CurrencyPipe, DecimalPipe, PercentPipe],
})
export class AddStatementDescriptionComponent implements OnInit {
  @ViewChild('desc_2') trigger: MatAutocompleteTrigger;
  public items: any[] = [];
  public form: FormGroup;
  public filteredOptions: Observable<any>;
  public desc_2: any;
  public value_1: any;
  public loading: boolean;
  public triggerModal: boolean = false;

  listItems: any[] = [];

  constructor(
    public dialog: MatDialog,
    private formBuilder: FormBuilder,
    private snackbar: MatSnackBar,
    private agentService: AgentService,
    private dialogRef: MatDialogRef<AddStatementDescriptionComponent>,
  ) {}

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      desc_2: null,
      value_1: null,
    });
    this.filteredOptions = this.form.get('desc_2').valueChanges.pipe(
      startWith(''),
      map((value) => (typeof value === 'string' ? value : '')),
      map((val) => this._filter(val)),
    );
  }

  getListByCode() {
    let code = this.form.get('value_1')?.value;
    this.loading = true;
    this.agentService
      .getAllStatementDescriptionsFromDEPOSIT(code)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe(
        (res) => {
          if (res.isSuccess) {
            this.listItems = res.value;
            this.trigger.openPanel();
            this.triggerModal = true;
          } else {
            this.snackbar.open('دریافت اطلاعات با خطا مواجه شد', 'بستن', {
              duration: 3000,
            });
          }
        },
        (error) => {
          this.snackbar.open('دریافت اطلاعات با خطا مواجه شد', 'بستن', {
            duration: 3000,
          });
        },
      );
  }

  private _filter(filterValue: string) {
    // این سرچ حتما خودت چک کن
    const filtered = this.listItems.filter(
      (option) => option.descriptiono.includes(filterValue) || option.code.includes(filterValue),
    );
    return filtered;
  }

  public submit() {
    let data = [
      {
        value_1: this.form.get('value_1').value,
        desc_2: this.form.get('desc_2').value,
      },
    ];
    this.agentService.insertStatementDescriptions(data).subscribe(
      (res) => {
        if (res.isSuccess) {
          this.dialogRef.close(res.value);
          this.snackbar.open('بستن', 'اطلاعات با موفقیت ثبت شد', {
            duration: 3000,
          });
        } else {
          this.snackbar.open('بستن', 'ثبت اطلاعات با خطا مواجه شد', {
            duration: 3000,
          });
        }
      },
      (error) => {
        this.snackbar.open('بستن', 'ثبت اطلاعات با خطا مواجه شد', {
          duration: 3000,
        });
      },
    );
  }
}
