import { useNavigate, useLocation } from 'react-router-dom';
import { LayoutDashboard, Users, LogOut } from 'lucide-react';
import { cn } from '../utils/helpers';
import { ROLES } from '../utils/constants';

const Layout = ({ user, children, onLogout }) => {
  const navigate = useNavigate();
  const location = useLocation();
  const canViewTeam = [ROLES.ADMIN, ROLES.PM].includes(user?.role);

  return (
    <div className="min-h-screen flex text-slate-700 font-sans">
      {/* SIDEBAR */}
      <aside className="hidden md:flex flex-col w-64 fixed h-full bg-white/70 backdrop-blur-2xl border-r border-white/60 z-30 shadow-xl">
        {/* Logo */}
        <div className="h-24 flex items-center px-6 border-b border-slate-100">
          <div className="h-9 w-9 bg-gradient-to-br from-sky-400 to-teal-500 rounded-lg flex items-center justify-center text-white shadow-lg shadow-sky-500/30 mr-3">
            <LayoutDashboard size={20} />
          </div>
          <span className="font-bold text-xl tracking-tight text-slate-800">TaskFlow</span>
        </div>

        {/* Navigation */}
        <nav className="flex-1 px-4 space-y-2 py-6">
          <button
            onClick={() => navigate('/')}
            className={cn(
              "w-full flex items-center gap-3 px-4 py-3 rounded-xl transition-all font-medium",
              location.pathname === '/'
                ? "bg-sky-50 text-sky-600 shadow-sm border border-sky-100"
                : "text-slate-500 hover:bg-white/50 hover:text-slate-800"
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
                  ? "bg-sky-50 text-sky-600 shadow-sm border border-sky-100"
                  : "text-slate-500 hover:bg-white/50 hover:text-slate-800"
              )}
            >
              <Users size={20} /> Team
            </button>
          )}
        </nav>

        {/* User Info + Logout */}
        <div className="p-4 border-t border-slate-100 bg-slate-50/50">
          <div className="flex items-center gap-3 px-4 py-3 mb-2">
            <div className="w-10 h-10 rounded-full bg-gradient-to-br from-sky-100 to-teal-100 border border-white flex items-center justify-center text-sky-600 font-bold shadow-sm">
              {user?.username?.[0]?.toUpperCase()}
            </div>
            <div className="overflow-hidden text-left">
              <p className="text-sm font-bold text-slate-700 truncate">{user?.username}</p>
              <p className="text-[10px] uppercase text-slate-400 tracking-wider font-semibold">{user?.role}</p>
            </div>
          </div>
          <button
            onClick={onLogout}
            className="w-full flex items-center justify-center gap-2 px-4 py-2 text-rose-500 hover:bg-rose-50 rounded-lg text-sm font-medium transition-colors border border-transparent hover:border-rose-100"
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