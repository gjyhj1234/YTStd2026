<template>
  <div class="dashboard-page">
    <DxLoadPanel
      :visible="loading"
      :show-indicator="true"
      :show-pane="true"
      :shading="true"
      :close-on-outside-click="false"
      shading-color="rgba(0,0,0,0.2)"
    />

    <!-- 页面标题 -->
    <h2 class="dashboard-title" data-testid="dashboard-title">{{ $t('仪表盘') }}</h2>
    <p class="dashboard-subtitle" data-testid="dashboard-subtitle">{{ $t('平台运营数据概览') }}</p>

    <!-- 统计卡片区 -->
    <div class="stat-cards" data-testid="stat-cards">
      <div class="stat-card" data-testid="stat-card-active-users">
        <div class="stat-icon" :style="{ backgroundColor: '#1890ff' }">
          <i class="dx-icon-group"></i>
        </div>
        <div class="stat-content">
          <div class="stat-label">{{ $t('活跃用户') }}</div>
          <div class="stat-value">{{ formatNumber(activeUserCount) }}</div>
        </div>
      </div>

      <div class="stat-card" data-testid="stat-card-new-users">
        <div class="stat-icon" :style="{ backgroundColor: '#52c41a' }">
          <i class="dx-icon-add"></i>
        </div>
        <div class="stat-content">
          <div class="stat-label">{{ $t('新增用户') }}</div>
          <div class="stat-value">{{ formatNumber(newUserCount) }}</div>
        </div>
      </div>

      <div class="stat-card" data-testid="stat-card-api-calls">
        <div class="stat-icon" :style="{ backgroundColor: '#fa8c16' }">
          <i class="dx-icon-globe"></i>
        </div>
        <div class="stat-content">
          <div class="stat-label">{{ $t('API调用次数') }}</div>
          <div class="stat-value">{{ formatNumber(apiCallCount) }}</div>
        </div>
      </div>

      <div class="stat-card" data-testid="stat-card-storage">
        <div class="stat-icon" :style="{ backgroundColor: '#722ed1' }">
          <i class="dx-icon-folder"></i>
        </div>
        <div class="stat-content">
          <div class="stat-label">{{ $t('存储使用量') }}</div>
          <div class="stat-value">{{ formatStorage(storageBytes) }}</div>
        </div>
      </div>
    </div>

    <!-- 图表区 -->
    <div class="chart-row">
      <div class="chart-container" data-testid="chart-active-users">
        <DxChart
          :data-source="dailyStats"
        >
          <DxChartTitle :text="$t('每日活跃用户趋势')" />
          <DxChartSeries
            argument-field="StatDate"
            value-field="ActiveUserCount"
            type="line"
          />
          <DxArgumentAxis>
            <DxChartLabel :customize-text="formatDateLabel" />
          </DxArgumentAxis>
          <DxChartLegend :visible="false" />
          <DxChartTooltip :enabled="true" />
        </DxChart>
      </div>

      <div class="chart-container" data-testid="chart-api-calls">
        <DxChart
          :data-source="dailyStats"
        >
          <DxChartTitle :text="$t('每日API调用趋势')" />
          <DxChartSeries
            argument-field="StatDate"
            value-field="ApiCallCount"
            type="bar"
          />
          <DxArgumentAxis>
            <DxChartLabel :customize-text="formatDateLabel" />
          </DxArgumentAxis>
          <DxChartLegend :visible="false" />
          <DxChartTooltip :enabled="true" />
        </DxChart>
      </div>
    </div>

    <div class="chart-row">
      <div class="chart-container chart-container-full" data-testid="chart-metrics">
        <DxPieChart
          :data-source="metricsData"
          palette="Bright"
        >
          <DxPieChartTitleComp :text="$t('监控指标分布')" />
          <DxPieSeries
            argument-field="MetricType"
            value-field="MetricValue"
          >
            <DxPieLabel :visible="true">
              <DxConnector :visible="true" />
            </DxPieLabel>
          </DxPieSeries>
          <DxPieLegend
            :visible="true"
            horizontal-alignment="right"
            vertical-alignment="top"
          />
          <DxPieTooltip :enabled="true" />
        </DxPieChart>
      </div>
    </div>

    <!-- 快捷操作区 -->
    <div class="quick-actions" data-testid="quick-actions">
      <h3 class="section-title">{{ $t('快捷操作') }}</h3>
      <div class="action-buttons">
        <DxButton
          v-if="authStore.hasPermission('tenant.create')"
          :text="$t('创建租户')"
          icon="add"
          type="default"
          styling-mode="outlined"
          data-testid="btn-create-tenant"
          @click="goTo('/tenants')"
        />
        <DxButton
          v-if="authStore.hasPermission('platform.user.create')"
          :text="$t('创建用户')"
          icon="add"
          type="default"
          styling-mode="outlined"
          data-testid="btn-create-user"
          @click="goTo('/platform-users')"
        />
        <DxButton
          v-if="authStore.hasPermission('audit.list')"
          :text="$t('查看审计日志')"
          icon="description"
          type="default"
          styling-mode="outlined"
          data-testid="btn-audit-logs"
          @click="goTo('/audit-logs')"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import {
  DxChart,
  DxSeries as DxChartSeries,
  DxArgumentAxis,
  DxValueAxis,
  DxLabel as DxChartLabel,
  DxTitle as DxChartTitle,
  DxLegend as DxChartLegend,
  DxTooltip as DxChartTooltip
} from 'devextreme-vue/chart'
import {
  DxPieChart,
  DxSeries as DxPieSeries,
  DxLabel as DxPieLabel,
  DxLegend as DxPieLegend,
  DxTooltip as DxPieTooltip,
  DxTitle as DxPieChartTitleComp,
  DxConnector
} from 'devextreme-vue/pie-chart'
import { DxLoadPanel } from 'devextreme-vue/load-panel'
import { DxButton } from 'devextreme-vue/button'

