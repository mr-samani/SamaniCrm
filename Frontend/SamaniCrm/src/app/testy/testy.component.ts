import { Directionality } from '@angular/cdk/bidi';
import { CdkDragDrop, CdkDragMove, moveItemInArray } from '@angular/cdk/drag-drop';
import { CommonModule } from '@angular/common';
import { AfterViewInit, ChangeDetectionStrategy, Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PaginationComponent } from '@shared/components/pagination/pagination.component';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { ColorSchemaService } from '@shared/services/color-schema.service';

@Component({
  selector: 'app-testy',
  standalone: true,
  imports: [CommonModule, MaterialCommonModule, FormsModule, PaginationComponent],
  templateUrl: './testy.component.html',
  styleUrl: './testy.component.scss',
})
export class TestyComponent implements AfterViewInit {
  name = '';
  dark: Boolean;
  page = 0;
  movies = [
    'Episode I - The Phantom Menace',
    'Episode II - Attack of the Clones',
    'Episode III - Revenge of the Sith',
    'Episode IV - A New Hope',
    'Episode V - The Empire Strikes Back',
    'Episode VI - Return of the Jedi',
    'Episode VII - The Force Awakens',
    'Episode VIII - The Last Jedi',
    'Episode IX - The Rise of Skywalker',
  ];

  constructor(
    private colorSchemaService: ColorSchemaService,
    private dir: Directionality,
  ) {
    this.dark = colorSchemaService.isDarkMode;
  }

  ngAfterViewInit(): void {}

  toggleDarkMode() {
    this.colorSchemaService.toggleMode();
  }

  drop(event: CdkDragDrop<string[]>) {
    moveItemInArray(this.movies, event.previousIndex, event.currentIndex);
  }

  onDragMoved(event: CdkDragMove) {
    const previewElement: HTMLElement | null = document.querySelector('.cdk-drag.cdk-drag-preview');
    console.log(event.pointerPosition.x);
    // if (previewElement) {
    //   const w = document.documentElement.offsetWidth;
    // //  setTimeout(() => {
    //     //let x =  event.pointerPosition.x - w;
    //     const x =
    //       this.dir.value === 'rtl'
    //         ? -event.pointerPosition.x
    //         : event.pointerPosition.x;
    //     previewElement.style.transform = `translate3d(${x}px, ${event.pointerPosition.y}px, 0) !important`;
    //     //   console.log('previewElement.style.transform',previewElement.style.transform);
    //     console.log('x', x, previewElement.style.transform,previewElement);
    //  // }, 500);
    // }
  }
}
