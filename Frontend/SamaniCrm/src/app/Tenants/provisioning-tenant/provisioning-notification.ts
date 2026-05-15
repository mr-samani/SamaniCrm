export class ProvisioningNotification {
  tenantSlug!: string;
  status!: ProvisioningStepStatus;
  message!: string;
  currentStep!: TenantProvisionStepsEnum;
  tenantId?: string;
  adminUserId?: string;
  timestamp?: string;
}
 
export enum TenantProvisionStepsEnum
{
    CreateTenant,
    CreateAdminUser,
    ProvisionDatabase,
    RunMigrations,
    SeedData,
    Finalize,
};export enum ProvisioningStepStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Failed = 3,
}
