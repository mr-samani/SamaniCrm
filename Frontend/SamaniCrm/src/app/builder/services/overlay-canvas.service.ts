import { Injectable, Renderer2, RendererFactory2 } from '@angular/core';

export type BoxType = 'padding' | 'margin' | 'border' | 'content' | 'all';
export type Side = 'top' | 'right' | 'bottom' | 'left' | 'all';

export interface HighlightOptions {
  type?: BoxType[];
  side?: Side[];
  label?: boolean; // show size text
}

@Injectable({ providedIn: 'root' })
export class OverlayCanvasService {
  private canvas: HTMLCanvasElement | null = null;
  private ctx: CanvasRenderingContext2D | null = null;
  private pattern: CanvasPattern | null = null;
  private dpr = window.devicePixelRatio || 1;
  private rafId = 0;
  private currentTarget: Element | null = null;
  private currentOptions: HighlightOptions | null = null;

  constructor(private rendererFactory: RendererFactory2) {}

  init() {
    if (this.canvas) return;
    const renderer = this.rendererFactory.createRenderer(null, null) as Renderer2;
    this.canvas = renderer.createElement('canvas') as HTMLCanvasElement;
    this.canvas.className = 'overlay-canvas';
    renderer.setStyle(this.canvas, 'position', 'fixed');
    renderer.setStyle(this.canvas, 'top', '0');
    renderer.setStyle(this.canvas, 'left', '0');
    renderer.setStyle(this.canvas, 'width', '100%');
    renderer.setStyle(this.canvas, 'height', '100%');
    renderer.setStyle(this.canvas, 'pointerEvents', 'none');
    renderer.setStyle(this.canvas, 'zIndex', '2147483647');
    document.body.appendChild(this.canvas);

    this.ctx = this.canvas.getContext('2d')!;
    this.createPattern();
    this.resize();

    window.addEventListener('resize', this.resizeBound);
    window.addEventListener('scroll', this.resizeBound, true); // on scroll we need to redraw
  }

  destroy() {
    if (!this.canvas) return;
    window.removeEventListener('resize', this.resizeBound);
    window.removeEventListener('scroll', this.resizeBound, true);
    if (this.canvas.parentElement) this.canvas.parentElement.removeChild(this.canvas);
    this.canvas = null;
    this.ctx = null;
    this.pattern = null;
    cancelAnimationFrame(this.rafId);
  }

  private resizeBound = () => this.resize();

  private resize() {
    if (!this.canvas || !this.ctx) return;
    this.dpr = window.devicePixelRatio || 1;
    const w = window.innerWidth;
    const h = window.innerHeight;
    // set real pixel size then use transform so we can draw in CSS pixels
    this.canvas.width = Math.round(w * this.dpr);
    this.canvas.height = Math.round(h * this.dpr);
    this.canvas.style.width = w + 'px';
    this.canvas.style.height = h + 'px';
    this.ctx.setTransform(this.dpr, 0, 0, this.dpr, 0, 0);
    this.clearImmediate();
    if (this.currentTarget && this.currentOptions) this.scheduleRender();
  }

  private createPattern(color = '#2080ff78') {
    if (!this.ctx) return;
    const p = document.createElement('canvas');
    p.width = 8;
    p.height = 8;
    const pc = p.getContext('2d')!;
    pc.clearRect(0, 0, 8, 8);
    pc.strokeStyle = color;
    pc.lineWidth = 1;
    pc.beginPath();
    pc.moveTo(0, 8);
    pc.lineTo(8, 0);
    pc.stroke();

    this.pattern = this.ctx.createPattern(p, 'repeat');
  }

  highlightElement(target: Element, options: HighlightOptions = { type: ['padding'], side: ['all'], label: true }) {
    this.init();
    this.currentTarget = target;
    this.currentOptions = options;
    this.scheduleRender();
  }

  clear() {
    this.currentTarget = null;
    this.currentOptions = null;
    this.clearImmediate();
  }

  private clearImmediate() {
    if (!this.ctx) return;
    const w = window.innerWidth;
    const h = window.innerHeight;
    this.ctx.clearRect(0, 0, w, h);
  }

