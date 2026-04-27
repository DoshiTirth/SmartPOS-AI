import { useState, useEffect } from 'react'
import api from '../api/api'
import { RefreshCw, Loader2, Search, Receipt, DollarSign, TrendingUp } from 'lucide-react'

function StatusBadge({ status }) {
  const styles = {
    Completed: 'badge-success',
    Voided:    'badge-danger',
    Refunded:  'badge-info',
  }
  return <span className={styles[status] ?? 'badge-info'}>{status}</span>
}

export default function Transactions() {
  const [transactions, setTransactions] = useState([])
  const [filtered, setFiltered]         = useState([])
  const [search, setSearch]             = useState('')
  const [loading, setLoading]           = useState(true)
  const [refreshing, setRefreshing]     = useState(false)

  const loadData = async () => {
    try {
      const { data } = await api.get('/transactions/recent?count=100')
      setTransactions(data)
      setFiltered(data)
    } catch (err) {
      console.error(err)
    } finally {
      setLoading(false)
      setRefreshing(false)
    }
  }

  useEffect(() => { loadData() }, [])

  useEffect(() => {
    const q = search.toLowerCase()
    setFiltered(
      transactions.filter(t =>
        t.transactionCode.toLowerCase().includes(q) ||
        t.paymentMethod.toLowerCase().includes(q) ||
        t.status.toLowerCase().includes(q)
      )
    )
  }, [search, transactions])

  const totalRevenue = transactions
    .filter(t => t.status === 'Completed')
    .reduce((sum, t) => sum + t.totalAmount, 0)

  const avgTransaction = transactions.length > 0
    ? totalRevenue / transactions.filter(t => t.status === 'Completed').length
    : 0

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
          <h1 className="text-2xl font-bold text-text">Transactions</h1>
          <p className="text-muted text-sm mt-1">View and manage all POS transactions</p>
        </div>
        <button
          onClick={() => { setRefreshing(true); loadData() }}
          className="btn-secondary"
          disabled={refreshing}
        >
          <RefreshCw size={14} className={refreshing ? 'animate-spin' : ''}/>
          Refresh
        </button>
      </div>

      {/* KPI Cards */}
      <div className="grid grid-cols-3 gap-4">
        <div className="kpi-card">
          <div className="flex items-center justify-between mb-3">
            <p className="text-sm font-medium text-muted">Total Transactions</p>
            <div className="w-9 h-9 bg-blue-50 rounded-lg flex items-center justify-center">
              <Receipt size={16} className="text-primary"/>
            </div>
          </div>
          <p className="text-3xl font-bold text-text mb-1">{transactions.length}</p>
          <p className="text-xs text-muted">Last 100 transactions</p>
        </div>

        <div className="kpi-card">
          <div className="flex items-center justify-between mb-3">
            <p className="text-sm font-medium text-muted">Total Revenue</p>
            <div className="w-9 h-9 bg-blue-50 rounded-lg flex items-center justify-center">
              <DollarSign size={16} className="text-primary"/>
            </div>
          </div>
          <p className="text-3xl font-bold text-primary mb-1">${totalRevenue.toFixed(2)}</p>
          <p className="text-xs text-muted">Completed transactions only</p>
        </div>

        <div className="kpi-card">
          <div className="flex items-center justify-between mb-3">
            <p className="text-sm font-medium text-muted">Average Transaction</p>
            <div className="w-9 h-9 bg-amber-50 rounded-lg flex items-center justify-center">
              <TrendingUp size={16} className="text-warning"/>
            </div>
          </div>
          <p className="text-3xl font-bold text-warning mb-1">${avgTransaction.toFixed(2)}</p>
          <p className="text-xs text-muted">Per completed transaction</p>
        </div>
      </div>

      {/* Table */}
      <div className="card p-0 overflow-hidden">

        {/* Search Bar */}
        <div className="p-4 border-b border-border">
          <div className="relative max-w-sm">
            <Search size={14} className="absolute left-3 top-1/2 -translate-y-1/2 text-muted"/>
            <input
              type="text"
              placeholder="Search transactions..."
              value={search}
              onChange={e => setSearch(e.target.value)}
              className="input pl-9 text-sm"
            />
          </div>
        </div>

        {/* Table Header */}
        <div className="grid grid-cols-6 table-header border-b border-border">
          <div>Transaction Code</div>
          <div>Cashier</div>
          <div>Total</div>
          <div>Payment</div>
          <div>Status</div>
          <div>Date</div>
        </div>

        {/* Table Rows */}
        <div className="divide-y divide-border">
          {filtered.length === 0 ? (
            <div className="py-12 text-center text-muted text-sm">
              No transactions found
            </div>
          ) : (
            filtered.map(t => (
              <div
                key={t.transactionId}
                className="grid grid-cols-6 table-row"
              >
                <div className="font-mono text-xs font-semibold text-primary">
                  {t.transactionCode}
                </div>
                <div className="text-sm text-text">{t.cashierName || '—'}</div>
                <div className="text-sm font-semibold text-text">
                  ${t.totalAmount?.toFixed(2)}
                </div>
                <div className="text-sm text-muted">{t.paymentMethod}</div>
                <div><StatusBadge status={t.status}/></div>
                <div className="text-xs text-muted">
                  {new Date(t.createdAt).toLocaleDateString('en-US', {
                    month: 'short', day: 'numeric', year: 'numeric',
                    hour: '2-digit', minute: '2-digit'
                  })}
                </div>
              </div>
            ))
          )}
        </div>

        {/* Footer */}
        <div className="px-4 py-3 border-t border-border bg-surface2">
          <p className="text-xs text-muted">
            Showing {filtered.length} of {transactions.length} transactions
          </p>
        </div>
      </div>
    </div>
  )
}