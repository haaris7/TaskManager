import { useState, useEffect } from 'react';
import { Plus, Search, Calendar, X, Trash2 } from 'lucide-react';
import { GlassCard, Button, Input, Select } from '../components/ui';
import { taskService, userService } from '../services/api';
import { cn, formatDate } from '../utils/helpers';
import { ROLES, STATUS_CONFIG } from '../utils/constants';

const Dashboard = ({ user }) => {
  const [tasks, setTasks] = useState([]);
  const [usersList, setUsersList] = useState([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [currentTask, setCurrentTask] = useState(null);
  const [filter, setFilter] = useState('All');
  const [search, setSearch] = useState('');

  const canManage = [ROLES.ADMIN, ROLES.PM].includes(user.role);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const t = await taskService.getAllTasks();
      setTasks(t);
      if (canManage) {
        const u = await userService.getAllUsers();
        setUsersList(u);
      }
    } catch (err) {
      console.error(err);
    }
  };

  const handleSave = async (e) => {
    e.preventDefault();
    const fd = new FormData(e.target);
    const data = Object.fromEntries(fd);
    data.assignedToUserId = parseInt(data.assignedToUserId);

    try {
      if (currentTask) {
        await taskService.updateTask(currentTask.id, data);
      } else {
        await taskService.createTask(data);
      }
      setIsModalOpen(false);
      loadData();
    } catch (err) {
      alert('Error saving task');
    }
  };

  const handleStatusChange = async (taskId, newStatus) => {
    try {
      setTasks(prev => prev.map(t => (t.id === taskId ? { ...t, status: newStatus } : t)));
      await taskService.changeTaskStatus(taskId, newStatus);
    } catch (e) {
      loadData();
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Delete task?')) return;
    try {
      await taskService.deleteTask(id);
      loadData();
    } catch (e) {
      console.error(e);
    }
  };

  const filteredTasks = tasks.filter(
    t => (filter === 'All' || t.status === filter) && t.name.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <>
      {/* Header */}
      <div className="flex flex-col md:flex-row justify-between items-start md:items-center mb-8 gap-4">
        <div>
          <h1 className="text-3xl font-bold text-white">Dashboard</h1>
          <p className="text-slate-400 mt-1">Overview of all active tasks.</p>
        </div>
        {canManage && (
          <Button icon={Plus} onClick={() => { setCurrentTask(null); setIsModalOpen(true); }}>
            New Task
          </Button>
        )}
      </div>

      {/* Filters */}
      <div className="flex flex-col md:flex-row gap-4 mb-8">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" size={18} />
          <input
            type="text"
            placeholder="Search tasks..."
            className="w-full h-11 pl-10 pr-4 bg-white/5 border border-white/10 rounded-xl text-white placeholder:text-slate-500 focus:outline-none focus:ring-2 focus:ring-indigo-500/50"
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
        </div>
        <div className="flex gap-2 overflow-x-auto pb-2 md:pb-0">
          {['All', ...Object.keys(STATUS_CONFIG)].map(s => (
            <button
              key={s}
              onClick={() => setFilter(s)}
              className={cn(
                "px-4 py-2 rounded-xl text-sm font-medium border transition-all whitespace-nowrap",
                filter === s
                  ? "bg-indigo-500/20 border-indigo-500/50 text-indigo-200 shadow-sm"
                  : "bg-transparent border-transparent text-slate-400 hover:bg-white/5"
              )}
            >
              {s === 'All' ? 'All' : STATUS_CONFIG[s].label}
            </button>
          ))}
        </div>
      </div>

      {/* Task Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
        {filteredTasks.map(task => (
          <GlassCard
            key={task.id}
            onClick={() => { if (canManage) { setCurrentTask(task); setIsModalOpen(true); } }}
            className="p-5 flex flex-col h-full group bg-glass-100 hover:bg-glass-200"
          >
            {/* Status + Delete */}
            <div className="flex justify-between items-start mb-4">
              <div onClick={e => e.stopPropagation()}>
                {canManage || user.id === task.assignedToUserId ? (
                  <select
                    value={task.status}
                    onChange={e => handleStatusChange(task.id, e.target.value)}
                    className={cn(
                      "appearance-none pl-3 pr-8 py-1 rounded-lg text-xs font-semibold border cursor-pointer outline-none focus:ring-2 bg-transparent",
                      STATUS_CONFIG[task.status]?.color
                    )}
                  >
                    {Object.keys(STATUS_CONFIG).map(k => (
                      <option key={k} value={k} className="bg-slate-800">{STATUS_CONFIG[k].label}</option>
                    ))}
                  </select>
                ) : (
                  <span className={cn("px-2.5 py-1 rounded-lg text-xs font-semibold border", STATUS_CONFIG[task.status]?.color)}>
                    {STATUS_CONFIG[task.status]?.label}
                  </span>
                )}
              </div>
              {user.role === ROLES.ADMIN && (
                <button
                  onClick={e => { e.stopPropagation(); handleDelete(task.id); }}
                  className="p-1.5 hover:bg-rose-500/20 text-slate-400 hover:text-rose-400 rounded-lg transition-colors"
                >
                  <Trash2 size={16} />
                </button>
              )}
            </div>

            {/* Task Info */}
            <h3 className="font-bold text-lg text-white mb-2">{task.name}</h3>
            <p className="text-slate-400 text-sm mb-4 line-clamp-3 flex-1">{task.description}</p>

            {/* Footer */}
            <div className="pt-4 border-t border-white/10 flex justify-between items-center text-xs text-slate-400">
              <div className="flex items-center gap-1.5">
                <Calendar size={12} className="text-indigo-400" />
                {formatDate(task.startDate)}
              </div>
              <div className="flex items-center gap-2">
                <div className="w-5 h-5 rounded-full bg-indigo-500 flex items-center justify-center text-[10px] font-bold text-white">
                  {task.assignedToUsername?.[0]}
                </div>
                {task.assignedToUsername}
              </div>
            </div>
          </GlassCard>
        ))}
      </div>

      {/* Modal */}
      {isModalOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm">
          <GlassCard className="w-full max-w-lg p-6 bg-slate-900 border-slate-700 shadow-2xl">
            <div className="flex justify-between mb-6 items-center">
              <h2 className="text-xl font-bold text-white">{currentTask ? 'Edit Task' : 'New Task'}</h2>
              <button onClick={() => setIsModalOpen(false)} className="text-slate-400 hover:text-white">
                <X size={20} />
              </button>
            </div>

            <form onSubmit={handleSave} className="space-y-4">
              <Input name="name" label="Name" defaultValue={currentTask?.name} required />

              <div className="space-y-1 w-full text-left">
                <label className="text-sm font-medium text-slate-300 ml-1">Description</label>
                <textarea
                  name="description"
                  defaultValue={currentTask?.description}
                  className="w-full bg-black/20 border border-white/10 rounded-xl px-4 py-3 text-white min-h-[100px] focus:outline-none focus:ring-2 focus:ring-indigo-500/50"
                  required
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <Input name="startDate" label="Start" type="datetime-local" defaultValue={currentTask?.startDate?.substring(0, 16)} required />
                <Input name="endDate" label="End" type="datetime-local" defaultValue={currentTask?.endDate?.substring(0, 16) || ''} />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <Input name="department" label="Dept" defaultValue={currentTask?.department} required />
                <Input name="clientCompany" label="Client" defaultValue={currentTask?.clientCompany} />
              </div>

              <Select
                name="assignedToUserId"
                label="Assign To"
                defaultValue={currentTask?.assignedToUserId || ''}
                options={usersList.map(u => ({ value: u.id, label: u.username }))}
              />

              <div className="flex gap-3 pt-4">
                <Button type="button" variant="secondary" onClick={() => setIsModalOpen(false)} className="flex-1">
                  Cancel
                </Button>
                <Button type="submit" className="flex-1">Save</Button>
              </div>
            </form>
          </GlassCard>
        </div>
      )}
    </>
  );
};

export default Dashboard;