  private scheduleRender() {
    cancelAnimationFrame(this.rafId);
    this.rafId = requestAnimationFrame(() => this.render());
  }

  private render() {
    if (!this.ctx || !this.currentTarget || !this.currentOptions) return;
    this.clearImmediate();

    const el = this.currentTarget as HTMLElement;
    const rect = el.getBoundingClientRect();
    const style = getComputedStyle(el);

    const pTop = parseFloat(style.paddingTop) || 0;
    const pRight = parseFloat(style.paddingRight) || 0;
    const pBottom = parseFloat(style.paddingBottom) || 0;
    const pLeft = parseFloat(style.paddingLeft) || 0;

    const bTop = parseFloat(style.borderTopWidth) || 0;
    const bRight = parseFloat(style.borderRightWidth) || 0;
    const bBottom = parseFloat(style.borderBottomWidth) || 0;
    const bLeft = parseFloat(style.borderLeftWidth) || 0;

    const mTop = parseFloat(style.marginTop) || 0;
    const mRight = parseFloat(style.marginRight) || 0;
    const mBottom = parseFloat(style.marginBottom) || 0;
    const mLeft = parseFloat(style.marginLeft) || 0;

    // boxes in viewport (CSS px coords)
    const borderBox = {
      left: rect.left,
      top: rect.top,
      right: rect.right,
      bottom: rect.bottom,
      width: rect.width,
      height: rect.height,
    };

    const paddingBox = {
      left: rect.left + bLeft,
      top: rect.top + bTop,
      right: rect.right - bRight,
      bottom: rect.bottom - bBottom,
      width: rect.width - bLeft - bRight,
      height: rect.height - bTop - bBottom,
    };

    const contentBox = {
      left: paddingBox.left + pLeft,
      top: paddingBox.top + pTop,
      right: paddingBox.right - pRight,
      bottom: paddingBox.bottom - pBottom,
      width: paddingBox.width - pLeft - pRight,
      height: paddingBox.height - pTop - pBottom,
    };

    const marginBox = {
      left: rect.left - mLeft,
      top: rect.top - mTop,
      right: rect.right + mRight,
      bottom: rect.bottom + mBottom,
      width: rect.width + mLeft + mRight,
      height: rect.height + mTop + mBottom,
    };

    const type = this.currentOptions.type || ['padding'];
    const side = this.currentOptions.side || ['all'];

    // helper to draw hatched rect between outer and inner box
    const drawBetween = (outer: any, inner: any, side: Side[], fillAlpha = 0.8) => {
      const ctx = this.ctx!;
      const drawRect = (x: number, y: number, w: number, h: number) => {
        if (w <= 0 || h <= 0) return;
        ctx.save();
        if (this.pattern) ctx.fillStyle = this.pattern as CanvasPattern;
        ctx.globalAlpha = fillAlpha;
        ctx.fillRect(x, y, w, h);
        ctx.globalAlpha = 1;
        ctx.setLineDash([6, 4]);
        ctx.strokeStyle = '#30313157';
        ctx.lineWidth = 1;
        // stroke with 0.5 offset for crisper 1px borders
        ctx.strokeRect(x + 0.5, y + 0.5, Math.max(0.5, w - 1), Math.max(0.5, h - 1));
        ctx.restore();
      };

      if (side.includes('all') || side.includes('top')) {
        const x = outer.left;
        const y = outer.top;
        const w = outer.right - outer.left;
        const h = inner.top - outer.top;
        drawRect(x, y, w, h);
      }
      if (side.includes('all') || side.includes('bottom')) {
        const x = outer.left;
        const y = inner.bottom;
        const w = outer.right - outer.left;
        const h = outer.bottom - inner.bottom;
        drawRect(x, y, w, h);
      }
      if (side.includes('all') || side.includes('left')) {
        const x = outer.left;
        const y = inner.top;
        const w = inner.left - outer.left;
        const h = inner.bottom - inner.top;
        drawRect(x, y, w, h);
      }
      if (side.includes('all') || side.includes('right')) {
        const x = inner.right;
        const y = inner.top;
        const w = outer.right - inner.right;
        const h = inner.bottom - inner.top;
        drawRect(x, y, w, h);
      }
    };

    // draw depending on type
    if (type.includes('padding')) {
      drawBetween(paddingBox, contentBox, side);
    }
    if (type.includes('margin')) {
      drawBetween(marginBox, borderBox, side, 0.8);
    }
    if (type.includes('border')) {
      // border is the area between borderBox and paddingBox
      drawBetween(borderBox, paddingBox, side, 0.5);
    }
    if (type.includes('content') || type.includes('all')) {
      // visual helper: draw outlines of boxes
      const ctx = this.ctx!;
      ctx.save();
      ctx.strokeStyle = '#8503ffff';
      ctx.setLineDash([]);
      ctx.lineWidth = 1;
      const boxes = [marginBox, borderBox, paddingBox, contentBox];
      boxes.forEach((b, i) => {
        // const hatch = this.createHatchPattern('#0385ff', 8, 1);
        // const pattern = ctx.createPattern(hatch, 'repeat');
        // ctx.fillStyle = pattern!;
        // ctx.fillRect(b.left + 0.5, b.top + 0.5, Math.max(1, b.right - b.left - 1), Math.max(1, b.bottom - b.top - 1));

        ctx.strokeRect(b.left + 0.5, b.top + 0.5, Math.max(1, b.right - b.left - 1), Math.max(1, b.bottom - b.top - 1));
      });
      ctx.restore();
    }

    // draw labels if requested
    if (this.currentOptions.label) {
      this.drawLabels({ pTop, pRight, pBottom, pLeft, mTop, mRight, mBottom, mLeft }, rect, side);
    }
  }
  private createHatchPattern(color = '#00f', spacing = 8, lineWidth = 1) {
    const pCanvas = document.createElement('canvas');
    pCanvas.width = spacing;
    pCanvas.height = spacing;
    const pCtx = pCanvas.getContext('2d')!;

    pCtx.strokeStyle = color;
    pCtx.lineWidth = lineWidth;

    pCtx.beginPath();
    pCtx.moveTo(0, spacing);
    pCtx.lineTo(spacing, 0);
    pCtx.stroke();

    return pCanvas;
  }

