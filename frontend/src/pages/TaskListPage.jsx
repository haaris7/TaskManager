// src/pages/TaskListPage.jsx
import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { taskService } from '../services/api';

function TaskListPage() {
  const [tasks, setTasks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const navigate = useNavigate();
  
  const user = JSON.parse(localStorage.getItem('user') || '{}');

  useEffect(() => {
    fetchTasks();
  }, []);

  const fetchTasks = async () => {
    try {
      const data = await taskService.getAllTasks();
      setTasks(data);
    } catch (err) {
      if (err.response?.status === 401) {
        // Token expired or invalid
        navigate('/login');
      } else {
        setError('Failed to load tasks');
      }
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    navigate('/login');
  };

  if (loading) return <div>Loading tasks...</div>;
  if (error) return <div>Error: {error}</div>;

  return (
    <div style={{ padding: '20px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '20px' }}>
        <h1>Tasks</h1>
        <div>
          <span>Welcome, {user.username} ({user.role})</span>
          <button onClick={handleLogout} style={{ marginLeft: '10px' }}>
            Logout
          </button>
        </div>
      </div>

      {tasks.length === 0 ? (
        <p>No tasks found</p>
      ) : (
        <div style={{ display: 'grid', gap: '15px' }}>
          {tasks.map(task => (
            <div 
              key={task.id} 
              style={{ 
                border: '1px solid #ccc', 
                padding: '15px', 
                borderRadius: '5px' 
              }}
            >
              <h3>{task.name}</h3>
              <p>{task.description}</p>
              <p>Status: <strong>{task.status}</strong></p>
              <p>Assigned to: {task.assignedToUsername}</p>
              <small>Start: {new Date(task.startDate).toLocaleDateString()}</small>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default TaskListPage;