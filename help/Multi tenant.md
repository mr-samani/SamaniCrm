### USER
من یه پروژه با انگولار 21 و دات نت 9 دارم معماری دات نت 9 clean architecture و cqrs نوشتم
الان میخوام برایم multi tenant بودن را برای یه پروژه حرفه‌ای و enterprise بنویسی
که هم پرفورمنس عالی باشه و هم فوق حرفه ای
نکات امنیتی 100 درصد رعایت کن
چند بهره‌برداری باید اختیاری باشه
برای امنیت شاید بگی بهتره توی هدر tenant ID پاس ندیم بهجاش توی سشن کاربر باشه
دقیق و کامل برام بنویس چه تغییراتی باید اعمال کنم
فیلد های کامل دیتابیسش
ثبت نام بهره‌بردار
مدیریتش
چند دیتابیسی اگر نیاز بود هر بهره بردار دیتابیس جدا داشته باشد بتواند
بعد از ثبت نام به بردار با سیگنال آر فرایند ایجاد به کلاینت هاست خبر بدم توی چه مرحله ای هست مثلا ثبت اطلاعات ایجاد دیتابیس اجرای مایگریشن و…
هر چی که خودت میدونی کامل و دقیق

### ASSISTANT
<details type="reasoning" done="true" duration="3">
<summary>Thought for 3 seconds</summary>
> The user is asking for a comprehensive multi-tenant architecture implementation for an Angular 21 + .NET 9 project using Clean Architecture and CQRS. They want:
> 
> 1. Professional/Enterprise-grade multi-tenancy
> 2. High performance
> 3. 100% security compliance
> 4. Optional multi-database support (separate database per tenant)
> 5. Tenant ID should come from session, not header
> 6. Complete database schema
> 7. Tenant registration/management
> 8. SignalR for real-time progress updates during tenant setup
> 9. Everything comprehensive and detailed
> 
> Let me create a complete, production-ready implementation.
> 
> ## Architecture Overview:
> 
> ### Database Schema:
> 1. Master/Host database for tenant management
> 2. Per-tenant databases (optional)
> 
> ### Key Components:
> 1. Tenant entity with all necessary fields
> 2. Tenant database configuration
> 3. Tenant resolution middleware
> 4. CQRS commands/queries for tenant management
> 5. SignalR hub for real-time progress
> 6. Security measures
> 
> Let me write this comprehensively in Persian since the user wrote in Persian.
> 
> I'll structure this as:
> 1. Database schema (SQL scripts)
> 2. Domain layer (entities, interfaces)
> 3. Application layer (CQRS, services)
> 4. Infrastructure layer (repositories, multi-tenant DbContext)
> 5. API layer (controllers, middleware, SignalR)
> 6. Angular frontend
> 
> This is going to be a very long and detailed response. Let me be thorough.
</details>
# پیاده‌سازی Multi-Tenant حرفه‌ای با Angular 21 + .NET 9

## 📋 فهرست مطالب
1. [معماری کلی](#معماری-کلی)
2. [اسکیمای دیتابیس](#اسکیمای-دیتابیس)
3. [Domain Layer](#domain-layer)
4. [Application Layer - CQRS](#application-layer---cqrs)
5. [Infrastructure Layer](#infrastructure-layer)
6. [API Layer](#api-layer)
7. [SignalR Hub](#signalr-hub)
8. [Middleware امنیتی](#middleware-امنیتی)
9. [Angular Frontend](#angular-frontend)

---

## 🏗️ معماری کلی

```
┌─────────────────────────────────────────────────────────────────┐
│                        API Gateway                               │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────────┐  │
│  │ Tenant      │  │ Auth        │  │ Main Application        │  │
│  │ Middleware  │  │ Middleware  │  │ Controllers             │  │
│  └─────────────┘  └─────────────┘  └─────────────────────────┘  │
├─────────────────────────────────────────────────────────────────┤
│                    Application Layer (CQRS)                      │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────┐   │
│  │ Tenant       │  │ Tenant       │  │ Tenant Provisioning  │   │
│  │ Commands     │  │ Queries      │  │ Service              │   │
│  └──────────────┘  └──────────────┘  └──────────────────────┘   │
├─────────────────────────────────────────────────────────────────┤
│                      Infrastructure Layer                        │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────┐   │
│  │ Master Db    │  │ Tenant Db    │  │ SignalR              │   │
│  │ Context      │  │ Factory      │  │ Notification         │   │
│  └──────────────┘  └──────────────┘  └──────────────────────┘   │
├─────────────────────────────────────────────────────────────────┤
│  ┌──────────────────────────────────────────────────────────┐   │
│  │              Master Database (Host)                       │   │
│  │  - Tenants, Users, Roles, Permissions, AuditLogs         │   │
│  └──────────────────────────────────────────────────────────┘   │
│  ┌────────────┐ ┌────────────┐ ┌────────────┐ ┌────────────┐   │
│  │ Tenant 1   │ │ Tenant 2   │ │ Tenant 3   │ │ Tenant N   │   │
│  │ Database   │ │ Database   │ │ Database   │ │ Database   │   │
│  └────────────┘ └────────────┘ └────────────┘ └────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🗄️ اسکیمای دیتابیس

### دیتابیس Master (Host)

```sql
-- ============================================
-- MASTER DATABASE SCHEMA
-- ============================================

-- 1. Tenant Table
CREATE TABLE [Tenants] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Name] NVARCHAR(200) NOT NULL,
    [Slug] NVARCHAR(100) NOT NULL UNIQUE,
    [LegalName] NVARCHAR(300) NULL,
    [RegistrationNumber] NVARCHAR(50) NULL,
    [TaxId] NVARCHAR(50) NULL,
    
    -- Contact Information
    [Email] NVARCHAR(256) NOT NULL,
    [Phone] NVARCHAR(20) NULL,
    [Mobile] NVARCHAR(20) NULL,
    [Website] NVARCHAR(200) NULL,
    
    -- Address
    [Country] NVARCHAR(100) NOT NULL DEFAULT N'Iran',
    [City] NVARCHAR(100) NOT NULL,
    [Address] NVARCHAR(500) NULL,
    [PostalCode] NVARCHAR(20) NULL,
    [Latitude] DECIMAL(10, 8) NULL,
    [Longitude] DECIMAL(11, 8) NULL,
    
    -- Database Configuration
    [DatabaseStrategy] INT NOT NULL DEFAULT 1, -- 1=Shared, 2=Isolated
    [ConnectionString] NVARCHAR(MAX) NULL, -- Encrypted for isolated DBs
    [DatabaseName] NVARCHAR(128) NULL,
    [ServerName] NVARCHAR(256) NULL,
    
    -- Branding
    [LogoUrl] NVARCHAR(500) NULL,
    [PrimaryColor] NVARCHAR(7) NULL DEFAULT N'#1976D2',
    [SecondaryColor] NVARCHAR(7) NULL DEFAULT N'#424242',
    [FaviconUrl] NVARCHAR(500) NULL,
    
    -- Subscription & Limits
    [SubscriptionPlan] NVARCHAR(50) NOT NULL DEFAULT N'Basic',
    [MaxUsers] INT NOT NULL DEFAULT 10,
    [MaxStorageMB] BIGINT NOT NULL DEFAULT 1024,
    [MaxApiCallsPerMonth] BIGINT NULL,
    [CurrentStorageMB] BIGINT NOT NULL DEFAULT 0,
    
    -- Status & Settings
    [Status] INT NOT NULL DEFAULT 1, -- 1=Pending, 2=Active, 3=Suspended, 4=Cancelled
    [IsTrial] BIT NOT NULL DEFAULT 1,
    [TrialEndsAt] DATETIME2 NULL,
    [SubscriptionStartsAt] DATETIME2 NULL,
    [SubscriptionEndsAt] DATETIME2 NULL,
    
    -- Provisioning Status
    [ProvisioningStatus] INT NOT NULL DEFAULT 0, -- 0=NotStarted, 1=Creating, 2=Ready, 3=Failed
    [ProvisioningSteps] NVARCHAR(MAX) NULL, -- JSON array of completed steps
    [ProvisioningError] NVARCHAR(MAX) NULL,
    
    -- Security
    [AllowedIpAddresses] NVARCHAR(MAX) NULL, -- JSON array
    [Require2FA] BIT NOT NULL DEFAULT 0,
    [SessionTimeoutMinutes] INT NOT NULL DEFAULT 30,
    [PasswordMinLength] INT NOT NULL DEFAULT 8,
    [PasswordRequireSpecialChar] BIT NOT NULL DEFAULT 1,
    
    -- Feature Flags (JSON)
    [FeatureFlags] NVARCHAR(MAX) NULL,
    
    -- Audit
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NULL,
    [ModifiedAt] DATETIME2 NULL,
    [ModifiedBy] UNIQUEIDENTIFIER NULL,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] UNIQUEIDENTIFIER NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    
    -- Row Version for Concurrency
    [RowVersion] ROWVERSION NOT NULL,
    
    CONSTRAINT [AK_Tenants_Slug] UNIQUE ([Slug]),
    CONSTRAINT [AK_Tenants_Email] UNIQUE ([Email])
);

-- 2. Tenant Users (Links users to tenants with roles)
CREATE TABLE [TenantUsers] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [RoleId] UNIQUEIDENTIFIER NOT NULL,
    
    -- User-specific settings for this tenant
    [DisplayName] NVARCHAR(200) NULL,
    [Department] NVARCHAR(100) NULL,
    [JobTitle] NVARCHAR(100) NULL,
    [EmployeeCode] NVARCHAR(50) NULL,
    
    -- Permissions override (JSON) - can override role permissions
    [CustomPermissions] NVARCHAR(MAX) NULL,
    
    -- Status
    [Status] INT NOT NULL DEFAULT 1, -- 1=Active, 2=Inactive, 3=PendingApproval
    [IsOwner] BIT NOT NULL DEFAULT 0,
    [InvitationStatus] INT NOT NULL DEFAULT 1, -- 1=Pending, 2=Accepted, 3=Declined
    [InvitationToken] NVARCHAR(256) NULL,
    [InvitationExpiresAt] DATETIME2 NULL,
    
    -- Activity
    [LastActivityAt] DATETIME2 NULL,
    [LastLoginAt] DATETIME2 NULL,
    [LoginCount] INT NOT NULL DEFAULT 0,
    
    -- Audit
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NULL,
    [ModifiedAt] DATETIME2 NULL,
    [ModifiedBy] UNIQUEIDENTIFIER NULL,
    
    CONSTRAINT [FK_TenantUsers_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id]),
    CONSTRAINT [FK_TenantUsers_User] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_TenantUsers_Role] FOREIGN KEY ([RoleId]) REFERENCES [Roles]([Id]),
    CONSTRAINT [AK_TenantUsers_TenantUser] UNIQUE ([TenantId], [UserId])
);

-- 3. Users (Global)
CREATE TABLE [Users] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Email] NVARCHAR(256) NOT NULL,
    [PasswordHash] NVARCHAR(500) NOT NULL,
    [PasswordSalt] NVARCHAR(500) NOT NULL,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [NationalCode] NVARCHAR(10) NULL,
    [Mobile] NVARCHAR(20) NULL,
    [AvatarUrl] NVARCHAR(500) NULL,
    
    -- Security
    [IsEmailConfirmed] BIT NOT NULL DEFAULT 0,
    [IsMobileConfirmed] BIT NOT NULL DEFAULT 0,
    [EmailConfirmationToken] NVARCHAR(256) NULL,
    [EmailConfirmationTokenExpiresAt] DATETIME2 NULL,
    [PasswordResetToken] NVARCHAR(256) NULL,
    [PasswordResetTokenExpiresAt] DATETIME2 NULL,
    [TwoFactorEnabled] BIT NOT NULL DEFAULT 0,
    [TwoFactorSecret] NVARCHAR(256) NULL,
    [TwoFactorRecoveryCodes] NVARCHAR(MAX) NULL,
    
    -- Lockout
    [LockoutEnd] DATETIME2 NULL,
    [FailedLoginAttempts] INT NOT NULL DEFAULT 0,
    
    -- Status
    [Status] INT NOT NULL DEFAULT 1, -- 1=Active, 2=Inactive, 3=Suspended, 4=Deleted
    [UserType] INT NOT NULL DEFAULT 1, -- 1=Regular, 2=Admin, 3=SuperAdmin
    
    -- Audit
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NULL,
    [ModifiedAt] DATETIME2 NULL,
    [ModifiedBy] UNIQUEIDENTIFIER NULL,
    [LastLoginAt] DATETIME2 NULL,
    
    CONSTRAINT [AK_Users_Email] UNIQUE ([Email]),
    CONSTRAINT [AK_Users_NationalCode] UNIQUE ([NationalCode])
);

-- 4. Roles
CREATE TABLE [Roles] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Name] NVARCHAR(100) NOT NULL,
    [DisplayName] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [IsSystemRole] BIT NOT NULL DEFAULT 0,
    [TenantId] UNIQUEIDENTIFIER NULL, -- NULL = Global role
    [Permissions] NVARCHAR(MAX) NOT NULL, -- JSON array of permission codes
    [Priority] INT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedAt] DATETIME2 NULL,
    CONSTRAINT [AK_Roles_Name_Tenant] UNIQUE ([Name], [TenantId])
);

-- 5. Permissions
CREATE TABLE [Permissions] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Code] NVARCHAR(100) NOT NULL UNIQUE,
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [Category] NVARCHAR(100) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1
);

-- 6. Tenant Settings
CREATE TABLE [TenantSettings] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [Category] NVARCHAR(100) NOT NULL,
    [Key] NVARCHAR(100) NOT NULL,
    [Value] NVARCHAR(MAX) NULL,
    [ValueType] INT NOT NULL DEFAULT 1, -- 1=String, 2=Number, 3=Boolean, 4=JSON
    [IsEncrypted] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedAt] DATETIME2 NULL,
    CONSTRAINT [FK_TenantSettings_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id]),
    CONSTRAINT [AK_TenantSettings_Key] UNIQUE ([TenantId], [Category], [Key])
);

-- 7. Audit Logs
CREATE TABLE [AuditLogs] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NULL,
    [UserId] UNIQUEIDENTIFIER NULL,
    [Action] NVARCHAR(100) NOT NULL,
    [EntityType] NVARCHAR(100) NOT NULL,
    [EntityId] NVARCHAR(100) NULL,
    [OldValues] NVARCHAR(MAX) NULL,
    [NewValues] NVARCHAR(MAX) NULL,
    [IpAddress] NVARCHAR(45) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [CorrelationId] UNIQUEIDENTIFIER NULL,
    [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [DurationMs] INT NULL
);

-- 8. Tenant Database Connections (for isolated DBs)
CREATE TABLE [TenantDatabaseConnections] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [DatabaseType] INT NOT NULL DEFAULT 1, -- 1=SQLServer, 2=PostgreSQL, 3=MySQL
    [ServerName] NVARCHAR(256) NOT NULL,
    [DatabaseName] NVARCHAR(128) NOT NULL,
    [Username] NVARCHAR(100) NOT NULL,
    [EncryptedPassword] NVARCHAR(500) NOT NULL,
    [ConnectionString] NVARCHAR(MAX) NOT NULL,
    [IsMaster] BIT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [LastHealthCheck] DATETIME2 NULL,
    [HealthStatus] INT NOT NULL DEFAULT 1, -- 1=Healthy, 2=Degraded, 3=Unhealthy
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_TenantDatabaseConnections_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id])
);

-- 9. Provisioning Logs
CREATE TABLE [ProvisioningLogs] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWIDID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [Step] NVARCHAR(100) NOT NULL,
    [Status] INT NOT NULL, -- 1=Started, 2=Completed, 3=Failed
    [Message] NVARCHAR(MAX) NULL,
    [ErrorDetails] NVARCHAR(MAX) NULL,
    [StartedAt] DATETIME2 NOT NULL,
    [CompletedAt] DATETIME2 NULL,
    [DurationMs] INT NULL,
    [Metadata] NVARCHAR(MAX) NULL
);

-- 10. Tenant Invitations
CREATE TABLE [TenantInvitations] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [Email] NVARCHAR(256) NOT NULL,
    [RoleId] UNIQUEIDENTIFIER NOT NULL,
    [Token] NVARCHAR(256) NOT NULL UNIQUE,
    [InvitedBy] UNIQUEIDENTIFIER NOT NULL,
    [Status] INT NOT NULL DEFAULT 1, -- 1=Pending, 2=Accepted, 3=Declined, 4=Expired
    [ExpiresAt] DATETIME2 NOT NULL,
    [AcceptedAt] DATETIME2 NULL,
    [DeclinedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_TenantInvitations_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id])
);

-- Indexes
CREATE INDEX [IX_Tenants_Slug] ON [Tenants]([Slug]);
CREATE INDEX [IX_Tenants_Status] ON [Tenants]([Status]);
CREATE INDEX [IX_TenantUsers_TenantId] ON [TenantUsers]([TenantId]);
CREATE INDEX [IX_TenantUsers_UserId] ON [TenantUsers]([UserId]);
CREATE INDEX [IX_AuditLogs_TenantId] ON [AuditLogs]([TenantId]);
CREATE INDEX [IX_AuditLogs_UserId] ON [AuditLogs]([UserId]);
CREATE INDEX [IX_AuditLogs_Timestamp] ON [AuditLogs]([Timestamp]);
CREATE INDEX [IX_ProvisioningLogs_TenantId] ON [ProvisioningLogs]([TenantId]);
```

### دیتابیس Tenant (هر بهره‌بردار یک دیتابیس جدا)

```sql
-- ============================================
-- TENANT DATABASE SCHEMA (Per-Tenant)
-- ============================================

-- BaseEntity for all tenant tables
-- Every tenant entity should have:
-- [CreatedAt], [CreatedBy], [ModifiedAt], [ModifiedBy], [DeletedAt], [DeletedBy], [IsDeleted]

-- Example: Products
CREATE TABLE [Products] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Name] NVARCHAR(300) NOT NULL,
    [Sku] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [CategoryId] UNIQUEIDENTIFIER NULL,
    [Price] DECIMAL(18, 2) NOT NULL,
    [Cost] DECIMAL(18, 2) NULL,
    [Currency] NVARCHAR(3) NOT NULL DEFAULT N'IRR',
    [Stock] INT NOT NULL DEFAULT 0,
    [MinStock] INT NOT NULL DEFAULT 0,
    [Images] NVARCHAR(MAX) NULL, -- JSON array
    [IsActive] BIT NOT NULL DEFAULT 1,
    [Metadata] NVARCHAR(MAX) NULL, -- JSON for custom fields
    
    -- Audit
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [ModifiedAt] DATETIME2 NULL,
    [ModifiedBy] UNIQUEIDENTIFIER NULL,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] UNIQUEIDENTIFIER NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [RowVersion] ROWVERSION NOT NULL,
    
    CONSTRAINT [AK_Products_Sku] UNIQUE ([Sku])
);

-- Add TenantId to all tables (for future shared DB migration)
ALTER TABLE [Products] ADD [TenantId] UNIQUEIDENTIFIER NOT NULL DEFAULT @CurrentTenantId;
CREATE INDEX [IX_Products_TenantId] ON [Products]([TenantId]);
```

---

## 📁 Domain Layer

### ساختار پوشه‌ها

```
src/
├── Domain/
│   ├── Entities/
│   │   ├── Tenant.cs
│   │   ├── TenantUser.cs
│   │   ├── User.cs
│   │   ├── Role.cs
│   │   ├── Permission.cs
│   │   └── TenantSetting.cs
│   ├── Enums/
│   │   ├── TenantStatus.cs
│   │   ├── DatabaseStrategy.cs
│   │   ├── ProvisioningStatus.cs
│   │   └── TenantUserStatus.cs
│   ├── Interfaces/
│   │   ├── ITenant.cs
│   │   ├── ITenantRepository.cs
│   │   ├── ITenantDbContextFactory.cs
│   │   └── ITenantResolver.cs
│   └── Events/
│       ├── TenantCreatedEvent.cs
│       └── TenantProvisioningCompletedEvent.cs
```

### Entities

```csharp
// Domain/Entities/Tenant.cs
namespace Domain.Entities;

