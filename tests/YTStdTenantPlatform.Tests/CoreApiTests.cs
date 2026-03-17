using System;
using System.Collections.Generic;
using Xunit;
using YTStdTenantPlatform.Application.Dtos;
using YTStdTenantPlatform.Application.Services;

namespace YTStdTenantPlatform.Tests
{
    /// <summary>核心 API DTO、服务、端点测试</summary>
    public class CoreApiTests
    {
        // ──────────────────────────────────────────────────────
        // ApiResult 测试
        // ──────────────────────────────────────────────────────

        [Fact]
        public void ApiResult_Ok_ReturnsSuccess()
        {
            var result = ApiResult.Ok();
            Assert.True(result.Success);
            Assert.Equal("操作成功", result.Message);
        }

        [Fact]
        public void ApiResult_Fail_ReturnsFailure()
        {
            var result = ApiResult.Fail("测试失败");
            Assert.False(result.Success);
            Assert.Equal("测试失败", result.Message);
        }

        [Fact]
        public void ApiResultT_Ok_ReturnsDataAndSuccess()
        {
            var result = ApiResult<long>.Ok(42L);
            Assert.True(result.Success);
            Assert.Equal(42L, result.Data);
            Assert.Equal("操作成功", result.Message);
        }

        [Fact]
        public void ApiResultT_Fail_ReturnsFailureAndNoData()
        {
            var result = ApiResult<long>.Fail("创建失败");
            Assert.False(result.Success);
            Assert.Equal("创建失败", result.Message);
            Assert.Equal(0L, result.Data);
        }

        // ──────────────────────────────────────────────────────
        // PagedRequest 测试
        // ──────────────────────────────────────────────────────

        [Fact]
        public void PagedRequest_DefaultValues()
        {
            var req = new PagedRequest();
            Assert.Equal(1, req.Page);
            Assert.Equal(20, req.PageSize);
            Assert.Null(req.Keyword);
            Assert.Null(req.Status);
        }

        [Fact]
        public void PagedRequest_NormalizedPage_ClampedTo1()
        {
            var req = new PagedRequest { Page = 0 };
            Assert.Equal(1, req.NormalizedPage);

            req.Page = -5;
            Assert.Equal(1, req.NormalizedPage);

            req.Page = 3;
            Assert.Equal(3, req.NormalizedPage);
        }

        [Fact]
        public void PagedRequest_NormalizedPageSize_ClampedTo1And200()
        {
            var req = new PagedRequest { PageSize = 0 };
            Assert.Equal(20, req.NormalizedPageSize);

            req.PageSize = -1;
            Assert.Equal(20, req.NormalizedPageSize);

            req.PageSize = 500;
            Assert.Equal(200, req.NormalizedPageSize);

            req.PageSize = 50;
            Assert.Equal(50, req.NormalizedPageSize);
        }

        [Fact]
        public void PagedRequest_Offset_Computed()
        {
            var req = new PagedRequest { Page = 3, PageSize = 10 };
            Assert.Equal(20, req.Offset);
        }

        // ──────────────────────────────────────────────────────
        // PagedResult 测试
        // ──────────────────────────────────────────────────────

        [Fact]
        public void PagedResult_TotalPages_Computed()
        {
            var result = new PagedResult<string>
            {
                Total = 55,
                PageSize = 20,
                Page = 1,
                Items = Array.Empty<string>()
            };
            Assert.Equal(3, result.TotalPages);
        }

        [Fact]
        public void PagedResult_TotalPages_ExactDivision()
        {
            var result = new PagedResult<string>
            {
                Total = 40,
                PageSize = 20,
                Page = 1,
                Items = Array.Empty<string>()
            };
            Assert.Equal(2, result.TotalPages);
        }

        [Fact]
        public void PagedResult_TotalPages_ZeroPageSize()
        {
            var result = new PagedResult<string>
            {
                Total = 10,
                PageSize = 0,
                Page = 1,
                Items = Array.Empty<string>()
            };
            Assert.Equal(0, result.TotalPages);
        }

        // ──────────────────────────────────────────────────────
        // DTO 结构测试
        // ──────────────────────────────────────────────────────

        [Fact]
        public void PlatformUserDto_HasExpectedProperties()
        {
            var dto = new PlatformUserDto
            {
                Id = 1,
                Username = "admin",
                Email = "admin@test.com",
                DisplayName = "管理员",
                Status = "active",
                MfaEnabled = false,
                CreatedAt = DateTime.UtcNow
            };
            Assert.Equal(1, dto.Id);
            Assert.Equal("admin", dto.Username);
            Assert.Equal("active", dto.Status);
        }

