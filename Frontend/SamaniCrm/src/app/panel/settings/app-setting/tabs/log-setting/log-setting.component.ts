import { Component, OnInit, Pipe, PipeTransform } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { AdminLogServiceProxy } from '@shared/service-proxies/api/admin-log.service';
import { GetLogSettingQuery } from '@shared/service-proxies/model/get-log-setting-query';
import { TenantLogSettingDto } from '@shared/service-proxies/model/tenant-log-setting-dto';
import { UpdateLogSettingCommand } from '@shared/service-proxies/model/update-log-setting-command';
import { finalize } from 'rxjs/operators';
import { LOG_LEVELS } from './models/LOG_LEVELS';
import { LOG_SINKS } from './models/LOG_SINKS';
import { LogLevelFlag } from './models/log-level-flag';
import { LogSinkFlag } from './models/LogSinkFlag';
import { RETENTION_OPTIONS } from './models/RETENTION_OPTIONS';
import { Bitmask } from '@shared/helper/bit-mask.utils';

@Component({
    standalone: false,
    selector: 'app-log-setting',
    templateUrl: './log-setting.component.html',
    styleUrls: ['./log-setting.component.scss'],
})
export class LogSettingComponent extends AppComponentBase implements OnInit {
    loading = true;
    saving = false;
    settings?: TenantLogSettingDto;
    logLevels = LOG_LEVELS;
    logSinks = LOG_SINKS;
    retentionOptions = RETENTION_OPTIONS;

    enabledLevels: LogLevelFlag[] = [];
    enabledSinks: LogSinkFlag[] = [];
    retentionDays?: number | null = null;
    isEnabled = false;
    constructor(private logService: AdminLogServiceProxy) {
        super();
    }

    ngOnInit() {
        this.getSettings();
    }

    public get LogLevelFlag(): typeof LogLevelFlag {
        return LogLevelFlag;
    }

    public get LogSinkFlag(): typeof LogSinkFlag {
        return LogSinkFlag;
    }

    setOnlyErrors() {
        this.enabledLevels = [LogLevelFlag.Warning, LogLevelFlag.Error, LogLevelFlag.Critical];
    }

    setErrorsAndInfo() {
        this.enabledLevels = [
            LogLevelFlag.Information,
            LogLevelFlag.Warning,
            LogLevelFlag.Error,
            LogLevelFlag.Critical,
        ];
    }
    setAllLogLevels() {
        this.enabledLevels = [
            LogLevelFlag.Trace,
            LogLevelFlag.Debug,
            LogLevelFlag.Information,
            LogLevelFlag.Warning,
            LogLevelFlag.Error,
            LogLevelFlag.Critical,
        ];
    }

    hasFlag(mask: number, flag: number) {
        return (mask & flag) !== 0;
    }

    getSettings() {
        this.loading = true;
        this.logService
            .getSettings()
            .pipe(
                finalize(() => {
                    this.loading = false;
                    this.chdr.detectChanges();
                })
            )
            .subscribe((response) => {
                this.settings = response.data;
                this.retentionDays = this.settings?.retentionDays;
                if (this.settings?.enabledLevels) {
                    this.enabledLevels = Bitmask.getActiveFlagInfos(this.settings.enabledLevels, LogLevelFlag);
                } else {
                    this.enabledLevels = [];
                }
                if (this.settings?.enabledSinks) {
                    this.enabledSinks = Bitmask.getActiveFlagInfos(this.settings.enabledSinks, LogSinkFlag);
                } else {
                    this.enabledSinks = [];
                }
            });
    }

    save() {
        this.loading = true;
        const input = new UpdateLogSettingCommand();
        input.enabledLevels = Bitmask.enumArrayToFlag(this.enabledLevels);
        input.enabledSinks = Bitmask.enumArrayToFlag(this.enabledSinks);
        input.isEnabled = this.isEnabled;
        input.retentionDays = this.retentionDays ?? undefined;
        this.logService
            .updateSettings(input)
            .pipe(
                finalize(() => {
                    this.loading = false;
                    this.chdr.detectChanges();
                })
            )
            .subscribe((response) => {
                if (response.success) {
                    this.notify.success(this.l('SavedSuccessfully'));
                    this.getSettings();
                }
            });
    }
}
