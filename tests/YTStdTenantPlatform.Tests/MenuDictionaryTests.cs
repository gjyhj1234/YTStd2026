using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using YTStdTenantPlatform.Application.Dtos;
using YTStdTenantPlatform.Entity.TenantPlatform;
using YTStdTenantPlatform.Infrastructure.Initialization.SeedData;

namespace YTStdTenantPlatform.Tests
{
    /// <summary>菜单与字典 DTO 及种子数据测试</summary>
    public class MenuDictionaryTests
    {
        // ============================================================
        // 菜单 DTO 测试
        // ============================================================

        [Fact]
        public void MenuRepDTO_HasExpectedProperties()
        {
            var dto = new MenuRepDTO
            {
                Id = 1,
                ParentId = 0,
                Code = "menu:platform",
                Name = "平台管理",
                Icon = "shield",
                RoutePath = null,
                ComponentPath = null,
                PermissionCode = null,
                MenuType = 1,
                IsEnabled = true,
                IsExternal = false,
                IsVisible = true,
                SortOrder = 10,
                CreatedAt = DateTime.UtcNow,
                Children = null
            };
            Assert.Equal("menu:platform", dto.Code);
            Assert.Equal(1, dto.MenuType);
            Assert.True(dto.IsEnabled);
            Assert.Null(dto.Children);
        }

        [Fact]
        public void MenuRepDTO_Children_CanBePopulated()
        {
            var child = new MenuRepDTO
            {
                Id = 2,
                ParentId = 1,
                Code = "menu:platform:user",
                Name = "用户管理",
                MenuType = 2,
                SortOrder = 10
            };
            var parent = new MenuRepDTO
            {
                Id = 1,
                Code = "menu:platform",
                Name = "平台管理",
                MenuType = 1,
                Children = new List<MenuRepDTO> { child }
            };
            Assert.NotNull(parent.Children);
            Assert.Single(parent.Children);
            Assert.Equal("menu:platform:user", parent.Children[0].Code);
        }

        [Fact]
        public void CreateMenuReqDTO_DefaultValues()
        {
            var req = new CreateMenuReqDTO();
            Assert.Equal("", req.Code);
            Assert.Equal("", req.Name);
            Assert.Null(req.Icon);
            Assert.Null(req.RoutePath);
            Assert.Null(req.ComponentPath);
            Assert.Null(req.PermissionCode);
            Assert.Equal(0, req.MenuType);
            Assert.False(req.IsExternal);
            Assert.True(req.IsVisible);
            Assert.Equal(0, req.SortOrder);
            Assert.Null(req.Remark);
        }

        [Fact]
        public void UpdateMenuReqDTO_NullableProperties()
        {
            var req = new UpdateMenuReqDTO();
            Assert.Null(req.Name);
            Assert.Null(req.Icon);
            Assert.Null(req.RoutePath);
            Assert.Null(req.ComponentPath);
            Assert.Null(req.PermissionCode);
            Assert.Null(req.MenuType);
            Assert.Null(req.IsExternal);
            Assert.Null(req.IsVisible);
            Assert.Null(req.SortOrder);
            Assert.Null(req.Remark);
        }

        [Fact]
        public void MenuSortReqDTO_DefaultValues()
        {
            var req = new MenuSortReqDTO();
            Assert.Equal(0, req.SortOrder);
        }

        // ============================================================
        // 字典 DTO 测试
        // ============================================================

        [Fact]
        public void DictionaryRepDTO_HasExpectedProperties()
        {
            var dto = new DictionaryRepDTO
            {
                Id = 1,
                TypeCode = "gender",
                TypeName = "性别",
                ItemCode = "male",
                ItemName = "男",
                ItemValue = "1",
                IsEnabled = true,
                SortOrder = 10,
                CreatedAt = DateTime.UtcNow
            };
            Assert.Equal("gender", dto.TypeCode);
            Assert.Equal("male", dto.ItemCode);
            Assert.Equal("1", dto.ItemValue);
            Assert.True(dto.IsEnabled);
        }

        [Fact]
        public void DictionaryTypeRepDTO_HasExpectedProperties()
        {
            var dto = new DictionaryTypeRepDTO
            {
                TypeCode = "gender",
                TypeName = "性别",
                ItemCount = 3
            };
            Assert.Equal("gender", dto.TypeCode);
            Assert.Equal("性别", dto.TypeName);
            Assert.Equal(3, dto.ItemCount);
        }

        [Fact]
        public void CreateDictionaryReqDTO_DefaultValues()
        {
            var req = new CreateDictionaryReqDTO();
            Assert.Equal("", req.TypeCode);
            Assert.Equal("", req.TypeName);
            Assert.Equal("", req.ItemCode);
            Assert.Equal("", req.ItemName);
            Assert.Null(req.ItemValue);
            Assert.Equal(0, req.SortOrder);
            Assert.Null(req.Remark);
        }

