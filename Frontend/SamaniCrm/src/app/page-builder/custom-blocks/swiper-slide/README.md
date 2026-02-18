# Enhanced Swiper Slider Component

A professional, feature-rich Angular Swiper component with **preset templates**, **lazy loading modules**, and **comprehensive customization options**.

## 🚀 Features

### ✨ Preset Templates
Choose from 7 professionally designed presets:
- **Default Slider** - Classic slider with navigation and pagination
- **Cards Stack** - Tinder-like card swiper (Instagram Reels style)
- **Stories** - Instagram stories style horizontal scroll
- **Product Showcase** - Grid layout perfect for e-commerce
- **Coverflow** - Apple iTunes style 3D effect
- **Cube Effect** - 3D cube transition
- **Creative** - Custom creative effects with parallax

### 🎯 Smart Lazy Loading
- Only loads required Swiper modules based on your settings
- Dynamically imports CSS files on-demand
- Reduces initial bundle size significantly
- Better performance and faster page loads

### 🎨 Comprehensive Customization
- **Basic Settings**: Effect, direction, speed, slides per view, spacing
- **Navigation**: Arrows, pagination (bullets/fraction/progressbar), scrollbar
- **Autoplay**: Configurable delay, pause on hover, disable on interaction
- **Advanced**: Keyboard control, mousewheel, zoom, free mode, lazy image loading
- **Responsive**: Define different settings for multiple breakpoints
- **Grid Layout**: Support for multi-row product grids

### 📱 Fully Responsive
- Mobile-first design
- Customizable breakpoints for different screen sizes
- Touch-friendly on all devices

## 📦 Installation

### 1. Install Swiper
```bash
npm install swiper@12.1.0
```

### 2. Copy Files
Copy the `swiper-slide-enhanced` folder into your Angular components directory.

### 3. Register Component
In your page builder configuration:
```typescript
import { SWIPER_SLIDE_BLOCK } from './path/to/swiper-slide-definition';

// Add to your blocks array
blocks: [
  SWIPER_SLIDE_BLOCK,
  // ... other blocks
]
```

## 🎯 Usage

### Quick Start with Presets

1. **Add the component** to your page via the page builder
2. **Open settings** and go to the **Presets** tab
3. **Click on any preset** to instantly apply pre-configured settings
4. **Go to Slides tab** to add your images and content
5. **Customize further** in Basic, Advanced, or Responsive tabs if needed

### Available Presets

#### 🎞️ Default Slider
Perfect for: Hero sections, image galleries, testimonials
```
- Horizontal slide effect
- Navigation arrows
- Pagination bullets
- Loop enabled
- Responsive breakpoints
```

#### 🃏 Cards Stack
Perfect for: Dating apps, product highlights, featured content
```
- Cards effect (Tinder-style)
- Single card view
- Swipe gesture
- No navigation/pagination
- Centered presentation
```

#### 📸 Stories
Perfect for: Social media style content, quick previews
```
- Horizontal auto-width slides
- Free mode scrolling
- No pagination
- Touch-friendly
- Instagram stories layout
```

#### 🛍️ Product Showcase
Perfect for: E-commerce, product catalogs, portfolios
```
- Grid layout (2 rows)
- Multiple slides per view
- Navigation arrows
- Responsive grid (2-5 columns)
- Loop enabled
```

#### 💿 Coverflow
Perfect for: Albums, music players, media galleries
```
- 3D coverflow effect
- 3 slides visible
- Centered active slide
- Depth and rotation
- Pagination bullets
```

#### 🎲 Cube Effect
Perfect for: Premium presentations, modern showcases
```
- 3D cube transition
- Single slide view
- Fraction pagination
- Shadow effects
- Unique perspective
```

#### ✨ Creative
Perfect for: Artistic presentations, unique experiences
```
- Custom parallax effects
- Slide transformations
- Scale and opacity
- Creative transitions
- Highly customizable
```

## 🎛️ Settings Tabs

### 1️⃣ Presets Tab
- Quick selection of pre-configured templates
- Visual cards with descriptions
- One-click application