import { getTenantStatisticsApi, getMonitorMetricsApi } from '../../api/platform-operations'
import { useAuthStore } from '../../store/auth'
import type { TenantDailyStatRepDTO, PlatformMonitorMetricRepDTO } from '../../types/platform-operations'

const router = useRouter()
const authStore = useAuthStore()

const loading = ref(false)
const dailyStats = ref<TenantDailyStatRepDTO[]>([])
const metricsData = ref<PlatformMonitorMetricRepDTO[]>([])

const activeUserCount = ref(0)
const newUserCount = ref(0)
const apiCallCount = ref(0)
const storageBytes = ref(0)

function formatNumber(value: number): string {
  if (value >= 1_000_000) {
    return (value / 1_000_000).toFixed(1) + 'M'
  }
  if (value >= 1_000) {
    return value.toLocaleString('en-US')
  }
  return String(value)
}

function formatStorage(bytes: number): string {
  if (bytes >= 1_073_741_824) {
    return (bytes / 1_073_741_824).toFixed(2) + ' GB'
  }
  if (bytes >= 1_048_576) {
    return (bytes / 1_048_576).toFixed(2) + ' MB'
  }
  if (bytes >= 1024) {
    return (bytes / 1024).toFixed(2) + ' KB'
  }
  return bytes + ' B'
}

function formatDateLabel(info: { value: string | Date }): string {
  const d = new Date(info.value)
  const month = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  return `${month}-${day}`
}

function goTo(path: string): void {
  router.push(path)
}

async function loadData(): Promise<void> {
  loading.value = true
  try {
    const [statsResult, metricsResult] = await Promise.all([
      getTenantStatisticsApi(1, 100).catch(() => null),
      getMonitorMetricsApi(1, 100).catch(() => null)
    ])

    if (statsResult && statsResult.Items) {
      dailyStats.value = statsResult.Items

      // Aggregate the latest day's data for stat cards
      let totalActive = 0
      let totalNew = 0
      let totalApi = 0
      let totalStorage = 0

      if (statsResult.Items.length > 0) {
        // Find the latest date
        const sorted = [...statsResult.Items].sort(
          (a, b) => new Date(b.StatDate).getTime() - new Date(a.StatDate).getTime()
        )
        const latestDate = sorted[0].StatDate

        for (const item of sorted) {
          if (item.StatDate === latestDate) {
            totalActive += item.ActiveUserCount
            totalNew += item.NewUserCount
            totalApi += item.ApiCallCount
            totalStorage += item.StorageBytes
          }
        }
      }

      activeUserCount.value = totalActive
      newUserCount.value = totalNew
      apiCallCount.value = totalApi
      storageBytes.value = totalStorage
    }

    if (metricsResult && metricsResult.Items) {
      metricsData.value = metricsResult.Items
    }
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.dashboard-page {
  padding: 20px;
}

.dashboard-title {
  margin: 0 0 4px 0;
  font-size: 24px;
  font-weight: 600;
  color: #333;
}

.dashboard-subtitle {
  margin: 0 0 24px 0;
  font-size: 14px;
  color: #999;
}

.stat-cards {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
  margin-bottom: 24px;
}

.stat-card {
  display: flex;
  align-items: center;
  padding: 20px;
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.08);
}

.stat-icon {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  margin-right: 16px;
}

.stat-icon i {
  font-size: 22px;
  color: #fff;
}

.stat-content {
  flex: 1;
}

.stat-label {
  font-size: 13px;
  color: #999;
  margin-bottom: 4px;
}

.stat-value {
  font-size: 24px;
  font-weight: 700;
  color: #333;
}

.chart-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
  margin-bottom: 24px;
}

.chart-container {
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.08);
  padding: 16px;
  min-height: 300px;
}

.chart-container-full {
  grid-column: 1 / -1;
}

.quick-actions {
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.08);
  padding: 20px;
}

.section-title {
  margin: 0 0 16px 0;
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.action-buttons {
  display: flex;
  gap: 12px;
}

@media (max-width: 1024px) {
  .stat-cards {
    grid-template-columns: repeat(2, 1fr);
  }

  .chart-row {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 600px) {
  .stat-cards {
    grid-template-columns: 1fr;
  }
}
</style>