        [Fact]
        public void UpdateDictionaryReqDTO_NullableProperties()
        {
            var req = new UpdateDictionaryReqDTO();
            Assert.Null(req.ItemName);
            Assert.Null(req.ItemValue);
            Assert.Null(req.SortOrder);
            Assert.Null(req.Remark);
        }

        // ============================================================
        // 菜单种子数据测试
        // ============================================================

        [Fact]
        public void DefaultMenus_HasAtLeast14TopLevelDirectories()
        {
            var menus = DefaultMenus.GetDefaultMenus();
            Assert.NotEmpty(menus);

            var topDirs = menus.Where(m => m.ParentCode == null).ToList();
            Assert.True(topDirs.Count >= 14,
                $"顶级目录数量 {topDirs.Count} 少于预期 14");
        }

        [Fact]
        public void DefaultMenus_CodesAreUnique()
        {
            var menus = DefaultMenus.GetDefaultMenus();
            var codes = menus.Select(m => m.Menu.Code).ToList();
            var distinct = codes.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            Assert.Equal(distinct.Count, codes.Count);
        }

        [Fact]
        public void DefaultMenus_ParentCodesReferExistingCodes()
        {
            var menus = DefaultMenus.GetDefaultMenus();
            var allCodes = new HashSet<string>(
                menus.Select(m => m.Menu.Code),
                StringComparer.OrdinalIgnoreCase);

            foreach (var menu in menus)
            {
                if (!string.IsNullOrEmpty(menu.ParentCode))
                {
                    Assert.True(allCodes.Contains(menu.ParentCode),
                        $"菜单 {menu.Menu.Code} 引用了不存在的父级 {menu.ParentCode}");
                }
            }
        }

        [Fact]
        public void DefaultMenus_AllHaveNonEmptyName()
        {
            var menus = DefaultMenus.GetDefaultMenus();
            foreach (var menu in menus)
            {
                Assert.False(string.IsNullOrWhiteSpace(menu.Menu.Name),
                    $"菜单 {menu.Menu.Code} 名称为空");
                Assert.False(string.IsNullOrWhiteSpace(menu.Menu.Code),
                    "发现编码为空的菜单");
            }
        }

        [Fact]
        public void DefaultMenus_DirectoriesHaveMenuType1()
        {
            var menus = DefaultMenus.GetDefaultMenus();
            var dirs = menus.Where(m => m.ParentCode == null).ToList();
            foreach (var dir in dirs)
            {
                Assert.Equal(1, dir.Menu.MenuType);
            }
        }

        [Fact]
        public void DefaultMenus_PagesHaveMenuType2()
        {
            var menus = DefaultMenus.GetDefaultMenus();
            var pages = menus.Where(m => m.ParentCode != null).ToList();
            foreach (var page in pages)
            {
                Assert.Equal(2, page.Menu.MenuType);
            }
        }

        [Fact]
        public void DefaultMenus_PagesHaveRouteAndComponent()
        {
            var menus = DefaultMenus.GetDefaultMenus();
            var pages = menus.Where(m => m.Menu.MenuType == 2).ToList();
            Assert.NotEmpty(pages);

            foreach (var page in pages)
            {
                Assert.False(string.IsNullOrEmpty(page.Menu.RoutePath),
                    $"页面菜单 {page.Menu.Code} 应有 RoutePath");
                Assert.False(string.IsNullOrEmpty(page.Menu.ComponentPath),
                    $"页面菜单 {page.Menu.Code} 应有 ComponentPath");
            }
        }

        [Fact]
        public void DefaultMenus_AllAreEnabledAndVisible()
        {
            var menus = DefaultMenus.GetDefaultMenus();
            foreach (var menu in menus)
            {
                Assert.True(menu.Menu.IsEnabled,
                    $"菜单 {menu.Menu.Code} 应默认启用");
                Assert.True(menu.Menu.IsVisible,
                    $"菜单 {menu.Menu.Code} 应默认可见");
            }
        }

        [Fact]
        public void DefaultMenus_ContainsKeyModules()
        {
            var menus = DefaultMenus.GetDefaultMenus();
            var codes = new HashSet<string>(
                menus.Select(m => m.Menu.Code),
                StringComparer.OrdinalIgnoreCase);

            // 验证关键模块目录存在
            Assert.Contains("menu:platform", codes);
            Assert.Contains("menu:tenant-lifecycle", codes);
            Assert.Contains("menu:tenant-info", codes);
            Assert.Contains("menu:tenant-config", codes);
            Assert.Contains("menu:package", codes);
            Assert.Contains("menu:subscription", codes);
            Assert.Contains("menu:billing", codes);
            Assert.Contains("menu:api", codes);
            Assert.Contains("menu:operation", codes);
            Assert.Contains("menu:log", codes);
            Assert.Contains("menu:notification", codes);
            Assert.Contains("menu:storage", codes);
            Assert.Contains("menu:system", codes);
        }

