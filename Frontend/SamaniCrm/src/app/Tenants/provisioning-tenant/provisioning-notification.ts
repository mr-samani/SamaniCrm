import { ProvisioningStepStatus } from '@shared/service-proxies/model/provisioning-step-status';
import { TenantProvisionStepsEnum } from '@shared/service-proxies/model/tenant-provision-steps-enum';

export class ProvisioningNotification {
  tenantSlug!: string;
  status!: ProvisioningStepStatus;
  message!: string;
  currentStep!: TenantProvisionStepsEnum;
  timestamp?: string;
}