### 2️⃣ Slides Tab
- Add/remove slides
- Upload images
- Set title, description, and links
- Accordion interface for easy management

### 3️⃣ Basic Tab
- Effect type selection
- Direction (horizontal/vertical)
- Slides per view
- Space between slides
- Transition speed
- Common toggles (loop, grab cursor, etc.)

### 4️⃣ Advanced Tab
Organized into groups:
- **Navigation & Controls**: Arrows, pagination, scrollbar, keyboard, mousewheel
- **Autoplay**: Enable/disable, delay, pause on hover
- **Performance**: Lazy loading, zoom, free mode
- **Grid Layout**: Rows, fill direction

### 5️⃣ Responsive Tab
- Add multiple breakpoints
- Configure slides per view for each screen size
- Adjust spacing for different devices

## 🔧 Technical Details

### Lazy Loading Architecture

The component intelligently loads only the required modules:

```typescript
// Example: If you enable Navigation + Pagination + Cards Effect
// Only these modules will be loaded:
import { Navigation, Pagination, EffectCards } from 'swiper/modules';
import 'swiper/css/navigation';
import 'swiper/css/pagination';
import 'swiper/css/effect-cards';
```

### Module Mapping

| Feature | Module Loaded | CSS Loaded |
|---------|--------------|------------|
| Navigation | `Navigation` | `swiper/css/navigation` |
| Pagination | `Pagination` | `swiper/css/pagination` |
| Scrollbar | `Scrollbar` | `swiper/css/scrollbar` |
| Autoplay | `Autoplay` | - |
| Keyboard | `Keyboard` | - |
| Mousewheel | `Mousewheel` | - |
| Zoom | `Zoom` | `swiper/css/zoom` |
| Free Mode | `FreeMode` | - |
| Grid | `Grid` | `swiper/css/grid` |
| Cards Effect | `EffectCards` | `swiper/css/effect-cards` |
| Cube Effect | `EffectCube` | `swiper/css/effect-cube` |
| Coverflow | `EffectCoverflow` | `swiper/css/effect-coverflow` |
| Fade Effect | `EffectFade` | `swiper/css/effect-fade` |
| Flip Effect | `EffectFlip` | `swiper/css/effect-flip` |
| Creative | `EffectCreative` | `swiper/css/effect-creative` |

## 🎨 Customization Examples

### Custom Preset Configuration

You can modify `SwiperSlideSetting.ts` to add your own presets:

```typescript
private applyMyCustomPreset() {
  this.effect = 'fade';
  this.slidesPerView = 1;
  this.autoplay = true;
  this.autoplayDelay = 5000;
  this.navigation = true;
  this.pagination = true;
  this.paginationType = 'progressbar';
  // ... more settings
}
```

### Styling Slides

Modify `swiper-slide.component.scss` to customize slide appearance:

```scss
.slide-content {
  // Your custom styles
}

.slide-info {
  // Customize title/description area
}
```

## 📱 Responsive Behavior

The component is mobile-first and handles different screen sizes gracefully:

- **Cards Effect**: Automatically scales down on mobile (240px × 320px)
- **Stories**: Always maintains compact story format
- **Product Grid**: Adapts columns based on breakpoints (2-5 columns)
- **Default Slider**: Responsive slides per view configuration

## 🐛 Troubleshooting

### Slides not appearing
1. Make sure slides array has data: `settings.sliders.length > 0`
2. Check that `detectChanges()` is called before `update()`
3. Verify images are accessible at `baseUrl + item.image`

### Effect not working
1. Ensure the effect CSS is loaded (check browser Network tab)
2. Verify the effect module is in the loaded modules list (console log)
3. Some effects require specific settings (e.g., Cards needs `centeredSlides`)

### Lazy loading not working
1. Check that `settings.lazyLoading` is `true`
2. Verify images use `data-src` attribute
3. Ensure slides have `swiper-lazy` class

## 📄 License

This component uses Swiper.js which is licensed under MIT.

## 🙏 Credits

Built with:
- [Swiper.js](https://swiperjs.com/) - The most modern mobile touch slider
- Angular 18+
- Bootstrap 5 (for form styling)
