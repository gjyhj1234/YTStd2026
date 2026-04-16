<template>
  <div class="platform-permissions-page">
    <DxLoadPanel
      :visible="loading"
      :show-indicator="true"
      :show-pane="true"
      :shading="true"
      :close-on-outside-click="false"
      shading-color="rgba(0,0,0,0.2)"
    />

    <h2 class="page-title">{{ $t('平台权限管理') }}</h2>

    <FunctionDescriptionCard v-model:visible="showDescription">
      <p>{{ $t('查看平台权限树层级结构，权限数据由系统维护') }}</p>
    </FunctionDescriptionCard>

    <!-- Search -->
    <div class="search-bar">
      <DxTextBox
        v-model:value="keyword"
        :placeholder="$t('请输入权限编码或名称')"
        :show-clear-button="true"
        width="320"
        mode="search"
        @value-changed="onSearch"
      />
    </div>

    <!-- Permission Tree -->
    <DxTreeList
      :data-source="filteredData"
      key-expr="Id"
      parent-id-expr="ParentId"
      :auto-expand-all="true"
      :show-borders="true"
      :column-auto-width="true"
      :hover-state-enabled="true"
      :no-data-text="$t('暂无权限数据')"
    >
      <DxColumn data-field="Id" :caption="$t('ID')" :width="80" />
      <DxColumn data-field="Code" :caption="$t('权限编码')" />
      <DxColumn data-field="Name" :caption="$t('权限名称')" />
      <DxColumn data-field="PermissionType" :caption="$t('权限类型')" :width="120" cell-template="typeCell" />
      <DxColumn data-field="Path" :caption="$t('路径')" />
      <DxColumn data-field="Method" :caption="$t('HTTP方法')" :width="100" cell-template="methodCell" />

      <template #typeCell="{ data: cellData }">
        <span :class="'type-tag type-' + cellData.value" :style="getTypeStyle(cellData.value)">
          {{ getTypeLabel(cellData.value) }}
        </span>
      </template>

      <template #methodCell="{ data: cellData }">
        <span v-if="cellData.value" :style="getMethodStyle(cellData.value)">
          {{ cellData.value }}
        </span>
      </template>
    </DxTreeList>

    <OperationGuideDrawer v-model:visible="showGuide">
      <template #content>
        <div class="platform-permissions-page">
          <!-- main content is above -->
        </div>
      </template>
      <ol>
        <li>{{ $t('查看平台权限树层级结构，权限数据由系统维护') }}</li>
      </ol>
    </OperationGuideDrawer>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { DxTreeList, DxColumn } from 'devextreme-vue/tree-list'
import { DxTextBox } from 'devextreme-vue/text-box'
import { DxLoadPanel } from 'devextreme-vue/load-panel'
import FunctionDescriptionCard from '../../components/FunctionDescriptionCard.vue'
import OperationGuideDrawer from '../../components/OperationGuideDrawer.vue'
import { getPermissionTreeApi } from '../../api/platform-permissions'
import type { PlatformPermissionRepDTO, FlatPermission } from '../../types/platform-permissions'

const { t } = useI18n()

const loading = ref(false)
const showDescription = ref(true)
const showGuide = ref(false)
const keyword = ref('')
const flatData = ref<FlatPermission[]>([])

function flattenTree(nodes: PlatformPermissionRepDTO[], result: FlatPermission[] = []): FlatPermission[] {
  for (const node of nodes) {
    result.push({
      Id: node.Id,
      Code: node.Code,
      Name: node.Name,
      PermissionType: node.PermissionType,
      ParentId: node.ParentId,
      Path: node.Path,
      Method: node.Method
    })
    if (node.Children && node.Children.length > 0) {
      flattenTree(node.Children, result)
    }
  }
  return result
}

const filteredData = computed(() => {
  if (!keyword.value) return flatData.value
  const kw = keyword.value.toLowerCase()
  const matchedIds = new Set<number>()
  // Find matching nodes
  for (const item of flatData.value) {
    if (item.Code.toLowerCase().includes(kw) || item.Name.toLowerCase().includes(kw)) {
      matchedIds.add(item.Id)
      // Add all ancestors
      let parentId = item.ParentId
      while (parentId !== null && parentId !== undefined) {
        matchedIds.add(parentId)
        const parent = flatData.value.find(p => p.Id === parentId)
        parentId = parent ? parent.ParentId : null
      }
    }
  }
  return flatData.value.filter(item => matchedIds.has(item.Id))
})

function onSearch(): void {
  // Reactive via computed
}

function getTypeLabel(type: string): string {
  const map: Record<string, string> = {
    menu: t('菜单权限'),
    api: t('API权限'),
    action: t('操作权限'),
    data: t('数据权限')
  }
  return map[type] || type
}

function getTypeStyle(type: string): Record<string, string> {
  const colorMap: Record<string, string> = {
    menu: '#1890ff',
    api: '#52c41a',
    action: '#fa8c16',
    data: '#722ed1'
  }
  const color = colorMap[type] || '#999'
  return {
    backgroundColor: color + '1a',
    color,
    padding: '2px 8px',
    borderRadius: '4px',
    fontSize: '12px',
    fontWeight: '500'
  }
}

function getMethodStyle(method: string): Record<string, string> {
  const colorMap: Record<string, string> = {
    GET: '#52c41a',
    POST: '#1890ff',
    PUT: '#fa8c16',
    DELETE: '#f5222d'
  }
  const color = colorMap[method] || '#999'
  return {
    backgroundColor: color + '1a',
    color,
    padding: '2px 8px',
    borderRadius: '4px',
    fontSize: '12px',
    fontWeight: '500'
  }
}

async function loadData(): Promise<void> {
  loading.value = true
  try {
    const tree = await getPermissionTreeApi()
    flatData.value = flattenTree(tree || [])
  } catch {
    flatData.value = []
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.platform-permissions-page {
  padding: 16px;
}

.page-title {
  margin: 0 0 12px 0;
  font-size: 18px !important;
  font-weight: 600;
  color: var(--base-text-color);
}

.search-bar {
  margin-bottom: 12px;
}

/* Mobile responsive */
@media (max-width: 768px) {
  .platform-permissions-page {
    padding: 8px;
  }

  .page-title {
    font-size: 16px;
  }
}
</style>
