import { useState, useEffect } from 'react'
import api from '../api/api'
import { RefreshCw, Loader2, Search, Package, AlertTriangle, DollarSign, XCircle } from 'lucide-react'

function StockBadge({ stock, threshold }) {
  if (stock === 0)           return <span className="badge-danger">Out of Stock</span>
  if (stock <= threshold)    return <span className="badge-warning">Low Stock</span>
  return <span className="badge-success">In Stock</span>
}

export default function Inventory() {
  const [products, setProducts]   = useState([])
  const [filtered, setFiltered]   = useState([])
  const [search, setSearch]       = useState('')
  const [filter, setFilter]       = useState('all')
  const [loading, setLoading]     = useState(true)
  const [refreshing, setRefreshing] = useState(false)

  const loadData = async () => {
    try {
      const { data } = await api.get('/products')
      setProducts(data)
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
    let result = products
    if (search) {
      const q = search.toLowerCase()
      result = result.filter(p =>
        p.productName.toLowerCase().includes(q) ||
        p.sku.toLowerCase().includes(q) ||
        p.categoryName.toLowerCase().includes(q)
      )
    }
    if (filter === 'low')  result = result.filter(p => p.stockQuantity <= p.lowStockThreshold && p.stockQuantity > 0)
    if (filter === 'out')  result = result.filter(p => p.stockQuantity === 0)
    if (filter === 'ok')   result = result.filter(p => p.stockQuantity > p.lowStockThreshold)
    setFiltered(result)
  }, [search, filter, products])

  const totalValue    = products.reduce((s, p) => s + p.unitPrice * p.stockQuantity, 0)
  const lowStockCount = products.filter(p => p.stockQuantity <= p.lowStockThreshold && p.stockQuantity > 0).length
  const outOfStock    = products.filter(p => p.stockQuantity === 0).length

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
          <h1 className="text-2xl font-bold text-text">Inventory</h1>
          <p className="text-muted text-sm mt-1">Monitor stock levels across all products</p>
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
      <div className="grid grid-cols-4 gap-4">
        <div className="kpi-card">
          <div className="flex items-center justify-between mb-3">
            <p className="text-sm font-medium text-muted">Total Products</p>
            <div className="w-9 h-9 bg-blue-50 rounded-lg flex items-center justify-center">
              <Package size={16} className="text-primary"/>
            </div>
          </div>
          <p className="text-3xl font-bold text-text mb-1">{products.length}</p>
          <p className="text-xs text-muted">Active products</p>
        </div>

        <div className="kpi-card">
          <div className="flex items-center justify-between mb-3">
            <p className="text-sm font-medium text-muted">Inventory Value</p>
            <div className="w-9 h-9 bg-blue-50 rounded-lg flex items-center justify-center">
              <DollarSign size={16} className="text-primary"/>
            </div>
          </div>
          <p className="text-3xl font-bold text-primary mb-1">${totalValue.toLocaleString()}</p>
          <p className="text-xs text-muted">At retail price</p>
        </div>

        <div className="kpi-card">
          <div className="flex items-center justify-between mb-3">
            <p className="text-sm font-medium text-muted">Low Stock</p>
            <div className="w-9 h-9 bg-amber-50 rounded-lg flex items-center justify-center">
              <AlertTriangle size={16} className="text-warning"/>
            </div>
          </div>
          <p className="text-3xl font-bold text-warning mb-1">{lowStockCount}</p>
          <p className="text-xs text-muted">Need restocking</p>
        </div>

        <div className="kpi-card">
          <div className="flex items-center justify-between mb-3">
            <p className="text-sm font-medium text-muted">Out of Stock</p>
            <div className="w-9 h-9 bg-red-50 rounded-lg flex items-center justify-center">
              <XCircle size={16} className="text-danger"/>
            </div>
          </div>
          <p className="text-3xl font-bold text-danger mb-1">{outOfStock}</p>
          <p className="text-xs text-muted">Unavailable products</p>
        </div>
      </div>

      {/* Table */}
      <div className="card p-0 overflow-hidden">

        {/* Filters */}
        <div className="p-4 border-b border-border flex items-center gap-4">
          <div className="relative max-w-sm flex-1">
            <Search size={14} className="absolute left-3 top-1/2 -translate-y-1/2 text-muted"/>
            <input
              type="text"
              placeholder="Search products..."
              value={search}
              onChange={e => setSearch(e.target.value)}
              className="input pl-9 text-sm"
            />
          </div>
          <div className="flex gap-2">
            {[
              { key: 'all', label: 'All' },
              { key: 'ok',  label: 'In Stock' },
              { key: 'low', label: 'Low Stock' },
              { key: 'out', label: 'Out of Stock' },
            ].map(({ key, label }) => (
              <button
                key={key}
                onClick={() => setFilter(key)}
                className={`px-3 py-1.5 rounded-lg text-xs font-medium transition-colors duration-150 cursor-pointer
                  ${filter === key
                    ? 'bg-primary text-white'
                    : 'bg-surface2 text-muted hover:bg-border'
                  }`}
              >
                {label}
              </button>
            ))}
          </div>
        </div>

        {/* Table Header */}
        <div className="grid grid-cols-6 table-header border-b border-border">
          <div className="col-span-2">Product</div>
          <div>SKU</div>
          <div>Category</div>
          <div>Stock</div>
          <div>Status</div>
        </div>

        {/* Table Rows */}
        <div className="divide-y divide-border">
          {filtered.length === 0 ? (
            <div className="py-12 text-center text-muted text-sm">
              No products found
            </div>
          ) : (
            filtered.map(p => (
              <div key={p.productId} className="grid grid-cols-6 table-row">
                <div className="col-span-2">
                  <p className="text-sm font-medium text-text">{p.productName}</p>
                  <p className="text-xs text-muted">${p.unitPrice.toFixed(2)}</p>
                </div>
                <div className="text-xs font-mono text-muted self-center">{p.sku}</div>
                <div className="text-sm text-muted self-center">{p.categoryName}</div>
                <div className="self-center">
                  <span className="text-sm font-semibold text-text">{p.stockQuantity}</span>
                  <span className="text-xs text-muted ml-1">/ min {p.lowStockThreshold}</span>
                </div>
                <div className="self-center">
                  <StockBadge stock={p.stockQuantity} threshold={p.lowStockThreshold}/>
                </div>
              </div>
            ))
          )}
        </div>

        {/* Footer */}
        <div className="px-4 py-3 border-t border-border bg-surface2">
          <p className="text-xs text-muted">
            Showing {filtered.length} of {products.length} products
          </p>
        </div>
      </div>
    </div>
  )
}