public class Tenant : BaseEntity, ITenant
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? TaxId { get; set; }
    
    // Contact
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Website { get; set; }
    
    // Address
    public string Country { get; set; } = "Iran";
    public string City { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    
    // Database Configuration
    public DatabaseStrategy DatabaseStrategy { get; set; } = DatabaseStrategy.Shared;
    public string? ConnectionString { get; set; }
    public string? DatabaseName { get; set; }
    public string? ServerName { get; set; }
    
    // Branding
    public string? LogoUrl { get; set; }
    public string PrimaryColor { get; set; } = "#1976D2";
    public string SecondaryColor { get; set; } = "#424242";
    public string? FaviconUrl { get; set; }
    
    // Subscription
    public string SubscriptionPlan { get; set; } = "Basic";
    public int MaxUsers { get; set; } = 10;
    public long MaxStorageMB { get; set; } = 1024;
    public long? MaxApiCallsPerMonth { get; set; }
    public long CurrentStorageMB { get; set; } = 0;
    
    // Status
    public TenantStatus Status { get; set; } = TenantStatus.Pending;
    public bool IsTrial { get; set; } = true;
    public DateTime? TrialEndsAt { get; set; }
    public DateTime? SubscriptionStartsAt { get; set; }
    public DateTime? SubscriptionEndsAt { get; set; }
    
    // Provisioning
    public ProvisioningStatus ProvisioningStatus { get; set; } = ProvisioningStatus.NotStarted;
    public List<ProvisioningStep>? ProvisioningSteps { get; set; }
    public string? ProvisioningError { get; set; }
    
    // Security
    public List<string>? AllowedIpAddresses { get; set; }
    public bool Require2FA { get; set; } = false;
    public int SessionTimeoutMinutes { get; set; } = 30;
    public int PasswordMinLength { get; set; } = 8;
    public bool PasswordRequireSpecialChar { get; set; } = true;
    
    // Feature Flags
    public Dictionary<string, bool>? FeatureFlags { get; set; }
    
    // Navigation
    public virtual ICollection<TenantUser> TenantUsers { get; set; } = new List<TenantUser>();
    public virtual ICollection<TenantSetting> Settings { get; set; } = new List<TenantSetting>();
    public virtual ICollection<TenantDatabaseConnection> DatabaseConnections { get; set; } = new List<TenantDatabaseConnection>();
    
    // Computed
    public bool IsActive => Status == TenantStatus.Active;
    public bool IsSuspended => Status == TenantStatus.Suspended;
    public bool IsTrialExpired => IsTrial && TrialEndsAt.HasValue && TrialEndsAt.Value < DateTime.UtcNow;
}

// Domain/Entities/TenantUser.cs
public class TenantUser : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    
    public string? DisplayName { get; set; }
    public string? Department { get; set; }
    public string? JobTitle { get; set; }
    public string? EmployeeCode { get; set; }
    
    public Dictionary<string, bool>? CustomPermissions { get; set; }
    
    public TenantUserStatus Status { get; set; } = TenantUserStatus.Active;
    public bool IsOwner { get; set; } = false;
    public InvitationStatus InvitationStatus { get; set; } = InvitationStatus.Pending;
    public string? InvitationToken { get; set; }
    public DateTime? InvitationExpiresAt { get; set; }
    
    public DateTime? LastActivityAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int LoginCount { get; set; } = 0;
    
    // Navigation
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
    
    // Computed
    public string FullName => User?.FullName ?? DisplayName ?? "Unknown";
    public bool IsInvitationExpired => InvitationExpiresAt.HasValue && InvitationExpiresAt.Value < DateTime.UtcNow;
}

// Domain/Entities/User.cs
public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? NationalCode { get; set; }
    public string? Mobile { get; set; }
    public string? AvatarUrl { get; set; }
    
    public bool IsEmailConfirmed { get; set; } = false;
    public bool IsMobileConfirmed { get; set; } = false;
    public string? EmailConfirmationToken { get; set; }
    public DateTime? EmailConfirmationTokenExpiresAt { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiresAt { get; set; }
    
    public bool TwoFactorEnabled { get; set; } = false;
    public string? TwoFactorSecret { get; set; }
    public List<string>? TwoFactorRecoveryCodes { get; set; }
    
    public DateTime? LockoutEnd { get; set; }
    public int FailedLoginAttempts { get; set; } = 0;
    
    public UserStatus Status { get; set; } = UserStatus.Active;
    public UserType UserType { get; set; } = UserType.Regular;
    
    // Navigation
    public virtual ICollection<TenantUser> TenantUsers { get; set; } = new List<TenantUser>();
    
    // Computed
    public string FullName => $"{FirstName} {LastName}";
    public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;
}

// Domain/Entities/BaseEntity.cs
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public byte[]? RowVersion { get; set; }
}
```

### Enums

```csharp
// Domain/Enums/TenantStatus.cs
public enum TenantStatus
{
    Pending = 1,
    Active = 2,
    Suspended = 3,
    Cancelled = 4
}

// Domain/Enums/DatabaseStrategy.cs
public enum DatabaseStrategy
{
    Shared = 1,      // All tenants in one database
    Isolated = 2     // Each tenant has its own database
}

// Domain/Enums/ProvisioningStatus.cs
public enum ProvisioningStatus
{
    NotStarted = 0,
    Creating = 1,
    Ready = 2,
    Failed = 3
}

// Domain/Enums/ProvisioningStep.cs
public class ProvisioningStep
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProvisioningStepStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public enum ProvisioningStepStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Failed = 3,
    Skipped = 4
}

// Domain/Enums/TenantUserStatus.cs
public enum TenantUserStatus
{
    Active = 1,
    Inactive = 2,
    PendingApproval = 3
}

public enum InvitationStatus
{
    Pending = 1,
    Accepted = 2,
    Declined = 3,
    Expired = 4
}
```

### Interfaces

```csharp
// Domain/Interfaces/ITenant.cs
public interface ITenant
{
    Guid Id { get; }
    string Name { get; }
    string Slug { get; }
    TenantStatus Status { get; }
    DatabaseStrategy DatabaseStrategy { get; }
    string? ConnectionString { get; }
    bool Require2FA { get; }
    int SessionTimeoutMinutes { get; }
    Dictionary<string, bool>? FeatureFlags { get; }
    bool IsActive { get; }
}

// Domain/Interfaces/ITenantResolver.cs
public interface ITenantResolver
{
    Task<ITenant?> ResolveAsync(HttpContext context);
    Task<ITenant?> ResolveByIdAsync(Guid tenantId);
    Task<ITenant?> ResolveBySlugAsync(string slug);
}

// Domain/Interfaces/ITenantRepository.cs
public interface ITenantRepository
{
    Task<ITenant?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ITenant?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<ITenant?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<IEnumerable<ITenant>> GetAllAsync(TenantFilter filter, CancellationToken ct = default);
    Task<ITenant> CreateAsync(Tenant tenant, CancellationToken ct = default);
    Task<ITenant> UpdateAsync(Tenant tenant, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsAsync(string slug, CancellationToken ct = default);
    Task<int> GetCountAsync(TenantFilter? filter = null, CancellationToken ct = default);
}

// Domain/Interfaces/ITenantDbContextFactory.cs
public interface ITenantDbContextFactory
{
    TenantDbContext CreateDbContext(Guid tenantId);
    TenantDbContext CreateDbContext(string connectionString);
    Task<bool> TestConnectionAsync(string connectionString, CancellationToken ct = default);
}

// Domain/Interfaces/ICurrentTenant.cs
public interface ICurrentTenant
{
    Guid? TenantId { get; }
    ITenant? Tenant { get; }
    Guid? UserId { get; }
    bool IsAuthenticated { get; }
    void SetTenant(ITenant tenant);
    void ClearTenant();
}
```

---

## 📝 Application Layer - CQRS

### ساختار پوشه‌ها

```
src/
├── Application/
│   ├── Features/
│   │   ├── Tenants/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateTenant/
│   │   │   │   │   ├── CreateTenantCommand.cs
│   │   │   │   │   ├── CreateTenantCommandValidator.cs
│   │   │   │   │   └── CreateTenantCommandHandler.cs
│   │   │   │   ├── UpdateTenant/
│   │   │   │   ├── SuspendTenant/
│   │   │   │   └── DeleteTenant/
│   │   │   └── Queries/
│   │   │       ├── GetTenantById/
│   │   │       ├── GetTenantBySlug/
│   │   │       └── GetTenantsList/
│   │   ├── TenantUsers/
│   │   │   ├── Commands/
│   │   │   └── Queries/
│   │   └── Auth/
│   │       ├── Commands/
│   │       └── Queries/
│   ├── Services/
│   │   ├── TenantProvisioningService.cs
│   │   ├── TenantDatabaseService.cs
│   │   └── TenantNotificationService.cs
│   ├── Interfaces/
│   │   ├── ITenantProvisioningService.cs
│   │   └── ITenantNotificationService.cs
│   └── Common/
│       ├── Behaviors/
│       │   ├── TenantValidationBehavior.cs
│       │   └── TenantAuthorizationBehavior.cs
│       └── Extensions/
```

### Commands

```csharp
// Application/Features/Tenants/Commands/CreateTenant/CreateTenantCommand.cs
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Tenants.Commands.CreateTenant;

public class CreateTenantCommand : IRequest<CreateTenantResponse>
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 2)]
    [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", 
        ErrorMessage = "Slug must be lowercase alphanumeric with hyphens")]
    public string Slug { get; set; } = string.Empty;
    
    [StringLength(300)]
    public string? LegalName { get; set; }
    
    [StringLength(50)]
    public string? RegistrationNumber { get; set; }
    
    [StringLength(50)]
    public string? TaxId { get; set; }
    
    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;
    
    [Phone]
    public string? Phone { get; set; }
    
    [Phone]
    public string? Mobile { get; set; }
    
    [StringLength(200)]
    public string? Website { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Country { get; set; } = "Iran";
    
    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Address { get; set; }
    
    [StringLength(20)]
    public string? PostalCode { get; set; }
    
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    
    public DatabaseStrategy DatabaseStrategy { get; set; } = DatabaseStrategy.Shared;
    
    // Admin User Data
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string AdminFirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string AdminLastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string AdminEmail { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string AdminPassword { get; set; } = string.Empty;
    
    [Phone]
    public string? AdminMobile { get; set; }
    
    [StringLength(10)]
    public string? AdminNationalCode { get; set; }
    
    // Subscription
    public string SubscriptionPlan { get; set; } = "Basic";
    public int MaxUsers { get; set; } = 10;
    public long MaxStorageMB { get; set; } = 1024;
    public bool IsTrial { get; set; } = true;
    public int TrialDays { get; set; } = 14;
    
    // Branding
    public string? LogoUrl { get; set; }
    public string PrimaryColor { get; set; } = "#1976D2";
    public string SecondaryColor { get; set; } = "#424242";
    
    // Security Settings
    public bool Require2FA { get; set; } = false;
    public int SessionTimeoutMinutes { get; set; } = 30;
    public int PasswordMinLength { get; set; } = 8;
    public bool PasswordRequireSpecialChar { get; set; } = true;
    public List<string>? AllowedIpAddresses { get; set; }
    
    // Feature Flags
    public Dictionary<string, bool>? FeatureFlags { get; set; }
}

public class CreateTenantResponse
{
    public Guid TenantId { get; set; }
    public Guid AdminUserId { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public ProvisioningStatus ProvisioningStatus { get; set; }
    public DateTime? TrialEndsAt { get; set; }
}
```

```csharp
// Application/Features/Tenants/Commands/CreateTenant/CreateTenantCommandValidator.cs
using FluentValidation;
using Application.Common.Validators;

namespace Application.Features.Tenants.Commands.CreateTenant;

public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    private readonly ITenantUniquenessChecker _tenantUniqueness;

    public CreateTenantCommandValidator(ITenantUniquenessChecker tenantUniqueness)
    {
        _tenantUniqueness = tenantUniqueness;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام بهره‌بردار الزامی است")
            .MaximumLength(200)
            .MinimumLength(2);

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug الزامی است")
            .MaximumLength(100)
            .MinimumLength(2)
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug باید فقط شامل حروف کوچک انگلیسی، اعداد و خط تیره باشد")
            .MustAsync(BeUniqueSlugAsync)
            .WithMessage("این Slug قبلاً استفاده شده است");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("ایمیل الزامی است")
            .EmailAddress().WithMessage("فرمت ایمیل صحیح نیست")
            .MaximumLength(256)
            .MustAsync(BeUniqueEmailAsync)
            .WithMessage("این ایمیل قبلاً ثبت شده است");

        RuleFor(x => x.AdminEmail)
            .NotEmpty().WithMessage("ایمیل مدیر الزامی است")
            .EmailAddress().WithMessage("فرمت ایمیل مدیر صحیح نیست")
            .MustAsync(BeUniqueAdminEmailAsync)
            .WithMessage("این ایمیل قبلاً ثبت شده است");

        RuleFor(x => x.AdminPassword)
            .NotEmpty().WithMessage("رمز عبور الزامی است")
            .MinimumLength(8).WithMessage("رمز عبور باید حداقل ۸ کاراکتر باشد")
            .Must(PasswordMustHaveSpecialChar)
            .WithMessage("رمز عبور باید شامل حداقل یک کاراکتر خاص باشد");

        RuleFor(x => x.AdminNationalCode)
            .Must(BeValidNationalCode)
            .When(x => !string.IsNullOrEmpty(x.AdminNationalCode))
            .WithMessage("کد ملی صحیح نیست");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("شهر الزامی است");

        RuleFor(x => x.MaxUsers)
            .GreaterThan(0).WithMessage("تعداد کاربران باید بیشتر از صفر باشد")
            .LessThanOrEqualTo(10000).WithMessage("تعداد کاربران نمی‌تواند بیشتر از ۱۰۰۰۰ باشد");

        RuleFor(x => x.TrialDays)
            .InclusiveBetween(0, 90)
            .WithMessage("مدت آزمایشی باید بین ۰ تا ۹۰ روز باشد");
    }

    private async Task<bool> BeUniqueSlugAsync(string slug, CancellationToken ct)
    {
        return !await _tenantUniqueness.ExistsBySlugAsync(slug, ct);
    }

    private async Task<bool> BeUniqueEmailAsync(string email, CancellationToken ct)
    {
        return !await _tenantUniqueness.ExistsByEmailAsync(email, ct);
    }

    private async Task<bool> BeUniqueAdminEmailAsync(string email, CancellationToken ct)
    {
        return !await _tenantUniqueness.ExistsByUserEmailAsync(email, ct);
    }

    private bool PasswordMustHaveSpecialChar(string password)
    {
        if (string.IsNullOrEmpty(password)) return false;
        return password.Any(c => !char.IsLetterOrDigit(c));
    }

    private bool BeValidNationalCode(string? nationalCode)
    {
        if (string.IsNullOrEmpty(nationalCode)) return true;
        if (nationalCode.Length != 10) return false;
        if (!nationalCode.All(char.IsDigit)) return false;
        
        // Iranian National Code validation algorithm
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += int.Parse(nationalCode[i].ToString()) * (10 - i);
        }
        int remainder = sum % 11;
        int checkDigit = int.Parse(nationalCode[9].ToString());
        
        return (remainder < 2 && checkDigit == remainder) || 
               (remainder >= 2 && checkDigit == 11 - remainder);
    }
}
```

```csharp
// Application/Features/Tenants/Commands/CreateTenant/CreateTenantCommandHandler.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Security;
using Domain.Entities;
using Domain.Enums;

namespace Application.Features.Tenants.Commands.CreateTenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, CreateTenantResponse>
{
    private readonly IMasterDbContext _masterDb;
    private readonly ITenantProvisioningService _provisioningService;
    private readonly ICurrentUserService _currentUser;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITenantNotificationService _notificationService;
    private readonly ILogger<CreateTenantCommandHandler> _logger;
    private readonly TenantSettings _settings;

