import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss'],
  encapsulation: ViewEncapsulation.None,
  standalone: false,
})
export class AccountComponent extends AppComponentBase implements OnInit {
  backgroundImage = '';
  backgroundColor = '';
  constructor(
    injector: Injector,
    private http: HttpClient,
  ) {
    super(injector);
    this.setBackgroundImage();
  }

  ngOnInit(): void {}

  setBackgroundImage() {
    const imageCount = 7;
    const rndImg = Math.floor(Math.random() * imageCount) + 1;

    this.backgroundImage = AppConst.baseUrl + '/images/login/' + rndImg + '/image.png';
    const cssUrl = AppConst.baseUrl + '/images/login/' + rndImg + '/background.css';
    const headers = new HttpHeaders().set('Content-Type', 'text/plain; charset=utf-8');
    this.http.get<string>(cssUrl, { headers: headers, responseType: 'text' as any }).subscribe((c) => {
      this.backgroundColor = c;
    });
  }
}
