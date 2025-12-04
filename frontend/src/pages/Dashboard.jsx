import { useState, useEffect } from 'react';
import { Plus, Search, Calendar, Trash2, Edit2 } from 'lucide-react';
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
          <h1 className="text-3xl font-bold text-slate-800">Dashboard</h1>
          <p className="text-slate-500 mt-1">Manage and track your projects.</p>
        </div>
        {canManage && (
          <Button icon={Plus} onClick={() => { setCurrentTask(null); setIsModalOpen(true); }}>
            New Task
          </Button>
        )}
      </div>

      {/* Filters */}
      <div className="flex flex-col md:flex-row gap-4 mb-6">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" size={18} />
          <input
            type="text"
            placeholder="Search tasks..."
            className="w-full h-11 pl-10 pr-4 bg-white/70 border border-white/60 rounded-xl text-slate-700 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-sky-500/30 shadow-sm"
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
                  ? "bg-sky-50 border-sky-200 text-sky-700 shadow-sm"
                  : "bg-white/40 border-transparent text-slate-500 hover:bg-white/60"
              )}
            >
              {s === 'All' ? 'All' : STATUS_CONFIG[s].label}
            </button>
          ))}
        </div>
      </div>

      {/* Task Rows */}
      <div className="space-y-3">
        {filteredTasks.length === 0 && (
            <div className="text-center py-20 text-slate-400">No tasks found.</div>
        )}
        
        {filteredTasks.map(task => (
          <GlassCard
            key={task.id}
            className="p-4 flex flex-col md:flex-row items-start md:items-center gap-4 hover:shadow-md border-white/80 bg-white/80"
            onClick={() => { if (canManage) { setCurrentTask(task); setIsModalOpen(true); } }}
          >
            {/* Status Dot */}
            <div className="flex items-center self-start md:self-center mt-1 md:mt-0">
               <div 
                 className={cn(
                   "w-3 h-3 rounded-full transition-all duration-500", 
                   STATUS_CONFIG[task.status]?.color,
                   STATUS_CONFIG[task.status]?.shadow
                 )} 
                 title={STATUS_CONFIG[task.status]?.label}
               />
            </div>

            {/* Content */}
            <div className="flex-1 min-w-0">
              <div className="flex items-center gap-3">
                 <h3 className="font-bold text-slate-800 text-lg truncate">{task.name}</h3>
                 {user.role === ROLES.ADMIN && (
                    <span className="text-[10px] px-2 py-0.5 bg-slate-100 text-slate-500 rounded border border-slate-200">
                        {task.clientCompany || task.department}
                    </span>
                 )}
              </div>
              <p className="text-slate-500 text-sm truncate max-w-2xl">{task.description}</p>
            </div>

            {/* Meta & Actions */}
            <div className="flex items-center gap-4 w-full md:w-auto justify-between md:justify-end mt-2 md:mt-0">
                {/* Assignee */}
                <div className="flex items-center gap-2" title="Assigned To">
                    <div className="w-8 h-8 rounded-full bg-gradient-to-br from-sky-100 to-indigo-100 border border-white flex items-center justify-center text-xs font-bold text-sky-700 shadow-sm">
                        {task.assignedToUsername?.[0]}
                    </div>
                    <span className="text-sm font-medium text-slate-600 hidden lg:block">{task.assignedToUsername}</span>
                </div>

                {/* Date */}
                <div className="flex items-center gap-1.5 text-xs text-slate-400 bg-slate-50 px-3 py-1.5 rounded-lg border border-slate-100">
                    <Calendar size={14} className="text-sky-400" />
                    {formatDate(task.startDate)}
                </div>

                {/* Status Dropdown (Stop Propagation to prevent modal open) */}
                <div onClick={e => e.stopPropagation()}>
                    {(canManage || user.id === task.assignedToUserId) ? (
                        <select
                            value={task.status}
                            onChange={e => handleStatusChange(task.id, e.target.value)}
                            className="text-xs font-semibold bg-white border border-slate-200 rounded-lg px-2 py-1.5 text-slate-600 focus:ring-2 focus:ring-sky-500/20 cursor-pointer shadow-sm outline-none"
                        >
                            {Object.keys(STATUS_CONFIG).map(k => (
                                <option key={k} value={k}>{STATUS_CONFIG[k].label}</option>
                            ))}
                        </select>
                    ) : (
                        <span className="text-xs font-semibold text-slate-500 px-2 py-1">
                            {STATUS_CONFIG[task.status]?.label}
                        </span>
                    )}
                </div>
                 
                {/* Admin Delete */}
                {user.role === ROLES.ADMIN && (
                    <button
                        onClick={e => { e.stopPropagation(); handleDelete(task.id); }}
                        className="p-2 text-slate-300 hover:text-rose-500 hover:bg-rose-50 rounded-lg transition-colors"
                    >
                        <Trash2 size={16} />
                    </button>
                )}
            </div>
          </GlassCard>
        ))}
      </div>

      {/* Modal */}
      {isModalOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/20 backdrop-blur-sm">
          <GlassCard className="w-full max-w-lg p-8 bg-white/95 border-white shadow-2xl">
            <h2 className="text-2xl font-bold text-slate-800 mb-6">
              {currentTask ? 'Edit Task' : 'New Task'}
            </h2>

            <form onSubmit={handleSave} className="space-y-5">
              <Input name="name" label="Name" defaultValue={currentTask?.name} required placeholder="Task title..." />

              <div className="space-y-1.5 w-full text-left">
                <label className="text-sm font-semibold text-slate-600 ml-1">Description</label>
                <textarea
                  name="description"
                  defaultValue={currentTask?.description}
                  className="w-full bg-white/60 border border-white/50 rounded-xl px-4 py-3 text-slate-800 min-h-[100px] focus:outline-none focus:ring-2 focus:ring-sky-500/50 shadow-sm"
                  required
                  placeholder="Task details..."
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <Input name="startDate" label="Start" type="datetime-local" defaultValue={currentTask?.startDate?.substring(0, 16)} required />
                <Input name="endDate" label="End" type="datetime-local" defaultValue={currentTask?.endDate?.substring(0, 16) || ''} />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <Input name="department" label="Dept" defaultValue={currentTask?.department} required placeholder="Engineering..." />
                <Input name="clientCompany" label="Client" defaultValue={currentTask?.clientCompany} placeholder="Optional..." />
              </div>

              <Select
                name="assignedToUserId"
                label="Assign To"
                defaultValue={currentTask?.assignedToUserId || ''}
                options={usersList.map(u => ({ value: u.id, label: u.username }))}
              />

              <div className="flex gap-3 pt-6">
                <Button type="button" variant="secondary" onClick={() => setIsModalOpen(false)} className="flex-1">
                  Cancel
                </Button>
                <Button type="submit" className="flex-1">Save Task</Button>
              </div>
            </form>
          </GlassCard>
        </div>
      )}
    </>
  );
};

export default Dashboard;