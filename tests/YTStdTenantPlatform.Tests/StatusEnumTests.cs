using System;
using System.Collections.Generic;
using Xunit;
using YTStdTenantPlatform.Domain.Enums;

namespace YTStdTenantPlatform.Tests
{
    /// <summary>状态枚举完整性测试 — 验证所有业务状态枚举的值正确且不冲突</summary>
    public class StatusEnumTests
    {
        // ============================================================
        // 租户生命周期状态
        // ============================================================

        [Fact]
        public void TenantLifecycleStatus_HasExpectedValues()
        {
            Assert.Equal(0, (int)TenantLifecycleStatus.Trial);
            Assert.Equal(1, (int)TenantLifecycleStatus.Active);
            Assert.Equal(2, (int)TenantLifecycleStatus.Expiring);
            Assert.Equal(3, (int)TenantLifecycleStatus.Expired);
            Assert.Equal(4, (int)TenantLifecycleStatus.Suspended);
            Assert.Equal(5, (int)TenantLifecycleStatus.Closed);
            Assert.Equal(6, (int)TenantLifecycleStatus.Deleted);
        }

        [Fact]
        public void TenantLifecycleStatus_HasSevenValues()
        {
            var values = Enum.GetValues<TenantLifecycleStatus>();
            Assert.Equal(7, values.Length);
        }

        [Fact]
        public void TenantLifecycleStatus_AllValuesUnique()
        {
            AssertEnumValuesUnique<TenantLifecycleStatus>();
        }

        // ============================================================
        // SaaS 套餐状态
        // ============================================================

        [Fact]
        public void SaasPackageStatus_HasExpectedValues()
        {
            Assert.Equal(0, (int)SaasPackageStatus.Draft);
            Assert.Equal(1, (int)SaasPackageStatus.Published);
            Assert.Equal(2, (int)SaasPackageStatus.Unpublished);
            Assert.Equal(3, (int)SaasPackageStatus.Deleted);
        }

        [Fact]
        public void SaasPackageStatus_HasFourValues()
        {
            var values = Enum.GetValues<SaasPackageStatus>();
            Assert.Equal(4, values.Length);
        }

        [Fact]
        public void SaasPackageStatus_AllValuesUnique()
        {
            AssertEnumValuesUnique<SaasPackageStatus>();
        }

        // ============================================================
        // 订阅状态
        // ============================================================

        [Fact]
        public void SubscriptionStatus_HasExpectedValues()
        {
            Assert.Equal(0, (int)SubscriptionStatus.Active);
            Assert.Equal(1, (int)SubscriptionStatus.Expiring);
            Assert.Equal(2, (int)SubscriptionStatus.Expired);
            Assert.Equal(3, (int)SubscriptionStatus.Suspended);
            Assert.Equal(4, (int)SubscriptionStatus.Cancelled);
        }

        [Fact]
        public void SubscriptionStatus_HasFiveValues()
        {
            var values = Enum.GetValues<SubscriptionStatus>();
            Assert.Equal(5, values.Length);
        }

        [Fact]
        public void SubscriptionStatus_AllValuesUnique()
        {
            AssertEnumValuesUnique<SubscriptionStatus>();
        }

        // ============================================================
        // 试用状态
        // ============================================================

        [Fact]
        public void TrialStatus_HasExpectedValues()
        {
            Assert.Equal(0, (int)TrialStatus.Active);
            Assert.Equal(1, (int)TrialStatus.Expired);
            Assert.Equal(2, (int)TrialStatus.Converted);
            Assert.Equal(3, (int)TrialStatus.Cancelled);
        }

        [Fact]
        public void TrialStatus_HasFourValues()
        {
            var values = Enum.GetValues<TrialStatus>();
            Assert.Equal(4, values.Length);
        }

        [Fact]
        public void TrialStatus_AllValuesUnique()
        {
            AssertEnumValuesUnique<TrialStatus>();
        }

        // ============================================================
        // 平台用户状态
        // ============================================================

        [Fact]
        public void PlatformUserStatus_HasExpectedValues()
        {
            Assert.Equal(0, (int)PlatformUserStatus.Active);
            Assert.Equal(1, (int)PlatformUserStatus.Disabled);
            Assert.Equal(2, (int)PlatformUserStatus.Deleted);
            Assert.Equal(3, (int)PlatformUserStatus.Locked);
        }

        [Fact]
        public void PlatformUserStatus_HasFourValues()
        {
            var values = Enum.GetValues<PlatformUserStatus>();
            Assert.Equal(4, values.Length);
        }

        [Fact]
        public void PlatformUserStatus_AllValuesUnique()
        {
            AssertEnumValuesUnique<PlatformUserStatus>();
        }

        // ============================================================
        // 平台角色状态
        // ============================================================

        [Fact]
        public void PlatformRoleStatus_HasExpectedValues()
        {
            Assert.Equal(0, (int)PlatformRoleStatus.Active);
            Assert.Equal(1, (int)PlatformRoleStatus.Disabled);
        }

        [Fact]
        public void PlatformRoleStatus_HasTwoValues()
        {
            var values = Enum.GetValues<PlatformRoleStatus>();
            Assert.Equal(2, values.Length);
        }

        // ============================================================
        // 发票状态
        // ============================================================

