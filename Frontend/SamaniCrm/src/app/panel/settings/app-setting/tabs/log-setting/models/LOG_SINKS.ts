import { LogSinkFlag } from './LogSinkFlag';

export const LOG_SINKS = [
    { value: LogSinkFlag.File, label: '📁 File', icon: '📁' },
    { value: LogSinkFlag.Database, label: '🗄️ Database', icon: '🗄️' },
    { value: LogSinkFlag.Telegram, label: '📱 Telegram', icon: '📱' },
    { value: LogSinkFlag.Bale, label: '📱 Bale', icon: '📱' },
    { value: LogSinkFlag.ExternalApi, label: '🌐 External API', icon: '🌐' },
];
