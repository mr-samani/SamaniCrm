export interface MaintenanceDto {
  cacheKeys: CacheKey[];
}

export interface CacheKey {
  key: string;
  title: string;
  loading?: boolean;
}