        // ============================================================
        // 字典种子数据测试
        // ============================================================

        [Fact]
        public void DefaultDictionaries_Has5Types()
        {
            var dicts = DefaultDictionaries.GetDefaultDictionaries();
            Assert.NotEmpty(dicts);

            var types = dicts.Select(d => d.TypeCode).Distinct().ToList();
            Assert.Equal(5, types.Count);
        }

        [Fact]
        public void DefaultDictionaries_ContainsExpectedTypes()
        {
            var dicts = DefaultDictionaries.GetDefaultDictionaries();
            var types = new HashSet<string>(dicts.Select(d => d.TypeCode));

            Assert.Contains("gender", types);
            Assert.Contains("industry", types);
            Assert.Contains("customer_level", types);
            Assert.Contains("notification_channel", types);
            Assert.Contains("lifecycle_status", types);
        }

        [Fact]
        public void DefaultDictionaries_GenderHas3Items()
        {
            var dicts = DefaultDictionaries.GetDefaultDictionaries();
            var genderItems = dicts.Where(d => d.TypeCode == "gender").ToList();
            Assert.Equal(3, genderItems.Count);
        }

        [Fact]
        public void DefaultDictionaries_AllItemsHaveNonEmptyFields()
        {
            var dicts = DefaultDictionaries.GetDefaultDictionaries();
            foreach (var dict in dicts)
            {
                Assert.False(string.IsNullOrWhiteSpace(dict.TypeCode),
                    "字典 TypeCode 不能为空");
                Assert.False(string.IsNullOrWhiteSpace(dict.TypeName),
                    "字典 TypeName 不能为空");
                Assert.False(string.IsNullOrWhiteSpace(dict.ItemCode),
                    $"字典 {dict.TypeCode} 的 ItemCode 不能为空");
                Assert.False(string.IsNullOrWhiteSpace(dict.ItemName),
                    $"字典 {dict.TypeCode}/{dict.ItemCode} 的 ItemName 不能为空");
            }
        }

        [Fact]
        public void DefaultDictionaries_ItemCodesUniqueWithinType()
        {
            var dicts = DefaultDictionaries.GetDefaultDictionaries();
            var grouped = dicts.GroupBy(d => d.TypeCode);

            foreach (var group in grouped)
            {
                var itemCodes = group.Select(d => d.ItemCode).ToList();
                var distinct = itemCodes.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                Assert.Equal(distinct.Count, itemCodes.Count);
            }
        }

        [Fact]
        public void DefaultDictionaries_AllAreEnabled()
        {
            var dicts = DefaultDictionaries.GetDefaultDictionaries();
            foreach (var dict in dicts)
            {
                Assert.True(dict.IsEnabled,
                    $"字典 {dict.TypeCode}/{dict.ItemCode} 应默认启用");
            }
        }

        [Fact]
        public void DefaultDictionaries_SortOrdersArePositive()
        {
            var dicts = DefaultDictionaries.GetDefaultDictionaries();
            foreach (var dict in dicts)
            {
                Assert.True(dict.SortOrder > 0,
                    $"字典 {dict.TypeCode}/{dict.ItemCode} 的 SortOrder 应大于 0");
            }
        }

        // ============================================================
        // Phase C 新增 DTO 测试
        // ============================================================

        [Fact]
        public void RenewSubscriptionReqDTO_DefaultValues()
        {
            var req = new RenewSubscriptionReqDTO();
            Assert.Equal(12, req.Months);
        }

        [Fact]
        public void UpgradeSubscriptionReqDTO_DefaultValues()
        {
            var req = new UpgradeSubscriptionReqDTO();
            Assert.Equal(0, req.TargetPackageVersionId);
        }

        // ============================================================
        // MenuSeed 结构测试
        // ============================================================

        [Fact]
        public void MenuSeed_StoresMenuAndParentCode()
        {
            var menu = new PlatformMenu
            {
                Code = "test",
                Name = "测试",
                MenuType = 1,
                IsEnabled = true,
                IsVisible = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var seed = new MenuSeed(menu, "parent:code");

            Assert.Equal("test", seed.Menu.Code);
            Assert.Equal("parent:code", seed.ParentCode);
        }

        [Fact]
        public void MenuSeed_ParentCode_CanBeNull()
        {
            var menu = new PlatformMenu { Code = "root", Name = "根", MenuType = 1 };
            var seed = new MenuSeed(menu, null);
            Assert.Null(seed.ParentCode);
        }
    }
}
