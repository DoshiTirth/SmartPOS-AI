import { useState, useEffect } from 'react'
import api from '../api/api'
import {
  DollarSign, ShoppingCart, AlertTriangle,
  Shield, TrendingUp, RefreshCw, Loader2
} from 'lucide-react'
import {
  AreaChart, Area, XAxis, YAxis, CartesianGrid,
  Tooltip, ResponsiveContainer, BarChart, Bar
} from 'recharts'

function KPICard({ title, value, subtitle, icon: Icon, color, bg }) {
  return (
    <div className="kpi-card">
      <div className="flex items-center justify-between mb-3">
        <p className="text-sm font-medium text-muted">{title}</p>
        <div className={`w-9 h-9 ${bg} rounded-lg flex items-center justify-center`}>
          <Icon size={16} className={color}/>
        </div>
      </div>
      <p className={`text-3xl font-bold ${color} mb-1`}>{value}</p>
      <p className="text-xs text-muted">{subtitle}</p>
    </div>
  )
}

const CustomTooltip = ({ active, payload, label }) => {
  if (active && payload && payload.length) {
    return (
      <div className="bg-surface border border-border rounded-lg shadow-lg p-3">
        <p className="text-xs text-muted mb-1">{label}</p>
        <p className="text-sm font-bold text-primary">
          ${payload[0].value?.toFixed(2)}
        </p>
      </div>
    )
  }
  return null
}

