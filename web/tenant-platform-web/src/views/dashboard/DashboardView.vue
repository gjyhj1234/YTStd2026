<template>
  <div>
    <div class="page-header">
      <h2>{{ $t('route.dashboard') }}</h2>
      <div class="page-header-actions">
        <PageHelpEntry @click="showGuide = true" />
      </div>
    </div>

    <FunctionDescriptionCard
      :purpose="$t('展示平台核心运营指标概览')"
      :data-scope="$t('全平台汇总数据实时更新')"
      :permission-note="$t('仪表盘对所有已登录用户可见')"
      :collapsible="true"
    />

    <DxLoadPanel
      :visible="isLoading"
      :show-indicator="true"
      :show-pane="true"
      :shading="true"
      :close-on-outside-click="false"
      :message="$t('加载中')"
    />

    <div class="dashboard-stats">
      <div class="stat-card">
        <div class="stat-value">{{ stats.TotalTenants }}</div>
        <div class="stat-label">{{ $t('总租户数') }}</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">{{ stats.ActiveTenants }}</div>
        <div class="stat-label">{{ $t('活跃租户') }}</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">{{ stats.TotalSubscriptions }}</div>
        <div class="stat-label">{{ $t('有效订阅') }}</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">{{ stats.TotalUsers }}</div>
        <div class="stat-label">{{ $t('平台用户') }}</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">{{ stats.ExpiringTenants }}</div>
        <div class="stat-label">{{ $t('即将到期') }}</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">{{ stats.TrialTenants }}</div>
        <div class="stat-label">{{ $t('试用中') }}</div>
      </div>
    </div>

    <div class="dashboard-charts">
      <div class="card chart-card">
        <h3 style="margin-bottom: 12px">{{ $t('租户增长趋势') }}</h3>
        <template v-if="tenantTrend.length > 0">
          <DxChart :data-source="tenantTrend">
            <DxArgumentAxis :value-margins-enabled="false">
              <DxLabel :visible="true" />
            </DxArgumentAxis>
            <DxSeries argument-field="Date" value-field="Count" :name="$t('新增租户')" type="bar" color="var(--primary-color, #1976d2)" />
            <DxTooltip :enabled="true" />
            <DxLegend :visible="false" />
          </DxChart>
        </template>
        <div v-else-if="!isLoading" class="empty-chart">{{ $t('暂无数据') }}</div>
      </div>

      <div class="card chart-card">
        <h3 style="margin-bottom: 12px">{{ $t('订阅分布') }}</h3>
        <template v-if="subscriptionDist.length > 0">
          <DxPieChart :data-source="subscriptionDist" :palette="'Soft Pastel'">
            <DxPieSeries argument-field="PackageName" value-field="Count">
              <DxPieLabel :visible="true" :connector="{ visible: true }" />
            </DxPieSeries>
            <DxPieTooltip :enabled="true" />
            <DxPieLegend :visible="true" horizontal-alignment="center" vertical-alignment="bottom" />
          </DxPieChart>
        </template>
        <div v-else-if="!isLoading" class="empty-chart">{{ $t('暂无数据') }}</div>
      </div>
    </div>

    <div class="card">
      <h3 style="margin-bottom: 12px">{{ $t('快捷入口') }}</h3>
      <div style="display: flex; gap: 12px; flex-wrap: wrap">
        <DxButton :text="$t('menu.tenants')" icon="globe" @click="router.push('/tenants')" />
        <DxButton :text="$t('menu.platformUsers')" icon="group" @click="router.push('/platform-users')" />
        <DxButton :text="$t('menu.packages')" icon="box" @click="router.push('/saas-packages')" />
        <DxButton :text="$t('menu.subscriptionList')" icon="clock" @click="router.push('/subscriptions')" />
        <DxButton :text="$t('menu.operationLogs')" icon="textdocument" @click="router.push('/operation-logs')" />
      </div>
    </div>

    <OperationGuideDrawer
      v-model:visible="showGuide"
      :title="$t('仪表盘操作指引')"
      :entry-path="$t('登录后默认进入仪表盘页面')"
      :steps="[
        $t('查看顶部统计卡片了解平台概况'),
        $t('查看租户增长趋势图了解增长情况'),
        $t('查看订阅分布图了解套餐使用情况'),
        $t('点击快捷入口快速进入常用功能模块'),
        $t('左侧菜单可访问所有管理模块'),
      ]"
      :field-notes="[$t('统计数据实时更新来源于后端接口')]"
      :error-notes="[$t('若统计数据加载失败请检查网络连接')]"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { DxButton } from 'devextreme-vue/button'
import { DxLoadPanel } from 'devextreme-vue/load-panel'
import {
  DxChart,
  DxSeries,
  DxArgumentAxis,
  DxLabel,
  DxTooltip,
  DxLegend,
} from 'devextreme-vue/chart'
import {
  DxPieChart,
  DxSeries as DxPieSeries,
  DxLabel as DxPieLabel,
  DxTooltip as DxPieTooltip,
  DxLegend as DxPieLegend,
} from 'devextreme-vue/pie-chart'
import FunctionDescriptionCard from '@/components/help/FunctionDescriptionCard.vue'
import OperationGuideDrawer from '@/components/help/OperationGuideDrawer.vue'
import PageHelpEntry from '@/components/help/PageHelpEntry.vue'
import { getDashboardStats, getTenantTrend, getSubscriptionDist } from '@/api/dashboard'
import type { DashboardStats, TenantTrendItem, SubscriptionDistItem } from '@/types/dashboard'

const router = useRouter()
const showGuide = ref(false)
const isLoading = ref(true)

const stats = ref<DashboardStats>({
  TotalTenants: 0,
  ActiveTenants: 0,
  TotalSubscriptions: 0,
  TotalUsers: 0,
  ExpiringTenants: 0,
  TrialTenants: 0,
})

const tenantTrend = ref<TenantTrendItem[]>([])
const subscriptionDist = ref<SubscriptionDistItem[]>([])

async function loadStats() {
  try {
    const res = await getDashboardStats()
    if (res.Data) {
      stats.value = res.Data
    }
  } catch {
    // 接口未就绪时保持默认值
  }
}

async function loadTenantTrend() {
  try {
    const res = await getTenantTrend()
    if (res.Data) {
      tenantTrend.value = res.Data
    }
  } catch {
    // 接口未就绪时保持空数组
  }
}

async function loadSubscriptionDist() {
  try {
    const res = await getSubscriptionDist()
    if (res.Data) {
      subscriptionDist.value = res.Data
    }
  } catch {
    // 接口未就绪时保持空数组
  }
}

async function loadAllData() {
  isLoading.value = true
  try {
    await Promise.all([loadStats(), loadTenantTrend(), loadSubscriptionDist()])
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  loadAllData()
})
</script>

<style scoped>
.dashboard-charts {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
  margin-bottom: 16px;
}

.chart-card {
  min-height: 300px;
}

.empty-chart {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 200px;
  color: var(--text-color);
  opacity: 0.5;
  font-size: 14px;
}

@media (max-width: 1024px) {
  .dashboard-charts {
    grid-template-columns: 1fr;
  }
}
</style>
