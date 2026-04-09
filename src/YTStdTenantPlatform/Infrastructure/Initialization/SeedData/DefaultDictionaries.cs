using System;
using System.Collections.Generic;
using YTStdTenantPlatform.Entity.TenantPlatform;

namespace YTStdTenantPlatform.Infrastructure.Initialization.SeedData
{
    /// <summary>默认数据字典种子数据</summary>
    public static class DefaultDictionaries
    {
        /// <summary>获取默认字典列表</summary>
        public static IReadOnlyList<PlatformDictionary> GetDefaultDictionaries()
        {
            var now = DateTime.UtcNow;
            var list = new List<PlatformDictionary>();

            // ── 性别 ──
            AddItem(list, "gender", "性别", "male", "男", "1", 10, now);
            AddItem(list, "gender", "性别", "female", "女", "2", 20, now);
            AddItem(list, "gender", "性别", "unknown", "未知", "0", 30, now);

            // ── 行业分类 ──
            AddItem(list, "industry", "行业分类", "internet", "互联网/IT", "internet", 10, now);
            AddItem(list, "industry", "行业分类", "finance", "金融", "finance", 20, now);
            AddItem(list, "industry", "行业分类", "education", "教育", "education", 30, now);
            AddItem(list, "industry", "行业分类", "healthcare", "医疗健康", "healthcare", 40, now);
            AddItem(list, "industry", "行业分类", "manufacturing", "制造业", "manufacturing", 50, now);
            AddItem(list, "industry", "行业分类", "retail", "零售/电商", "retail", 60, now);
            AddItem(list, "industry", "行业分类", "logistics", "物流", "logistics", 70, now);
            AddItem(list, "industry", "行业分类", "realestate", "房地产", "realestate", 80, now);
            AddItem(list, "industry", "行业分类", "government", "政府/公共事业", "government", 90, now);
            AddItem(list, "industry", "行业分类", "other", "其他", "other", 100, now);

            // ── 客户等级 ──
            AddItem(list, "customer_level", "客户等级", "basic", "基础版", "basic", 10, now);
            AddItem(list, "customer_level", "客户等级", "standard", "标准版", "standard", 20, now);
            AddItem(list, "customer_level", "客户等级", "premium", "高级版", "premium", 30, now);
            AddItem(list, "customer_level", "客户等级", "enterprise", "企业版", "enterprise", 40, now);

            // ── 通知渠道 ──
            AddItem(list, "notification_channel", "通知渠道", "email", "邮件", "email", 10, now);
            AddItem(list, "notification_channel", "通知渠道", "sms", "短信", "sms", 20, now);
            AddItem(list, "notification_channel", "通知渠道", "webhook", "Webhook", "webhook", 30, now);
            AddItem(list, "notification_channel", "通知渠道", "in_app", "站内信", "in_app", 40, now);

            // ── 生命周期状态 ──
            AddItem(list, "lifecycle_status", "生命周期状态", "trial", "试用", "trial", 10, now);
            AddItem(list, "lifecycle_status", "生命周期状态", "active", "正常", "active", 20, now);
            AddItem(list, "lifecycle_status", "生命周期状态", "expiring", "即将到期", "expiring", 30, now);
            AddItem(list, "lifecycle_status", "生命周期状态", "expired", "已过期", "expired", 40, now);
            AddItem(list, "lifecycle_status", "生命周期状态", "suspended", "已暂停", "suspended", 50, now);
            AddItem(list, "lifecycle_status", "生命周期状态", "closed", "已关闭", "closed", 60, now);

            return list;
        }

        /// <summary>添加字典项</summary>
        private static void AddItem(List<PlatformDictionary> list, string typeCode, string typeName, string itemCode, string itemName, string itemValue, int sortOrder, DateTime now)
        {
            list.Add(new PlatformDictionary
            {
                TypeCode = typeCode,
                TypeName = typeName,
                ItemCode = itemCode,
                ItemName = itemName,
                ItemValue = itemValue,
                IsEnabled = true,
                SortOrder = sortOrder,
                CreatedAt = now,
                UpdatedAt = now
            });
        }
    }
}
