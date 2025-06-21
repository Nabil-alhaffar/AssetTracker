namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents the type of audit event.
    /// </summary>
    public enum AuditEventType
    {
        /// <summary>
        /// User account created.
        /// </summary>
        AccountCreated,

        /// <summary>
        /// User account updated.
        /// </summary>
        AccountUpdated,

        /// <summary>
        /// User account deleted.
        /// </summary>
        AccountDeleted,

        /// <summary>
        /// User logged in.
        /// </summary>
        Login,

        /// <summary>
        /// User logged out.
        /// </summary>
        Logout,

        /// <summary>
        /// Failed login attempt.
        /// </summary>
        FailedLogin,

        /// <summary>
        /// Password changed.
        /// </summary>
        PasswordChanged,

        /// <summary>
        /// Password reset requested.
        /// </summary>
        PasswordResetRequested,

        /// <summary>
        /// Two-factor authentication enabled.
        /// </summary>
        TwoFactorEnabled,

        /// <summary>
        /// Two-factor authentication disabled.
        /// </summary>
        TwoFactorDisabled,

        /// <summary>
        /// Account locked.
        /// </summary>
        AccountLocked,

        /// <summary>
        /// Account unlocked.
        /// </summary>
        AccountUnlocked,

        /// <summary>
        /// KYC status updated.
        /// </summary>
        KycStatusUpdated,

        /// <summary>
        /// AML status updated.
        /// </summary>
        AmlStatusUpdated,

        /// <summary>
        /// Trading permissions updated.
        /// </summary>
        TradingPermissionsUpdated,

        /// <summary>
        /// Margin approval status updated.
        /// </summary>
        MarginApprovalUpdated,

        /// <summary>
        /// Options approval status updated.
        /// </summary>
        OptionsApprovalUpdated,

        /// <summary>
        /// Crypto approval status updated.
        /// </summary>
        CryptoApprovalUpdated,

        /// <summary>
        /// Compliance note added.
        /// </summary>
        ComplianceNoteAdded,

        /// <summary>
        /// Regulatory restriction added.
        /// </summary>
        RegulatoryRestrictionAdded,

        /// <summary>
        /// Account status changed.
        /// </summary>
        AccountStatusChanged,

        /// <summary>
        /// Profile information updated.
        /// </summary>
        ProfileUpdated,

        /// <summary>
        /// Preferences updated.
        /// </summary>
        PreferencesUpdated,

        /// <summary>
        /// Security settings updated.
        /// </summary>
        SecuritySettingsUpdated,


        /// <summary>
        /// Role Added.
        /// </summary>
        RoleAdded,


        /// <summary>
        /// RoleRemoved.
        /// </summary>
        RoleRemoved,

        /// <summary>
        /// Other audit event.
        /// </summary>
        Other
    }
} 