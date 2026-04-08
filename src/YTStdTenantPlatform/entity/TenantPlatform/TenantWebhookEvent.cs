using System;
using YTStdEntity.Attributes;

namespace YTStdTenantPlatform.Entity.TenantPlatform;

/// <summary>租户 Webhook 事件</summary>
[Entity(TableName = "sys_webhook_event_binding")]
[Index("uq_sys_webhook_event_binding_webhook_event", "webhook_id", "event_id", Kind = IndexKind.Unique)]
public class TenantWebhookEvent
{
    /// <summary>主键</summary>
    [Column(IsPrimaryKey = true)]
    public long Id { get; set; }

    /// <summary>Webhook ID</summary>
    public long WebhookId { get; set; }

    /// <summary>事件 ID</summary>
    public long EventId { get; set; }

    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; }
}
