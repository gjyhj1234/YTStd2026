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

    <!-- 统计卡片区 -->
    <div class="stat-cards" data-testid="stat-cards">
      <div class="stat-card" data-testid="stat-card-active-users">
        <div class="stat-icon stat-icon-blue">
          <i class="dx-icon-group"></i>
        </div>
        <div class="stat-content">
          <div class="stat-label">{{ $t('活跃用户') }}</div>
          <div class="stat-value">{{ formatNumber(activeUserCount) }}</div>
        </div>
      </div>

      <div class="stat-card" data-testid="stat-card-new-users">
        <div class="stat-icon stat-icon-green">
          <i class="dx-icon-add"></i>
        </div>
        <div class="stat-content">
          <div class="stat-label">{{ $t('新增用户') }}</div>
          <div class="stat-value">{{ formatNumber(newUserCount) }}</div>
        </div>
      </div>

      <div class="stat-card" data-testid="stat-card-api-calls">
        <div class="stat-icon stat-icon-orange">
          <i class="dx-icon-globe"></i>
        </div>
        <div class="stat-content">
          <div class="stat-label">{{ $t('API调用次数') }}</div>
          <div class="stat-value">{{ formatNumber(apiCallCount) }}</div>
        </div>
      </div>

      <div class="stat-card" data-testid="stat-card-storage">
        <div class="stat-icon stat-icon-purple">
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
          <DxAdaptiveLayout :height="200" :width="200" />
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
          <DxAdaptiveLayout :height="200" :width="200" />
        </DxChart>
      </div>
    </div>

    <div class="chart-row chart-row-full">
      <div class="chart-container" data-testid="chart-metrics">
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
          <DxPieAdaptiveLayout :height="200" :width="200" />
        </DxPieChart>
      </div>
    </div>

    <!-- 快捷操作区 -->
    <div class="quick-actions" data-testid="quick-actions">
      <h3 class="section-title">{{ $t('快捷操作') }}</h3>
      <div class="action-buttons">
        <DxButton
          :text="$t('创建租户')"
          icon="add"
          type="default"
          styling-mode="outlined"
          data-testid="btn-create-tenant"
          @click="goTo('/tenants')"
        />
        <DxButton
          :text="$t('创建用户')"
          icon="add"
          type="default"
          styling-mode="outlined"
          data-testid="btn-create-user"
          @click="goTo('/platform-users')"
        />
        <DxButton
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
  DxLabel as DxChartLabel,
  DxTitle as DxChartTitle,
  DxLegend as DxChartLegend,
  DxTooltip as DxChartTooltip,
  DxAdaptiveLayout
} from 'devextreme-vue/chart'
import {
  DxPieChart,
  DxSeries as DxPieSeries,
  DxLabel as DxPieLabel,
  DxLegend as DxPieLegend,
  DxTooltip as DxPieTooltip,
  DxTitle as DxPieChartTitleComp,
  DxConnector,
  DxAdaptiveLayout as DxPieAdaptiveLayout
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


.dashboard-title {
  margin: 0 0 12px 0;
  font-size: 18px !important;
  font-weight: 600;
  color: var(--base-text-color);
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
  background: var(--base-bg);
  border-radius: 8px;
  border: 1px solid var(--dx-color-border);
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

.stat-icon-blue {
  background-color: #1890ff;
}

.stat-icon-green {
  background-color: #52c41a;
}

.stat-icon-orange {
  background-color: #fa8c16;
}

.stat-icon-purple {
  background-color: #722ed1;
}

.stat-icon i {
  font-size: 22px;
  color: #fff;
}

.stat-content {
  flex: 1;
  min-width: 0;
}

.stat-label {
  font-size: 13px;
  color: var(--base-text-color-alpha-7);
  margin-bottom: 4px;
}

.stat-value {
  font-size: 24px;
  font-weight: 700;
  color: var(--base-text-color);
}

.chart-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
  margin-bottom: 24px;
}

.chart-row-full {
  grid-template-columns: 1fr;
}

.chart-container {
  background: var(--base-bg);
  border-radius: 8px;
  border: 1px solid var(--dx-color-border);
  padding: 16px;
  height: 350px;
  overflow: hidden;
}

.quick-actions {
  background: var(--base-bg);
  border-radius: 8px;
  border: 1px solid var(--dx-color-border);
  padding: 20px;
}

.section-title {
  margin: 0 0 16px 0;
  font-size: 16px;
  font-weight: 600;
  color: var(--base-text-color);
}

.action-buttons {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
}

@media (max-width: 1024px) {
  .stat-cards {
    grid-template-columns: repeat(2, 1fr);
  }

  .chart-row {
    grid-template-columns: 1fr;
  }

  .chart-container {
    height: 320px;
  }
}

@media (max-width: 600px) {
  .stat-cards {
    grid-template-columns: 1fr;
  }

  .chart-container {
    height: 280px;
  }

  .action-buttons {
    flex-direction: column;
  }
}
</style>
