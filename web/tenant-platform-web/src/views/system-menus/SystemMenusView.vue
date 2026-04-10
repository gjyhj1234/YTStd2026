<template>
  <div>
    <div class="page-header">
      <h2>{{ $t('route.systemMenus') }}</h2>
      <div class="page-header-actions">
        <PageHelpEntry @click="showGuide = true" />
        <DxButton :text="$t('common.create')" icon="add" type="default" @click="showCreatePopup = true" />
      </div>
    </div>

    <FunctionDescriptionCard
      purpose="管理平台侧边栏菜单结构，包括目录和页面的层级关系、排序、启用/禁用等。"
      data-scope="全局菜单树，影响所有用户的导航结构。"
      permission-note="需要 platform:permission:view 权限。"
      risk-note="禁用菜单将导致对应页面无法通过导航进入。"
      :collapsible="true"
    />

    <div class="card">
      <DxTreeList
        :data-source="treeData"
        :show-borders="true"
        :column-auto-width="true"
        :hover-state-enabled="true"
        key-expr="Id"
        parent-id-expr="ParentId"
        :root-value="0"
        :auto-expand-all="true"
      >
        <DxColumn data-field="Name" :caption="$t('common.name')" />
        <DxColumn data-field="Code" :caption="$t('菜单编码')" />
        <DxColumn data-field="Icon" :caption="$t('图标')" :width="80" />
        <DxColumn data-field="RoutePath" :caption="$t('路由路径')" />
        <DxColumn data-field="PermissionCode" :caption="$t('权限编码')" />
        <DxColumn data-field="SortOrder" :caption="$t('排序')" :width="70" />
        <DxColumn data-field="IsEnabled" :caption="$t('common.status')" cell-template="statusCell" :width="80" />
        <DxColumn :caption="$t('common.actions')" cell-template="actionCell" :width="180" />
        <template #statusCell="{ data: cellData }">
          <StatusTag :status="cellData.value ? 'Active' : 'Disabled'" />
        </template>
        <template #actionCell="{ data: cellData }">
          <DxButton :text="$t('common.edit')" styling-mode="text" @click="onEdit(cellData.data)" />
          <DxButton
            v-if="cellData.data.IsEnabled"
            :text="$t('common.disable')"
            styling-mode="text"
            type="danger"
            @click="onDisable(cellData.data.Id)"
          />
          <DxButton
            v-if="!cellData.data.IsEnabled"
            :text="$t('common.enable')"
            styling-mode="text"
            type="success"
            @click="onEnable(cellData.data.Id)"
          />
        </template>
      </DxTreeList>
    </div>

    <DxPopup
      :visible="showCreatePopup"
      :title="$t('新增菜单')"
      :width="500"
      :height="'auto'"
      :show-close-button="true"
      @hiding="showCreatePopup = false"
    >
      <DxForm :form-data="createForm" :col-count="1" label-mode="floating">
        <DxSimpleItem data-field="Code">
          <DxLabel :text="$t('菜单编码')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="Name">
          <DxLabel :text="$t('common.name')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="Icon">
          <DxLabel :text="$t('图标')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="RoutePath">
          <DxLabel :text="$t('路由路径')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="PermissionCode">
          <DxLabel :text="$t('权限编码')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="Remark" editor-type="dxTextArea">
          <DxLabel :text="$t('common.remark')" />
        </DxSimpleItem>
        <DxButtonItem>
          <DxButtonOptions :text="$t('common.submit')" type="default" :use-submit-behavior="false" @click="handleCreate" />
        </DxButtonItem>
      </DxForm>
    </DxPopup>

    <OperationGuideDrawer
      v-model:visible="showGuide"
      title="菜单管理操作指引"
      entry-path="左侧菜单 → 系统设置 → 菜单管理"
      :steps="['查看菜单树结构', '点击新增创建菜单或目录', '拖拽或手动设置排序', '启用/禁用控制菜单可见性']"
      :field-notes="['编码：全局唯一标识', '路由路径：对应前端页面路由', '权限编码：关联权限控制']"
      :error-notes="['编码重复将创建失败', '禁用父菜单会隐藏所有子菜单']"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { DxTreeList, DxColumn } from 'devextreme-vue/tree-list'
import { DxButton } from 'devextreme-vue/button'
import { DxPopup } from 'devextreme-vue/popup'
import { DxForm, DxSimpleItem, DxLabel, DxButtonItem, DxButtonOptions } from 'devextreme-vue/form'
import StatusTag from '@/components/StatusTag.vue'
import FunctionDescriptionCard from '@/components/help/FunctionDescriptionCard.vue'
import OperationGuideDrawer from '@/components/help/OperationGuideDrawer.vue'
import PageHelpEntry from '@/components/help/PageHelpEntry.vue'
import { getMenuTree, createMenu, enableMenu, disableMenu, type MenuRepDTO, type CreateMenuReqDTO } from '@/api/menus'

const showGuide = ref(false)
const showCreatePopup = ref(false)
const treeData = ref<MenuRepDTO[]>([])

const createForm = reactive<CreateMenuReqDTO>({
  ParentId: 0,
  Code: '',
  Name: '',
  Icon: '',
  RoutePath: '',
  PermissionCode: '',
  MenuType: 1,
  Remark: '',
})

/** 递归展平菜单树为列表（DxTreeList 需要平铺数据） */
function flattenTree(items: MenuRepDTO[]): MenuRepDTO[] {
  const result: MenuRepDTO[] = []
  for (const item of items) {
    result.push(item)
    if (item.Children && item.Children.length > 0) {
      const children = flattenTree(item.Children)
      for (const child of children) {
        result.push(child)
      }
    }
  }
  return result
}

async function loadData() {
  try {
    const res = await getMenuTree()
    treeData.value = flattenTree(res.Data ?? [])
  } catch {
    // 接口未就绪时保持空列表
  }
}

async function handleCreate() {
  try {
    await createMenu(createForm)
    showCreatePopup.value = false
    Object.assign(createForm, { ParentId: 0, Code: '', Name: '', Icon: '', RoutePath: '', PermissionCode: '', MenuType: 1, Remark: '' })
    await loadData()
  } catch {
    // 错误由 http 层统一处理
  }
}

function onEdit(_menu: MenuRepDTO) {
  // 后续阶段完善编辑功能
}

async function onEnable(id: number) {
  try {
    await enableMenu(id)
    await loadData()
  } catch {
    // 错误由 http 层统一处理
  }
}

async function onDisable(id: number) {
  try {
    await disableMenu(id)
    await loadData()
  } catch {
    // 错误由 http 层统一处理
  }
}

onMounted(loadData)
</script>
