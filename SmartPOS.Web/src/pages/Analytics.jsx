import { useState, useEffect, useRef } from 'react'
import {
  AreaChart, Area, BarChart, Bar, LineChart, Line,
  PieChart, Pie, Cell, XAxis, YAxis, CartesianGrid,
  Tooltip, ResponsiveContainer, Legend
} from 'recharts'
import {
  TrendingUp, TrendingDown, DollarSign, ShoppingCart,
  Users, Package, Calendar, Download, RefreshCw,
  ArrowUpRight, ArrowDownRight, Zap, Target, Award
} from 'lucide-react'

// Mock data 

const revenueData = [
  { month: 'Jul', revenue: 42800, transactions: 1240, target: 40000 },
  { month: 'Aug', revenue: 51200, transactions: 1480, target: 45000 },
  { month: 'Sep', revenue: 47600, transactions: 1320, target: 48000 },
  { month: 'Oct', revenue: 58900, transactions: 1710, target: 50000 },
  { month: 'Nov', revenue: 63400, transactions: 1890, target: 55000 },
  { month: 'Dec', revenue: 78200, transactions: 2340, target: 60000 },
  { month: 'Jan', revenue: 54100, transactions: 1560, target: 62000 },
  { month: 'Feb', revenue: 59800, transactions: 1720, target: 65000 },
  { month: 'Mar', revenue: 71300, transactions: 2060, target: 68000 },
  { month: 'Apr', revenue: 68500, transactions: 1980, target: 70000 },
  { month: 'May', revenue: 82100, transactions: 2390, target: 75000 },
  { month: 'Jun', revenue: 91400, transactions: 2650, target: 80000 },
]

const dailyData = [
  { day: 'Mon', revenue: 12400, transactions: 360 },
  { day: 'Tue', revenue: 14800, transactions: 420 },
  { day: 'Wed', revenue: 11200, transactions: 310 },
  { day: 'Thu', revenue: 16900, transactions: 490 },
  { day: 'Fri', revenue: 21300, transactions: 620 },
  { day: 'Sat', revenue: 18700, transactions: 540 },
  { day: 'Sun', revenue: 9800,  transactions: 280 },
]

const categoryData = [
  { name: 'Electronics',   value: 34.2, revenue: 31245, color: '#1d4ed8' },
  { name: 'Clothing',      value: 22.8, revenue: 20831, color: '#d97706' },
  { name: 'Food & Bev',    value: 18.4, revenue: 16812, color: '#16a34a' },
  { name: 'Home & Garden', value: 13.1, revenue: 11973, color: '#9333ea' },
  { name: 'Sports',        value: 7.6,  revenue: 6945,  color: '#0891b2' },
  { name: 'Other',         value: 3.9,  revenue: 3564,  color: '#64748b' },
]

const topProducts = [
  { name: 'Wireless Headphones Pro', sku: 'EL-001', sold: 284, revenue: 56716, trend: 12.4  },
  { name: 'Slim Fit Chino Pants',    sku: 'CL-042', sold: 412, revenue: 20188, trend: 8.1   },
  { name: 'Organic Cold Brew 12-pk', sku: 'FB-118', sold: 631, revenue: 15144, trend: 24.7  },
  { name: '4K Smart Monitor 27"',    sku: 'EL-007', sold: 98,  revenue: 48902, trend: -3.2  },
  { name: 'Yoga Mat Premium',        sku: 'SP-033', sold: 327, revenue: 16023, trend: 18.9  },
]

const hourlyData = Array.from({ length: 24 }, (_, h) => ({
  hour: `${String(h).padStart(2, '0')}:00`,
  transactions: h < 8  ? Math.floor(Math.random() * 15 + 2)
    : h < 12 ? Math.floor(Math.random() * 80 + 40)
    : h < 14 ? Math.floor(Math.random() * 120 + 80)
    : h < 18 ? Math.floor(Math.random() * 100 + 60)
    : h < 21 ? Math.floor(Math.random() * 70 + 30)
    :           Math.floor(Math.random() * 20 + 5),
}))

const paymentData = [
  { method: 'Credit Card', pct: 48.3, color: '#1d4ed8' },
  { method: 'Debit Card',  pct: 31.7, color: '#0891b2' },
  { method: 'Cash',        pct: 12.4, color: '#d97706' },
  { method: 'Mobile Pay',  pct: 7.6,  color: '#16a34a' },
]

// Reusable components 