  private drawLabels(sizes: any, rect: DOMRect, side: Side[]) {
    if (!this.ctx) return;
    const ctx = this.ctx;
    ctx.save();
    ctx.font = '12px system-ui, Arial';
    ctx.textBaseline = 'middle';
    ctx.fillStyle = 'rgba(0,0,0,0.8)';

    const drawText = (text: string, x: number, y: number) => {
      const padding = 6;
      const metrics = ctx.measureText(text);
      const w = metrics.width + padding * 2;
      const h = 18;
      ctx.save();
      ctx.fillStyle = 'rgba(255,255,255,0.95)';
      ctx.fillRect(x - w / 2, y - h / 2, w, h);
      ctx.strokeStyle = 'rgba(0,0,0,0.12)';
      ctx.strokeRect(x - w / 2 + 0.5, y - h / 2 + 0.5, w - 1, h - 1);
      ctx.fillStyle = 'rgba(0,0,0,0.85)';
      ctx.fillText(text, x - metrics.width / 2, y);
      ctx.restore();
    };

    // compute positions for center of each side
    const centerX = rect.left + rect.width / 2;
    const centerY = rect.top + rect.height / 2;

    if (side.includes('all') || side.includes('top')) {
      const text = Math.round(sizes.pTop || sizes.mTop) + 'px';
      drawText(text, centerX, rect.top - 10);
    }
    if (side.includes('all') || side.includes('bottom')) {
      const text = Math.round(sizes.pBottom || sizes.mBottom) + 'px';
      drawText(text, centerX, rect.bottom + 10);
    }
    if (side.includes('all') || side.includes('left')) {
      const text = Math.round(sizes.pLeft || sizes.mLeft) + 'px';
      drawText(text, rect.left - 24, centerY);
    }
    if (side.includes('all') || side.includes('right')) {
      const text = Math.round(sizes.pRight || sizes.mRight) + 'px';
      drawText(text, rect.right + 24, centerY);
    }

    ctx.restore();
  }
}