        [Fact]
        public void PlatformRoleDto_HasExpectedProperties()
        {
            var dto = new PlatformRoleDto
            {
                Id = 1,
                Code = "super_admin",
                Name = "超级管理员",
                Status = "active"
            };
            Assert.Equal("super_admin", dto.Code);
        }

        [Fact]
        public void TenantDto_HasExpectedProperties()
        {
            var dto = new TenantDto
            {
                Id = 1,
                TenantCode = "demo",
                TenantName = "演示租户",
                LifecycleStatus = "active",
                IsolationMode = "shared",
                Enabled = true
            };
            Assert.Equal("demo", dto.TenantCode);
            Assert.True(dto.Enabled);
        }

        [Fact]
        public void TenantGroupDto_Children_CanBeEmpty()
        {
            var dto = new TenantGroupDto
            {
                Id = 1,
                GroupCode = "g1",
                GroupName = "分组1",
                Children = new List<TenantGroupDto>()
            };
            Assert.NotNull(dto.Children);
            Assert.Empty(dto.Children);
        }

        [Fact]
        public void PlatformPermissionDto_Children_CanBeNull()
        {
            var dto = new PlatformPermissionDto
            {
                Id = 1,
                Code = "platform:user:list",
                Name = "用户列表",
                PermissionType = "api"
            };
            Assert.Null(dto.Children);
        }

        // ──────────────────────────────────────────────────────
        // CreateRequest 验证测试
        // ──────────────────────────────────────────────────────

        [Fact]
        public void CreatePlatformUserRequest_DefaultValues()
        {
            var req = new CreatePlatformUserRequest();
            Assert.Equal("", req.Username);
            Assert.Equal("", req.Email);
            Assert.Equal("", req.Password);
            Assert.Null(req.Phone);
        }

        [Fact]
        public void CreateTenantRequest_DefaultValues()
        {
            var req = new CreateTenantRequest();
            Assert.Equal("manual", req.SourceType);
            Assert.Equal("shared", req.IsolationMode);
            Assert.Equal("zh-CN", req.DefaultLanguage);
            Assert.Equal("Asia/Shanghai", req.DefaultTimezone);
        }

        [Fact]
        public void CreatePlatformRoleRequest_DefaultValues()
        {
            var req = new CreatePlatformRoleRequest();
            Assert.Equal("", req.Code);
            Assert.Equal("", req.Name);
            Assert.Null(req.Description);
        }

        [Fact]
        public void RolePermissionBindRequest_DefaultEmpty()
        {
            var req = new RolePermissionBindRequest();
            Assert.Empty(req.PermissionIds);
        }

        [Fact]
        public void RoleMemberBindRequest_DefaultEmpty()
        {
            var req = new RoleMemberBindRequest();
            Assert.Empty(req.UserIds);
        }

        [Fact]
        public void TagBindRequest_DefaultEmpty()
        {
            var req = new TagBindRequest();
            Assert.Empty(req.TagIds);
        }

        [Fact]
        public void SaveTenantResourceQuotaRequest_Defaults()
        {
            var req = new SaveTenantResourceQuotaRequest();
            Assert.Equal(0, req.TenantRefId);
            Assert.Equal("", req.QuotaType);
            Assert.Equal(0, req.QuotaLimit);
        }

        [Fact]
        public void SaveTenantFeatureFlagRequest_Defaults()
        {
            var req = new SaveTenantFeatureFlagRequest();
            Assert.Equal("all", req.RolloutType);
            Assert.False(req.Enabled);
        }

        [Fact]
        public void SaveTenantParameterRequest_Defaults()
        {
            var req = new SaveTenantParameterRequest();
            Assert.Equal("string", req.ParamType);
        }

        // ──────────────────────────────────────────────────────
        // 租户配置 DTO 测试
        // ──────────────────────────────────────────────────────

        [Fact]
        public void TenantSystemConfigDto_Properties()
        {
            var dto = new TenantSystemConfigDto
            {
                Id = 1,
                TenantRefId = 100,
                SystemName = "测试系统",
                DefaultLanguage = "zh-CN"
            };
            Assert.Equal("测试系统", dto.SystemName);
        }

        [Fact]
        public void TenantFeatureFlagDto_Properties()
        {
            var dto = new TenantFeatureFlagDto
            {
                Id = 1,
                FeatureKey = "advanced_search",
                Enabled = true,
                RolloutType = "all"
            };
            Assert.True(dto.Enabled);
            Assert.Equal("advanced_search", dto.FeatureKey);
        }

