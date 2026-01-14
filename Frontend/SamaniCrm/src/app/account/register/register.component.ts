import { Component, Injector, OnInit } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
  animations: [accountModuleAnimation()],
  standalone: false,
})
export class RegisterComponent extends AppComponentBase implements OnInit {
  registerForm: FormGroup;
  loading = false;
  constructor(injector: Injector) {
    super(injector);
    this.registerForm = this.fb.group({
      name: ['', [Validators.required]],
      username: ['', [Validators.required]],
      mobile: ['', [Validators.required]],
      password: ['', [Validators.required]],
      password_confirmation: ['', [Validators.required]],
    });
  }

  ngOnInit(): void {}

  signUp() {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      this.notify.warning(this.l('CompleteFormField'));
      return;
    }
    this.loading = true;
    const formValue = this.registerForm.value;
    // this.authService
    //   .register(formValue)
    //   .pipe(finalize(() => (this.loading = false)))
    //   .subscribe((result: any) => {
    //     this.notify.success('ثبت نام شما با موفقیت انجام شد');
    //     this.router.navigate(['/app']);
    //   });
  }
}
