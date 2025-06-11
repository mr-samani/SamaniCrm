import { bootstrapApplication } from '@angular/platform-browser';
import { App } from './app/app';
import { config } from './app/app.config.server';

// برای غیر فعال کردن بررسی ssl
process.env['NODE_TLS_REJECT_UNAUTHORIZED'] = '0';

const bootstrap = () => bootstrapApplication(App, config);

export default bootstrap;
