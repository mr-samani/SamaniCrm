import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'userAgent',
    standalone: true
})
export class UserAgentPipe implements PipeTransform {

    transform(userAgent?: string | null): string {

        if (!userAgent?.trim()) {
            return 'Unknown';
        }

        const ua = userAgent;

        const os = this.detectOs(ua);
        const browser = this.detectBrowser(ua);

        if (os === 'Unknown' && browser === 'Unknown') {
            return ua;
        }

        return `${os} • ${browser}`;
    }

    private detectOs(ua: string): string {

        let match: RegExpMatchArray | null;

        // Android
        match = ua.match(/Android\s([\d.]+)/i);
        if (match) {
            return `Android ${match[1]}`;
        }

        // iPhone
        match = ua.match(/iPhone OS ([\d_]+)/i);
        if (match) {
            return `iPhone (iOS ${match[1].replace(/_/g, '.')})`;
        }

        // iPad
        match = ua.match(/CPU OS ([\d_]+)/i);
        if (match && ua.includes('iPad')) {
            return `iPad (iPadOS ${match[1].replace(/_/g, '.')})`;
        }

        // Windows 11 / 10
        if (/Windows NT 10.0/i.test(ua)) {
            return 'Windows';
        }

        // Mac
        match = ua.match(/Mac OS X ([\d_]+)/i);
        if (match) {
            return `MacOS ${match[1].replace(/_/g, '.')}`;
        }

        // Linux
        if (/Linux/i.test(ua)) {
            return 'Linux';
        }

        return 'Unknown';
    }

    private detectBrowser(ua: string): string {

        let match: RegExpMatchArray | null;

        // Edge
        match = ua.match(/Edg\/([\d.]+)/i);
        if (match) {
            return `Edge ${this.major(match[1])}`;
        }

        // Opera
        match = ua.match(/OPR\/([\d.]+)/i);
        if (match) {
            return `Opera ${this.major(match[1])}`;
        }

        // Firefox
        match = ua.match(/Firefox\/([\d.]+)/i);
        if (match) {
            return `Firefox ${this.major(match[1])}`;
        }

        // Chrome
        match = ua.match(/Chrome\/([\d.]+)/i);
        if (match) {
            return `Chrome ${this.major(match[1])}`;
        }

        // Safari
        match = ua.match(/Version\/([\d.]+).*Safari/i);
        if (match) {
            return `Safari ${this.major(match[1])}`;
        }

        return 'Unknown';
    }

    private major(version: string): string {
        return version.split('.')[0];
    }
}