function KpiCard({ icon: Icon, label, value, change, color = 'blue', prefix = '', suffix = '' }) {
  const positive = change >= 0
  const colorMap = {
    blue:   { ring: 'bg-blue-100',   icon: 'text-blue-700'   },
    amber:  { ring: 'bg-amber-100',  icon: 'text-amber-700'  },
    green:  { ring: 'bg-green-100',  icon: 'text-green-700'  },
    purple: { ring: 'bg-purple-100', icon: 'text-purple-700' },
  }
  const c = colorMap[color]

  return (
    <div className="card-hover">
      <div className="flex items-start justify-between mb-4">
        <div className={`w-10 h-10 rounded-xl ${c.ring} flex items-center justify-center`}>
          <Icon size={18} className={c.icon} />
        </div>
        <span className={`flex items-center gap-1 text-xs font-semibold px-2 py-1 rounded-full ${
          positive ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'
        }`}>
          {positive ? <ArrowUpRight size={11} /> : <ArrowDownRight size={11} />}
          {Math.abs(change)}%
        </span>
      </div>
      <p className="text-muted text-xs font-medium uppercase tracking-wider mb-1">{label}</p>
      <p className="text-text font-bold text-2xl font-mono">
        {prefix}{typeof value === 'number' ? value.toLocaleString() : value}{suffix}
      </p>
      <p className="text-muted text-xs mt-1">vs. last month</p>
    </div>
  )
}

const ChartTooltip = ({ active, payload, label, prefix = '', suffix = '' }) => {
  if (!active || !payload?.length) return null
  return (
    <div className="bg-surface border border-border rounded-lg shadow-lg p-3 text-sm min-w-[140px]">
      <p className="text-muted text-xs mb-2 font-medium">{label}</p>
      {payload.map((p, i) => (
        <div key={i} className="flex items-center justify-between gap-4">
          <span className="text-muted flex items-center gap-1.5">
            <span className="w-2 h-2 rounded-full inline-block" style={{ background: p.color }} />
            {p.name}
          </span>
          <span className="text-text font-semibold font-mono">
            {prefix}{typeof p.value === 'number' ? p.value.toLocaleString() : p.value}{suffix}
          </span>
        </div>
      ))}
    </div>
  )
}

// Main page