export default function Dashboard() {
  const [summary, setSummary]       = useState(null)
  const [salesData, setSalesData]   = useState([])
  const [topProducts, setTopProducts] = useState([])
  const [loading, setLoading]       = useState(true)
  const [refreshing, setRefreshing] = useState(false)

  const loadData = async () => {
    try {
      const [summaryRes, salesRes, productsRes] = await Promise.all([
        api.get('/dashboard/summary'),
        api.get('/dashboard/sales/last30days'),
        api.get('/dashboard/products/topselling?topN=5')
      ])

      setSummary(summaryRes.data)

      const formatted = salesRes.data.map(d => ({
        date:    new Date(d.saleDate).toLocaleDateString('en-US', { month: 'short', day: 'numeric' }),
        revenue: d.dailyRevenue,
        count:   d.transactionCount
      }))
      setSalesData(formatted)
      setTopProducts(productsRes.data)
    } catch (err) {
      console.error('Dashboard load error:', err)
    } finally {
      setLoading(false)
      setRefreshing(false)
    }
  }

  useEffect(() => { loadData() }, [])

  const handleRefresh = () => {
    setRefreshing(true)
    loadData()
  }

  if (loading) return (
    <div className="flex items-center justify-center h-full">
      <Loader2 size={32} className="animate-spin text-primary"/>
    </div>
  )

  return (
    <div className="p-8 space-y-6">

      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-text">Dashboard</h1>
          <p className="text-muted text-sm mt-1">
            Real-time store performance overview
          </p>
        </div>
        <button onClick={handleRefresh} className="btn-secondary" disabled={refreshing}>
          <RefreshCw size={14} className={refreshing ? 'animate-spin' : ''}/>
          Refresh
        </button>
      </div>

      {/* KPI Cards */}
      <div className="grid grid-cols-4 gap-4">
        <KPICard
          title="Revenue Today"
          value={`$${summary?.totalRevenueToday?.toFixed(2) ?? '0.00'}`}
          subtitle={`${summary?.totalTransactionsToday ?? 0} transactions`}
          icon={DollarSign}
          color="text-primary"
          bg="bg-blue-50"
        />
        <KPICard
          title="Revenue This Month"
          value={`$${summary?.totalRevenueMonth?.toFixed(2) ?? '0.00'}`}
          subtitle={`${summary?.totalTransactionsMonth ?? 0} transactions`}
          icon={TrendingUp}
          color="text-secondary"
          bg="bg-blue-50"
        />
        <KPICard
          title="Low Stock Alerts"
          value={summary?.lowStockProductCount ?? 0}
          subtitle="products need restocking"
          icon={AlertTriangle}
          color="text-warning"
          bg="bg-amber-50"
        />
        <KPICard
          title="Unreviewed Anomalies"
          value={summary?.unreviewedAnomalies ?? 0}
          subtitle="flagged transactions"
          icon={Shield}
          color="text-danger"
          bg="bg-red-50"
        />
      </div>

      {/* Charts Row */}
      <div className="grid grid-cols-3 gap-4">

        {/* Sales Chart - 2/3 width */}
        <div className="card col-span-2">
          <div className="flex items-center justify-between mb-6">
            <div>
              <h2 className="font-semibold text-text">Sales Last 30 Days</h2>
              <p className="text-xs text-muted mt-0.5">Daily revenue trend</p>
            </div>
            <div className="flex items-center gap-2">
              <div className="w-3 h-3 bg-primary rounded-full"/>
              <span className="text-xs text-muted">Revenue</span>
            </div>
          </div>
          {salesData.length > 0 ? (
            <ResponsiveContainer width="100%" height={220}>
              <AreaChart data={salesData}>
                <defs>
                  <linearGradient id="revenueGrad" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="5%"  stopColor="#1E40AF" stopOpacity={0.15}/>
                    <stop offset="95%" stopColor="#1E40AF" stopOpacity={0}/>
                  </linearGradient>
                </defs>
                <CartesianGrid strokeDasharray="3 3" stroke="#E2E8F0"/>
                <XAxis
                  dataKey="date"
                  tick={{ fontSize: 11, fill: '#64748B' }}
                  tickLine={false}
                  axisLine={false}
                />
                <YAxis
                  tick={{ fontSize: 11, fill: '#64748B' }}
                  tickLine={false}
                  axisLine={false}
                  tickFormatter={v => `$${v}`}
                />
                <Tooltip content={<CustomTooltip />}/>
                <Area
                  type="monotone"
                  dataKey="revenue"
                  stroke="#1E40AF"
                  strokeWidth={2}
                  fill="url(#revenueGrad)"
                />
              </AreaChart>
            </ResponsiveContainer>
          ) : (
            <div className="h-[220px] flex items-center justify-center">
              <p className="text-muted text-sm">No sales data available yet</p>
            </div>
          )}
        </div>

        {/* Top Products - 1/3 width */}
        <div className="card">
          <h2 className="font-semibold text-text mb-1">Top Products</h2>
          <p className="text-xs text-muted mb-4">By units sold (30 days)</p>
          {topProducts.length > 0 ? (
            <div className="space-y-3">
              {topProducts.map((p, i) => (
                <div key={p.productId}>
                  <div className="flex items-center justify-between mb-1">
                    <span className="text-sm text-text truncate max-w-[140px]" title={p.productName}>
                      {p.productName}
                    </span>
                    <span className="text-xs font-semibold text-primary">
                      {p.totalUnitsSold} units
                    </span>
                  </div>
                  <div className="w-full bg-surface2 rounded-full h-1.5">
                    <div
                      className="bg-primary h-1.5 rounded-full transition-all duration-500"
                      style={{
                        width: `${(p.totalUnitsSold / topProducts[0].totalUnitsSold) * 100}%`
                      }}
                    />
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="flex items-center justify-center h-32">
              <p className="text-muted text-sm">No data yet</p>
            </div>
          )}
        </div>
      </div>

      {/* Transaction Volume Chart */}
      {salesData.length > 0 && (
        <div className="card">
          <div className="flex items-center justify-between mb-6">
            <div>
              <h2 className="font-semibold text-text">Transaction Volume</h2>
              <p className="text-xs text-muted mt-0.5">Number of transactions per day</p>
            </div>
          </div>
          <ResponsiveContainer width="100%" height={180}>
            <BarChart data={salesData}>
              <CartesianGrid strokeDasharray="3 3" stroke="#E2E8F0"/>
              <XAxis
                dataKey="date"
                tick={{ fontSize: 11, fill: '#64748B' }}
                tickLine={false}
                axisLine={false}
              />
              <YAxis
                tick={{ fontSize: 11, fill: '#64748B' }}
                tickLine={false}
                axisLine={false}
                allowDecimals={false}
              />
              <Tooltip
                contentStyle={{
                  background: '#FFFFFF',
                  border: '1px solid #E2E8F0',
                  borderRadius: '8px',
                  fontSize: '12px'
                }}
              />
              <Bar dataKey="count" fill="#3B82F6" radius={[4,4,0,0]} name="Transactions"/>
            </BarChart>
          </ResponsiveContainer>
        </div>
      )}

      {/* Quick Stats */}
      <div className="card">
        <h2 className="font-semibold text-text mb-4">Store Status</h2>
        <div className="grid grid-cols-3 gap-6">
          <div className="flex items-center gap-3">
            <div className="w-2 h-2 bg-success rounded-full"/>
            <div>
              <p className="text-sm font-medium text-text">POS Terminal</p>
              <p className="text-xs text-muted">Operational</p>
            </div>
          </div>
          <div className="flex items-center gap-3">
            <div className="w-2 h-2 bg-success rounded-full"/>
            <div>
              <p className="text-sm font-medium text-text">API Service</p>
              <p className="text-xs text-muted">Running</p>
            </div>
          </div>
          <div className="flex items-center gap-3">
            <div className="w-2 h-2 bg-success rounded-full"/>
            <div>
              <p className="text-sm font-medium text-text">Database</p>
              <p className="text-xs text-muted">Connected</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}