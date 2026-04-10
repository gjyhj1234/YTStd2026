<template>
  <div>
    <div class="page-header">
      <h2>{{ $t('平台权限管理') }}</h2>
      <div class="page-header-actions">
        <PageHelpEntry @click="showGuide = true" />
      </div>
    </div>

    <FunctionDescriptionCard
      :purpose="$t('权限管理展示平台所有权限码层级结构')"
      :data-scope="$t('平台全部权限数据由种子数据维护')"
      :permission-note="$t('需要权限查看权限')"
      :risk-note="$t('权限数据由系统种子数据管理仅供查看')"
      :collapsible="true"
    />

    <div class="card">
      <div class="filter-bar">
        <DxTextBox
          v-model:value="filterKeyword"
          :placeholder="$t('搜索权限编码或名称')"
          :width="260"
          mode="search"
          value-change-event="input"
          @value-changed="onFilterChanged"
        />
      </div>

      <DxLoadPanel :visible="isLoading" :position="{ of: '.card' }" />

      <DxTreeList
        :data-source="treeData"
        :show-borders="true"
        :column-auto-width="true"
        :hover-state-enabled="true"
        key-expr="Id"
        parent-id-expr="ParentId"
        :auto-expand-all="true"
        :filter-mode="'fullBranch'"
        :search-panel="{ visible: false }"
      >
        <DxColumn data-field="Id" caption="ID" :width="80" />
        <DxColumn data-field="Code" :caption="$t('权限编码')" />
        <DxColumn data-field="Name" :caption="$t('权限名称')" />
        <DxColumn data-field="PermissionType" :caption="$t('权限类型')" cell-template="typeCell" :width="120" />
        <DxColumn data-field="Path" :caption="$t('路径')" />
        <DxColumn data-field="Method" :caption="$t('HTTP方法')" :width="100" />
        <template #typeCell="{ data: cellData }">
          <span class="permission-type-tag" :class="permissionTypeClass(cellData.value)">
            {{ permissionTypeLabel(cellData.value) }}
          </span>
        </template>
      </DxTreeList>
    </div>

    <OperationGuideDrawer
      v-model:visible="showGuide"
      :title="$t('权限管理操作指引')"
      :entry-path="$t('权限管理入口路径')"
      :steps="guideSteps"
      :field-notes="guideFieldNotes"
      :error-notes="guideErrorNotes"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { DxTreeList, DxColumn } from 'devextreme-vue/tree-list'
import { DxTextBox } from 'devextreme-vue/text-box'
import { DxLoadPanel } from 'devextreme-vue/load-panel'
import FunctionDescriptionCard from '@/components/help/FunctionDescriptionCard.vue'
import OperationGuideDrawer from '@/components/help/OperationGuideDrawer.vue'
import PageHelpEntry from '@/components/help/PageHelpEntry.vue'
import {
  getPermissions,
  type PlatformPermissionRepDTO,
} from '@/api/platformPermissions'

interface FlatPermission {
  Id: number
  Code: string
  Name: string
  PermissionType: string
  ParentId: number | null
  Path: string | null
  Method: string | null
}

const filterKeyword = ref('')
const showGuide = ref(false)
const isLoading = ref(false)
const { t } = useI18n()

const allData = ref<FlatPermission[]>([])
const treeData = ref<FlatPermission[]>([])

function flattenTree(nodes: PlatformPermissionRepDTO[]): FlatPermission[] {
  const result: FlatPermission[] = []
  for (const node of nodes) {
    result.push({
      Id: node.Id,
      Code: node.Code,
      Name: node.Name,
      PermissionType: node.PermissionType,
      ParentId: node.ParentId,
      Path: node.Path,
      Method: node.Method,
    })
    if (node.Children && node.Children.length > 0) {
      result.push(...flattenTree(node.Children))
    }
  }
  return result
}

function permissionTypeLabel(type: string): string {
  switch (type) {
    case 'Menu':
      return t('菜单权限')
    case 'Api':
      return t('API权限')
    case 'Operation':
      return t('操作权限')
    case 'Data':
      return t('数据权限')
    default:
      return type
  }
}

function permissionTypeClass(type: string): string {
  switch (type) {
    case 'Menu':
      return 'menu'
    case 'Api':
      return 'api'
    case 'Operation':
      return 'operation'
    case 'Data':
      return 'data'
    default:
      return ''
  }
}

function onFilterChanged() {
  const keyword = filterKeyword.value?.toLowerCase()
  if (!keyword) {
    treeData.value = allData.value
    return
  }
  const matchedIds = new Set<number>()
  for (const item of allData.value) {
    if (item.Code.toLowerCase().includes(keyword) || item.Name.toLowerCase().includes(keyword)) {
      matchedIds.add(item.Id)
      let current = item
      while (current.ParentId !== null) {
        matchedIds.add(current.ParentId)
        const parent = allData.value.find(p => p.Id === current.ParentId)
        if (!parent) break
        current = parent
      }
    }
  }
  treeData.value = allData.value.filter(item => matchedIds.has(item.Id))
}

async function loadData() {
  isLoading.value = true
  try {
    const res = await getPermissions()
    allData.value = flattenTree(res.Data!)
    treeData.value = allData.value
  } finally {
    isLoading.value = false
  }
}

const guideSteps = computed(() => [
  t('进入权限管理页面查看权限层级结构'),
  t('使用搜索框按权限编码或名称筛选'),
  t('展开折叠树节点查看子权限'),
])
const guideFieldNotes = computed(() => [
  t('权限编码唯一标识格式为模块资源操作'),
  t('权限类型菜单权限控制页面可见性API权限控制接口'),
  t('路径和方法仅API权限类型显示'),
])
const guideErrorNotes = computed(() => [
  t('权限数据由系统种子数据管理无法直接修改'),
])

onMounted(loadData)
</script>

<style scoped>
.permission-type-tag {
  display: inline-block;
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 12px;
}
.permission-type-tag.menu {
  background-color: var(--dx-color-primary-light, #e3f2fd);
  color: var(--dx-color-primary, #1565c0);
}
.permission-type-tag.api {
  background-color: var(--dx-color-success-light, #e8f5e9);
  color: var(--dx-color-success, #2e7d32);
}
.permission-type-tag.operation {
  background-color: var(--dx-color-warning-light, #fff3e0);
  color: var(--dx-color-warning, #e65100);
}
.permission-type-tag.data {
  background-color: var(--dx-color-info-light, #f3e5f5);
  color: var(--dx-color-info, #7b1fa2);
}
</style>