        [Fact]
        public void InvoiceStatus_HasExpectedValues()
        {
            Assert.Equal(0, (int)InvoiceStatus.Pending);
            Assert.Equal(1, (int)InvoiceStatus.Issued);
            Assert.Equal(2, (int)InvoiceStatus.Paid);
            Assert.Equal(3, (int)InvoiceStatus.Overdue);
            Assert.Equal(4, (int)InvoiceStatus.Cancelled);
        }

        [Fact]
        public void InvoiceStatus_HasFiveValues()
        {
            var values = Enum.GetValues<InvoiceStatus>();
            Assert.Equal(5, values.Length);
        }

        [Fact]
        public void InvoiceStatus_AllValuesUnique()
        {
            AssertEnumValuesUnique<InvoiceStatus>();
        }

        // ============================================================
        // 支付状态
        // ============================================================

        [Fact]
        public void PaymentStatus_HasExpectedValues()
        {
            Assert.Equal(0, (int)PaymentStatus.Pending);
            Assert.Equal(1, (int)PaymentStatus.Paid);
            Assert.Equal(2, (int)PaymentStatus.Failed);
            Assert.Equal(3, (int)PaymentStatus.Cancelled);
            Assert.Equal(4, (int)PaymentStatus.Refunded);
            Assert.Equal(5, (int)PaymentStatus.PartialRefunded);
        }

        [Fact]
        public void PaymentStatus_HasSixValues()
        {
            var values = Enum.GetValues<PaymentStatus>();
            Assert.Equal(6, values.Length);
        }

        [Fact]
        public void PaymentStatus_AllValuesUnique()
        {
            AssertEnumValuesUnique<PaymentStatus>();
        }

        // ============================================================
        // 退款状态
        // ============================================================

        [Fact]
        public void RefundStatus_HasExpectedValues()
        {
            Assert.Equal(0, (int)RefundStatus.Pending);
            Assert.Equal(1, (int)RefundStatus.Success);
            Assert.Equal(2, (int)RefundStatus.Failed);
            Assert.Equal(3, (int)RefundStatus.Cancelled);
        }

        [Fact]
        public void RefundStatus_HasFourValues()
        {
            var values = Enum.GetValues<RefundStatus>();
            Assert.Equal(4, values.Length);
        }

        [Fact]
        public void RefundStatus_AllValuesUnique()
        {
            AssertEnumValuesUnique<RefundStatus>();
        }

        // ============================================================
        // Webhook 交付状态
        // ============================================================

        [Fact]
        public void WebhookDeliveryStatus_AllValuesUnique()
        {
            AssertEnumValuesUnique<WebhookDeliveryStatus>();
        }

        // ============================================================
        // 通知发送状态
        // ============================================================

        [Fact]
        public void NotificationSendStatus_AllValuesUnique()
        {
            AssertEnumValuesUnique<NotificationSendStatus>();
        }

        // ============================================================
        // 菜单类型
        // ============================================================

        [Fact]
        public void MenuType_AllValuesUnique()
        {
            AssertEnumValuesUnique<MenuType>();
        }

        // ============================================================
        // 权限类型
        // ============================================================

        [Fact]
        public void PermissionType_AllValuesUnique()
        {
            AssertEnumValuesUnique<PermissionType>();
        }

        // ============================================================
        // 数据隔离模式
        // ============================================================

        [Fact]
        public void TenantIsolationMode_AllValuesUnique()
        {
            AssertEnumValuesUnique<TenantIsolationMode>();
        }

        // ============================================================
        // 登录状态
        // ============================================================

        [Fact]
        public void LoginStatus_AllValuesUnique()
        {
            AssertEnumValuesUnique<LoginStatus>();
        }

        // ============================================================
        // 所有枚举类型数量验证
        // ============================================================

        [Fact]
        public void DomainEnums_Namespace_ContainsExpectedCount()
        {
            var assembly = typeof(TenantLifecycleStatus).Assembly;
            int enumCount = 0;
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsEnum && type.Namespace == "YTStdTenantPlatform.Domain.Enums")
                {
                    enumCount++;
                }
            }
            // 至少 40+ 个枚举类型（当前约 55 个）
            Assert.True(enumCount >= 40, "Domain.Enums 命名空间应至少有 40 个枚举，实际: " + enumCount);
        }

        [Fact]
        public void AllDomainEnums_HaveAtLeastTwoValues()
        {
            var assembly = typeof(TenantLifecycleStatus).Assembly;
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsEnum && type.Namespace == "YTStdTenantPlatform.Domain.Enums")
                {
                    var values = Enum.GetValues(type);
                    Assert.True(values.Length >= 2,
                        type.Name + " 枚举至少应有 2 个值");
                }
            }
        }

        // ============================================================
        // 辅助方法
        // ============================================================

        /// <summary>验证枚举的所有值唯一（无重复的 int 值）</summary>
        private static void AssertEnumValuesUnique<T>() where T : struct, Enum
        {
            var values = Enum.GetValues<T>();
            var intSet = new HashSet<int>();
            foreach (var v in values)
            {
                var intVal = Convert.ToInt32(v);
                Assert.True(intSet.Add(intVal),
                    typeof(T).Name + " 包含重复的 int 值: " + intVal);
            }
        }
    }
}
