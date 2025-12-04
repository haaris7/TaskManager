import { useState, useEffect } from 'react';
import { Plus, Mail, Building2, X, Shield, Briefcase, Edit } from 'lucide-react';
import { GlassCard, Button, Input, Select } from '../components/ui';
import { userService } from '../services/api';
import { ROLES } from '../utils/constants';

const Team = () => {
  const [allUsers, setAllUsers] = useState([]);
  const [visibleUsers, setVisibleUsers] = useState([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [viewingUser, setViewingUser] = useState(null); // For Read-only view
  const [isEditing, setIsEditing] = useState(false); // Mode toggle
  
  // Get current user from local storage to check role/dept
  const currentUser = JSON.parse(localStorage.getItem('user') || '{}');
  const isAdmin = currentUser.role === ROLES.ADMIN;

  useEffect(() => {
    loadUsers();
  }, []);

  const loadUsers = async () => {
    try {
      const data = await userService.getAllUsers();
      setAllUsers(data);
      filterUsers(data);
    } catch (e) {
      console.error(e);
    }
  };

  // Client-side filtering logic as requested
  const filterUsers = (users) => {
    if (currentUser.role === ROLES.ADMIN) {
        setVisibleUsers(users);
    } else {
        // PMs and Employees only see their own department
        const dept = currentUser.department;
        const filtered = users.filter(u => u.department === dept);
        setVisibleUsers(filtered);
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
      if (isEditing && viewingUser) {
        await userService.updateUser(viewingUser.id, data);
      } else {
        await userService.createUser(data);
      }
      setIsModalOpen(false);
      setIsEditing(false);
      setViewingUser(null);
      loadUsers();
    } catch (e) {
      alert(e.message || "Error saving user");
    }
  };

  const openUserModal = (user, editMode = false) => {
      setViewingUser(user);
      setIsEditing(editMode);
      setIsModalOpen(true);
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
          <h1 className="text-3xl font-bold text-slate-800">Team Members</h1>
          <p className="text-slate-500">
            {isAdmin ? "Manage all system users." : `Colleagues in ${currentUser.department || 'your department'}`}
          </p>
        </div>
        {isAdmin && (
            <Button icon={Plus} onClick={() => openUserModal(null, true)}>
            Add User
            </Button>
        )}
      </div>

      {/* User Rows */}
      <div className="space-y-3">
        {visibleUsers.length === 0 && <div className="text-slate-400">No users found in this scope.</div>}

        {visibleUsers.map(u => (
          <GlassCard
            key={u.id}
            className="p-4 flex flex-col md:flex-row items-center gap-4 hover:bg-white/90 bg-white/60 border-white/70"
            // Clicking opens View Mode (Read Only)
            onClick={() => openUserModal(u, false)}
          >
            {/* Avatar & Name */}
            <div className="flex items-center gap-4 flex-1 w-full">
              <div className="w-12 h-12 rounded-xl bg-gradient-to-tr from-sky-200 to-indigo-200 border-2 border-white flex items-center justify-center text-sky-700 text-lg font-bold shadow-sm">
                {u.username?.[0]?.toUpperCase()}
              </div>
              <div className="text-left">
                <h3 className="font-bold text-slate-800 text-lg">{u.username}</h3>
                <div className="flex items-center gap-2">
                    <span className="text-[10px] uppercase font-bold bg-sky-100 px-2 py-0.5 rounded text-sky-600 border border-sky-200">
                    {u.role}
                    </span>
                </div>
              </div>
            </div>

            {/* Info Columns */}
            <div className="flex flex-col md:flex-row gap-4 md:gap-8 w-full md:w-auto text-sm text-slate-500 md:mr-4">
                <div className="flex items-center gap-2 min-w-[150px]">
                    <Mail size={16} className="text-sky-400" /> 
                    <span className="truncate">{u.email}</span>
                </div>
                {(u.department || u.company) && (
                    <div className="flex items-center gap-2 min-w-[150px]">
                        <Building2 size={16} className="text-teal-400" />
                        <span className="truncate">{u.department || u.company}</span>
                    </div>
                )}
            </div>

            {/* Admin Actions - ONLY Admin sees Update Button explicitly here if they want quick access, 
                or they can click the row and then click update */}
            {isAdmin && (
                 <Button 
                    variant="secondary" 
                    className="shrink-0 h-10 w-10 p-0 rounded-full"
                    onClick={(e) => { e.stopPropagation(); openUserModal(u, true); }}
                 >
                    <Edit size={16} />
                 </Button>
            )}
          </GlassCard>
        ))}
      </div>

      {/* Modal */}
      {isModalOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/20 backdrop-blur-sm">
          <GlassCard className="w-full max-w-lg p-8 bg-white/95 border-white shadow-2xl">
            <div className="flex justify-between mb-6">
              <h2 className="text-2xl font-bold text-slate-800">
                {!viewingUser ? 'Add User' : (isEditing ? 'Update Details' : 'User Profile')}
              </h2>
              <button 
                onClick={() => setIsModalOpen(false)} 
                className="text-slate-400 hover:text-slate-600 transition-colors"
              >
                <X size={24} />
              </button>
            </div>

            {/* If viewing in Read-only mode and user is Admin, show button to switch to Edit */}
            {!isEditing && isAdmin && viewingUser && (
                <div className="mb-6 flex justify-end">
                    <Button onClick={() => setIsEditing(true)} icon={Edit}>
                        Update Details
                    </Button>
                </div>
            )}

            <form onSubmit={handleSave} className="space-y-5">
              <Input 
                name="username" 
                label="Username" 
                defaultValue={viewingUser?.username} 
                readOnly={!isEditing}
                className={!isEditing ? "bg-slate-50 border-transparent" : ""}
                required 
              />
              
              <Input 
                name="email" 
                label="Email" 
                type="email" 
                defaultValue={viewingUser?.email} 
                readOnly={!isEditing}
                className={!isEditing ? "bg-slate-50 border-transparent" : ""}
                required 
              />

              {/* Password only shown when creating new user or explicitly editing (if backend supported password update here, usually generic update doesn't include password field for security unless separate) */}
              {isEditing && !viewingUser && (
                  <Input name="password" label="Password" type="password" required />
              )}

              {/* Role Select */}
              {isEditing ? (
                  <Select name="role" label="Role" defaultValue={viewingUser?.role || 'Employee'} options={roleOptions} />
              ) : (
                  <Input label="Role" value={viewingUser?.role} readOnly className="bg-slate-50 border-transparent" />
              )}

              {/* Department/Company */}
              {isEditing ? (
                  <Input name="department" label="Department / Company" defaultValue={viewingUser?.department || viewingUser?.company} />
              ) : (
                  <Input label="Department" value={viewingUser?.department || viewingUser?.company || 'N/A'} readOnly className="bg-slate-50 border-transparent" />
              )}

              {isEditing && (
                <div className="flex gap-3 pt-6">
                    <Button type="button" variant="secondary" onClick={() => setIsModalOpen(false)} className="flex-1">
                    Cancel
                    </Button>
                    <Button type="submit" className="flex-1">Save Changes</Button>
                </div>
              )}
            </form>
          </GlassCard>
        </div>
      )}
    </>
  );
};

export default Team;