import { useState, useEffect } from 'react';
import { Plus, Mail, Building2, X } from 'lucide-react';
import { GlassCard, Button, Input, Select } from '../components/ui';
import { userService } from '../services/api';

const Team = () => {
  const [users, setUsers] = useState([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingUser, setEditingUser] = useState(null);

  useEffect(() => {
    loadUsers();
  }, []);

  const loadUsers = async () => {
    try {
      setUsers(await userService.getAllUsers());
    } catch (e) {
      console.error(e);
    }
  };

  const handleSave = async (e) => {
    e.preventDefault();
    const data = Object.fromEntries(new FormData(e.target));

    // Remove empty fields
    Object.keys(data).forEach(k => {
      if (data[k] === '' || data[k] === null) delete data[k];
    });

    try {
      if (editingUser) {
        await userService.updateUser(editingUser.id, data);
      } else {
        await userService.createUser(data);
      }
      setIsModalOpen(false);
      loadUsers();
    } catch (e) {
      alert(e.message);
    }
  };

  const roleOptions = [
    { value: 'Employee', label: 'Employee' },
    { value: 'ProjectManager', label: 'Project Manager' },
    { value: 'Admin', label: 'Admin' },
    { value: 'Client', label: 'Client' },
  ];

  return (
    <>
      {/* Header */}
      <div className="flex justify-between items-center mb-8">
        <div>
          <h1 className="text-3xl font-bold text-white">Team</h1>
          <p className="text-slate-400">Manage access and roles.</p>
        </div>
        <Button icon={Plus} onClick={() => { setEditingUser(null); setIsModalOpen(true); }}>
          Add User
        </Button>
      </div>

      {/* User Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {users.map(u => (
          <GlassCard
            key={u.id}
            className="p-6 relative group bg-glass-100 hover:bg-glass-200"
            onClick={() => { setEditingUser(u); setIsModalOpen(true); }}
          >
            <div className="flex gap-4 items-center mb-4">
              <div className="w-12 h-12 rounded-2xl bg-gradient-to-br from-purple-500 to-indigo-600 flex items-center justify-center text-white text-xl font-bold shadow-lg">
                {u.username?.[0]}
              </div>
              <div>
                <h3 className="font-bold text-white text-lg">{u.username}</h3>
                <span className="text-[10px] uppercase font-bold bg-white/10 px-2 py-0.5 rounded text-indigo-300 border border-indigo-500/20">
                  {u.role}
                </span>
              </div>
            </div>

            <div className="space-y-2 text-sm text-slate-400">
              <div className="flex items-center gap-2">
                <Mail size={14} /> {u.email}
              </div>
              {(u.department || u.company) && (
                <div className="flex items-center gap-2">
                  <Building2 size={14} /> {u.department || u.company}
                </div>
              )}
            </div>
          </GlassCard>
        ))}
      </div>

      {/* Modal */}
      {isModalOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm">
          <GlassCard className="w-full max-w-lg p-6 bg-slate-900 border-slate-700">
            <div className="flex justify-between mb-6">
              <h2 className="text-xl font-bold text-white">
                {editingUser ? 'Edit User' : 'Add User'}
              </h2>
              <button onClick={() => setIsModalOpen(false)} className="text-slate-400 hover:text-white">
                <X size={20} />
              </button>
            </div>

            <form onSubmit={handleSave} className="space-y-4">
              <Input name="username" label="Username" defaultValue={editingUser?.username} required />
              <Input name="email" label="Email" type="email" defaultValue={editingUser?.email} required />
              {!editingUser && <Input name="password" label="Password" type="password" required />}
              <Select name="role" label="Role" defaultValue={editingUser?.role || 'Employee'} options={roleOptions} />
              <Input name="department" label="Department" defaultValue={editingUser?.department} />

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

export default Team;