        [Fact]
        public void TenantDomainDto_Properties()
        {
            var dto = new TenantDomainDto
            {
                Id = 1,
                Domain = "demo.example.com",
                DomainType = "custom",
                IsPrimary = false,
                VerificationStatus = "pending"
            };
            Assert.Equal("pending", dto.VerificationStatus);
        }

        [Fact]
        public void TenantTagDto_Properties()
        {
            var dto = new TenantTagDto
            {
                Id = 1,
                TagKey = "industry",
                TagValue = "tech",
                TagType = "custom"
            };
            Assert.Equal("industry", dto.TagKey);
        }

        [Fact]
        public void TenantLifecycleEventDto_Properties()
        {
            var dto = new TenantLifecycleEventDto
            {
                Id = 1,
                TenantRefId = 100,
                EventType = "status_changed",
                FromStatus = "pending",
                ToStatus = "active"
            };
            Assert.Equal("status_changed", dto.EventType);
        }

        [Fact]
        public void TenantParameterDto_Properties()
        {
            var dto = new TenantParameterDto
            {
                ParamKey = "max_users",
                ParamType = "number",
                ParamValue = "100"
            };
            Assert.Equal("100", dto.ParamValue);
        }

        // ──────────────────────────────────────────────────────
        // 状态流转验证测试
        // ──────────────────────────────────────────────────────

        [Fact]
        public void TenantStatusChangeRequest_Properties()
        {
            var req = new TenantStatusChangeRequest
            {
                TargetStatus = "active",
                Reason = "审批通过"
            };
            Assert.Equal("active", req.TargetStatus);
            Assert.Equal("审批通过", req.Reason);
        }

        [Fact]
        public void UpdateTenantRequest_NullableProperties()
        {
            var req = new UpdateTenantRequest();
            Assert.Null(req.TenantName);
            Assert.Null(req.EnterpriseName);
            Assert.Null(req.ContactName);
            Assert.Null(req.ContactPhone);
            Assert.Null(req.ContactEmail);
        }

        [Fact]
        public void UpdatePlatformUserRequest_NullableProperties()
        {
            var req = new UpdatePlatformUserRequest();
            Assert.Null(req.DisplayName);
            Assert.Null(req.Phone);
            Assert.Null(req.Email);
            Assert.Null(req.Remark);
        }

        [Fact]
        public void UpdatePlatformRoleRequest_NullableProperties()
        {
            var req = new UpdatePlatformRoleRequest();
            Assert.Null(req.Name);
            Assert.Null(req.Description);
        }

        [Fact]
        public void UpdateTenantSystemConfigRequest_NullableProperties()
        {
            var req = new UpdateTenantSystemConfigRequest();
            Assert.Null(req.SystemName);
            Assert.Null(req.LogoUrl);
            Assert.Null(req.SystemTheme);
            Assert.Null(req.DefaultLanguage);
            Assert.Null(req.DefaultTimezone);
        }

        [Fact]
        public void CreateTenantDomainRequest_Defaults()
        {
            var req = new CreateTenantDomainRequest();
            Assert.Equal("custom", req.DomainType);
            Assert.Equal(0, req.TenantRefId);
        }

        [Fact]
        public void CreateTenantTagRequest_Defaults()
        {
            var req = new CreateTenantTagRequest();
            Assert.Equal("custom", req.TagType);
            Assert.Null(req.Description);
        }

        [Fact]
        public void CreateTenantGroupRequest_Defaults()
        {
            var req = new CreateTenantGroupRequest();
            Assert.Null(req.ParentId);
            Assert.Null(req.Description);
        }

        // ──────────────────────────────────────────────────────
        // 权限缓存接口测试
        // ──────────────────────────────────────────────────────

        [Fact]
        public void PlatformPermissionAppService_GetByCode_ReturnsNullForUnknown()
        {
            var result = PlatformPermissionAppService.GetByCode("nonexistent:code");
            Assert.Null(result);
        }

        // ──────────────────────────────────────────────────────
        // 资源配额 DTO 测试
        // ──────────────────────────────────────────────────────

        [Fact]
        public void TenantResourceQuotaDto_Properties()
        {
            var dto = new TenantResourceQuotaDto
            {
                Id = 1,
                TenantRefId = 100,
                QuotaType = "storage",
                QuotaLimit = 1024,
                WarningThreshold = 800,
                ResetCycle = "monthly"
            };
            Assert.Equal(1024, dto.QuotaLimit);
            Assert.Equal(800, dto.WarningThreshold);
            Assert.Equal("monthly", dto.ResetCycle);
        }
    }
}
