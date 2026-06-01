import { AppPermissions } from '@shared/permissions/app-permissions';
import { IWidgetDefinition } from './IWidgetDefinition';

export const WIDGET_DEFINITIONS: IWidgetDefinition[] = [
  {
    title: 'Note',
    name: 'NoteWidget',
    component: () => import('./note/note.component').then((w) => w.NoteComponent),
    data: {
      title: 'This is a text!',
    },
  },
  {
    title: 'LogStats',
    name: 'LogStatsWidget',
    component: () => import('./log-stats/log-stats.component').then((c) => c.LogStatsComponent),
    permission: AppPermissions.LoggingSystem_AppLogs_GetStats,
  },
];
