// Role definitions
export const ROLES = {
  ADMIN: 'Admin',
  PM: 'ProjectManager',
  EMPLOYEE: 'Employee',
  CLIENT: 'Client'
};

// Task status styling configuration
export const STATUS_CONFIG = {
  NotStarted: {
    label: 'Not Started',
    color: 'bg-slate-500/20 text-slate-200 border-slate-500/30'
  },
  InProgress: {
    label: 'In Progress',
    color: 'bg-blue-500/20 text-blue-200 border-blue-500/30'
  },
  Completed: {
    label: 'Completed',
    color: 'bg-emerald-500/20 text-emerald-200 border-emerald-500/30'
  },
  OnHold: {
    label: 'On Hold',
    color: 'bg-amber-500/20 text-amber-200 border-amber-500/30'
  },
  Cancelled: {
    label: 'Cancelled',
    color: 'bg-rose-500/20 text-rose-200 border-rose-500/30'
  }
};