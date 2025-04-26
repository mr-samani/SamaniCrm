import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AccountServiceProxy } from '../shared/service-proxies';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
  providers:[
    AccountServiceProxy
  ]
})
export class AppComponent implements OnInit {
  title = 'SamaniCrm';
  constructor(private service: AccountServiceProxy) {}
  ngOnInit(): void {
    this.service
      .login({
        Password: '123qwe1',
        UserName: 'samani',
      })
      .subscribe({
        next: (r) => {
          console.log(r);
        },
      });
  }
}