    public CreateTenantCommandHandler(
        IMasterDbContext masterDb,
        ITenantProvisioningService provisioningService,
        ICurrentUserService currentUser,
        IPasswordHasher passwordHasher,
        ITenantNotificationService notificationService,
        ILogger<CreateTenantCommandHandler> logger,
        IOptions<TenantSettings> settings)
    {
        _masterDb = masterDb;
        _provisioningService = provisioningService;
        _currentUser = currentUser;
        _passwordHasher = passwordHasher;
        _notificationService = notificationService;
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task<CreateTenantResponse> Handle(CreateTenantCommand request, CancellationToken ct)
    {
        // Start provisioning tracking
        var provisioningSteps = GetInitialProvisioningSteps();
        
        try
        {
            // Step 1: Create Tenant Entity
            await _notificationService.SendProgressAsync(request.Slug, "Creating tenant entity...", 1, provisioningSteps);
            
            var tenant = new Tenant
            {
                Name = request.Name,
                Slug = request.Slug.ToLowerInvariant(),
                LegalName = request.LegalName,
                RegistrationNumber = request.RegistrationNumber,
                TaxId = request.TaxId,
                Email = request.Email.ToLowerInvariant(),
                Phone = request.Phone,
                Mobile = request.Mobile,
                Website =

### USER
ادامه بده

### ASSISTANT
<details type="reasoning" done="true" duration="0">
<summary>Thought for 0 seconds</summary>
> The user wants me to continue from where I left off. I was in the middle of the CreateTenantCommandHandler. Let me continue with the complete implementation.
</details>
# ادامه پیاده‌سازی Multi-Tenant

## Application Layer - CQRS (ادامه)

```csharp
// Application/Features/Tenants/Commands/CreateTenant/CreateTenantCommandHandler.cs
public async Task<CreateTenantResponse> Handle(CreateTenantCommand request, CancellationToken ct)
{
    var provisioningSteps = GetInitialProvisioningSteps();
    
    try
    {
        // Step 1: Create Tenant Entity
        await _notificationService.SendProgressAsync(
            request.Slug, "Creating tenant entity...", 1, provisioningSteps);
        
        var tenant = new Tenant
        {
            Name = request.Name,
            Slug = request.Slug.ToLowerInvariant(),
            LegalName = request.LegalName,
            RegistrationNumber = request.RegistrationNumber,
            TaxId = request.TaxId,
            Email = request.Email.ToLowerInvariant(),
            Phone = request.Phone,
            Mobile = request.Mobile,
            Website = request.Website,
            Country = request.Country,
            City = request.City,
            Address = request.Address,
            PostalCode = request.PostalCode,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            DatabaseStrategy = request.DatabaseStrategy,
            LogoUrl = request.LogoUrl,
            PrimaryColor = request.PrimaryColor,
            SecondaryColor = request.SecondaryColor,
            SubscriptionPlan = request.SubscriptionPlan,
            MaxUsers = request.MaxUsers,
            MaxStorageMB = request.MaxStorageMB,
            IsTrial = request.IsTrial,
            TrialEndsAt = request.IsTrial ? DateTime.UtcNow.AddDays(request.TrialDays) : null,
            Status = TenantStatus.Pending,
            ProvisioningStatus = ProvisioningStatus.Creating,
            Require2FA = request.Require2FA,
            SessionTimeoutMinutes = request.SessionTimeoutMinutes,
            PasswordMinLength = request.PasswordMinLength,
            PasswordRequireSpecialChar = request.PasswordRequireSpecialChar,
            AllowedIpAddresses = request.AllowedIpAddresses,
            FeatureFlags = request.FeatureFlags ?? GetDefaultFeatureFlags(),
            CreatedBy = _currentUser.UserId
        };

        await _masterDb.Tenants.AddAsync(tenant, ct);
        await _masterDb.SaveChangesAsync(ct);

        // Step 2: Create Admin User
        await _notificationService.SendProgressAsync(
            request.Slug, "Creating admin user...", 2, provisioningSteps);

        var (passwordHash, passwordSalt) = _passwordHasher.HashPassword(request.AdminPassword);
        
        var adminUser = new User
        {
            Email = request.AdminEmail.ToLowerInvariant(),
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            FirstName = request.AdminFirstName,
            LastName = request.AdminLastName,
            Mobile = request.AdminMobile,
            NationalCode = request.AdminNationalCode,
            Status = UserStatus.Active,
            UserType = UserType.Admin,
            IsEmailConfirmed = false,
            EmailConfirmationToken = GenerateSecureToken(),
            EmailConfirmationTokenExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedBy = _currentUser.UserId
        };

        await _masterDb.Users.AddAsync(adminUser, ct);
        await _masterDb.SaveChangesAsync(ct);

        // Step 3: Create Default Roles
        await _notificationService.SendProgressAsync(
            request.Slug, "Creating default roles...", 3, provisioningSteps);

        var ownerRole = new Role
        {
            Name = "Owner",
            DisplayName = "مالک",
            Description = "دسترسی کامل به تمام بخش‌ها",
            IsSystemRole = true,
            TenantId = tenant.Id,
            Permissions = GetAllPermissions(),
            Priority = 100,
            IsActive = true
        };

        var adminRole = new Role
        {
            Name = "Admin",
            DisplayName = "مدیر",
            Description = "دسترسی مدیریتی",
            IsSystemRole = true,
            TenantId = tenant.Id,
            Permissions = GetAdminPermissions(),
            Priority = 90,
            IsActive = true
        };

        var userRole = new Role
        {
            Name = "User",
            DisplayName = "کاربر",
            Description = "دسترسی پایه",
            IsSystemRole = true,
            TenantId = tenant.Id,
            Permissions = GetBasicPermissions(),
            Priority = 10,
            IsActive = true
        };

        await _masterDb.Roles.AddRangeAsync(ownerRole, adminRole, userRole, ct);
        await _masterDb.SaveChangesAsync(ct);

        // Step 4: Link Admin to Tenant
        var tenantUser = new TenantUser
        {
            TenantId = tenant.Id,
            UserId = adminUser.Id,
            RoleId = ownerRole.Id,
            DisplayName = $"{adminUser.FirstName} {adminUser.LastName}",
            IsOwner = true,
            Status = TenantUserStatus.Active,
            InvitationStatus = InvitationStatus.Accepted,
            CreatedBy = _currentUser.UserId
        };

        await _masterDb.TenantUsers.AddAsync(tenantUser, ct);
        await _masterDb.SaveChangesAsync(ct);

        // Step 5: Provision Database (if isolated)
        if (request.DatabaseStrategy == DatabaseStrategy.Isolated)
        {
            await _notificationService.SendProgressAsync(
                request.Slug, "Creating isolated database...", 4, provisioningSteps);
            
            await _provisioningService.ProvisionIsolatedDatabaseAsync(tenant, ct);
        }

        // Step 6: Run Migrations
        await _notificationService.SendProgressAsync(
            request.Slug, "Running database migrations...", 5, provisioningSteps);
        
        await _provisioningService.RunMigrationsAsync(tenant, ct);

        // Step 7: Seed Initial Data
        await _notificationService.SendProgressAsync(
            request.Slug, "Seeding initial data...", 6, provisioningSteps);
        
        await _provisioningService.SeedInitialDataAsync(tenant, ct);

        // Step 8: Complete Provisioning
        await _notificationService.SendProgressAsync(
            request.Slug, "Finalizing setup...", 7, provisioningSteps);

        tenant.ProvisioningStatus = ProvisioningStatus.Ready;
        tenant.Status = TenantStatus.Active;
        tenant.ModifiedAt = DateTime.UtcNow;
        tenant.ModifiedBy = _currentUser.UserId;

        await _masterDb.SaveChangesAsync(ct);

        // Send completion notification
        await _notificationService.SendCompletionAsync(
            request.Slug,
            "Tenant created successfully!",
            tenant.Id,
            adminUser.Id);

        _logger.LogInformation(
            "Tenant {TenantId} ({Slug}) created successfully with admin {AdminId}",
            tenant.Id, tenant.Slug, adminUser.Id);

        return new CreateTenantResponse
        {
            TenantId = tenant.Id,
            AdminUserId = adminUser.Id,
            Slug = tenant.Slug,
            Status = tenant.Status.ToString(),
            ProvisioningStatus = tenant.ProvisioningStatus,
            TrialEndsAt = tenant.TrialEndsAt
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating tenant {Slug}", request.Slug);
        
        // Update provisioning status
        var tenant = await _masterDb.Tenants
            .FirstOrDefaultAsync(t => t.Slug == request.Slug, ct);
        
        if (tenant != null)
        {
            tenant.ProvisioningStatus = ProvisioningStatus.Failed;
            tenant.ProvisioningError = ex.Message;
            await _masterDb.SaveChangesAsync(ct);
        }

        await _notificationService.SendErrorAsync(
            request.Slug,
            "Failed to create tenant: " + ex.Message);

        throw;
    }
}

private List<ProvisioningStep> GetInitialProvisioningSteps() => new()
{
    new() { Name = "CreateTenant", Description = "ایجاد موجودیت بهره‌بردار", Status = ProvisioningStepStatus.Pending },
    new() { Name = "CreateAdminUser", Description = "ایجاد کاربر مدیر", Status = ProvisioningStepStatus.Pending },
    new() { Name = "CreateRoles", Description = "ایجاد نقش‌های پیش‌فرض", Status = ProvisioningStepStatus.Pending },
    new() { Name = "ProvisionDatabase", Description = "ایجاد دیتابیس", Status = ProvisioningStepStatus.Pending },
    new() { Name = "RunMigrations", Description = "اجرای مایگریشن‌ها", Status = ProvisioningStepStatus.Pending },
    new() { Name = "SeedData", Description = "بارگذاری داده‌های اولیه", Status = ProvisioningStepStatus.Pending },
    new() { Name = "Finalize", Description = "تکمیل تنظیمات", Status = ProvisioningStepStatus.Pending }
};

private Dictionary<string, bool> GetDefaultFeatureFlags() => new()
{
    ["dashboard"] = true,
    ["reports"] = true,
    ["api_access"] = true,
    ["sso"] = false,
    ["audit_logs"] = true,
    ["file_storage"] = true,
    ["notifications"] = true
};

private string GetAllPermissions() => JsonSerializer.Serialize(new[]
{
    "*" // All permissions
});

private string GetAdminPermissions() => JsonSerializer.Serialize(new[]
{
    "users.view", "users.create", "users.edit", "users.delete",
    "roles.view", "roles.create", "roles.edit",
    "settings.view", "settings.edit",
    "reports.view", "reports.export",
    "audit.view"
});

private string GetBasicPermissions() => JsonSerializer.Serialize(new[]
{
    "dashboard.view",
    "profile.view", "profile.edit"
});

private string GenerateSecureToken()
{
    var bytes = new byte[32];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(bytes);
    return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_");
}
```

### Tenant Provisioning Service

```csharp
// Application/Services/TenantProvisioningService.cs
public interface ITenantProvisioningService
{
    Task ProvisionIsolatedDatabaseAsync(Tenant tenant, CancellationToken ct = default);
    Task RunMigrationsAsync(Tenant tenant, CancellationToken ct = default);
    Task SeedInitialDataAsync(Tenant tenant, CancellationToken ct = default);
    Task<bool> TestDatabaseConnectionAsync(string connectionString, CancellationToken ct = default);
}

public class TenantProvisioningService : ITenantProvisioningService
{
    private readonly MasterDbContext _masterDb;
    private readonly ITenantDatabaseService _databaseService;
    private readonly ITenantNotificationService _notificationService;
    private readonly ILogger<TenantProvisioningService> _logger;
    private readonly IEnumerable<ITenantDataSeeder> _seeders;

    public TenantProvisioningService(
        MasterDbContext masterDb,
        ITenantDatabaseService databaseService,
        ITenantNotificationService notificationService,
        ILogger<TenantProvisioningService> logger,
        IEnumerable<ITenantDataSeeder> seeders)
    {
        _masterDb = masterDb;
        _databaseService = databaseService;
        _notificationService = notificationService;
        _logger = logger;
        _seeders = seeders;
    }

    public async Task ProvisionIsolatedDatabaseAsync(Tenant tenant, CancellationToken ct = default)
    {
        var databaseName = $"Tenant_{tenant.Slug}_{tenant.Id:N}";
        var serverName = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
        var adminUsername = Environment.GetEnvironmentVariable("DB_ADMIN_USER") ?? "sa";
        var adminPassword = Environment.GetEnvironmentVariable("DB_ADMIN_PASSWORD") ?? "";

        // Create database
        await _databaseService.CreateDatabaseAsync(
            serverName, databaseName, adminUsername, adminPassword, ct);

        // Generate connection string
        var connectionString = _databaseService.GenerateConnectionString(
            serverName, databaseName, adminUsername, adminPassword);

        // Save encrypted connection string
        tenant.DatabaseName = databaseName;
        tenant.ServerName = serverName;
        tenant.ConnectionString = _databaseService.EncryptConnectionString(connectionString);
        
        // Save database connection record
        var dbConnection = new TenantDatabaseConnection
        {
            TenantId = tenant.Id,
            DatabaseType = DatabaseType.SqlServer,
            ServerName = serverName,
            DatabaseName = databaseName,
            Username = adminUsername,
            EncryptedPassword = _databaseService.Encrypt(adminPassword),
            ConnectionString = tenant.ConnectionString,
            IsMaster = false,
            IsActive = true,
            HealthStatus = HealthStatus.Healthy
        };

        await _masterDb.TenantDatabaseConnections.AddAsync(dbConnection, ct);
        await _masterDb.SaveChangesAsync(ct);

        _logger.LogInformation("Database {DatabaseName} created for tenant {TenantId}", 
            databaseName, tenant.Id);
    }

    public async Task RunMigrationsAsync(Tenant tenant, CancellationToken ct = default)
    {
        if (tenant.DatabaseStrategy == DatabaseStrategy.Isolated && !string.IsNullOrEmpty(tenant.ConnectionString))
        {
            var connectionString = _databaseService.DecryptConnectionString(tenant.ConnectionString);
            await _databaseService.RunMigrationsAsync(connectionString, ct);
        }
        else
        {
            // For shared database, migrations are handled by EF Core automatically
            await _masterDb.Database.MigrateAsync(ct);
        }
    }

    public async Task SeedInitialDataAsync(Tenant tenant, CancellationToken ct = default)
    {
        foreach (var seeder in _seeders)
        {
            await seeder.SeedAsync(tenant, ct);
        }
    }

    public async Task<bool> TestDatabaseConnectionAsync(string connectionString, CancellationToken ct = default)
    {
        return await _databaseService.TestConnectionAsync(connectionString, ct);
    }
}

// Application/Services/TenantDatabaseService.cs
public interface ITenantDatabaseService
{
    Task CreateDatabaseAsync(string server, string databaseName, string username, 
        string password, CancellationToken ct = default);
    Task<bool> TestConnectionAsync(string connectionString, CancellationToken ct = default);
    string GenerateConnectionString(string server, string database, string username, string password);
    string EncryptConnectionString(string connectionString);
    string DecryptConnectionString(string encryptedConnectionString);
    Task RunMigrationsAsync(string connectionString, CancellationToken ct = default);
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}

public class TenantDatabaseService : ITenantDatabaseService
{
    private readonly ILogger<TenantDatabaseService> _logger;
    private readonly IEncryptionService _encryption;
    private readonly string _encryptionKey;

    public TenantDatabaseService(
        ILogger<TenantDatabaseService> logger,
        IEncryptionService encryption)
    {
        _logger = logger;
        _encryption = encryption;
        _encryptionKey = Environment.GetEnvironmentVariable("CONNECTION_STRING_ENCRYPTION_KEY") 
            ?? throw new InvalidOperationException("Encryption key not configured");
    }

    public async Task CreateDatabaseAsync(string server, string databaseName, 
        string username, string password, CancellationToken ct = default)
    {
        var masterConnectionString = GenerateConnectionString(server, "master", username, password);
        
        await using var connection = new SqlConnection(masterConnectionString);
        await connection.OpenAsync(ct);

        var createDbCommand = $@"
            IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '{databaseName}')
            BEGIN
                CREATE DATABASE [{databaseName}];
            END";
        
        await using var cmd = new SqlCommand(createDbCommand, connection);
        await cmd.ExecuteNonQueryAsync(ct);

        _logger.LogInformation("Database {DatabaseName} created or already exists", databaseName);
    }

    public async Task<bool> TestConnectionAsync(string connectionString, CancellationToken ct = default)
    {
        try
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(ct);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GenerateConnectionString(string server, string database, string username, string password)
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = server,
            InitialCatalog = database,
            UserID = username,
            Password = password,
            TrustServerCertificate = true,
            ConnectTimeout = 30,
            MultipleActiveResultSets = true,
            Encrypt = true
        };
        
        return builder.ConnectionString;
    }

    public string EncryptConnectionString(string connectionString)
    {
        return _encryption.Encrypt(connectionString, _encryptionKey);
    }

    public string DecryptConnectionString(string encryptedConnectionString)
    {
        return _encryption.Decrypt(encryptedConnectionString, _encryptionKey);
    }

    public async Task RunMigrationsAsync(string connectionString, CancellationToken ct = default)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        
        using var context = new TenantDbContext(optionsBuilder.Options);
        await context.Database.MigrateAsync(ct);
    }

    public string Encrypt(string plainText)
    {
        return _encryption.Encrypt(plainText, _encryptionKey);
    }

    public string Decrypt(string cipherText)
    {
        return _encryption.Decrypt(cipherText, _encryptionKey);
    }
}
```

### SignalR Notification Service

```csharp
// Application/Services/TenantNotificationService.cs
public interface ITenantNotificationService
{
    Task SendProgressAsync(string tenantSlug, string message, int step, 
        List<ProvisioningStep> steps, CancellationToken ct = default);
    Task SendCompletionAsync(string tenantSlug, string message, Guid tenantId, 
        Guid adminUserId, CancellationToken ct = default);
    Task SendErrorAsync(string tenantSlug, string errorMessage, CancellationToken ct = default);
}

public class TenantNotificationService : ITenantNotificationService
{
    private readonly IHubContext<ProvisioningHub, IProvisioningClient> _hubContext;
    private readonly ILogger<TenantNotificationService> _logger;

    public TenantNotificationService(
        IHubContext<ProvisioningHub, IProvisioningClient> hubContext,
        ILogger<TenantNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendProgressAsync(string tenantSlug, string message, int step,
        List<ProvisioningStep> steps, CancellationToken ct = default)
    {
        var notification = new ProvisioningNotification
        {
            TenantSlug = tenantSlug,
            Status = ProvisioningNotificationStatus.InProgress,
            CurrentStep = step,
            TotalSteps = steps.Count,
            Message = message,
            Steps = steps,
            Timestamp = DateTime.UtcNow
        };

        await _hubContext.Clients
            .Group($"tenant-{tenantSlug}")
            .OnProgress(notification);

        _logger.LogDebug("Sent progress notification for {TenantSlug}: Step {Step}/{Total}", 
            tenantSlug, step, steps.Count);
    }

    public async Task SendCompletionAsync(string tenantSlug, string message, 
        Guid tenantId, Guid adminUserId, CancellationToken ct = default)
    {
        var notification = new ProvisioningNotification
        {
            TenantSlug = tenantSlug,
            Status = ProvisioningNotificationStatus.Completed,
            Message = message,
            TenantId = tenantId,
            AdminUserId = adminUserId,
            Timestamp = DateTime.UtcNow
        };

        await _hubContext.Clients
            .Group($"tenant-{tenantSlug}")
            .OnCompleted(notification);

        _logger.LogInformation("Sent completion notification for {TenantSlug}", tenantSlug);
    }

    public async Task SendErrorAsync(string tenantSlug, string errorMessage, 
        CancellationToken ct = default)
    {
        var notification = new ProvisioningNotification
        {
            TenantSlug = tenantSlug,
            Status = ProvisioningNotificationStatus.Failed,
            Message = errorMessage,
            Timestamp = DateTime.UtcNow
        };

        await _hubContext.Clients
            .Group($"tenant-{tenantSlug}")
            .OnError(notification);

        _logger.LogError("Sent error notification for {TenantSlug}: {Error}", tenantSlug, errorMessage);
    }
}

// Shared Models for SignalR
public class ProvisioningNotification
{
    public string TenantSlug { get; set; } = string.Empty;
    public ProvisioningNotificationStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public int CurrentStep { get; set; }
    public int TotalSteps { get; set; }
    public List<ProvisioningStep>? Steps { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? AdminUserId { get; set; }
    public DateTime Timestamp { get; set; }
}

public enum ProvisioningNotificationStatus
{
    InProgress = 1,
    Completed = 2,
    Failed = 3
}
```

---

## Infrastructure Layer

### ساختار پوشه‌ها

```
src/
├── Infrastructure/
│   ├── Data/
│   │   ├── MasterDbContext.cs
│   │   ├── TenantDbContext.cs
│   │   ├── Configurations/
│   │   │   ├── TenantConfiguration.cs
│   │   │   ├── UserConfiguration.cs
│   │   │   └── TenantUserConfiguration.cs
│   │   └── Migrations/
│   ├── Identity/
│   │   ├── JwtTokenService.cs
│   │   ├── PasswordHasher.cs
│   │   └── CurrentUserService.cs
│   ├── Security/
│   │   ├── EncryptionService.cs
│   │   └── TenantSecurityService.cs
│   ├── Middleware/
│   │   ├── TenantResolverMiddleware.cs
│   │   ├── TenantSecurityMiddleware.cs
│   │   └── AuditMiddleware.cs
│   └── Services/
│       └── TenantResolver.cs
```

### Master DbContext

```csharp
// Infrastructure/Data/MasterDbContext.cs
public class MasterDbContext : DbContext, IMasterDbContext
{
    private readonly ICurrentTenant _currentTenant;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditableSaveChangesInterceptor _auditableInterceptor;

    public MasterDbContext(
        DbContextOptions<MasterDbContext> options,
        ICurrentTenant currentTenant,
        ICurrentUserService currentUser,
        IAuditableSaveChangesInterceptor auditableInterceptor)
        : base(options)
    {
        _currentTenant = currentTenant;
        _currentUser = currentUser;
        _auditableInterceptor = auditableInterceptor;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<TenantUser> TenantUsers => Set<TenantUser>();
    public DbSet<TenantSetting> TenantSettings => Set<TenantSetting>();
    public DbSet<TenantDatabaseConnection> TenantDatabaseConnections => Set<TenantDatabaseConnection>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<ProvisioningLog> ProvisioningLogs => Set<ProvisioningLog>();
    public DbSet<TenantInvitation> TenantInvitations => Set<TenantInvitation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("master");
        
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Global query filters
        modelBuilder.Entity<Tenant>().HasQueryFilter(t => !t.IsDeleted);
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<TenantUser>().HasQueryFilter(tu => !tu.IsDeleted);
        
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Apply audit values
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUser.UserId;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedAt = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = _currentUser.UserId;
                    break;
                case EntityState.Deleted:
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    entry.Entity.DeletedBy = _currentUser.UserId;
                    entry.State = EntityState.Modified;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

// Infrastructure/Data/TenantDbContext.cs
public class TenantDbContext : DbContext
{
    private readonly Guid _tenantId;

    public TenantDbContext(DbContextOptions<TenantDbContext> options, Guid tenantId)
        : base(options)
    {
        _tenantId = tenantId;
    }

    // Tenant-specific DbSets
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Customer> Customers => Set<Customer>();
    // Add more as needed...

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tenant");
        
        // Add TenantId to all entities
        modelBuilder.Entity<Product>().HasQueryFilter(p => p.TenantId == _tenantId && !p.IsDeleted);
        
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    if (entry.Entity.TenantId == Guid.Empty)
                        entry.Entity.TenantId = _tenantId;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedAt = DateTime.UtcNow;
                    break;
                case EntityState.Deleted:
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    entry.State = EntityState.Modified;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
```

### Middleware‌های امنیتی

```csharp
// Infrastructure/Middleware/TenantResolverMiddleware.cs
public class TenantResolverMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolverMiddleware> _logger;

    public TenantResolverMiddleware(RequestDelegate next, ILogger<TenantResolverMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ITenantResolver tenantResolver,
        ICurrentTenant currentTenant)
    {
        // Skip tenant resolution for public endpoints
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null ||
            endpoint?.Metadata.GetMetadata<PublicEndpointAttribute>() != null)
        {
            await _next(context);
            return;
        }

        // Try to resolve tenant from multiple sources (in order of priority)
        ITenant? tenant = null;

        // 1. From subdomain
        tenant = await ResolveFromSubdomainAsync(context, tenantResolver);
        
        // 2. From route
        if (tenant == null)
        {
            tenant = await ResolveFromRouteAsync(context, tenantResolver);
        }
        
        // 3. From authenticated user's session (MOST SECURE)
        if (tenant == null && context.User.Identity?.IsAuthenticated == true)
        {
            tenant = await ResolveFromUserSessionAsync(context, tenantResolver);
        }

        // 4. From header (only for specific internal services)
        if (tenant == null && context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdHeader))
        {
            if (IsInternalServiceRequest(context))
            {
                tenant = await tenantResolver.ResolveByIdAsync(Guid.Parse(tenantIdHeader!));
            }
        }

        if (tenant == null)
        {
            _logger.LogWarning("Tenant could not be resolved for request {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = "Tenant not identified" });
            return;
        }

        // Validate tenant status
        if (!tenant.IsActive)
        {
            _logger.LogWarning("Tenant {TenantId} is not active", tenant.Id);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { error = "Tenant is not active" });
            return;
        }

        // Set current tenant
        currentTenant.SetTenant(tenant);

        // Add tenant info to response headers (for debugging)
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-Tenant-Id"] = tenant.Id.ToString();
            context.Response.Headers["X-Tenant-Slug"] = tenant.Slug;
            return Task.CompletedTask;
        });

        await _next(context);
    }

    private async Task<ITenant?> ResolveFromSubdomainAsync(
        HttpContext context, ITenantResolver resolver)
    {
        var host = context.Request.Host.Host;
        var parts = host.Split('.');
        
        // Format: tenant-slug.domain.com
        if (parts.Length >= 3 && !parts[0].Equals("www", StringComparison.OrdinalIgnoreCase))
        {
            var slug = parts[0];
            return await resolver.ResolveBySlugAsync(slug);
        }
        
        return null;
    }

    private async Task<ITenant?> ResolveFromRouteAsync(
        HttpContext context, ITenantResolver resolver)
    {
        if (context.Request.RouteValues.TryGetValue("tenantSlug", out var slug))
        {
            return await resolver.ResolveBySlugAsync(slug?.ToString()!);
        }
        
        if (context.Request.RouteValues.TryGetValue("tenantId", out var id))
        {
            return await resolver.ResolveByIdAsync(Guid.Parse(id?.ToString()!));
        }
        
        return null;
    }

    private async Task<ITenant?> ResolveFromUserSessionAsync(
        HttpContext context, ITenantResolver resolver)
    {
        // Get tenant ID from user's claims (set during authentication)
        var tenantIdClaim = context.User.FindFirst("tenant_id");
        
        if (tenantIdClaim != null && Guid.TryParse(tenantIdClaim.Value, out var tenantId))
        {
            return await resolver.ResolveByIdAsync(tenantId);
        }
        
        return null;
    }

    private bool IsInternalServiceRequest(HttpContext context)
    {
        // Only allow tenant header from internal network or specific IPs
        var remoteIp = context.Connection.RemoteIpAddress;
        var internalIps = new[] { "127.0.0.1", "::1", "10.0.0.0/8", "172.16.0.0/12", "192.168.0.0/16" };
        
        return internalIps.Any(ip => remoteIp.IsInSubnet(IPAddress.Parse(ip.Split('/')[0]), 
            int.Parse(ip.Split('/')[1])));
    }
}

// Infrastructure/Middleware/TenantSecurityMiddleware.cs
public class TenantSecurityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantSecurityMiddleware> _logger;

    public TenantSecurityMiddleware(RequestDelegate next, ILogger<TenantSecurityMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ICurrentTenant currentTenant,
        IAuthorizationService authorizationService)
    {
        if (currentTenant.Tenant == null)
        {
            await _next(context);
            return;
        }

        var tenant = currentTenant.Tenant;

        // Check IP restriction
        if (tenant.AllowedIpAddresses?.Any() == true)
        {
            var clientIp = GetClientIpAddress(context);
            if (!tenant.AllowedIpAddresses.Contains(clientIp))
            {
                _logger.LogWarning(
                    "Access denied for IP {Ip} to tenant {TenantId}",
                    clientIp, tenant.Id);
                
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { error = "IP address not allowed" });
                return;
            }
        }

        // Check 2FA requirement
        if (tenant.Require2FA && context.User.Identity?.IsAuthenticated == true)
        {
            var has2FaClaim = context.User.HasClaim("amr", "mfa");
            if (!has2FaClaim)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new 
                { 
                    error = "2FA is required for this tenant",
                    require2FA = true
                });
                return;
            }
        }

        // Check session timeout
        var lastActivity = context.User.FindFirst("last_activity")?.Value;
        if (!string.IsNullOrEmpty(lastActivity) && 
            DateTime.TryParse(lastActivity, out var lastActivityTime))
        {
            var timeout = TimeSpan.FromMinutes(tenant.SessionTimeoutMinutes);
            if (DateTime.UtcNow - lastActivityTime > timeout)
            {
                _logger.LogWarning(
                    "Session timeout for user in tenant {TenantId}",
                    tenant.Id);
                
                await context.SignOutAsync();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Session expired" });
                return;
            }
        }

        await _next(context);
    }

    private string GetClientIpAddress(HttpContext context)
    {
        // Check for forwarded IP (behind proxy/load balancer)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}

// Infrastructure/Middleware/AuditMiddleware.cs
public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ICurrentUserService _currentUser;
    private readonly ICurrentTenant _currentTenant;
    private readonly IAuditLogService _auditLog;

    public AuditMiddleware(
        RequestDelegate next,
        ICurrentUserService currentUser,
        ICurrentTenant currentTenant,
        IAuditLogService auditLog)
    {
        _next = next;
        _currentUser = currentUser;
        _currentTenant = currentTenant;
        _auditLog = auditLog;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = Guid.NewGuid().ToString();
        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers["X-Correlation-Id"] = correlationId;

        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            // Only log for authenticated requests or significant endpoints
            if (_currentUser.IsAuthenticated || IsAuditableEndpoint(requestPath))
            {
                await _auditLog.LogAsync(new AuditLogEntry
                {
                    CorrelationId = Guid.Parse(correlationId),
                    TenantId = _currentTenant.TenantId,
                    UserId = _currentUser.UserId,
                    Action = $"{requestMethod} {requestPath}",
                    EntityType = ExtractEntityType(requestPath),
                    IpAddress = GetClientIp(context),
                    UserAgent = context.Request.Headers.UserAgent.ToString(),
                    StatusCode = context.Response.StatusCode,
                    DurationMs = (int)stopwatch.ElapsedMilliseconds,
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }

    private bool IsAuditableEndpoint(PathString path)
    {
        var auditablePaths = new[] { "/api/tenants", "/api/users", "/api/roles", "/api/settings" };
        return auditablePaths.Any(p => path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase));
    }

    private string ExtractEntityType(PathString path)
    {
        var segments = path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments?.Length >= 2 ? segments[1].Singularize() : "Unknown";
    }

    private string GetClientIp(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        return forwardedFor?.Split(',')[0].Trim() 
            ?? context.Connection.RemoteIpAddress?.ToString() 
            ?? "unknown";
    }
}
```

---

## API Layer

### Program.cs

```csharp
// API/Program.cs
var builder = WebApplication.CreateBuilder(args);

// Configuration
var connectionString = builder.Configuration.GetConnectionString("MasterDb");
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
var encryptionKey = builder.Configuration["Encryption:Key"] ?? throw new InvalidOperationException("Encryption Key not configured");

// Add Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Multi-Tenant API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database
builder.Services.AddDbContext<MasterDbContext>(options =>
    options.UseSqlServer(connectionString, sql =>
    {
        sql.EnableRetryOnFailure(3);
        sql.CommandTimeout(30);
    }));

// Multi-Tenancy
builder.Services.AddScoped<ICurrentTenant, CurrentTenant>();
builder.Services.AddScoped<ITenantResolver, TenantResolver>();
builder.Services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>();

// Authentication
builder.Services.AddJwtAuthentication(jwtSecret);
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TenantOwner", policy => 
        policy.RequireClaim("tenant_role", "Owner"));
    options.AddPolicy("TenantAdmin", policy => 
        policy.RequireClaim("tenant_role", "Owner", "Admin"));
});

// SignalR
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, TenantUserIdProvider>();

// Application Services
builder.Services.AddScoped<ITenantProvisioningService, TenantProvisioningService>();
builder.Services.AddScoped<ITenantDatabaseService, TenantDatabaseService>();
builder.Services.AddScoped<ITenantNotificationService, TenantNotificationService>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// CQRS
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TenantValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TenantAuthorizationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSubdomains", policy =>
    {
        policy.WithOrigins("https://*.yourdomain.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<MasterDbContext>("master_db")
    .AddCheck<TenantDatabaseHealthCheck>("tenant_db");

var app = builder.Build();

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSubdomains");

// Security Headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

// Multi-Tenant Middleware Pipeline
app.UseMiddleware<TenantResolverMiddleware>();
app.UseMiddleware<TenantSecurityMiddleware>();
app.UseMiddleware<AuditMiddleware>();

app.MapControllers();
app.MapHub<ProvisioningHub>("/hubs/provisioning");
app.MapHealthChecks("/health");

app.Run();
```

### SignalR Hub

```csharp
// API/Hubs/ProvisioningHub.cs
[Authorize]
public class ProvisioningHub : Hub<IProvisioningClient>
{
    private readonly ILogger<ProvisioningHub> _logger;

    public ProvisioningHub(ILogger<ProvisioningHub> logger)
    {
        _logger = logger;
    }

    public async Task JoinTenantGroup(string tenantSlug)
    {
        var groupName = $"tenant-{tenantSlug}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        
        _logger.LogInformation(
            "Client {ConnectionId} joined tenant group {Group}",
            Context.ConnectionId, groupName);
    }

    public async Task LeaveTenantGroup(string tenantSlug)
    {
        var groupName = $"tenant-{tenantSlug}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        
        _logger.LogInformation(
            "Client {ConnectionId} left tenant group {Group}",
            Context.ConnectionId, groupName);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception != null)
        {
            _logger.LogError(exception, 
                "Client disconnected with error: {ConnectionId}", 
                Context.ConnectionId);
        }
        else
        {
            _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}

public interface IProvisioningClient
{
    Task OnProgress(ProvisioningNotification notification);
    Task OnCompleted(ProvisioningNotification notification);
    Task OnError(ProvisioningNotification notification);
}

public class TenantUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst("sub")?.Value 
            ?? connection.ConnectionId;
    }
}
```

### Controllers

```csharp
// API/Controllers/TenantsController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class TenantsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentTenant _currentTenant;

    public TenantsController(IMediator mediator, ICurrentTenant currentTenant)
    {
        _mediator = mediator;
        _currentTenant = currentTenant;
    }

    /// <summary>
    /// Create a new tenant
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateTenantResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTenantCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.TenantId }, result);
    }

    /// <summary>
    /// Get tenant by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetTenantByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Get tenant by slug
    /// </summary>
    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await _mediator.Send(new GetTenantBySlugQuery(slug));
        return Ok(result);
    }

    /// <summary>
    /// Get all tenants with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<TenantListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] TenantListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Update tenant
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTenantCommand command)
    {
        if (id != command.Id) return BadRequest();
        
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Suspend tenant
    /// </summary>
    [HttpPost("{id:guid}/suspend")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Suspend(Guid id)
    {
        await _mediator.Send(new SuspendTenantCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Activate suspended tenant
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _mediator.Send(new ActivateTenantCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Delete tenant (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new

### USER
ادامه بده

### ASSISTANT
<details type="reasoning" done="true" duration="0">
<summary>Thought for 0 seconds</summary>
> The user wants me to continue the implementation. I should continue from where I left off - the Controllers section, then move on to the Angular frontend implementation.
</details>
# ادامه پیاده‌سازی Multi-Tenant

## API Layer (ادامه)

### Controllers

```csharp
// API/Controllers/TenantsController.cs (ادامه)
[HttpDelete("{id:guid}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
public async Task<IActionResult> Delete(Guid id)
{
    await _mediator.Send(new DeleteTenantCommand(id));
    return NoContent();
}

/// <summary>
/// Get tenant settings
/// </summary>
[HttpGet("{id:guid}/settings")]
[ProducesResponseType(typeof(TenantSettingsDto), StatusCodes.Status200OK)]
public async Task<IActionResult> GetSettings(Guid id)
{
    var result = await _mediator.Send(new GetTenantSettingsQuery(id));
    return Ok(result);
}

/// <summary>
/// Update tenant settings
/// </summary>
[HttpPut("{id:guid}/settings")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
public async Task<IActionResult> UpdateSettings(Guid id, [FromBody] UpdateTenantSettingsCommand command)
{
    if (id != command.TenantId) return BadRequest();
    await _mediator.Send(command);
    return NoContent();
}

/// <summary>
/// Get tenant provisioning status
/// </summary>
[HttpGet("{id:guid}/provisioning-status")]
[ProducesResponseType(typeof(ProvisioningStatusDto), StatusCodes.Status200OK)]
public async Task<IActionResult> GetProvisioningStatus(Guid id)
{
    var result = await _mediator.Send(new GetProvisioningStatusQuery(id));
    return Ok(result);
}

/// <summary>
/// Retry failed provisioning
/// </summary>
[HttpPost("{id:guid}/retry-provisioning")]
[ProducesResponseType(StatusCodes.Status202Accepted)]
public async Task<IActionResult> RetryProvisioning(Guid id)
{
    await _mediator.Send(new RetryProvisioningCommand(id));
    return Accepted();
}

/// <summary>
/// Get tenant database info
/// </summary>
[HttpGet("{id:guid}/database")]
[ProducesResponseType(typeof(TenantDatabaseDto), StatusCodes.Status200OK)]
public async Task<IActionResult> GetDatabaseInfo(Guid id)
{
    var result = await _mediator.Send(new GetTenantDatabaseQuery(id));
    return Ok(result);
}

/// <summary>
/// Test tenant database connection
/// </summary>
[HttpPost("{id:guid}/test-connection")]
[ProducesResponseType(typeof(ConnectionTestResult), StatusCodes.Status200OK)]
public async Task<IActionResult> TestConnection(Guid id)
{
    var result = await _mediator.Send(new TestTenantConnectionQuery(id));
    return Ok(result);
}

/// <summary>
/// Get tenant usage statistics
/// </summary>
[HttpGet("{id:guid}/usage")]
[ProducesResponseType(typeof(TenantUsageDto), StatusCodes.Status200OK)]
public async Task<IActionResult> GetUsage(Guid id)
{
    var result = await _mediator.Send(new GetTenantUsageQuery(id));
    return Ok(result);
}
```

```csharp
// API/Controllers/TenantUsersController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TenantUsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentTenant _currentTenant;

    public TenantUsersController(IMediator mediator, ICurrentTenant currentTenant)
    {
        _mediator = mediator;
        _currentTenant = currentTenant;
    }

    /// <summary>
    /// Get all users for current tenant
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<TenantUserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers([FromQuery] TenantUserListQuery query)
    {
        query.TenantId = _currentTenant.TenantId!.Value;
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TenantUserDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetTenantUserByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Invite user to tenant
    /// </summary>
    [HttpPost("invite")]
    [ProducesResponseType(typeof(InvitationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> InviteUser([FromBody] InviteUserCommand command)
    {
        command.TenantId = _currentTenant.TenantId!.Value;
        var result = await _mediator.Send(command);
        return Created("", result);
    }

    /// <summary>
    /// Accept invitation
    /// </summary>
    [HttpPost("accept-invitation")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AcceptInvitation([FromBody] AcceptInvitationCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Update user role
    /// </summary>
    [HttpPut("{id:guid}/role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateUserRoleCommand command)
    {
        if (id != command.UserId) return BadRequest();
        command.TenantId = _currentTenant.TenantId!.Value;
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Remove user from tenant
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveUser(Guid id)
    {
        await _mediator.Send(new RemoveUserCommand(id, _currentTenant.TenantId!.Value));
        return NoContent();
    }

    /// <summary>
    /// Get user permissions
    /// </summary>
    [HttpGet("{id:guid}/permissions")]
    [ProducesResponseType(typeof(UserPermissionsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPermissions(Guid id)
    {
        var result = await _mediator.Send(new GetUserPermissionsQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Update user permissions
    /// </summary>
    [HttpPut("{id:guid}/permissions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdatePermissions(Guid id, [FromBody] UpdateUserPermissionsCommand command)
    {
        if (id != command.UserId) return BadRequest();
        command.TenantId = _currentTenant.TenantId!.Value;
        await _mediator.Send(command);
        return NoContent();
    }
}
```

```csharp
// API/Controllers/AuthController.cs
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentTenant _currentTenant;

    public AuthController(IMediator mediator, ICurrentTenant currentTenant)
    {
        _mediator = mediator;
        _currentTenant = currentTenant;
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Login with tenant slug
    /// </summary>
    [HttpPost("login/{tenantSlug}")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginWithTenant(string tenantSlug, [FromBody] LoginCommand command)
    {
        command.TenantSlug = tenantSlug;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Register new user (for public registration if enabled)
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return Created("", result);
    }

    /// <summary>
    /// Confirm email
    /// </summary>
    [HttpPost("confirm-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Email confirmed successfully" });
    }

    /// <summary>
    /// Request password reset
    /// </summary>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Password reset email sent" });
    }

    /// <summary>
    /// Reset password with token
    /// </summary>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Password reset successfully" });
    }

    /// <summary>
    /// Enable 2FA
    /// </summary>
    [HttpPost("2fa/enable")]
    [Authorize]
    [ProducesResponseType(typeof(TwoFactorSetupResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Enable2FA()
    {
        var result = await _mediator.Send(new Enable2FACommand(_currentTenant.TenantId!.Value));
        return Ok(result);
    }

    /// <summary>
    /// Verify and activate 2FA
    /// </summary>
    [HttpPost("2fa/verify")]
    [Authorize]
    [ProducesResponseType(typeof(TwoFactorRecoveryCodes), StatusCodes.Status200OK)]
    public async Task<IActionResult> Verify2FA([FromBody] Verify2FACommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Verify 2FA code during login
    /// </summary>
    [HttpPost("2fa/login-verify")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Verify2FALogin([FromBody] Verify2FALoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Refresh token
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Logout
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        await _mediator.Send(new LogoutCommand());
        return NoContent();
    }
}
```

### Auth Commands

```csharp
// Application/Features/Auth/Commands/Login/LoginCommand.cs
public class LoginCommand : IRequest<AuthResponse>
{
    public string? TenantSlug { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IMasterDbContext _db;
    private readonly ITenantResolver _tenantResolver;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentTenant _currentTenant;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IMasterDbContext db,
        ITenantResolver tenantResolver,
        IJwtTokenService jwtTokenService,
        IPasswordHasher passwordHasher,
        ICurrentTenant currentTenant,
        ILogger<LoginCommandHandler> logger)
    {
        _db = db;
        _tenantResolver = tenantResolver;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
        _currentTenant = currentTenant;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        // Resolve tenant
        ITenant? tenant = null;
        if (!string.IsNullOrEmpty(request.TenantSlug))
        {
            tenant = await _tenantResolver.ResolveBySlugAsync(request.TenantSlug);
        }
        else if (_currentTenant.Tenant != null)
        {
            tenant = _currentTenant.Tenant;
        }

        if (tenant == null)
        {
            throw new UnauthorizedAccessException("Tenant not found");
        }

        if (!tenant.IsActive)
        {
            throw new UnauthorizedAccessException("Tenant is not active");
        }

        // Find user
        var user = await _db.Users
            .Include(u => u.TenantUsers)
            .ThenInclude(tu => tu.Tenant)
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant(), ct);

        if (user == null)
        {
            _logger.LogWarning("Login failed: User not found for email {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        // Check if user belongs to this tenant
        var tenantUser = user.TenantUsers.FirstOrDefault(tu => tu.TenantId == tenant.Id);
        if (tenantUser == null)
        {
            _logger.LogWarning("User {UserId} attempted to login to wrong tenant {TenantId}", 
                user.Id, tenant.Id);
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        // Check lockout
        if (user.IsLockedOut)
        {
            _logger.LogWarning("Login failed: User {UserId} is locked out", user.Id);
            throw new UnauthorizedAccessException("Account is locked. Try again later.");
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            // Increment failed attempts
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(30);
            }
            await _db.SaveChangesAsync(ct);
            
            _logger.LogWarning("Login failed: Invalid password for user {UserId}", user.Id);
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        // Reset failed attempts
        user.FailedLoginAttempts = 0;
        user.LastLoginAt = DateTime.UtcNow;

        // Update tenant user login info
        tenantUser.LastLoginAt = DateTime.UtcNow;
        tenantUser.LoginCount++;
        tenantUser.LastActivityAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        // Generate tokens
        var permissions = GetUserPermissions(user, tenantUser);
        var accessToken = _jwtTokenService.GenerateAccessToken(user, tenant, permissions);
        var refreshToken = _jwtTokenService.GenerateRefreshToken(user.Id);

        _logger.LogInformation("User {UserId} logged in successfully to tenant {TenantId}", 
            user.Id, tenant.Id);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600, // 1 hour
            TokenType = "Bearer",
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AvatarUrl = user.AvatarUrl
            },
            Tenant = new TenantBasicDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Slug = tenant.Slug,
                LogoUrl = tenant.LogoUrl,
                PrimaryColor = tenant.PrimaryColor
            },
            Permissions = permissions,
            Require2FA = tenant.Require2FA && !user.TwoFactorEnabled
        };
    }

    private List<string> GetUserPermissions(User user, TenantUser tenantUser)
    {
        var rolePermissions = JsonSerializer.Deserialize<List<string>>(tenantUser.Role.Permissions) 
            ?? new List<string>();
        
        var customPermissions = tenantUser.CustomPermissions ?? new Dictionary<string, bool>();
        
        // Merge role and custom permissions
        var allPermissions = new HashSet<string>(rolePermissions);
        foreach (var (key, value) in customPermissions)
        {
            if (value)
                allPermissions.Add(key);
            else
                allPermissions.Remove(key);
        }

        // If user has "*" permission, they have all
        if (allPermissions.Contains("*"))
            return new List<string> { "*" };

        return allPermissions.ToList();
    }
}
```

---

## Angular Frontend

### ساختار پوشه‌ها

```
src/
├── app/
│   ├── core/
│   │   ├── auth/
│   │   │   ├── auth.service.ts
│   │   │   ├── auth.guard.ts
│   │   │   ├── jwt.interceptor.ts
│   │   │   └── auth.models.ts
│   │   ├── tenant/
│   │   │   ├── tenant.service.ts
│   │   │   ├── tenant.guard.ts
│   │   │   ├── tenant.resolver.ts
│   │   │   └── tenant.models.ts
│   │   ├── services/
│   │   │   ├── api.service.ts
│   │   │   ├── storage.service.ts
│   │   │   └── notification.service.ts
│   │   └── shared/
│   │       ├── components/
│   │       └── pipes/
│   ├── features/
│   │   ├── auth/
│   │   │   ├── login/
│   │   │   ├── register/
│   │   │   └── forgot-password/
│   │   ├── tenant-management/
│   │   │   ├── tenant-list/
│   │   │   ├── tenant-create/
│   │   │   ├── tenant-details/
│   │   │   └── tenant-settings/
│   │   └── user-management/
│   │       ├── user-list/
│   │       ├── user-invite/
│   │       └── user-edit/
│   ├── shared/
│   │   ├── components/
│   │   ├── directives/
│   │   └── pipes/
│   └── app.routes.ts
```

### Core Services

```typescript
// core/auth/auth.service.ts
import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, BehaviorSubject, tap, catchError, of, map } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  tokenType: string;
  user: UserInfo;
  tenant: TenantInfo;
  permissions: string[];
  require2FA?: boolean;
}

export interface UserInfo {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  avatarUrl?: string;
}

export interface TenantInfo {
  id: string;
  name: string;
  slug: string;
  logoUrl?: string;
  primaryColor: string;
}

export interface LoginRequest {
  tenantSlug?: string;
  email: string;
  password: string;
  rememberMe?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly API_URL = environment.apiUrl;
  private readonly TOKEN_KEY = 'access_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private readonly USER_KEY = 'user_info';
  private readonly TENANT_KEY = 'tenant_info';

  // Signals for reactive state
  private _isAuthenticated = signal<boolean>(this.hasValidToken());
  private _currentUser = signal<UserInfo | null>(this.getStoredUser());
  private _currentTenant = signal<TenantInfo | null>(this.getStoredTenant());
  private _permissions = signal<string[]>(this.getStoredPermissions());

  // Public computed signals
  readonly isAuthenticated = computed(() => this._isAuthenticated());
  readonly currentUser = computed(() => this._currentUser());
  readonly currentTenant = computed(() => this._currentTenant());
  readonly permissions = computed(() => this._permissions());
  readonly isOwner = computed(() => this.hasPermission('*') || this.hasRole('Owner'));
  readonly isAdmin = computed(() => this.hasPermission('*') || this.hasRole('Owner') || this.hasRole('Admin'));

  // Auth state stream for components
  private authStateSubject = new BehaviorSubject<AuthResponse | null>(this.getStoredAuthState());
  authState$ = this.authStateSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    // Check token validity on init
    this.validateToken();
  }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.API_URL}/api/auth/login`, credentials).pipe(
      tap(response => this.handleAuthSuccess(response)),
      catchError(error => {
        console.error('Login failed:', error);
        throw error;
      })
    );
  }

  loginWith2FA(userId: string, code: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.API_URL}/api/auth/2fa/login-verify`, {
      userId,
      code
    }).pipe(
      tap(response => this.handleAuthSuccess(response)),
      catchError(error => {
        console.error('2FA verification failed:', error);
        throw error;
      })
    );
  }

  logout(): Observable<void> {
    const refreshToken = this.getRefreshToken();
    
    return this.http.post<void>(`${this.API_URL}/api/auth/logout`, { refreshToken }).pipe(
      tap(() => this.clearAuthState()),
      catchError(() => {
        this.clearAuthState();
        return of(void 0);
      })
    );
  }

  refreshToken(): Observable<AuthResponse> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      throw new Error('No refresh token available');
    }

    return this.http.post<AuthResponse>(`${this.API_URL}/api/auth/refresh-token`, {
      refreshToken
    }).pipe(
      tap(response => this.handleAuthSuccess(response)),
      catchError(error => {
        this.clearAuthState();
        throw error;
      })
    );
  }

  register(request: {
    email: string;
    password: string;
    firstName: string;
    lastName: string;
    tenantName: string;
    tenantSlug: string;
  }): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.API_URL}/api/auth/register`, request).pipe(
      tap(response => this.handleAuthSuccess(response))
    );
  }

  confirmEmail(token: string): Observable<void> {
    return this.http.post<void>(`${this.API_URL}/api/auth/confirm-email`, { token });
  }

  forgotPassword(email: string): Observable<void> {
    return this.http.post<void>(`${this.API_URL}/api/auth/forgot-password`, { email });
  }

  resetPassword(token: string, newPassword: string): Observable<void> {
    return this.http.post<void>(`${this.API_URL}/api/auth/reset-password`, {
      token,
      newPassword
    });
  }

  enable2FA(): Observable<{ qrCode: string; secret: string }> {
    return this.http.post<{ qrCode: string; secret: string }>(
      `${this.API_URL}/api/auth/2fa/enable`,
      {}
    );
  }

  verify2FA(code: string): Observable<{ recoveryCodes: string[] }> {
    return this.http.post<{ recoveryCodes: string[] }>(
      `${this.API_URL}/api/auth/2fa/verify`,
      { code }
    );
  }

  // Permission helpers
  hasPermission(permission: string): boolean {
    const perms = this._permissions();
    return perms.includes('*') || perms.includes(permission);
  }

  hasRole(role: string): boolean {
    // Check role from user claims
    const user = this._currentUser();
    // This would come from JWT claims
    return false;
  }

  hasAnyPermission(permissions: string[]): boolean {
    return permissions.some(p => this.hasPermission(p));
  }

  hasAllPermissions(permissions: string[]): boolean {
    return permissions.every(p => this.hasPermission(p));
  }

  // Token management
  getAccessToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  private handleAuthSuccess(response: AuthResponse): void {
    // Store tokens
    localStorage.setItem(this.TOKEN_KEY, response.accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, response.refreshToken);
    
    // Store user and tenant info
    localStorage.setItem(this.USER_KEY, JSON.stringify(response.user));
    localStorage.setItem(this.TENANT_KEY, JSON.stringify(response.tenant));
    localStorage.setItem('permissions', JSON.stringify(response.permissions));
    localStorage.setItem('auth_state', JSON.stringify(response));

    // Update signals
    this._isAuthenticated.set(true);
    this._currentUser.set(response.user);
    this._currentTenant.set(response.tenant);
    this._permissions.set(response.permissions);

    // Update state
    this.authStateSubject.next(response);
  }

  private clearAuthState(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    localStorage.removeItem(this.TENANT_KEY);
    localStorage.removeItem('permissions');
    localStorage.removeItem('auth_state');

    this._isAuthenticated.set(false);
    this._currentUser.set(null);
    this._currentTenant.set(null);
    this._permissions.set([]);

    this.authStateSubject.next(null);
  }

  private hasValidToken(): boolean {
    const token = this.getAccessToken();
    if (!token) return false;

    try {
      const payload = this.parseJwt(token);
      const exp = payload.exp * 1000;
      return Date.now() < exp;
    } catch {
      return false;
    }
  }

  private parseJwt(token: string): any {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(jsonPayload);
  }

  private validateToken(): void {
    const token = this.getAccessToken();
    if (!token) return;

    // Check if token is expired
    try {
      const payload = this.parseJwt(token);
      const exp = payload.exp * 1000;
      const now = Date.now();
      
      // If token expires in less than 5 minutes, refresh it
      if (exp - now < 5 * 60 * 1000) {
        this.refreshToken().subscribe({
          error: () => this.clearAuthState()
        });
      }
    } catch {
      this.clearAuthState();
    }
  }

  private getStoredUser(): UserInfo | null {
    const stored = localStorage.getItem(this.USER_KEY);
    return stored ? JSON.parse(stored) : null;
  }

  private getStoredTenant(): TenantInfo | null {
    const stored = localStorage.getItem(this.TENANT_KEY);
    return stored ? JSON.parse(stored) : null;
  }

  private getStoredPermissions(): string[] {
    const stored = localStorage.getItem('permissions');
    return stored ? JSON.parse(stored) : [];
  }

  private getStoredAuthState(): AuthResponse | null {
    const stored = localStorage.getItem('auth_state');
    return stored ? JSON.parse(stored) : null;
  }
}
```

```typescript
// core/auth/jwt.interceptor.ts
import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, filter, take, switchMap, finalize } from 'rxjs/operators';
import { AuthService } from './auth.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);

  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Skip auth header for certain requests
    if (this.shouldSkipAuth(request)) {
      return next.handle(request);
    }

    // Add auth header
    const token = this.authService.getAccessToken();
    if (token) {
      request = this.addAuthHeader(request, token);
    }

    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401 && !request.url.includes('/auth/')) {
          return this.handle401Error(request, next);
        }
        return throwError(() => error);
      })
    );
  }

  private addAuthHeader(request: HttpRequest<any>, token: string): HttpRequest<any> {
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  private shouldSkipAuth(request: HttpRequest<any>): boolean {
    const skipUrls = [
      '/api/auth/login',
      '/api/auth/register',
      '/api/auth/forgot-password',
      '/api/auth/reset-password',
      '/api/auth/confirm-email',
      '/health'
    ];
    return skipUrls.some(url => request.url.includes(url));
  }

  private handle401Error(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      return this.authService.refreshToken().pipe(
        switchMap((response) => {
          this.refreshTokenSubject.next(response.accessToken);
          return next.handle(this.addAuthHeader(request, response.accessToken));
        }),
        catchError((error) => {
          this.authService.logout();
          return throwError(() => error);
        }),
        finalize(() => {
          this.isRefreshing = false;
        })
      );
    }

    // Wait for the token to be refreshed
    return this.refreshTokenSubject.pipe(
      filter(token => token !== null),
      take(1),
      switchMap(token => next.handle(this.addAuthHeader(request, token!)))
    );
  }
}
```

```typescript
// core/tenant/tenant.service.ts
import { Injectable, signal, computed } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, tap, BehaviorSubject } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Tenant {
  id: string;
  name: string;
  slug: string;
  legalName?: string;
  registrationNumber?: string;
  taxId?: string;
  email: string;
  phone?: string;
  mobile?: string;
  website?: string;
  country: string;
  city: string;
  address?: string;
  postalCode?: string;
  logoUrl?: string;
  primaryColor: string;
  secondaryColor: string;
  subscriptionPlan: string;
  maxUsers: number;
  maxStorageMB: number;
  currentStorageMB: number;
  status: TenantStatus;
  isTrial: boolean;
  trialEndsAt?: string;
  subscriptionStartsAt?: string;
  subscriptionEndsAt?: string;
  provisioningStatus: ProvisioningStatus;
  require2FA: boolean;
  sessionTimeoutMinutes: number;
  createdAt: string;
  modifiedAt?: string;
}

export enum TenantStatus {
  Pending = 1,
  Active = 2,
  Suspended = 3,
  Cancelled = 4
}

export enum ProvisioningStatus {
  NotStarted = 0,
  Creating = 1,
  Ready = 2,
  Failed = 3
}

export interface CreateTenantRequest {
  name: string;
  slug: string;
  legalName?: string;
  registrationNumber?: string;
  taxId?: string;
  email: string;
  phone?: string;
  mobile?: string;
  website?: string;
  country: string;
  city: string;
  address?: string;
  postalCode?: string;
  databaseStrategy: number;
  adminFirstName: string;
  adminLastName: string;
  adminEmail: string;
  adminPassword: string;
  adminMobile?: string;
  adminNationalCode?: string;
  subscriptionPlan: string;
  maxUsers: number;
  maxStorageMB: number;
  isTrial: boolean;
  trialDays: number;
  logoUrl?: string;
  primaryColor: string;
  secondaryColor: string;
  require2FA: boolean;
  sessionTimeoutMinutes: number;
  passwordMinLength: number;
  passwordRequireSpecialChar: boolean;
  allowedIpAddresses?: string[];
  featureFlags?: Record<string, boolean>;
}

export interface TenantListQuery {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: TenantStatus;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  pageIndex: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class TenantService {
  private readonly API_URL = environment.apiUrl;

  // Signals
  private _tenants = signal<Tenant[]>([]);
  private _currentTenant = signal<Tenant | null>(null);
  private _loading = signal<boolean>(false);

  readonly tenants = computed(() => this._tenants());
  readonly currentTenant = computed(() => this._currentTenant());
  readonly loading = computed(() => this._loading());

  constructor(private http: HttpClient) {}

  // CRUD Operations
  getTenants(query: TenantListQuery): Observable<PaginatedResult<Tenant>> {
    let params = new HttpParams()
      .set('page', query.page?.toString() || '1')
      .set('pageSize', query.pageSize?.toString() || '20');

    if (query.search) params = params.set('search', query.search);
    if (query.status) params = params.set('status', query.status.toString());
    if (query.sortBy) params = params.set('sortBy', query.sortBy);
    if (query.sortOrder) params = params.set('sortOrder', query.sortOrder);

    return this.http.get<PaginatedResult<Tenant>>(`${this.API_URL}/api/tenants`, { params }).pipe(
      tap(result => this._tenants.set(result.items))
    );
  }

  getTenantById(id: string): Observable<Tenant> {
    return this.http.get<Tenant>(`${this.API_URL}/api/tenants/${id}`).pipe(
      tap(tenant => this._currentTenant.set(tenant))
    );
  }

  getTenantBySlug(slug: string): Observable<Tenant> {
    return this.http.get<Tenant>(`${this.API_URL}/api/tenants/slug/${slug}`).pipe(
      tap(tenant => this._currentTenant.set(tenant))
    );
  }

  createTenant(request: CreateTenantRequest): Observable<{ tenantId: string; adminUserId: string; slug: string }> {
    return this.http.post<{ tenantId: string; adminUserId: string; slug: string }>(
      `${this.API_URL}/api/tenants`,
      request
    );
  }

  updateTenant(id: string, request: Partial<Tenant>): Observable<Tenant> {
    return this.http.put<Tenant>(`${this.API_URL}/api/tenants/${id}`, request).pipe(
      tap(tenant => {
        const current = this._tenants();
        const index = current.findIndex(t => t.id === id);
        if (index >= 0) {
          this._tenants.set([
            ...current.slice(0, index),
            tenant,
            ...current.slice(index + 1)
          ]);
        }
      })
    );
  }

  suspendTenant(id: string): Observable<void> {
    return this.http.post<void>(`${this.API_URL}/api/tenants/${id}/suspend`, {});
  }

  activateTenant(id: string): Observable<void> {
    return this.http.post<void>(`${this.API_URL}/api/tenants/${id}/activate`, {});
  }

  deleteTenant(id: string): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/api/tenants/${id}`).pipe(
      tap(() => {
        const current = this._tenants();
        this._tenants.set(current.filter(t => t.id !== id));
      })
    );
  }

  // Settings
  getTenantSettings(tenantId: string): Observable<Record<string, any>> {
    return this.http.get<Record<string, any>>(`${this.API_URL}/api/tenants/${tenantId}/settings`);
  }

  updateTenantSettings(tenantId: string, settings: Record<string, any>): Observable<void> {
    return this.http.put<void>(`${this.API_URL}/api/tenants/${tenantId}/settings`, settings);
  }

  // Provisioning
  getProvisioningStatus(tenantId: string): Observable<ProvisioningStatusDto> {
    return this.http.get<ProvisioningStatusDto>(`${this.API_URL}/api/tenants/${tenantId}/provisioning-status`);
  }

  retryProvisioning(tenantId: string): Observable<void> {
    return this.http.post<void>(`${this.API_URL}/api/tenants/${tenantId}/retry-provisioning`, {});
  }

  // Database
  getDatabaseInfo(tenantId: string): Observable<TenantDatabaseDto> {
    return this.http.get<TenantDatabaseDto>(`${this.API_URL}/api/tenants/${tenantId}/database`);
  }

  testConnection(tenantId: string): Observable<{ success: boolean; message: string }> {
    return this.http.post<{ success: boolean; message: string }>(
      `${this.API_URL}/api/tenants/${tenantId}/test-connection`,
      {}
    );
  }

  // Usage
  getUsage(tenantId: string): Observable<TenantUsageDto> {
    return this.http.get<TenantUsageDto>(`${this.API_URL}/api/tenants/${tenantId}/usage`);
  }

  // Validation
  checkSlugAvailability(slug: string): Observable<boolean> {
    return this.http.get<boolean>(`${this.API_URL}/api/tenants/check-slug/${slug}`);
  }

  checkEmailAvailability(email: string): Observable<boolean> {
    return this.http.get<boolean>(`${this.API_URL}/api/tenants/check-email/${email}`);
  }
}

export interface ProvisioningStatusDto {
  status: ProvisioningStatus;
  steps: ProvisioningStep[];
  error?: string;
}

export interface ProvisioningStep {
  name: string;
  description: string;
  status: number;
  errorMessage?: string;
  startedAt?: string;
  completedAt?: string;
}

export interface TenantDatabaseDto {
  databaseType: number;
  serverName: string;
  databaseName: string;
  isActive: boolean;
  healthStatus: number;
  lastHealthCheck?: string;
}

export interface TenantUsageDto {
  userCount: number;
  maxUsers: number;
  storageUsedMB: number;
  maxStorageMB: number;
  apiCallsThisMonth: number;
  maxApiCallsPerMonth?: number;
}
```

### SignalR Service برای Provisioning

```typescript
// core/tenant/provisioning.service.ts
import { Injectable, signal, computed } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ProvisioningNotification {
  tenantSlug: string;
  status: 'InProgress' | 'Completed' | 'Failed';
  currentStep: number;
  totalSteps: number;
  message: string;
  steps?: ProvisioningStep[];
  tenantId?: string;
  adminUserId?: string;
  timestamp: string;
}

export interface ProvisioningStep {
  name: string;
  description: string;
  status: number;
  errorMessage?: string;
  startedAt?: string;
  completedAt?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProvisioningService {
  private hubConnection: HubConnection | null = null;
  
  // Signals
  private _isConnected = signal<boolean>(false);
  private _currentProgress = signal<ProvisioningNotification | null>(null);
  private _provisioningHistory = signal<Map<string, ProvisioningNotification[]>>(new Map());

  readonly isConnected = computed(() => this._isConnected());
  readonly currentProgress = computed(() => this._currentProgress());
  readonly provisioningHistory = computed(() => this._provisioningHistory());

  // Observable streams
  private notificationSubject = new Subject<ProvisioningNotification>();
  notification$ = this.notificationSubject.asObservable();

  private completionSubject = new Subject<ProvisioningNotification>();
  completion$ = this.completionSubject.asObservable();

  private errorSubject = new Subject<ProvisioningNotification>();
  error$ = this.errorSubject.asObservable();

  constructor() {
    this.initializeHub();
  }

  private initializeHub(): void {
    const token = localStorage.getItem('access_token');
    
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${environment.hubUrl}/hubs/provisioning`, {
        accessTokenFactory: () => token || ''
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          if (retryContext.previousRetryCount < 3) {
            return 2000;
          }
          return null; // Stop reconnecting
        }
      })
      .configureLogging('information')
      .build();

    this.registerHandlers();
    
    this.hubConnection.onclose((error) => {
      console.error('SignalR connection closed:', error);
      this._isConnected.set(false);
    });

    this.hubConnection.onreconnected(() => {
      console.log('SignalR reconnected');
      this._isConnected.set(true);
    });

    this.hubConnection.onreconnecting((error) => {
      console.warn('SignalR reconnecting:', error);
      this._isConnected.set(false);
    });
  }

  private registerHandlers(): void {
    if (!this.hubConnection) return;

    this.hubConnection.on('OnProgress', (notification: ProvisioningNotification) => {
      this._currentProgress.set(notification);
      this.notificationSubject.next(notification);
      this.addToHistory(notification);
    });

    this.hubConnection.on('OnCompleted', (notification: ProvisioningNotification) => {
      this._currentProgress.set(notification);
      this.completionSubject.next(notification);
      this.addToHistory(notification);
    });

    this.hubConnection.on('OnError', (notification: ProvisioningNotification) => {
      this._currentProgress.set(notification);
      this.errorSubject.next(notification);
      this.addToHistory(notification);
    });
  }

  async startConnection(): Promise<void> {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      return;
    }

    try {
      await this.hubConnection?.start();
      this._isConnected.set(true);
      console.log('SignalR connection started');
    } catch (error) {
      console.error('Error starting SignalR connection:', error);
      this._isConnected.set(false);
      throw error;
    }
  }

  async stopConnection(): Promise<void> {
    if (this.hubConnection?.state === HubConnectionState.Disconnected) {
      return;
    }

    try {
      await this.hubConnection?.stop();
      this._isConnected.set(false);
      console.log('SignalR connection stopped');
    } catch (error) {
      console.error('Error stopping SignalR connection:', error);
    }
  }

  async joinTenantGroup(tenantSlug: string): Promise<void> {
    if (this.hubConnection?.state !== HubConnectionState.Connected) {
      await this.startConnection();
    }

    await this.hubConnection?.invoke('JoinTenantGroup', tenantSlug);
    console.log(`Joined tenant group: ${tenantSlug}`);
  }

  async leaveTenantGroup(tenantSlug: string): Promise<void> {
    await this.hubConnection?.invoke('LeaveTenantGroup', tenantSlug);
    console.log(`Left tenant group: ${tenantSlug}`);
  }

  private addToHistory(notification: ProvisioningNotification): void {
    const history = new Map(this._provisioningHistory());
    const tenantHistory = history.get(notification.tenantSlug) || [];
    tenantHistory.push(notification);
    history.set(notification.tenantSlug, tenantHistory);
    this._provisioningHistory.set(history);
  }

  getHistoryForTenant(tenantSlug: string): ProvisioningNotification[] {
    return this._provisioningHistory().get(tenantSlug) || [];
  }

  clearHistory(tenantSlug?: string): void {
    if (tenantSlug) {
      const history = new Map(this._provisioningHistory());
      history.delete(tenantSlug);
      this._provisioningHistory.set(history);
    } else {

### USER
ادامه بده

### ASSISTANT
<details type="reasoning" done="true" duration="0">
<summary>Thought for 0 seconds</summary>
> The user wants me to continue the Angular frontend implementation. I should continue from where I left off - the ProvisioningService and then add the components for tenant management.
</details>
# ادامه پیاده‌سازی Multi-Tenant - Angular Frontend

## Angular Components

### Provisioning Service (ادامه)

```typescript
// core/tenant/provisioning.service.ts (ادامه)
  clearHistory(tenantSlug?: string): void {
    if (tenantSlug) {
      const history = new Map(this._provisioningHistory());
      history.delete(tenantSlug);
      this._provisioningHistory.set(history);
    } else {
      this._provisioningHistory.set(new Map());
    }
  }

  // Cleanup on destroy
  destroy(): void {
    this.stopConnection();
    this.hubConnection = null;
  }
}
```

### Tenant Create Component

```typescript
// features/tenant-management/tenant-create/tenant-create.component.ts
import { Component, signal, computed, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Subject, takeUntil, firstValueFrom } from 'rxjs';
import { TenantService, CreateTenantRequest } from '../../../core/tenant/tenant.service';
import { ProvisioningService, ProvisioningNotification } from '../../../core/tenant/provisioning.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-tenant-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="tenant-create-container">
      <div class="header">
        <h1>ایجاد بهره‌بردار جدید</h1>
        <p class="subtitle">فرم ثبت‌نام بهره‌بردار جدید را تکمیل کنید</p>
      </div>

      <!-- Progress Steps -->
      @if (isCreating()) {
        <div class="provisioning-progress">
          <div class="progress-header">
            <h3>در حال ایجاد بهره‌بردار...</h3>
            <span class="step-counter">{{ currentStep() }} از {{ totalSteps() }}</span>
          </div>
          
          <div class="steps-list">
            @for (step of steps(); track step.name) {
              <div class="step-item" [class.completed]="step.status === 2" 
                   [class.in-progress]="step.status === 1" [class.failed]="step.status === 3">
                <div class="step-icon">
                  @switch (step.status) {
                    @case (0) { <span class="pending">○</span> }
                    @case (1) { <span class="spinner"></span> }
                    @case (2) { <span class="check">✓</span> }
                    @case (3) { <span class="error">✗</span> }
                  }
                </div>
                <div class="step-content">
                  <span class="step-name">{{ step.description }}</span>
                  @if (step.errorMessage) {
                    <span class="step-error">{{ step.errorMessage }}</span>
                  }
                </div>
              </div>
            }
          </div>

          @if (isCompleted()) {
            <div class="completion-message success">
              <h4>✓ بهره‌بردار با موفقیت ایجاد شد!</h4>
              <p>ایمیل تأیید به مدیر ارسال شده است.</p>
              <div class="completion-actions">
                <button class="btn btn-primary" (click)="goToTenant()">
                  مشاهده بهره‌بردار
                </button>
                <button class="btn btn-secondary" (click)="resetForm()">
                  ایجاد بهره‌بردار جدید
                </button>
              </div>
            </div>
          }

          @if (hasError()) {
            <div class="completion-message error">
              <h4>✗ خطا در ایجاد بهره‌بردار</h4>
              <p>{{ errorMessage() }}</p>
              <button class="btn btn-primary" (click)="retryCreation()">
                تلاش مجدد
              </button>
            </div>
          }
        </div>
      }

      <!-- Registration Form -->
      @if (!isCreating()) {
        <form [formGroup]="form" (ngSubmit)="onSubmit()" class="tenant-form">
          <!-- Step Navigation -->
          <div class="form-steps">
            <button type="button" class="step-btn" [class.active]="currentFormStep() === 1"
                    (click)="currentFormStep.set(1)">۱. اطلاعات بهره‌بردار</button>
            <button type="button" class="step-btn" [class.active]="currentFormStep() === 2"
                    (click)="currentFormStep.set(2)">۲. اطلاعات مدیر</button>
            <button type="button" class="step-btn" [class.active]="currentFormStep() === 3"
                    (click)="currentFormStep.set(3)">۳. تنظیمات</button>
          </div>

          <!-- Step 1: Tenant Info -->
          @if (currentFormStep() === 1) {
            <div class="form-step">
              <h3>اطلاعات بهره‌بردار</h3>
              
              <div class="form-row">
                <div class="form-group">
                  <label for="name">نام بهره‌بردار *</label>
                  <input type="text" id="name" formControlName="name" 
                         [class.error]="isFieldInvalid('name')">
                  @if (isFieldInvalid('name')) {
                    <span class="error-message">نام الزامی است</span>
                  }
                </div>

                <div class="form-group">
                  <label for="slug">Slug *</label>
                  <div class="input-with-prefix">
                    <span class="prefix">{{ slugPrefix }}</span>
                    <input type="text" id="slug" formControlName="slug" 
                           (input)="onSlugChange()" [class.error]="isFieldInvalid('slug')">
                  </div>
                  @if (isFieldInvalid('slug')) {
                    <span class="error-message">Slug الزامی و باید یکتا باشد</span>
                  }
                  @if (slugChecking()) {
                    <span class="checking">در حال بررسی...</span>
                  }
                  @if (slugAvailable() === true) {
                    <span class="available">✓ در دسترس</span>
                  }
                  @if (slugAvailable() === false) {
                    <span class="not-available">✗ قبلاً استفاده شده</span>
                  }
                </div>
              </div>

              <div class="form-row">
                <div class="form-group">
                  <label for="legalName">نام حقوقی</label>
                  <input type="text" id="legalName" formControlName="legalName">
                </div>

                <div class="form-group">
                  <label for="registrationNumber">شماره ثبت</label>
                  <input type="text" id="registrationNumber" formControlName="registrationNumber">
                </div>
              </div>

              <div class="form-row">
                <div class="form-group">
                  <label for="email">ایمیل اصلی *</label>
                  <input type="email" id="email" formControlName="email" 
                         [class.error]="isFieldInvalid('email')">
                  @if (isFieldInvalid('email')) {
                    <span class="error-message">ایمیل معتبر وارد کنید</span>
                  }
                </div>

                <div class="form-group">
                  <label for="phone">تلفن ثابت</label>
                  <input type="tel" id="phone" formControlName="phone">
                </div>

                <div class="form-group">
                  <label for="mobile">تلفن همراه</label>
                  <input type="tel" id="mobile" formControlName="mobile">
                </div>
              </div>

              <div class="form-row">
                <div class="form-group">
                  <label for="country">کشور *</label>
                  <select id="country" formControlName="country">
                    <option value="Iran">ایران</option>
                    <option value="Afghanistan">افغانستان</option>
                    <option value="Turkey">ترکیه</option>
                  </select>
                </div>

                <div class="form-group">
                  <label for="city">شهر *</label>
                  <input type="text" id="city" formControlName="city" 
                         [class.error]="isFieldInvalid('city')">
                </div>
              </div>

              <div class="form-group">
                <label for="address">آدرس</label>
                <textarea id="address" formControlName="address" rows="3"></textarea>
              </div>

              <div class="form-group">
                <label for="website">وب‌سایت</label>
                <input type="url" id="website" formControlName="website" placeholder="https://">
              </div>
            </div>
          }

          <!-- Step 2: Admin Info -->
          @if (currentFormStep() === 2) {
            <div class="form-step">
              <h3>اطلاعات مدیر اصلی</h3>
              
              <div class="form-row">
                <div class="form-group">
                  <label for="adminFirstName">نام *</label>
                  <input type="text" id="adminFirstName" formControlName="adminFirstName"
                         [class.error]="isFieldInvalid('adminFirstName')">
                </div>

                <div class="form-group">
                  <label for="adminLastName">نام خانوادگی *</label>
                  <input type="text" id="adminLastName" formControlName="adminLastName"
                         [class.error]="isFieldInvalid('adminLastName')">
                </div>
              </div>

              <div class="form-row">
                <div class="form-group">
                  <label for="adminEmail">ایمیل *</label>
                  <input type="email" id="adminEmail" formControlName="adminEmail"
                         [class.error]="isFieldInvalid('adminEmail')">
                  @if (isFieldInvalid('adminEmail')) {
                    <span class="error-message">ایمیل معتبر وارد کنید</span>
                  }
                </div>

                <div class="form-group">
                  <label for="adminMobile">تلفن همراه</label>
                  <input type="tel" id="adminMobile" formControlName="adminMobile">
                </div>
              </div>

              <div class="form-group">
                <label for="adminNationalCode">کد ملی</label>
                <input type="text" id="adminNationalCode" formControlName="adminNationalCode" 
                       maxlength="10">
                @if (isFieldInvalid('adminNationalCode')) {
                  <span class="error-message">کد ملی معتبر وارد کنید</span>
                }
              </div>

              <div class="form-row">
                <div class="form-group">
                  <label for="adminPassword">رمز عبور *</label>
                  <div class="password-input">
                    <input [type]="showPassword() ? 'text' : 'password'" 
                           id="adminPassword" formControlName="adminPassword"
                           [class.error]="isFieldInvalid('adminPassword')">
                    <button type="button" class="toggle-password" (click)="showPassword.set(!showPassword())">
                      {{ showPassword() ? '👁' : '👁‍🗨' }}
                    </button>
                  </div>
                  @if (isFieldInvalid('adminPassword')) {
                    <span class="error-message">رمز عبور باید حداقل ۸ کاراکتر و شامل حرف خاص باشد</span>
                  }
                </div>

                <div class="form-group">
                  <label for="confirmPassword">تکرار رمز عبور *</label>
                  <input type="password" id="confirmPassword" formControlName="confirmPassword"
                         [class.error]="form.hasError('passwordMismatch')">
                  @if (form.hasError('passwordMismatch')) {
                    <span class="error-message">رمز عبور مطابقت ندارد</span>
                  }
                </div>
              </div>

              <div class="password-strength">
                <span>قدرت رمز عبور:</span>
                <div class="strength-bar">
                  <div class="strength-fill" [style.width.%]="passwordStrength()"></div>
                </div>
                <span class="strength-text" [class]="passwordStrengthClass()">
                  {{ passwordStrengthText() }}
                </span>
              </div>
            </div>
          }

          <!-- Step 3: Settings -->
          @if (currentFormStep() === 3) {
            <div class="form-step">
              <h3>تنظیمات و پیکربندی</h3>
              
              <div class="form-group">
                <label for="subscriptionPlan">پلن اشتراک</label>
                <select id="subscriptionPlan" formControlName="subscriptionPlan">
                  <option value="Basic">پایه (۱۰ کاربر)</option>
                  <option value="Standard">استاندارد (۵۰ کاربر)</option>
                  <option value="Professional">حرفه‌ای (۲۰۰ کاربر)</option>
                  <option value="Enterprise">سازمانی (نامحدود)</option>
                </select>
              </div>

              <div class="form-row">
                <div class="form-group">
                  <label for="maxUsers">حداکثر کاربران</label>
                  <input type="number" id="maxUsers" formControlName="maxUsers" min="1" max="10000">
                </div>

                <div class="form-group">
                  <label for="maxStorageMB">حداکثر فضای ذخیره‌سازی (MB)</label>
                  <input type="number" id="maxStorageMB" formControlName="maxStorageMB" min="100">
                </div>
              </div>

              <div class="form-group">
                <label class="checkbox-label">
                  <input type="checkbox" formControlName="isTrial">
                  <span>دوره آزمایشی</span>
                </label>
              </div>

              @if (form.get('isTrial')?.value) {
                <div class="form-group">
                  <label for="trialDays">مدت آزمایشی (روز)</label>
                  <input type="number" id="trialDays" formControlName="trialDays" 
                         min="0" max="90">
                </div>
              }

              <div class="form-group">
                <label for="databaseStrategy">استراتژی دیتابیس</label>
                <select id="databaseStrategy" formControlName="databaseStrategy">
                  <option [value]="1">مشترک (همه بهره‌بردارها در یک دیتابیس)</option>
                  <option [value]="2">جدا (هر بهره‌بردار دیتابیس جدا)</option>
                </select>
                <p class="help-text">
                  @if (form.get('databaseStrategy')?.value === 2) {
                    امنیت بالاتر اما هزینه بیشتر
                  } @else {
                    صرفه‌جویی در هزینه، مناسب برای شروع
                  }
                </p>
              </div>

              <h4>تنظیمات امنیتی</h4>
              
              <div class="form-group">
                <label class="checkbox-label">
                  <input type="checkbox" formControlName="require2FA">
                  <span>الزام به احراز هویت دو مرحله‌ای (2FA)</span>
                </label>
              </div>

              <div class="form-row">
                <div class="form-group">
                  <label for="sessionTimeoutMinutes">زمان انقضای جلسه (دقیقه)</label>
                  <input type="number" id="sessionTimeoutMinutes" formControlName="sessionTimeoutMinutes" 
                         min="5" max="480">
                </div>

                <div class="form-group">
                  <label for="passwordMinLength">حداقل طول رمز عبور</label>
                  <input type="number" id="passwordMinLength" formControlName="passwordMinLength" 
                         min="6" max="32">
                </div>
              </div>

              <div class="form-group">
                <label class="checkbox-label">
                  <input type="checkbox" formControlName="passwordRequireSpecialChar">
                  <span>الزام به کاراکتر خاص در رمز عبور</span>
                </label>
              </div>

              <h4>برندینگ</h4>
              
              <div class="form-row">
                <div class="form-group">
                  <label for="primaryColor">رنگ اصلی</label>
                  <div class="color-input">
                    <input type="color" id="primaryColor" formControlName="primaryColor">
                    <span>{{ form.get('primaryColor')?.value }}</span>
                  </div>
                </div>

                <div class="form-group">
                  <label for="secondaryColor">رنگ ثانویه</label>
                  <div class="color-input">
                    <input type="color" id="secondaryColor" formControlName="secondaryColor">
                    <span>{{ form.get('secondaryColor')?.value }}</span>
                  </div>
                </div>
              </div>
            </div>
          }

          <!-- Form Actions -->
          <div class="form-actions">
            @if (currentFormStep() > 1) {
              <button type="button" class="btn btn-secondary" (click)="prevStep()">
                قبلی
              </button>
            }
            
            @if (currentFormStep() < 3) {
              <button type="button" class="btn btn-primary" (click)="nextStep()">
                بعدی
              </button>
            } @else {
              <button type="submit" class="btn btn-success" [disabled]="isSubmitting()">
                @if (isSubmitting()) {
                  <span class="spinner"></span> در حال ایجاد...
                } @else {
                  ایجاد بهره‌بردار
                }
              </button>
            }
          </div>
        </form>
      }
    </div>
  `,
  styles: [`
    .tenant-create-container {
      max-width: 900px;
      margin: 0 auto;
      padding: 2rem;
    }

    .header {
      margin-bottom: 2rem;
    }

    .header h1 {
      font-size: 1.75rem;
      margin-bottom: 0.5rem;
    }

    .subtitle {
      color: #666;
    }

    .form-steps {
      display: flex;
      gap: 1rem;
      margin-bottom: 2rem;
    }

    .step-btn {
      flex: 1;
      padding: 1rem;
      border: 2px solid #e0e0e0;
      background: white;
      border-radius: 8px;
      cursor: pointer;
      transition: all 0.3s;
      font-weight: 500;
    }

    .step-btn.active {
      border-color: #1976D2;
      background: #e3f2fd;
      color: #1976D2;
    }

    .tenant-form {
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .form-step h3 {
      margin-bottom: 1.5rem;
      padding-bottom: 0.5rem;
      border-bottom: 2px solid #f0f0f0;
    }

    .form-row {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 1rem;
    }

    .form-group {
      margin-bottom: 1.25rem;
    }

    .form-group label {
      display: block;
      margin-bottom: 0.5rem;
      font-weight: 500;
      color: #333;
    }

    .form-group input,
    .form-group select,
    .form-group textarea {
      width: 100%;
      padding: 0.75rem;
      border: 1px solid #ddd;
      border-radius: 6px;
      font-size: 1rem;
      transition: border-color 0.3s;
    }

    .form-group input:focus,
    .form-group select:focus,
    .form-group textarea:focus {
      outline: none;
      border-color: #1976D2;
    }

    .form-group input.error,
    .form-group select.error {
      border-color: #f44336;
    }

    .error-message {
      display: block;
      color: #f44336;
      font-size: 0.85rem;
      margin-top: 0.25rem;
    }

    .input-with-prefix {
      display: flex;
      align-items: stretch;
    }

    .input-with-prefix .prefix {
      padding: 0.75rem;
      background: #f5f5f5;
      border: 1px solid #ddd;
      border-right: none;
      border-radius: 6px 0 0 6px;
      color: #666;
    }

    .input-with-prefix input {
      border-radius: 0 6px 6px 0;
    }

    .password-input {
      position: relative;
    }

    .toggle-password {
      position: absolute;
      left: 10px;
      top: 50%;
      transform: translateY(-50%);
      background: none;
      border: none;
      cursor: pointer;
    }

    .checkbox-label {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      cursor: pointer;
    }

    .checkbox-label input {
      width: auto;
    }

    .color-input {
      display: flex;
      align-items: center;
      gap: 1rem;
    }

    .color-input input[type="color"] {
      width: 50px;
      height: 40px;
      padding: 0;
      border: none;
      cursor: pointer;
    }

    .form-actions {
      display: flex;
      justify-content: flex-end;
      gap: 1rem;
      margin-top: 2rem;
      padding-top: 1rem;
      border-top: 1px solid #eee;
    }

    .btn {
      padding: 0.75rem 1.5rem;
      border: none;
      border-radius: 6px;
      font-size: 1rem;
      cursor: pointer;
      transition: all 0.3s;
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .btn-primary {
      background: #1976D2;
      color: white;
    }

    .btn-primary:hover {
      background: #1565C0;
    }

    .btn-secondary {
      background: #e0e0e0;
      color: #333;
    }

    .btn-success {
      background: #4CAF50;
      color: white;
    }

    .btn:disabled {
      opacity: 0.7;
      cursor: not-allowed;
    }

    .spinner {
      width: 16px;
      height: 16px;
      border: 2px solid transparent;
      border-top-color: currentColor;
      border-radius: 50%;
      animation: spin 0.8s linear infinite;
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }

    /* Provisioning Progress */
    .provisioning-progress {
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .progress-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1.5rem;
    }

    .step-counter {
      color: #666;
    }

    .steps-list {
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
    }

    .step-item {
      display: flex;
      align-items: center;
      gap: 1rem;
      padding: 1rem;
      background: #f9f9f9;
      border-radius: 8px;
      transition: all 0.3s;
    }

    .step-item.completed {
      background: #e8f5e9;
    }

    .step-item.in-progress {
      background: #e3f2fd;
    }

    .step-item.failed {
      background: #ffebee;
    }

    .step-icon {
      width: 32px;
      height: 32px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: 50%;
      font-size: 1.25rem;
    }

    .step-item.completed .step-icon {
      background: #4CAF50;
      color: white;
    }

    .step-item.in-progress .step-icon .spinner {
      border-top-color: #1976D2;
    }

    .step-item.failed .step-icon {
      background: #f44336;
      color: white;
    }

    .step-content {
      flex: 1;
    }

    .step-name {
      display: block;
      font-weight: 500;
    }

    .step-error {
      display: block;
      color: #f44336;
      font-size: 0.85rem;
      margin-top: 0.25rem;
    }

    .completion-message {
      margin-top: 2rem;
      padding: 1.5rem;
      border-radius: 8px;
      text-align: center;
    }

    .completion-message.success {
      background: #e8f5e9;
    }

    .completion-message.error {
      background: #ffebee;
    }

    .completion-actions {
      display: flex;
      justify-content: center;
      gap: 1rem;
      margin-top: 1rem;
    }

    .help-text {
      font-size: 0.85rem;
      color: #666;
      margin-top: 0.25rem;
    }

    .password-strength {
      display: flex;
      align-items: center;
      gap: 1rem;
      margin-top: 0.5rem;
    }

    .strength-bar {
      flex: 1;
      height: 8px;
      background: #e0e0e0;
      border-radius: 4px;
      overflow: hidden;
    }

    .strength-fill {
      height: 100%;
      transition: all 0.3s;
    }

    .strength-text {
      font-size: 0.85rem;
      font-weight: 500;
    }

    .strength-text.weak { color: #f44336; }
    .strength-text.fair { color: #FF9800; }
    .strength-text.good { color: #8BC34A; }
    .strength-text.strong { color: #4CAF50; }
  `]
})
export class TenantCreateComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  form!: FormGroup;
  currentFormStep = signal<number>(1);
  
  // Slug validation
  slugPrefix = 'https://';
  slugChecking = signal<boolean>(false);
  slugAvailable = signal<boolean | null>(null);
  
  // Password visibility
  showPassword = signal<boolean>(false);
  
  // Submission
  isSubmitting = signal<boolean>(false);
  
  // Provisioning
  isCreating = signal<boolean>(false);
  currentStep = signal<number>(0);
  totalSteps = signal<number>(7);
  steps = signal<ProvisioningStep[]>([]);
  isCompleted = signal<boolean>(false);
  hasError = signal<boolean>(false);
  errorMessage = signal<string>('');
  createdTenantId = signal<string | null>(null);

  constructor(
    private fb: FormBuilder,
    private tenantService: TenantService,
    private provisioningService: ProvisioningService,
    private toastService: ToastService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.setupSlugValidation();
    this.setupProvisioningListeners();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.provisioningService.destroy();
  }

  private initForm(): void {
    this.form = this.fb.group({
      // Step 1
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(200)]],
      slug: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100),
                  Validators.pattern(/^[a-z0-9]+(?:-[a-z0-9]+)*$/)]],
      legalName: [''],
      registrationNumber: [''],
      taxId: [''],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      mobile: [''],
      country: ['Iran', Validators.required],
      city: ['', Validators.required],
      address: [''],
      postalCode: [''],
      website: [''],
      
      // Step 2
      adminFirstName: ['', [Validators.required, Validators.minLength(2)]],
      adminLastName: ['', [Validators.required, Validators.minLength(2)]],
      adminEmail: ['', [Validators.required, Validators.email]],
      adminMobile: [''],
      adminNationalCode: ['', [Validators.pattern(/^\d{10}$/)]],
      adminPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required],
      
      // Step 3
      subscriptionPlan: ['Basic'],
      maxUsers: [10, [Validators.required, Validators.min(1), Validators.max(10000)]],
      maxStorageMB: [1024, [Validators.required, Validators.min(100)]],
      isTrial: [true],
      trialDays: [14, [Validators.min(0), Validators.max(90)]],
      databaseStrategy: [1],
      require2FA: [false],
      sessionTimeoutMinutes: [30, [Validators.min(5), Validators.max(480)]],
      passwordMinLength: [8, [Validators.min(6), Validators.max(32)]],
      passwordRequireSpecialChar: [true],
      primaryColor: ['#1976D2'],
      secondaryColor: ['#424242']
    }, {
      validators: this.passwordMatchValidator
    });
  }

  private passwordMatchValidator(group: FormGroup): { [key: string]: boolean } | null {
    const password = group.get('adminPassword')?.value;
    const confirm = group.get('confirmPassword')?.value;
    return password === confirm ? null : { passwordMismatch: true };
  }

  private setupSlugValidation(): void {
    this.form.get('name')?.valueChanges.pipe(
      takeUntil(this.destroy$)
    ).subscribe(name => {
      if (name && !this.form.get('slug')?.dirty) {
        const slug = this.convertToSlug(name);
        this.form.patchValue({ slug }, { emitEvent: false });
      }
    });

    this.form.get('slug')?.valueChanges.pipe(
      takeUntil(this.destroy$),
      debounceTime(300)
    ).subscribe(async slug => {
      if (slug && this.form.get('slug')?.valid) {
        await this.checkSlugAvailability(slug);
      } else {
        this.slugAvailable.set(null);
      }
    });
  }

  private setupProvisioningListeners(): void {
    this.provisioningService.notification$.pipe(
      takeUntil(this.destroy$)
    ).subscribe(notification => {
      this.updateProvisioningUI(notification);
    });

    this.provisioningService.completion$.pipe(
      takeUntil(this.destroy$)
    ).subscribe(notification => {
      this.isCompleted.set(true);
      this.createdTenantId.set(notification.tenantId || null);
    });

    this.provisioningService.error$.pipe(
      takeUntil(this.destroy$)
    ).subscribe(notification => {
      this.hasError.set(true);
      this.errorMessage.set(notification.message);
    });
  }

  private updateProvisioningUI(notification: ProvisioningNotification): void {
    this.isCreating.set(true);
    this.currentStep.set(notification.currentStep);
    this.totalSteps.set(notification.totalSteps);
    if (notification.steps) {
      this.steps.set(notification.steps);
    }
  }

  private convertToSlug(text: string): string {
    return text
      .toLowerCase()
      .replace(/[^\w\s-]/g, '')
      .replace(/\s+/g, '-')
      .replace(/-+/g, '-')
      .trim();
  }

  private async checkSlugAvailability(slug: string): Promise<void> {
    this.slugChecking.set(true);
    try {
      const available = await firstValueFrom(this.tenantService.checkSlugAvailability(slug));
      this.slugAvailable.set(available);
    } catch {
      this.slugAvailable.set(null);
    } finally {
      this.slugChecking.set(false);
    }
  }

  onSlugChange(): void {
    this.slugAvailable.set(null);
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.form.get(fieldName);
    return field ? field.invalid && field.touched : false;
  }

  passwordStrength = computed(() => {
    const password = this.form.get('adminPassword')?.value || '';
    let score = 0;
    
    if (password.length >= 8) score += 25;
    if (password.length >= 12) score += 15;
    if (/[a-z]/.test(password)) score += 15;
    if (/[A-Z]/.test(password)) score += 15;
    if (/[0-9]/.test(password)) score += 15;
    if (/[^a-zA-Z0-9]/.test(password)) score += 15;
    
    return Math.min(score, 100);
  });

  passwordStrengthClass = computed(() => {
    const strength = this.passwordStrength();
    if (strength < 30) return 'weak';
    if (strength < 50) return 'fair';
    if (strength < 75) return 'good';
    return 'strong';
  });

  passwordStrengthText = computed(() => {
    const strength = this.passwordStrength();
    if (strength < 30) return 'ضعیف';
    if (strength < 50) return 'متوسط';
    if (strength < 75) return 'خوب';
    return 'قوی';
  });

  nextStep(): void {
    if (this.validateCurrentStep()) {
      this.currentFormStep.update(s => Math.min(s + 1, 3));
    }
  }

  prevStep(): void {
    this.currentFormStep.update(s => Math.max(s - 1, 1));
  }

  private validateCurrentStep(): boolean {
    const step = this.currentFormStep();
    let fieldsToValidate: string[] = [];

    switch (step) {
      case 1:
        fieldsToValidate = ['name', 'slug', 'email', 'city'];
        break;
      case 2:
        fieldsToValidate = ['adminFirstName', 'adminLastName', 'adminEmail', 'adminPassword', 'confirmPassword'];
        break;
    }

    fieldsToValidate.forEach(field => {
      this.form.get(field)?.markAsTouched();
    });

    return fieldsToValidate.every(field => this.form.get(field)?.valid);
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid) {
      this.toastService.error('لطفاً تمام فیلدهای الزامی را تکمیل کنید');
      return;
    }

    this.isSubmitting.set(true);

    try {
      const request: CreateTenantRequest = this.form.value;
      
      // Start SignalR connection
      await this.provisioningService.startConnection();
      
      // Create tenant
      const response = await firstValueFrom(this.tenantService.createTenant(request));
      
      // Join tenant group for updates
      await this.provisioningService.joinTenantGroup(request.slug);
      
      this.toastService.success('در حال ایجاد بهره‌بردار...');
    } catch (error: any) {
      this.toastService.error(error.error?.message || 'خطا در ایجاد بهره‌بردار');
      this.isSubmitting.set(false);
    }
  }

  goToTenant(): void {
    const tenantId = this.createdTenantId();
    if (tenantId) {
      this.router.navigate(['/tenants', tenantId]);
    }
  }

  resetForm(): void {
    this.form.reset();
    this.isCreating.set(false);
    this.isCompleted.set(false);
    this.hasError.set(false);
    this.currentFormStep.set(1);
    this.initForm();
  }

  async retryCreation(): Promise<void> {
    this.hasError.set(false);
    this.errorMessage.set('');
    this.isCreating.set(false);
    this.isSubmitting.set(false);
    await this.onSubmit();
  }
}

function debounceTime(delay: number): import('rxjs').MonoTypeOperatorFunction<unknown> {
  return import('rxjs').debounceTime(delay);
}
```

### Tenant List Component

```typescript
// features/tenant-management/tenant-list/tenant-list.component.ts
import { Component, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TenantService, Tenant, TenantStatus, TenantListQuery, PaginatedResult } from '../../../core/tenant/tenant.service';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-tenant-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="tenant-list-container">
      <div class="header">
        <div>
          <h1>مدیریت بهره‌بردارها</h1>
          <p class="subtitle">لیست تمام بهره‌بردارهای ثبت شده</p>
        </div>
        <a routerLink="/tenants/create" class="btn btn-primary">
          + ایجاد بهره‌بردار جدید
        </a>
      </div>

      <!-- Filters -->
      <div class="filters">
        <div class="search-box">
          <input type="text" [(ngModel)]="searchQuery" (input)="onSearch()" 
                 placeholder="جستجو در نام، slug یا ایمیل...">
          <span class="search-icon">🔍</span>
        </div>

        <select [(ngModel)]="statusFilter" (change)="loadTenants()">
          <option [ngValue]="null">همه وضعیت‌ها</option>
          <option [ngValue]="1">در انتظار</option>
          <option [ngValue]="2">فعال</option>
          <option [ngValue]="3">تعلیق</option>
          <option [ngValue]="4">لغو شده</option>
        </select>

        <select [(ngModel)]="sortOrder" (change)="loadTenants()">
          <option value="desc">جدیدترین</option>
          <option value="asc">قدیمی‌ترین</option>
        </select>
      </div>

      <!-- Stats Cards -->
      <div class="stats-grid">
        <div class="stat-card">
          <span class="stat-value">{{ totalCount() }}</span>
          <span class="stat-label">کل بهره‌بردارها</span>
        </div>
        <div class="stat-card active">
          <span class="stat-value">{{ activeCount() }}</span>
          <span class="stat-label">فعال</span>
        </div>
        <div class="stat-card pending">
          <span class="stat-value">{{ pendingCount() }}</span>
          <span class="stat-label">در انتظار</span>
        </div>
        <div class="stat-card suspended">
          <span class="stat-value">{{ suspendedCount() }}</span>
          <span class="stat-label">تعلیق</span>
        </div>
      </div>

      <!-- Table -->
      <div class="table-container">
        <table class="tenants-table">
          <thead>
            <tr>
              <th>بهره‌بردار</th>
              <th>Slug</th>
              <th>ایمیل</th>
              <th>پلن</th>
              <th>کاربران</th>
              <th>وضعیت</th>
              <th>تاریخ ایجاد</th>
              <th>عملیات</th>
            </tr>
          </thead>
          <tbody>
            @for (tenant of tenants(); track tenant.id) {
              <tr>
                <td>
                  <div class="tenant-info">
                    @if (tenant.logoUrl) {
                      <img [src]="tenant.logoUrl" [alt]="tenant.name" class="tenant-logo">
                    } @else {
                      <div class="tenant-avatar" [style.background-color]="tenant.primaryColor">
                        {{ tenant.name.charAt(0) }}
                      </div>
                    }
                    <div>
                      <span class="tenant-name">{{ tenant.name }}</span>
                      @if (tenant.legalName) {
                        <span class="tenant-legal">{{ tenant.legalName }}</span>
                      }
                    </div>
                  </div>
                </td>
                <td>
                  <code>{{ tenant.slug }}</code>
                </td>
                <td>{{ tenant.email }}</td>
                <td>
                  <span class="plan-badge" [class]="tenant.subscriptionPlan.toLowerCase()">
                    {{ tenant.subscriptionPlan }}
                  </span>
                </td>
                <td>
                  <div class="usage-bar">
                    <div class="usage-fill" [style.width.%]="(tenant.currentStorageMB / tenant.maxStorageMB) * 100"></div>
                  </div>
                  <span class="usage-text">{{ tenant.maxUsers }} کاربر</span>
                </td>
                <td>
                  <span class="status-badge" [class]="'status-' + tenant.status">
                    {{ getStatusText(tenant.status) }}
                  </span>
                  @if (tenant.isTrial && tenant.trialEndsAt) {
                    <span class="trial-badge">
                      {{ getDaysRemaining(tenant.trialEndsAt) }} روز
                    </span>
                  }
                </td>
                <td>{{ tenant.createdAt | date:'shortDate' }}</td>
                <td>
                  <div class="actions">
                    <button class="btn-icon" title="مشاهده" 
                            [routerLink]="['/tenants', tenant.id]">
                      👁
                    </button>
                    <button class="btn-icon" title="ویرایش"
                            [routerLink]="['/tenants', tenant.id, 'edit']">
                      ✏️
                    </button>
                    @if (tenant.status === 2) {
                      <button class="btn-icon warning" title="تعلیق"
                              (click)="suspendTenant(tenant)">
                        ⏸
                      </button>
                    }
                    @if (tenant.status === 3) {
                      <button class="btn-icon success" title="فعال‌سازی"
                              (click)="activateTenant(tenant)">
                        ▶️
                      </button>
                    }
                    <button class="btn-icon danger" title="حذف"
                            (click)="deleteTenant(tenant)">
                      🗑
                    </button>
                  </div>
                </td>
              </tr>
            } @empty {
              <tr>
                <td colspan="8" class="empty-state">
                  <div class="empty-content">
                    <span class="empty-icon">📋</span>
                    <p>هیچ بهره‌برداری یافت نشد</p>
                  </div>
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      @if (totalPages() > 1) {
        <div class="pagination">
          <button class="page-btn" [disabled]="page() === 1" (click)="goToPage(page() - 1)">
            قبلی
          </button>
          
          @for (p of visiblePages(); track p) {
            @if (p === -1) {
              <span class="page-ellipsis">...</span>
            } @else {
              <button

### USER
ادامه بده

### ASSISTANT
<details type="reasoning" done="true" duration="0">
<summary>Thought for 0 seconds</summary>
> The user wants me to continue the Angular frontend implementation. I was in the middle of the Tenant List Component, specifically the pagination section. Let me continue from there and complete the component, then add more components and services.
</details>
# ادامه پیاده‌سازی Multi-Tenant - Angular Frontend

## Tenant List Component (ادامه)

```typescript
// features/tenant-management/tenant-list/tenant-list.component.ts (ادامه)
              <button class="page-btn" [class.active]="p === page()" (click)="goToPage(p)">
                {{ p }}
              </button>
            }
          }
          
          <button class="page-btn" [disabled]="page() === totalPages()" (click)="goToPage(page() + 1)">
            بعدی
          </button>
        </div>
      }
    </div>
  `,
  styles: [`
    .tenant-list-container {
      padding: 1.5rem;
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1.5rem;
    }

    .header h1 {
      font-size: 1.5rem;
      margin-bottom: 0.25rem;
    }

    .subtitle {
      color: #666;
      font-size: 0.9rem;
    }

    .btn {
      padding: 0.75rem 1.25rem;
      border: none;
      border-radius: 6px;
      font-size: 0.95rem;
      cursor: pointer;
      text-decoration: none;
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
      transition: all 0.2s;
    }

    .btn-primary {
      background: #1976D2;
      color: white;
    }

    .btn-primary:hover {
      background: #1565C0;
    }

    .filters {
      display: flex;
      gap: 1rem;
      margin-bottom: 1.5rem;
      flex-wrap: wrap;
    }

    .search-box {
      flex: 1;
      min-width: 300px;
      position: relative;
    }

    .search-box input {
      width: 100%;
      padding: 0.75rem 1rem 0.75rem 2.5rem;
      border: 1px solid #ddd;
      border-radius: 8px;
      font-size: 0.95rem;
    }

    .search-icon {
      position: absolute;
      left: 10px;
      top: 50%;
      transform: translateY(-50%);
    }

    .filters select {
      padding: 0.75rem 1rem;
      border: 1px solid #ddd;
      border-radius: 8px;
      font-size: 0.95rem;
      background: white;
      cursor: pointer;
    }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
      gap: 1rem;
      margin-bottom: 1.5rem;
    }

    .stat-card {
      background: white;
      padding: 1.25rem;
      border-radius: 10px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.08);
      text-align: center;
    }

    .stat-card.active {
      border-left: 4px solid #4CAF50;
    }

    .stat-card.pending {
      border-left: 4px solid #FF9800;
    }

    .stat-card.suspended {
      border-left: 4px solid #f44336;
    }

    .stat-value {
      display: block;
      font-size: 2rem;
      font-weight: 700;
      color: #333;
    }

    .stat-label {
      color: #666;
      font-size: 0.85rem;
    }

    .table-container {
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.08);
      overflow: hidden;
    }

    .tenants-table {
      width: 100%;
      border-collapse: collapse;
    }

    .tenants-table th {
      background: #f8f9fa;
      padding: 1rem;
      text-align: right;
      font-weight: 600;
      color: #333;
      border-bottom: 2px solid #e0e0e0;
    }

    .tenants-table td {
      padding: 1rem;
      border-bottom: 1px solid #f0f0f0;
      vertical-align: middle;
    }

    .tenants-table tr:hover {
      background: #f8f9fa;
    }

    .tenant-info {
      display: flex;
      align-items: center;
      gap: 0.75rem;
    }

    .tenant-logo {
      width: 40px;
      height: 40px;
      border-radius: 8px;
      object-fit: cover;
    }

    .tenant-avatar {
      width: 40px;
      height: 40px;
      border-radius: 8px;
      display: flex;
      align-items: center;
      justify-content: center;
      color: white;
      font-weight: 700;
      font-size: 1.1rem;
    }

    .tenant-name {
      display: block;
      font-weight: 600;
    }

    .tenant-legal {
      display: block;
      font-size: 0.85rem;
      color: #666;
    }

    code {
      background: #f5f5f5;
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
      font-size: 0.85rem;
    }

    .plan-badge {
      padding: 0.25rem 0.75rem;
      border-radius: 20px;
      font-size: 0.8rem;
      font-weight: 500;
    }

    .plan-badge.basic {
      background: #e3f2fd;
      color: #1976D2;
    }

    .plan-badge.standard {
      background: #e8f5e9;
      color: #388E3C;
    }

    .plan-badge.professional {
      background: #fff3e0;
      color: #F57C00;
    }

    .plan-badge.enterprise {
      background: #f3e5f5;
      color: #7B1FA2;
    }

    .usage-bar {
      width: 60px;
      height: 6px;
      background: #e0e0e0;
      border-radius: 3px;
      overflow: hidden;
      margin-bottom: 0.25rem;
    }

    .usage-fill {
      height: 100%;
      background: #1976D2;
      border-radius: 3px;
    }

    .usage-text {
      font-size: 0.8rem;
      color: #666;
    }

    .status-badge {
      padding: 0.25rem 0.75rem;
      border-radius: 20px;
      font-size: 0.8rem;
      font-weight: 500;
    }

    .status-badge.status-1 {
      background: #fff3e0;
      color: #F57C00;
    }

    .status-badge.status-2 {
      background: #e8f5e9;
      color: #388E3C;
    }

    .status-badge.status-3 {
      background: #ffebee;
      color: #D32F2F;
    }

    .status-badge.status-4 {
      background: #eceff1;
      color: #546E7A;
    }

    .trial-badge {
      display: block;
      font-size: 0.75rem;
      color: #FF9800;
      margin-top: 0.25rem;
    }

    .actions {
      display: flex;
      gap: 0.5rem;
    }

    .btn-icon {
      width: 32px;
      height: 32px;
      border: none;
      border-radius: 6px;
      background: #f5f5f5;
      cursor: pointer;
      transition: all 0.2s;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .btn-icon:hover {
      background: #e0e0e0;
    }

    .btn-icon.warning:hover {
      background: #fff3e0;
    }

    .btn-icon.success:hover {
      background: #e8f5e9;
    }

    .btn-icon.danger:hover {
      background: #ffebee;
    }

    .empty-state {
      text-align: center;
      padding: 3rem !important;
    }

    .empty-content {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 0.5rem;
    }

    .empty-icon {
      font-size: 3rem;
    }

    .pagination {
      display: flex;
      justify-content: center;
      align-items: center;
      gap: 0.5rem;
      margin-top: 1.5rem;
    }

    .page-btn {
      padding: 0.5rem 1rem;
      border: 1px solid #ddd;
      background: white;
      border-radius: 6px;
      cursor: pointer;
      transition: all 0.2s;
    }

    .page-btn:hover:not(:disabled) {
      background: #f5f5f5;
    }

    .page-btn.active {
      background: #1976D2;
      color: white;
      border-color: #1976D2;
    }

    .page-btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .page-ellipsis {
      padding: 0.5rem;
      color: #666;
    }
  `]
})
export class TenantListComponent implements OnInit {
  // Query params
  searchQuery = '';
  statusFilter: TenantStatus | null = null;
  sortOrder: 'asc' | 'desc' = 'desc';

  // Data
  tenants = signal<Tenant[]>([]);
  totalCount = signal<number>(0);
  page = signal<number>(1);
  pageSize = signal<number>(20);
  totalPages = signal<number>(1);

  // Computed
  activeCount = computed(() => this.tenants().filter(t => t.status === TenantStatus.Active).length);
  pendingCount = computed(() => this.tenants().filter(t => t.status === TenantStatus.Pending).length);
  suspendedCount = computed(() => this.tenants().filter(t => t.status === TenantStatus.Suspended).length);

  visiblePages = computed(() => {
    const current = this.page();
    const total = this.totalPages();
    const pages: number[] = [];

    if (total <= 7) {
      for (let i = 1; i <= total; i++) pages.push(i);
    } else {
      pages.push(1);
      if (current > 3) pages.push(-1);
      for (let i = Math.max(2, current - 1); i <= Math.min(total - 1, current + 1); i++) {
        pages.push(i);
      }
      if (current < total - 2) pages.push(-1);
      pages.push(total);
    }

    return pages;
  });

  constructor(
    private tenantService: TenantService,
    private authService: AuthService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.loadTenants();
  }

  loadTenants(): void {
    const query: TenantListQuery = {
      page: this.page(),
      pageSize: this.pageSize(),
      search: this.searchQuery || undefined,
      status: this.statusFilter || undefined,
      sortOrder: this.sortOrder
    };

    this.tenantService.getTenants(query).subscribe({
      next: (result) => {
        this.tenants.set(result.items);
        this.totalCount.set(result.totalCount);
        this.totalPages.set(result.totalPages);
      },
      error: (error) => {
        this.toastService.error('خطا در بارگذاری لیست بهره‌بردارها');
        console.error(error);
      }
    });
  }

  onSearch(): void {
    this.page.set(1);
    this.loadTenants();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.page.set(page);
    this.loadTenants();
  }

  getStatusText(status: TenantStatus): string {
    const statusMap: Record<TenantStatus, string> = {
      [TenantStatus.Pending]: 'در انتظار',
      [TenantStatus.Active]: 'فعال',
      [TenantStatus.Suspended]: 'تعلیق',
      [TenantStatus.Cancelled]: 'لغو شده'
    };
    return statusMap[status];
  }

  getDaysRemaining(trialEndsAt: string): number {
    const end = new Date(trialEndsAt);
    const now = new Date();
    const diff = end.getTime() - now.getTime();
    return Math.max(0, Math.ceil(diff / (1000 * 60 * 60 * 24)));
  }

  suspendTenant(tenant: Tenant): void {
    if (confirm(`آیا از تعلیق "${tenant.name}" اطمینان دارید؟`)) {
      this.tenantService.suspendTenant(tenant.id).subscribe({
        next: () => {
          this.toastService.success('بهره‌بردار با موفقیت تعلیق شد');
          this.loadTenants();
        },
        error: () => {
          this.toastService.error('خطا در تعلیق بهره‌بردار');
        }
      });
    }
  }

  activateTenant(tenant: Tenant): void {
    this.tenantService.activateTenant(tenant.id).subscribe({
      next: () => {
        this.toastService.success('بهره‌بردار با موفقیت فعال شد');
        this.loadTenants();
      },
      error: () => {
        this.toastService.error('خطا در فعال‌سازی بهره‌بردار');
      }
    });
  }

  deleteTenant(tenant: Tenant): void {
    if (confirm(`آیا از حذف "${tenant.name}" اطمینان دارید؟ این عمل غیرقابل بازگشت است.`)) {
      this.tenantService.deleteTenant(tenant.id).subscribe({
        next: () => {
          this.toastService.success('بهره‌بردار با موفقیت حذف شد');
          this.loadTenants();
        },
        error: () => {
          this.toastService.error('خطا در حذف بهره‌بردار');
        }
      });
    }
  }
}
```

### Auth Guard

```typescript
// core/auth/auth.guard.ts
import { inject } from '@angular/core';
import { Router, CanActivateFn, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isAuthenticated()) {
    router.navigate(['/auth/login'], {
      queryParams: { returnUrl: state.url }
    });
    return false;
  }

  // Check for required permissions
  const requiredPermissions = route.data['permissions'] as string[] | undefined;
  if (requiredPermissions && requiredPermissions.length > 0) {
    if (!authService.hasAllPermissions(requiredPermissions)) {
      router.navigate(['/unauthorized']);
      return false;
    }
  }

  // Check for required roles
  const requiredRoles = route.data['roles'] as string[] | undefined;
  if (requiredRoles && requiredRoles.length > 0) {
    if (!requiredRoles.some(role => authService.hasRole(role))) {
      router.navigate(['/unauthorized']);
      return false;
    }
  }

  return true;
};

export const guestGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    router.navigate(['/dashboard']);
    return false;
  }

  return true;
};

export const tenantGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isAuthenticated()) {
    router.navigate(['/auth/login']);
    return false;
  }

  const tenant = authService.currentTenant();
  if (!tenant) {
    router.navigate(['/select-tenant']);
    return false;
  }

  return true;
};

export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isAdmin()) {
    router.navigate(['/unauthorized']);
    return false;
  }

  return true;
};
```

### Tenant Resolver

```typescript
// core/tenant/tenant.resolver.ts
import { inject } from '@angular/core';
import { ResolveFn, Router } from '@angular/router';
import { TenantService, Tenant } from './tenant.service';
import { of, catchError } from 'rxjs';

export const tenantResolver: ResolveFn<Tenant | null> = (route, state) => {
  const tenantService = inject(TenantService);
  const router = inject(Router);

  const tenantId = route.paramMap.get('id');
  const tenantSlug = route.paramMap.get('slug');

  if (tenantId) {
    return tenantService.getTenantById(tenantId).pipe(
      catchError(() => {
        router.navigate(['/tenants']);
        return of(null);
      })
    );
  }

  if (tenantSlug) {
    return tenantService.getTenantBySlug(tenantSlug).pipe(
      catchError(() => {
        router.navigate(['/tenants']);
        return of(null);
      })
    );
  }

  return of(null);
};

export const provisioningStatusResolver: ResolveFn<any> = (route, state) => {
  const tenantService = inject(TenantService);
  const tenantId = route.paramMap.get('id')!;

  return tenantService.getProvisioningStatus(tenantId);
};
```

### App Routes

```typescript
// app.routes.ts
import { Routes } from '@angular/router';
import { authGuard, guestGuard, adminGuard, tenantGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  // Public routes
  {
    path: '',
    loadComponent: () => import('./features/landing/landing.component')
  },
  {
    path: 'auth',
    canActivate: [guestGuard],
    children: [
      {
        path: 'login',
        loadComponent: () => import('./features/auth/login/login.component')
      },
      {
        path: 'register',
        loadComponent: () => import('./features/auth/register/register.component')
      },
      {
        path: 'forgot-password',
        loadComponent: () => import('./features/auth/forgot-password/forgot-password.component')
      },
      {
        path: 'reset-password',
        loadComponent: () => import('./features/auth/reset-password/reset-password.component')
      }
    ]
  },

  // Authenticated routes
  {
    path: '',
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard.component')
      },
      {
        path: 'profile',
        loadComponent: () => import('./features/profile/profile.component')
      },

      // Tenant Management (Admin only)
      {
        path: 'tenants',
        canActivate: [adminGuard],
        children: [
          {
            path: '',
            loadComponent: () => import('./features/tenant-management/tenant-list/tenant-list.component')
          },
          {
            path: 'create',
            loadComponent: () => import('./features/tenant-management/tenant-create/tenant-create.component')
          },
          {
            path: ':id',
            loadComponent: () => import('./features/tenant-management/tenant-details/tenant-details.component')
          },
          {
            path: ':id/edit',
            loadComponent: () => import('./features/tenant-management/tenant-edit/tenant-edit.component')
          }
        ]
      },

      // User Management
      {
        path: 'users',
        canActivate: [tenantGuard],
        children: [
          {
            path: '',
            loadComponent: () => import('./features/user-management/user-list/user-list.component')
          },
          {
            path: 'invite',
            loadComponent: () => import('./features/user-management/user-invite/user-invite.component')
          },
          {
            path: ':id',
            loadComponent: () => import('./features/user-management/user-edit/user-edit.component')
          }
        ]
      },

      // Settings
      {
        path: 'settings',
        canActivate: [tenantGuard],
        loadComponent: () => import('./features/settings/settings.component')
      }
    ]
  },

  // Error pages
  {
    path: 'unauthorized',
    loadComponent: () => import('./features/error/unauthorized.component')
  },
  {
    path: 'not-found',
    loadComponent: () => import('./features/error/not-found.component')
  },

  // Default redirect
  {
    path: '**',
    redirectTo: ''
  }
];
```

### App Config

```typescript
// app.config.ts
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { routes } from './app.routes';
import { jwtInterceptor } from './core/auth/jwt.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withInterceptors([jwtInterceptor])),
    provideAnimations(),
    
    // Toast notifications
    {
      provide: 'TOAST_CONFIG',
      useValue: {
        position: 'top-right',
        duration: 3000,
        animate: 'slideFromRight'
      }
    }
  ]
};
```

### Environment Configuration

```typescript
// environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001',
  hubUrl: 'https://localhost:5001',
  appUrl: 'http://localhost:4200'
};

// environments/environment.prod.ts
export const environment = {
  production: true,
  apiUrl: 'https://api.yourdomain.com',
  hubUrl: 'https://api.yourdomain.com',
  appUrl: 'https://app.yourdomain.com'
};
```

---

## Docker Compose برای Multi-Database

```yaml
# docker-compose.yml
version: '3.8'

services:
  # Main API
  api:
    build:
      context: .
      dockerfile: src/API/Dockerfile
    ports:
      - "5001:5001"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__MasterDb=Server=mssql,1433;Database=MasterDb;User Id=sa;Password=${SA_PASSWORD};TrustServerCertificate=True
      - Jwt__Secret=${JWT_SECRET}
      - Encryption__Key=${ENCRYPTION_KEY}
      - DB_SERVER=mssql
      - DB_ADMIN_USER=sa
      - DB_ADMIN_PASSWORD=${SA_PASSWORD}
    depends_on:
      - mssql
      - redis
    networks:
      - multitenant-network

  # SQL Server for Master Database
  mssql:
    image: mssql-server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=${SA_PASSWORD}
      - MSSQL_PID=Developer
    ports:
      - "1433:1433"
    volumes:
      - mssql-data:/var/opt/mssql
      - ./init-scripts:/init-scripts
    networks:
      - multitenant-network

  # Redis for caching and SignalR backplane
  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis-data:/data
    networks:
      - multitenant-network

  # Tenant Database Server (for isolated DBs)
  tenant-mssql:
    image: mssql-server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=${TENANT_DB_PASSWORD}
      - MSSQL_PID=Developer
    ports:
      - "1434:1433"
    volumes:
      - tenant-mssql-data:/var/opt/mssql
    networks:
      - multitenant-network

  # Nginx Reverse Proxy
  nginx:
    image: nginx:alpine
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx/nginx.conf:/etc/nginx/nginx.conf:ro
      - ./nginx/ssl:/etc/nginx/ssl:ro
    depends_on:
      - api
    networks:
      - multitenant-network

volumes:
  mssql-data:
  tenant-mssql-data:
  redis-data:

networks:
  multitenant-network:
    driver: bridge
```

```nginx
# nginx/nginx.conf
events {
    worker_connections 1024;
}

http {
    # Upstream for API
    upstream api_backend {
        least_conn;
        server api:5001;
    }

    # Rate limiting zones
    limit_req_zone $binary_remote_addr zone=api_limit:10m rate=10r/s;
    limit_req_zone $binary_remote_addr zone=auth_limit:10m rate=5r/m;

    server {
        listen 80;
        server_name _;
        return 301 https://$host$request_uri;
    }

    server {
        listen 443 ssl http2;
        server_name _;

        ssl_certificate /etc/nginx/ssl/cert.pem;
        ssl_certificate_key /etc/nginx/ssl/key.pem;
        ssl_protocols TLSv1.2 TLSv1.3;
        ssl_ciphers HIGH:!aNULL:!MD5;

        # Security headers
        add_header X-Frame-Options "SAMEORIGIN" always;
        add_header X-Content-Type-Options "nosniff" always;
        add_header X-XSS-Protection "1; mode=block" always;
        add_header Referrer-Policy "strict-origin-when-cross-origin" always;

        # CORS
        add_header Access-Control-Allow-Origin $http_origin always;
        add_header Access-Control-Allow-Methods "GET, POST, PUT, DELETE, OPTIONS" always;
        add_header Access-Control-Allow-Headers "Authorization, Content-Type, X-Tenant-Id" always;
        add_header Access-Control-Allow-Credentials "true" always;

        # API endpoints
        location /api/ {
            limit_req zone=api_limit burst=20 nodelay;
            
            proxy_pass http://api_backend;
            proxy_http_version 1.1;
            proxy_set_header Upgrade $http_upgrade;
            proxy_set_header Connection "upgrade";
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
            proxy_cache_bypass $http_upgrade;
        }

        # Auth endpoints with stricter rate limiting
        location /api/auth/ {
            limit_req zone=auth_limit burst=5 nodelay;
            
            proxy_pass http://api_backend;
            proxy_http_version 1.1;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
        }

        # SignalR
        location /hubs/ {
            proxy_pass http://api_backend;
            proxy_http_version 1.1;
            proxy_set_header Upgrade $http_upgrade;
            proxy_set_header Connection "upgrade";
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        }

        # Health check
        location /health {
            proxy_pass http://api_backend;
            proxy_http_version 1.1;
            proxy_set_header Host $host;
        }

        # Static files
        location / {
            root /usr/share/nginx/html;
            try_files $uri $uri/ /index.html;
        }
    }

    # Tenant subdomain routing
    server {
        listen 443 ssl http2;
        server_name ~^(?<tenant>.+)\.yourdomain\.com$;

        ssl_certificate /etc/nginx/ssl/cert.pem;
        ssl_certificate_key /etc/nginx/ssl/key.pem;

        # Set tenant header for backend
        location / {
            proxy_pass http://api_backend;
            proxy_http_version 1.1;
            proxy_set_header Host $host;
            proxy_set_header X-Tenant-Slug $tenant;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        }
    }
}
```

---

## Migration Commands

```bash
# Create initial migration
dotnet ef migrations add InitialCreate --project src/Infrastructure --startup-project src/API

# Add new migration
dotnet ef migrations add AddTenantTable --project src/Infrastructure --startup-project src/API

# Apply migrations
dotnet ef database update --project src/Infrastructure --startup-project src/API

# Remove last migration
dotnet ef migrations remove --project src/Infrastructure --startup-project src/API

# Generate SQL script
dotnet ef migrations script --project src/Infrastructure --startup-project src/API --output migrations.sql
```

---

## خلاصه معماری

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           CLIENT (Angular 21)                                │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐ │
│  │ Auth Guard  │  │ JWT Intcpt  │  │ Tenant Svc  │  │ Provisioning Hub    │ │
│  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────────────┘ │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                         API GATEWAY / Nginx                                  │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐│
│  │ Rate Limit   │  │ SSL/TLS     │  │ CORS        │  │ Subdomain Routing   ││
│  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────────────┘│
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                           API (.NET 9)                                       │
│  ┌─────────────────────────────────────────────────────────────────────────┐│
│  │                        Middleware Pipeline                               ││
│  │  Auth → TenantResolver → TenantSecurity → Audit → Controller           ││
│  └─────────────────────────────────────────────────────────────────────────┘│
│  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────────────┐│
│  │ CQRS Handlers    │  │ SignalR Hub       │  │ Health Checks            ││
│  │ (MediatR)        │  │ (Provisioning)    │  │                          ││
│  └──────────────────┘  └──────────────────┘  └──────────────────────────┘│
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                    ┌─────────────────┴─────────────────┐
                    ▼                                   ▼
┌─────────────────────────────┐         ┌─────────────────────────────────┐
│      Master Database        │         │      Tenant Databases           │
│  ┌─────────────────────┐   │         │  ┌─────────────────────────────┐│
│  │ Tenants              │   │         │  │ Tenant_ABC (Isolated)       ││
│  │ Users                │   │         │  │ - Products                  ││
│  │ Roles                │   │         │  │ - Orders                    ││
│  │ TenantUsers          │   │         │  │ - Customers                 ││
│  │ AuditLogs            │   │         │  └─────────────────────────────┘│
│  │ ProvisioningLogs     │   │         │  ┌─────────────────────────────┐│
│  └─────────────────────┘   │         │  │ Tenant_XYZ (Isolated)       ││
│                            │         │  │ - Products                  ││
│                            │         │  │ - Orders                    ││
│                            │         │  └─────────────────────────────┘│
└────────────────────────────┘         └─────────────────────────────────┘
```

---

## نکات مهم امنیتی

| نکته | توضیحات |
|------|---------|
| **Tenant Resolution** | از Session/Claims کاربر استفاده شود، نه از Header |
| **Connection Strings** | رمزگذاری شده در دیتابیس ذخیره شوند |
| **IP Restriction** | آی‌پی‌های مجاز در سطح Tenant قابل تنظیم باشند |
| **2FA** | قابل الزام‌سازی برای هر Tenant |
| **Session Timeout** | قابل تنظیم برای هر Tenant |
| **Audit Logging** | تمام عملیات‌ها لاگ شوند |
| **Rate Limiting** | در Nginx و API اعمال شود |
| **SQL Injection** | با EF Core و Parameterized Queries |
| **XSS** | با Content Security Policy |
| **CSRF** | با SameSite Cookies |

این پیاده‌سازی کامل‌ترین و حرفه‌ای‌ترین راه‌حل Multi-Tenant است که تمام نیازهای شما را پوشش می‌دهد. 🎯