export default function Analytics() {
  const [period, setPeriod] = useState('12m')
  const [activeCategory, setActiveCategory] = useState(null)
  const [refreshing, setRefreshing] = useState(false)

  const chartData = period === '7d' ? dailyData : revenueData
  const xKey      = period === '7d' ? 'day' : 'month'

  const handleRefresh = () => {
    setRefreshing(true)
    setTimeout(() => setRefreshing(false), 1200)
  }

  const totalRevenue = revenueData.reduce((s, d) => s + d.revenue, 0)
  const totalTx      = revenueData.reduce((s, d) => s + d.transactions, 0)
  const avgOrder     = Math.round(totalRevenue / totalTx)

  return (
    <div className="p-6 space-y-6">

      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-text text-xl font-bold">Analytics</h1>
          <p className="text-muted text-sm mt-0.5">Performance overview and business intelligence</p>
        </div>
        <div className="flex items-center gap-2">
          <div className="flex items-center bg-surface2 border border-border rounded-lg p-1 gap-1">
            {['7d', '30d', '12m'].map(p => (
              <button
                key={p}
                onClick={() => setPeriod(p)}
                className={`px-3 py-1.5 rounded-md text-xs font-semibold transition-all duration-150 ${
                  period === p ? 'bg-primary text-white shadow-sm' : 'text-muted hover:text-text'
                }`}
              >
                {p === '7d' ? '7 Days' : p === '30d' ? '30 Days' : '12 Months'}
              </button>
            ))}
          </div>
          <button onClick={handleRefresh} className="btn-secondary px-3 py-2 text-xs">
            <RefreshCw size={13} className={refreshing ? 'animate-spin' : ''} />
            Refresh
          </button>
          <button className="btn-primary px-3 py-2 text-xs">
            <Download size={13} />
            Export
          </button>
        </div>
      </div>

      {/* KPI cards */}
      <div className="grid grid-cols-4 gap-4">
        <KpiCard icon={DollarSign}   label="Total Revenue"   value={totalRevenue} change={14.2} color="blue"   prefix="$" />
        <KpiCard icon={ShoppingCart} label="Transactions"    value={totalTx}      change={9.7}  color="green"  />
        <KpiCard icon={Target}       label="Avg Order Value" value={avgOrder}     change={4.1}  color="amber"  prefix="$" />
        <KpiCard icon={Users}        label="Conversion Rate" value={68.4}         change={-1.3} color="purple" suffix="%" />
      </div>

      {/* Revenue vs Target */}
      <div className="card">
        <div className="flex items-center justify-between mb-5">
          <div>
            <h2 className="text-text font-semibold text-sm">Revenue vs Target</h2>
            <p className="text-muted text-xs mt-0.5">Actual performance against monthly goals</p>
          </div>
          <div className="flex items-center gap-4 text-xs text-muted">
            <span className="flex items-center gap-1.5">
              <span className="w-3 h-0.5 bg-primary inline-block rounded" /> Revenue
            </span>
            <span className="flex items-center gap-1.5">
              <span className="w-3 h-0.5 bg-amber-400 inline-block rounded" /> Target
            </span>
          </div>
        </div>
        <ResponsiveContainer width="100%" height={240}>
          <AreaChart data={chartData} margin={{ top: 4, right: 4, left: 0, bottom: 0 }}>
            <defs>
              <linearGradient id="revGrad" x1="0" y1="0" x2="0" y2="1">
                <stop offset="5%"  stopColor="#1d4ed8" stopOpacity={0.15} />
                <stop offset="95%" stopColor="#1d4ed8" stopOpacity={0}    />
              </linearGradient>
            </defs>
            <CartesianGrid strokeDasharray="3 3" stroke="rgba(100,116,139,0.12)" vertical={false} />
            <XAxis dataKey={xKey} tick={{ fill: '#64748b', fontSize: 11 }} axisLine={false} tickLine={false} />
            <YAxis tick={{ fill: '#64748b', fontSize: 11 }} axisLine={false} tickLine={false}
              tickFormatter={v => `$${(v / 1000).toFixed(0)}k`} />
            <Tooltip content={<ChartTooltip prefix="$" />} />
            <Area type="monotone" dataKey="revenue" name="Revenue" stroke="#1d4ed8"
              strokeWidth={2} fill="url(#revGrad)" dot={false} activeDot={{ r: 4, strokeWidth: 0 }} />
            <Line type="monotone" dataKey="target" name="Target" stroke="#d97706"
              strokeWidth={1.5} strokeDasharray="5 4" dot={false} />
          </AreaChart>
        </ResponsiveContainer>
      </div>

      {/* Hourly volume + Payment methods */}
      <div className="grid grid-cols-3 gap-4">

        <div className="card col-span-2">
          <div className="flex items-center justify-between mb-5">
            <div>
              <h2 className="text-text font-semibold text-sm">Hourly Transaction Volume</h2>
              <p className="text-muted text-xs mt-0.5">Today's traffic pattern by hour</p>
            </div>
            <span className="badge-info text-xs px-2 py-1 rounded-md">Today</span>
          </div>
          <ResponsiveContainer width="100%" height={180}>
            <BarChart data={hourlyData} margin={{ top: 0, right: 4, left: 0, bottom: 0 }}>
              <CartesianGrid strokeDasharray="3 3" stroke="rgba(100,116,139,0.1)" vertical={false} />
              <XAxis dataKey="hour" tick={{ fill: '#64748b', fontSize: 10 }} axisLine={false}
                tickLine={false} interval={3} />
              <YAxis tick={{ fill: '#64748b', fontSize: 10 }} axisLine={false} tickLine={false} />
              <Tooltip content={<ChartTooltip />} />
              <Bar dataKey="transactions" name="Transactions" radius={[3, 3, 0, 0]} maxBarSize={18}>
                {hourlyData.map((entry, i) => (
                  <Cell key={i} fill={
                    entry.transactions > 100 ? '#1d4ed8' :
                    entry.transactions > 60  ? '#3b82f6' : '#93c5fd'
                  } />
                ))}
              </Bar>
            </BarChart>
          </ResponsiveContainer>
        </div>

        <div className="card">
          <h2 className="text-text font-semibold text-sm mb-1">Payment Methods</h2>
          <p className="text-muted text-xs mb-5">This month's split</p>
          <div className="space-y-4">
            {paymentData.map(pm => (
              <div key={pm.method}>
                <div className="flex items-center justify-between mb-1.5">
                  <span className="text-text text-xs font-medium">{pm.method}</span>
                  <span className="text-muted text-xs font-mono">{pm.pct}%</span>
                </div>
                <div className="h-2 bg-surface2 rounded-full overflow-hidden">
                  <div className="h-full rounded-full" style={{ width: `${pm.pct}%`, background: pm.color }} />
                </div>
              </div>
            ))}
          </div>
          <div className="mt-5 pt-4 border-t border-border flex items-center justify-between text-xs">
            <span className="text-muted">Total processed</span>
            <span className="text-text font-semibold font-mono">$91,372</span>
          </div>
        </div>
      </div>

      {/* Category donut + Top products */}
      <div className="grid grid-cols-5 gap-4">

        <div className="card col-span-2">
          <h2 className="text-text font-semibold text-sm mb-1">Sales by Category</h2>
          <p className="text-muted text-xs mb-4">Revenue distribution this month</p>
          <div className="flex justify-center mb-4">
            <ResponsiveContainer width={180} height={180}>
              <PieChart>
                <Pie data={categoryData} cx="50%" cy="50%" innerRadius={52} outerRadius={80}
                  paddingAngle={2} dataKey="value"
                  onMouseEnter={(_, i) => setActiveCategory(i)}
                  onMouseLeave={() => setActiveCategory(null)}
                >
                  {categoryData.map((cat, i) => (
                    <Cell key={i} fill={cat.color} stroke="none"
                      opacity={activeCategory === null || activeCategory === i ? 1 : 0.35} />
                  ))}
                </Pie>
              </PieChart>
            </ResponsiveContainer>
          </div>
          <div className="space-y-1">
            {categoryData.map((cat, i) => (
              <div key={cat.name}
                className="flex items-center justify-between py-1.5 px-2 rounded-lg cursor-default"
                style={{ background: activeCategory === i ? `${cat.color}14` : 'transparent' }}
                onMouseEnter={() => setActiveCategory(i)}
                onMouseLeave={() => setActiveCategory(null)}
              >
                <div className="flex items-center gap-2">
                  <span className="w-2.5 h-2.5 rounded-sm flex-shrink-0" style={{ background: cat.color }} />
                  <span className="text-text text-xs">{cat.name}</span>
                </div>
                <div className="flex items-center gap-3">
                  <span className="text-muted text-xs font-mono">${cat.revenue.toLocaleString()}</span>
                  <span className="text-xs font-semibold font-mono" style={{ color: cat.color }}>{cat.value}%</span>
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="card col-span-3 flex flex-col">
          <div className="flex items-center justify-between mb-4">
            <div>
              <h2 className="text-text font-semibold text-sm">Top Products</h2>
              <p className="text-muted text-xs mt-0.5">By revenue this month</p>
            </div>
            <span className="flex items-center gap-1 text-xs text-amber-600 font-semibold bg-amber-50 px-2 py-1 rounded-lg">
              <Award size={12} /> Top 5
            </span>
          </div>

          <div className="grid grid-cols-12 table-header rounded-lg mb-1 gap-2">
            <div className="col-span-5">Product</div>
            <div className="col-span-2 text-center">Units</div>
            <div className="col-span-3 text-right">Revenue</div>
            <div className="col-span-2 text-right">Trend</div>
          </div>

          <div className="flex-1 space-y-0.5">
            {topProducts.map((p, i) => (
              <div key={p.sku} className="grid grid-cols-12 table-row rounded-lg gap-2 items-center">
                <div className="col-span-5 flex items-center gap-2">
                  <span className="w-5 h-5 rounded-md bg-surface2 text-muted text-xs font-bold flex items-center justify-center flex-shrink-0">
                    {i + 1}
                  </span>
                  <div className="min-w-0">
                    <p className="text-text text-xs font-medium truncate">{p.name}</p>
                    <p className="text-muted text-xs font-mono">{p.sku}</p>
                  </div>
                </div>
                <div className="col-span-2 text-center">
                  <span className="text-text text-xs font-semibold font-mono">{p.sold}</span>
                </div>
                <div className="col-span-3 text-right">
                  <span className="text-text text-xs font-semibold font-mono">${p.revenue.toLocaleString()}</span>
                </div>
                <div className="col-span-2 flex justify-end">
                  <span className={`flex items-center gap-0.5 text-xs font-semibold font-mono px-1.5 py-0.5 rounded-md ${
                    p.trend >= 0 ? 'text-green-700 bg-green-100' : 'text-red-700 bg-red-100'
                  }`}>
                    {p.trend >= 0 ? <TrendingUp size={10} /> : <TrendingDown size={10} />}
                    {Math.abs(p.trend)}%
                  </span>
                </div>
              </div>
            ))}
          </div>

          <div className="mt-4 bg-blue-50 border border-blue-100 rounded-xl p-3 flex items-start gap-2.5">
            <div className="w-6 h-6 bg-primary rounded-lg flex items-center justify-center flex-shrink-0 mt-0.5">
              <Zap size={12} className="text-white" />
            </div>
            <div>
              <p className="text-primary text-xs font-semibold">AI Insight</p>
              <p className="text-blue-700 text-xs mt-0.5 leading-relaxed">
                Organic Cold Brew is up 24.7% — restock recommended within 3 days.
                Consider a bundle promotion with Wireless Headphones for peak weekend traffic.
              </p>
            </div>
          </div>
        </div>

      </div>
    </div>
  )
}