import { LogLevelFlag } from './log-level-flag';

export const LOG_LEVELS = [
    { value: LogLevelFlag.Trace, label: '🔍 Trace', color: 'default' },
    { value: LogLevelFlag.Debug, label: '🐛 Debug', color: 'purple' },
    { value: LogLevelFlag.Information, label: 'ℹ️ Information', color: 'blue' },
    { value: LogLevelFlag.Warning, label: '⚠️ Warning', color: 'orange' },
    { value: LogLevelFlag.Error, label: '❌ Error', color: 'red' },
    { value: LogLevelFlag.Critical, label: '🔴 Critical', color: 'red' },
];
