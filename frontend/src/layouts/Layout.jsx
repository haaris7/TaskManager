import { useNavigate, useLocation } from 'react-router-dom';
import { LayoutDashboard, Users, LogOut } from 'lucide-react';
import { cn } from '../utils/helpers';
import { ROLES } from '../utils/constants';

const Layout = ({ user, children, onLogout }) => {
  const navigate = useNavigate();
  const location = useLocation();
  const canViewTeam = [ROLES.ADMIN, ROLES.PM].includes(user?.role);

  return (
    <div className="min-h-screen flex text-slate-100">
      {/* SIDEBAR */}
      <aside className="hidden md:flex flex-col w-64 fixed h-full bg-black/20 backdrop-blur-2xl border-r border-white/10 z-30">
        {/* Logo */}
        <div className="h-24 flex items-center px-6 border-b border-white/5">
          <div className="h-9 w-9 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-lg flex items-center justify-center text-white shadow-lg shadow-indigo-500/20 mr-3">
            <LayoutDashboard size={20} />
          </div>
          <span className="font-bold text-xl tracking-tight">TaskFlow</span>
        </div>

        {/* Navigation */}
        <nav className="flex-1 px-4 space-y-2 py-6">
          <button
            onClick={() => navigate('/')}
            className={cn(
              "w-full flex items-center gap-3 px-4 py-3 rounded-xl transition-all font-medium",
              location.pathname === '/'
                ? "bg-white/10 text-white shadow-inner"
                : "text-slate-400 hover:bg-white/5 hover:text-white"
            )}
          >
            <LayoutDashboard size={20} /> Dashboard
          </button>

          {canViewTeam && (
            <button
              onClick={() => navigate('/team')}
              className={cn(
                "w-full flex items-center gap-3 px-4 py-3 rounded-xl transition-all font-medium",
                location.pathname === '/team'
                  ? "bg-white/10 text-white shadow-inner"
                  : "text-slate-400 hover:bg-white/5 hover:text-white"
              )}
            >
              <Users size={20} /> Team
            </button>
          )}
        </nav>

        {/* User Info + Logout */}
        <div className="p-4 border-t border-white/5 bg-black/10">
          <div className="flex items-center gap-3 px-4 py-3 mb-2">
            <div className="w-10 h-10 rounded-full bg-indigo-500/20 border border-indigo-500/30 flex items-center justify-center text-indigo-200 font-bold">
              {user?.username?.[0]}
            </div>
            <div className="overflow-hidden">
              <p className="text-sm font-semibold truncate">{user?.username}</p>
              <p className="text-[10px] uppercase text-slate-400 tracking-wider">{user?.role}</p>
            </div>
          </div>
          <button
            onClick={onLogout}
            className="w-full flex items-center justify-center gap-2 px-4 py-2 text-rose-400 hover:bg-rose-500/10 rounded-lg text-sm font-medium transition-colors"
          >
            <LogOut size={16} /> Logout
          </button>
        </div>
      </aside>

      {/* MAIN CONTENT */}
      <main className="md:pl-64 w-full p-6 md:p-10 pt-24 md:pt-10">
        {children}
      </main>
    </div>
  );
};

export default Layout;