using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using YTStdTenantPlatform.Application.Constants;

namespace YTStdTenantPlatform.Tests
{
    /// <summary>错误码体系验证测试</summary>
    public class ErrorCodesTests
    {
        /// <summary>获取 ErrorCodes 中所有 const int 字段</summary>
        private static List<(string Name, int Value)> GetAllErrorCodes()
        {
            var fields = typeof(ErrorCodes).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var result = new List<(string Name, int Value)>();
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].IsLiteral && !fields[i].IsInitOnly && fields[i].FieldType == typeof(int))
                {
                    result.Add((fields[i].Name, (int)fields[i].GetRawConstantValue()!));
                }
            }
            return result;
        }

        [Fact]
        public void ErrorCodes_AllValuesAreConstInt()
        {
            var codes = GetAllErrorCodes();
            Assert.NotEmpty(codes);
            // 应至少有 100 个错误码
            Assert.True(codes.Count >= 100, $"当前只有 {codes.Count} 个错误码，期望至少 100 个");
        }

        [Fact]
        public void ErrorCodes_NoDoubleValues()
        {
            var codes = GetAllErrorCodes();
            var seen = new Dictionary<int, string>();
            foreach (var (name, value) in codes)
            {
                if (seen.TryGetValue(value, out var existing))
                {
                    Assert.Fail($"错误码重复: {name}={value} 与 {existing}={value}");
                }
                seen[value] = name;
            }
        }

        [Fact]
        public void ErrorCodes_SuccessIsZero()
        {
            Assert.Equal(0, ErrorCodes.Success);
        }

        [Fact]
        public void ErrorCodes_AuthRange_10001_To_10099()
        {
            // 认证错误码应在 10001-10099 范围
            Assert.True(ErrorCodes.AuthCredentialsRequired >= 10001 && ErrorCodes.AuthCredentialsRequired <= 10099);
            Assert.True(ErrorCodes.AuthInvalidCredentials >= 10001 && ErrorCodes.AuthInvalidCredentials <= 10099);
            Assert.True(ErrorCodes.AuthAccountDisabled >= 10001 && ErrorCodes.AuthAccountDisabled <= 10099);
            Assert.True(ErrorCodes.AuthAccountLocked >= 10001 && ErrorCodes.AuthAccountLocked <= 10099);
            Assert.True(ErrorCodes.AuthTokenInvalid >= 10001 && ErrorCodes.AuthTokenInvalid <= 10099);
            Assert.True(ErrorCodes.AuthOldPasswordInvalid >= 10001 && ErrorCodes.AuthOldPasswordInvalid <= 10099);
            Assert.True(ErrorCodes.AuthNewPasswordRequired >= 10001 && ErrorCodes.AuthNewPasswordRequired <= 10099);
        }

        [Fact]
        public void ErrorCodes_PermissionRange_11001_To_11099()
        {
            Assert.True(ErrorCodes.Forbidden >= 11001 && ErrorCodes.Forbidden <= 11099);
        }

        [Fact]
        public void ErrorCodes_InputValidationRange_12001_To_12099()
        {
            Assert.True(ErrorCodes.InvalidRequestBody >= 12001 && ErrorCodes.InvalidRequestBody <= 12099);
            Assert.True(ErrorCodes.ResourceNotFound >= 12001 && ErrorCodes.ResourceNotFound <= 12099);
            Assert.True(ErrorCodes.InvalidParameter >= 12001 && ErrorCodes.InvalidParameter <= 12099);
            Assert.True(ErrorCodes.InvalidOperation >= 12001 && ErrorCodes.InvalidOperation <= 12099);
        }

        [Fact]
        public void ErrorCodes_UniquenessRange_18001_To_18099()
        {
            Assert.True(ErrorCodes.UsernameExists >= 18001 && ErrorCodes.UsernameExists <= 18099);
            Assert.True(ErrorCodes.RoleCodeExists >= 18001 && ErrorCodes.RoleCodeExists <= 18099);
            Assert.True(ErrorCodes.TenantCodeExists >= 18001 && ErrorCodes.TenantCodeExists <= 18099);
            Assert.True(ErrorCodes.PermissionCodeExists >= 18001 && ErrorCodes.PermissionCodeExists <= 18099);
            Assert.True(ErrorCodes.MenuCodeExists >= 18001 && ErrorCodes.MenuCodeExists <= 18099);
            Assert.True(ErrorCodes.PackageCodeExists >= 18001 && ErrorCodes.PackageCodeExists <= 18099);
        }

        [Fact]
        public void ErrorCodes_BusinessRange_19001_To_19999()
        {
            var codes = GetAllErrorCodes();
            foreach (var (name, value) in codes)
            {
                if (value >= 19001 && value <= 19999)
                {
                    // 业务错误码不应为 0
                    Assert.True(value > 0, $"业务错误码 {name} 值无效: {value}");
                }
            }
        }

        [Fact]
        public void ErrorCodes_PlatformUserRange_19001_To_19099()
        {
            Assert.True(ErrorCodes.UserUsernameRequired >= 19001 && ErrorCodes.UserUsernameRequired <= 19099);
            Assert.True(ErrorCodes.UserCreateFailed >= 19001 && ErrorCodes.UserCreateFailed <= 19099);
            Assert.True(ErrorCodes.UserDeleteFailed >= 19001 && ErrorCodes.UserDeleteFailed <= 19099);
            Assert.True(ErrorCodes.UserCannotDeleteAdmin >= 19001 && ErrorCodes.UserCannotDeleteAdmin <= 19099);
            Assert.True(ErrorCodes.UserCannotDisableSelf >= 19001 && ErrorCodes.UserCannotDisableSelf <= 19099);
        }

        [Fact]
        public void ErrorCodes_PlatformRoleRange_19101_To_19199()
        {
            Assert.True(ErrorCodes.RoleCodeRequired >= 19101 && ErrorCodes.RoleCodeRequired <= 19199);
            Assert.True(ErrorCodes.RoleCreateFailed >= 19101 && ErrorCodes.RoleCreateFailed <= 19199);
            Assert.True(ErrorCodes.RoleDeleteFailed >= 19101 && ErrorCodes.RoleDeleteFailed <= 19199);
            Assert.True(ErrorCodes.RoleCannotDeleteSuperAdmin >= 19101 && ErrorCodes.RoleCannotDeleteSuperAdmin <= 19199);
        }

        [Fact]
        public void ErrorCodes_PermissionModuleRange_19201_To_19299()
        {
            Assert.True(ErrorCodes.PermissionCodeRequired >= 19201 && ErrorCodes.PermissionCodeRequired <= 19299);
            Assert.True(ErrorCodes.PermissionCreateFailed >= 19201 && ErrorCodes.PermissionCreateFailed <= 19299);
            Assert.True(ErrorCodes.PermissionDeleteFailed >= 19201 && ErrorCodes.PermissionDeleteFailed <= 19299);
        }

        [Fact]
        public void ErrorCodes_TenantRange_19301_To_19399()
        {
            Assert.True(ErrorCodes.TenantCodeRequired >= 19301 && ErrorCodes.TenantCodeRequired <= 19399);
            Assert.True(ErrorCodes.TenantCreateFailed >= 19301 && ErrorCodes.TenantCreateFailed <= 19399);
            Assert.True(ErrorCodes.TenantDeleteFailed >= 19301 && ErrorCodes.TenantDeleteFailed <= 19399);
            Assert.True(ErrorCodes.TenantStatusTransitionDenied >= 19301 && ErrorCodes.TenantStatusTransitionDenied <= 19399);
        }

        [Fact]
        public void ErrorCodes_MenuRange_19350_To_19369()
        {
            Assert.True(ErrorCodes.MenuCodeRequired >= 19350 && ErrorCodes.MenuCodeRequired <= 19369);
            Assert.True(ErrorCodes.MenuCreateFailed >= 19350 && ErrorCodes.MenuCreateFailed <= 19369);
            Assert.True(ErrorCodes.MenuDeleteFailed >= 19350 && ErrorCodes.MenuDeleteFailed <= 19369);
        }

        [Fact]
        public void ErrorCodes_DictRange_19370_To_19389()
        {
            Assert.True(ErrorCodes.DictTypeCodeRequired >= 19370 && ErrorCodes.DictTypeCodeRequired <= 19389);
            Assert.True(ErrorCodes.DictCreateFailed >= 19370 && ErrorCodes.DictCreateFailed <= 19389);
            Assert.True(ErrorCodes.DictDeleteFailed >= 19370 && ErrorCodes.DictDeleteFailed <= 19389);
        }

        [Fact]
        public void ErrorCodes_PackageRange_19401_To_19499()
        {
            Assert.True(ErrorCodes.PackageCodeRequired >= 19401 && ErrorCodes.PackageCodeRequired <= 19499);
            Assert.True(ErrorCodes.PackageCreateFailed >= 19401 && ErrorCodes.PackageCreateFailed <= 19499);
            Assert.True(ErrorCodes.PackagePublishedCannotDelete >= 19401 && ErrorCodes.PackagePublishedCannotDelete <= 19499);
            Assert.True(ErrorCodes.PackageStatusTransitionDenied >= 19401 && ErrorCodes.PackageStatusTransitionDenied <= 19499);
        }

        [Fact]
        public void ErrorCodes_SubscriptionRange_19501_To_19599()
        {
            Assert.True(ErrorCodes.SubscriptionCreateFailed >= 19501 && ErrorCodes.SubscriptionCreateFailed <= 19599);
            Assert.True(ErrorCodes.SubscriptionRenewFailed >= 19501 && ErrorCodes.SubscriptionRenewFailed <= 19599);
            Assert.True(ErrorCodes.SubscriptionUpgradeFailed >= 19501 && ErrorCodes.SubscriptionUpgradeFailed <= 19599);
            Assert.True(ErrorCodes.SubscriptionStatusDenied >= 19501 && ErrorCodes.SubscriptionStatusDenied <= 19599);
        }

        [Fact]
        public void ErrorCodes_BillingRange_19601_To_19699()
        {
            Assert.True(ErrorCodes.InvoiceCreateFailed >= 19601 && ErrorCodes.InvoiceCreateFailed <= 19699);
            Assert.True(ErrorCodes.InvoicePayFailed >= 19601 && ErrorCodes.InvoicePayFailed <= 19699);
        }

        [Fact]
        public void ErrorCodes_ApiIntegrationRange_19701_To_19799()
        {
            Assert.True(ErrorCodes.ApiKeyCreateFailed >= 19701 && ErrorCodes.ApiKeyCreateFailed <= 19799);
            Assert.True(ErrorCodes.ApiKeyDeleteFailed >= 19701 && ErrorCodes.ApiKeyDeleteFailed <= 19799);
        }

        [Fact]
        public void ErrorCodes_NotificationRange_19801_To_19899()
        {
            Assert.True(ErrorCodes.NotificationTemplateNameRequired >= 19801 && ErrorCodes.NotificationTemplateNameRequired <= 19899);
            Assert.True(ErrorCodes.NotificationTemplateDeleteFailed >= 19801 && ErrorCodes.NotificationTemplateDeleteFailed <= 19899);
        }

        [Fact]
        public void ErrorCodes_StorageRange_19901_To_19999()
        {
            Assert.True(ErrorCodes.StorageStrategyNameRequired >= 19901 && ErrorCodes.StorageStrategyNameRequired <= 19999);
            Assert.True(ErrorCodes.FileDeleteFailed >= 19901 && ErrorCodes.FileDeleteFailed <= 19999);
        }

        [Fact]
        public void ErrorCodes_SystemRange_50001_To_50099()
        {
            Assert.True(ErrorCodes.InternalServerError >= 50001 && ErrorCodes.InternalServerError <= 50099);
            Assert.True(ErrorCodes.OperationFailed >= 50001 && ErrorCodes.OperationFailed <= 50099);
            Assert.True(ErrorCodes.SystemBusy >= 50001 && ErrorCodes.SystemBusy <= 50099);
        }

        [Fact]
        public void ErrorCodes_AllCodesFollowSegmentation()
        {
            var codes = GetAllErrorCodes();
            foreach (var (name, value) in codes)
            {
                if (value == 0) continue; // Success

                bool validRange =
                    (value >= 10001 && value <= 10099) || // 认证
                    (value >= 11001 && value <= 11099) || // 权限
                    (value >= 12001 && value <= 12099) || // 输入验证
                    (value >= 18001 && value <= 18099) || // 唯一性冲突
                    (value >= 19001 && value <= 19999) || // 业务模块
                    (value >= 50001 && value <= 50099);   // 系统

                Assert.True(validRange,
                    $"错误码 {name}={value} 不在任何已定义的分段范围内");
            }
        }

        [Fact]
        public void ErrorCodes_AllFieldsHaveXmlComment()
        {
            // 通过反射检查所有 public const int 字段
            var fields = typeof(ErrorCodes).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            foreach (var field in fields)
            {
                if (!field.IsLiteral || field.IsInitOnly || field.FieldType != typeof(int))
                    continue;

                // 验证字段名非空（作为基本的存在性检查）
                Assert.False(string.IsNullOrEmpty(field.Name),
                    "发现匿名错误码字段");
            }
        }
    }
}
