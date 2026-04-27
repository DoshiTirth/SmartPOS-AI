import { Outlet, NavLink, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import {
  LayoutDashboard, Receipt, Package, BarChart3,
  LogOut, CreditCard, User, Clock
} from 'lucide-react'
import { useState, useEffect } from 'react'

const navItems = [
  { to: '/dashboard',    icon: LayoutDashboard, label: 'Dashboard' },
  { to: '/transactions', icon: Receipt,          label: 'Transactions' },
  { to: '/inventory',    icon: Package,          label: 'Inventory' },
  { to: '/analytics',    icon: BarChart3,        label: 'Analytics' },
]

export default function Layout() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()
  const [time, setTime] = useState(new Date())

  useEffect(() => {
    const timer = setInterval(() => setTime(new Date()), 1000)
    return () => clearInterval(timer)
  }, [])

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  return (
    <div className="flex h-screen bg-background overflow-hidden">

      {/* Sidebar */}
      <aside className="w-64 bg-surface border-r border-border flex flex-col shadow-sm">

        {/* Logo */}
        <div className="p-6 border-b border-border">
          <div className="flex items-center gap-3">
            <div className="w-9 h-9 bg-primary rounded-lg flex items-center justify-center">
              <CreditCard size={18} className="text-white"/>
            </div>
            <div>
              <h1 className="font-bold text-text text-base leading-tight">SmartPOS AI</h1>
              <p className="text-xs text-muted">Manager Portal</p>
            </div>
          </div>
        </div>

        {/* Nav */}
        <nav className="flex-1 p-4 space-y-1">
          <p className="text-xs text-muted font-semibold uppercase tracking-wider px-4 mb-3">
            Navigation
          </p>
          {navItems.map(({ to, icon: Icon, label }) => (
            <NavLink
              key={to}
              to={to}
              className={({ isActive }) =>
                isActive ? 'nav-item-active' : 'nav-item'
              }
            >
              <Icon size={18}/>
              <span>{label}</span>
            </NavLink>
          ))}
        </nav>

        {/* User Info */}
        <div className="p-4 border-t border-border">
          <div className="flex items-center gap-2 mb-3 px-2">
            <div className="w-8 h-8 bg-blue-100 rounded-full flex items-center justify-center">
              <User size={14} className="text-primary"/>
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-sm font-medium text-text truncate">{user?.fullName}</p>
              <p className="text-xs text-muted">{user?.role}</p>
            </div>
          </div>
          <div className="flex items-center gap-2 px-2 mb-3">
            <Clock size={12} className="text-muted"/>
            <span className="text-xs text-muted font-mono">
              {time.toLocaleTimeString()}
            </span>
          </div>
          <button onClick={handleLogout} className="btn-secondary w-full justify-center text-sm">
            <LogOut size={14}/>
            Sign Out
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <main className="flex-1 overflow-auto">
        <Outlet />
      </main>
    </div>
  )
}