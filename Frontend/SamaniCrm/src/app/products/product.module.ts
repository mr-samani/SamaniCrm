import { NgModule } from '@angular/core';
    import { CommonModule } from '@angular/common';
import { ProductRoutingModule } from './product-routing.module';
import { ProductComponent } from './product.component';
import { TranslateModule } from '@ngx-translate/core';
    
    @NgModule({
      declarations: [
        ProductComponent
      ],
      imports: [
        CommonModule,
         ProductRoutingModule,
         TranslateModule
      ]
    })
    export class ProductModule { }
    