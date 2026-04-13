import { execSync } from 'child_process'

/**
 * 数据库管理工具函数
 *
 * 用于 E2E 测试中的数据库重置、数据清理等操作。
 * 这些函数通过 psql 命令行工具直接操作 PostgreSQL。
 *
 * 前提条件：psql 命令可用（ubuntu-latest runner 默认安装）
 *
 * 安全说明：这些函数仅用于 E2E 测试环境，所有表名和 SQL 均为已知常量。
 *           不接受外部用户输入。
 */

// 数据库连接配置（与 Program.cs 中的 DB.Init 一致）
const DB_CONFIG = {
  host: 'localhost',
  port: 5432,
  database: 'test1',
  username: 'postgres',
  password: 'gjwq1234',
}

/** 仅允许字母、数字、下划线的标识符验证（防止 SQL 注入） */
function sanitizeIdentifier(name: string): string {
  if (!/^[a-zA-Z_][a-zA-Z0-9_]*$/.test(name)) {
    throw new Error(`Invalid identifier: ${name}`)
  }
  return name
}

/** 执行已知安全的 SQL 命令（仅内部使用，sql 参数必须由本模块构建） */
function executeInternalSql(sql: string, database?: string): string {
  const db = database || DB_CONFIG.database
  const cmd = `PGPASSWORD=${DB_CONFIG.password} psql -h ${DB_CONFIG.host} -p ${DB_CONFIG.port} -U ${DB_CONFIG.username} -d ${db} -t -A -c ${JSON.stringify(sql)} 2>&1`
  try {
    return execSync(cmd, { encoding: 'utf-8' })
  } catch (error) {
    const err = error as { stderr?: string; stdout?: string }
    console.error(`[db-helpers] SQL execution failed: ${err.stderr || err.stdout}`)
    throw error
  }
}

/** 删除并重建 test1 数据库 */
export function resetDatabase(): void {
  console.log('[db-helpers] 正在重置数据库...')

  // 断开所有连接
  try {
    executeInternalSql(
      "SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = 'test1' AND pid <> pg_backend_pid();",
      'postgres'
    )
  } catch {
    // 可能没有活动连接，忽略
  }

  // 删除数据库
  try {
    executeInternalSql('DROP DATABASE IF EXISTS test1;', 'postgres')
  } catch (error) {
    console.warn('[db-helpers] Drop database warning:', error)
  }

  // 重建数据库
  executeInternalSql('CREATE DATABASE test1;', 'postgres')
  console.log('[db-helpers] 数据库已重置')
}

/** 清空指定表（保留表结构） */
export function truncateTable(tableName: string): void {
  const safeName = sanitizeIdentifier(tableName)
  executeInternalSql(`TRUNCATE TABLE "${safeName}" CASCADE;`)
}

/** 清空所有业务数据表（保留种子数据相关表） */
export function truncateAllBusinessTables(): void {
  // 获取所有表名
  const result = executeInternalSql(
    "SELECT tablename FROM pg_tables WHERE schemaname = 'public' ORDER BY tablename;"
  )

  const tables = result
    .split('\n')
    .map(line => line.trim())
    .filter(line => line && /^[a-zA-Z_][a-zA-Z0-9_]*$/.test(line))

  if (tables.length > 0) {
    const tableList = tables.map(t => `"${sanitizeIdentifier(t)}"`).join(', ')
    executeInternalSql(`TRUNCATE TABLE ${tableList} CASCADE;`)
    console.log(`[db-helpers] 已清空 ${tables.length} 张表`)
  }
}

/** 检查数据库连接是否正常 */
export function checkDatabaseConnection(): boolean {
  try {
    const result = executeInternalSql('SELECT 1 AS test;')
    return result.includes('1')
  } catch {
    return false
  }
}

/** 检查指定表是否存在 */
export function tableExists(tableName: string): boolean {
  const safeName = sanitizeIdentifier(tableName)
  try {
    const result = executeInternalSql(
      `SELECT EXISTS (SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename = '${safeName}');`
    )
    return result.includes('t')
  } catch {
    return false
  }
}

/** 获取指定表的行数 */
export function getTableRowCount(tableName: string): number {
  const safeName = sanitizeIdentifier(tableName)
  try {
    const result = executeInternalSql(`SELECT COUNT(*) FROM "${safeName}";`)
    const match = result.match(/(\d+)/)
    return match ? parseInt(match[1], 10) : 0
  } catch {
    return -1
  }
}
